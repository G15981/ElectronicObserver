using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElectronicObserver.Window.Wpf.Compass;

/// <summary>
/// Interaction logic for CompassView.xaml
/// </summary>
public partial class CompassView : UserControl
{
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
		"ViewModel", typeof(CompassViewModel), typeof(CompassView), new PropertyMetadata(default(CompassViewModel)));

	public CompassViewModel ViewModel
	{
		get => (CompassViewModel)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}

	public CompassView()
	{
		InitializeComponent();
	}
	private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		var sv = (ScrollViewer)sender;

		// 按住shift键 → 水平滚
		if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
		{
			sv.ScrollToHorizontalOffset(sv.HorizontalOffset - e.Delta);
			e.Handled = true;
			return;
		}

		// 有垂直滚动能力 → 用垂直滚
		if (sv.ScrollableHeight > 0)
		{
			// 默认行为即可
			return;
		}

		// 没有垂直滚动 → 改成水平滚
		if (sv.ScrollableWidth > 0)
		{
			sv.ScrollToHorizontalOffset(sv.HorizontalOffset - e.Delta);
			e.Handled = true;
		}


	}

}
