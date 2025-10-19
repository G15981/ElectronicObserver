using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ElectronicObserver.Data;
using ElectronicObserver.Data.Battle;

namespace ElectronicObserver.Window.Wpf.Battle;

/// <summary>
/// Interaction logic for BattleView.xaml
/// </summary>
public partial class BattleView : UserControl
{
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
		"ViewModel", typeof(BattleViewModel), typeof(BattleView), new PropertyMetadata(default(BattleViewModel)));

	public BattleViewModel ViewModel
	{
		get => (BattleViewModel)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}

	public BattleView()
	{
		InitializeComponent();
	}

	// not really sure how to do this mvvm style
	private void FrameworkElement_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
	{
		var bm = KCDatabase.Instance.Battle;

		if (bm == null || bm.BattleMode == BattleManager.BattleModes.Undefined)
			e.Handled = true;
	}

	private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		var scrollViewer = (ScrollViewer)sender;
		// 根据滚轮方向左右滚动
		scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
		e.Handled = true; // 阻止默认的垂直滚动
	}
}
