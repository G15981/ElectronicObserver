namespace ElectronicObserver.Utility;

/// <summary>
/// Settings for automatically filling the DMM login form (https://accounts.dmm.com).
/// There is no settings UI for this feature - edit Settings\Configuration.xml manually
/// while the application is closed. The password is stored in plain text.
/// </summary>
public sealed class ConfigDmmAutoLogin : Configuration.ConfigurationData.ConfigPartBase
{
	/// <summary>Enable automatic filling of the DMM login form.</summary>
	public bool IsEnabled { get; set; }

	/// <summary>DMM login ID (e-mail address).</summary>
	public string LoginId { get; set; }

	/// <summary>DMM password. Stored in plain text.</summary>
	public string Password { get; set; }

	/// <summary>Check the "keep me logged in" checkbox on the login form.</summary>
	public bool AutoLogin { get; set; }

	public ConfigDmmAutoLogin()
	{
		IsEnabled = false;
		LoginId = "";
		Password = "";
		AutoLogin = false;
	}
}
