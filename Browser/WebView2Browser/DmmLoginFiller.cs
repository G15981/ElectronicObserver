using System;
using System.Text.Json;
using System.Threading.Tasks;
using BrowserLibCore;
using Microsoft.Web.WebView2.Core;

namespace Browser.WebView2Browser;

/// <summary>
/// Injects a script that fills the DMM login form (https://accounts.dmm.com/service/login/password/...).
///
/// The login page is a React app, so assigning .value directly does not update React state.
/// The script writes the values with the native HTMLInputElement prototype setter and dispatches
/// a bubbling "input" event, which React 17+ picks up at its root listener. It polls the page
/// until the form elements appear, because React may not have rendered them yet when
/// NavigationCompleted fires.
/// </summary>
public static class DmmLoginFiller
{
	private const int PollIntervalMs = 250;
	private const int MaxPollCount = 80; // 250ms x 80 = 20 seconds

	private const string FillScript = """
	(() => {
		const data = __LOGIN_DATA__;
		const nativeSetter = Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, 'value').set;

		const setInputValue = (input, value) => {
			nativeSetter.call(input, value);
			// bubbles must be true - React listens for "input" at the root container
			input.dispatchEvent(new Event('input', { bubbles: true }));
		};

		let attempts = 0;
		const timer = setInterval(() => {
			attempts += 1;

			const idInput = document.getElementById('login_id');
			const passwordInput = document.getElementById('password');
			const autoLoginCheckbox = document.getElementById('use_auto_login');

			// the form has not been rendered yet (or this is not the login page)
			if (!idInput || !passwordInput)
			{
				if (attempts >= __MAX_ATTEMPTS__) clearInterval(timer);
				return;
			}

			if (idInput.value !== data.loginId) setInputValue(idInput, data.loginId);
			if (passwordInput.value !== data.password) setInputValue(passwordInput, data.password);

			if (autoLoginCheckbox && autoLoginCheckbox.checked !== data.autoLogin)
			{
				// click() generates a real mouse event which React handles normally
				autoLoginCheckbox.click();
			}

			// stop as soon as everything is filled, so later manual edits are never overwritten
			const idDone = idInput.value === data.loginId;
			const passwordDone = passwordInput.value === data.password;
			const checkboxDone = !autoLoginCheckbox || autoLoginCheckbox.checked === data.autoLogin;

			if ((idDone && passwordDone && checkboxDone) || attempts >= __MAX_ATTEMPTS__)
			{
				clearInterval(timer);
			}
		}, __POLL_INTERVAL__);
	})();
	""";

	public static string BuildScript(string loginId, string password, bool autoLogin)
	{
		// serialize with System.Text.Json so quotes/backslashes/unicode in the password
		// can never break out of the JS string literal
		string data = JsonSerializer.Serialize(new
		{
			loginId,
			password,
			autoLogin,
		});

		return FillScript
			.Replace("__LOGIN_DATA__", data)
			.Replace("__MAX_ATTEMPTS__", MaxPollCount.ToString())
			.Replace("__POLL_INTERVAL__", PollIntervalMs.ToString());
	}

	/// <summary>
	/// Injects the auto-fill script. Call after NavigationCompleted on a accounts.dmm.com page.
	/// </summary>
	public static async Task TryFill(CoreWebView2 webView, BrowserConfiguration configuration, Action<int, string> log)
	{
		if (string.IsNullOrEmpty(configuration.DmmLoginId) || string.IsNullOrEmpty(configuration.DmmPassword))
		{
			return;
		}

		string script = BuildScript(configuration.DmmLoginId, configuration.DmmPassword, configuration.DmmAutoLoginCheck);

		// WebView2 sometimes rejects ExecuteScriptAsync right after NavigationCompleted
		// while navigation state is still settling, so retry a few times
		for (int attempt = 1; attempt <= 3; attempt++)
		{
			try
			{
				await webView.ExecuteScriptAsync(script);
				log(2, $"DMM login auto-fill script injected on {webView.Source}");
				return;
			}
			catch (Exception e)
			{
				if (attempt == 3)
				{
					log(2, $"DMM login auto-fill failed: {e.Message}");
					return;
				}

				await Task.Delay(500);
			}
		}
	}
}
