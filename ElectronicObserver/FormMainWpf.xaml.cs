using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell; // WindowChrome 所在命名空间

using ElectronicObserver.ViewModels;

namespace ElectronicObserver;

/// <summary>
/// Interaction logic for FormMainWpf.xaml
/// </summary>
public partial class FormMainWpf : System.Windows.Window
{
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
		"ViewModel", typeof(FormMainViewModel), typeof(FormMainWpf), new PropertyMetadata(default(FormMainViewModel)));

	public FormMainViewModel ViewModel
	{
		get => (FormMainViewModel)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}

	public FormMainWpf()
	{
		InitializeComponent();

		ViewModel = new(DockingManager, this);

		Loaded += (sender, _) => ViewModel.LoadLayout(sender);
		Closed += (sender, _) => ViewModel.SaveLayout(sender);

		// 初始化时隐藏 ModernWPF 标题栏
		ModernWpf.Controls.TitleBar.SetExtendViewIntoTitleBar(this, true);
		WindowChrome.GetWindowChrome(this)!.CaptionHeight = 0;

	}
	private void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		//if (e.ButtonState == MouseButtonState.Pressed)
		//	this.DragMove();
		if (e.Source is MenuItem)
		{
			// 点击到菜单项 -> 不拖动，让菜单正常工作
			return;
		}
		if (DataContext is FormMainViewModel vm && vm.LockLayout)
		{
			// 如果锁定窗口，退出
			return;
		}
		if (e.ButtonState == MouseButtonState.Pressed)
		{
			this.DragMove();
			e.Handled = true;
		}
	}

	//点击切换标题栏显示
	private void ToggleTitleBarVisibility(object sender, RoutedEventArgs e)
	{
		bool show = !ModernWpf.Controls.TitleBar.GetExtendViewIntoTitleBar(this);	//show为ture时显示标题栏
		ModernWpf.Controls.TitleBar.SetExtendViewIntoTitleBar(this, show);          //SetExtendViewIntoTitleBar与show为反值
		WindowChrome.GetWindowChrome(this)!.CaptionHeight = show ? 0 : 32;			//显示标题栏设置高度为32，隐藏标题栏为0
	}
}
