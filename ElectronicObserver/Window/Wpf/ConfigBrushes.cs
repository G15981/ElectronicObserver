using System.Windows.Media;
using ElectronicObserver.Utility;

namespace ElectronicObserver.Window.Wpf
{
	public static class ConfigBrushes
	{
		public static SolidColorBrush SubBackBrush => new(Configuration.Config.UI.SubBackColor.ToWpfColor());
	}
}
