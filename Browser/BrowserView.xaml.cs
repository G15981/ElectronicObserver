using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Browser.CefSharpBrowser;
using Browser.WebView2Browser;
using BrowserLibCore;
using Windows.UI.ViewManagement;

namespace Browser;

/// <summary>
/// Interaction logic for BrowserView.xaml
/// </summary>
public partial class BrowserView
{
	public BrowserViewModel ViewModel { get; }
	
	public BrowserView(string host, int port, string culture, BrowserOption browser)
	{
		InitializeComponent();

		ViewModel = browser switch
		{
			BrowserOption.WebView2 => new WebView2ViewModel(host, port, culture),
			_ => new CefSharpViewModel(host, port, culture),
		};

		Loaded += ViewModel.OnLoaded;
		DataContext = ViewModel;
	}

	private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (sender is not FrameworkElement control) return;

		ViewModel.DpiScale = VisualTreeHelper.GetDpi(this);
		ViewModel.TextScaleFactor = new UISettings().TextScaleFactor;

		ViewModel.ActualHeight = control.ActualHeight;
		ViewModel.ActualWidth = control.ActualWidth;
	}

	private void Refresh_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left)
		{
			// 左键点击 → 执行绑定的 RefreshCommand（无需手动执行）
			return;
		}
		else if (e.ChangedButton == MouseButton.Right)
		{
			// 右键点击 → 执行新的 ForceRefreshCommand
			ViewModel.ForceRefreshCommand.Execute(null);

			// 阻止事件继续向下传递，不触发原有 RefreshCommand
			e.Handled = true;
		}
	}
}
