namespace org.zxteam.lib.reusable.wpf
{
	using System.Windows;

	public class FullScreenWindow : Window
	{
		public FullScreenWindow()
		{
			base.WindowState = System.Windows.WindowState.Maximized;
			base.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
			base.WindowStyle = System.Windows.WindowStyle.None;
		}

		public new WindowStartupLocation WindowStartupLocation { get { return base.WindowStartupLocation; } }
		public new WindowState WindowState { get { return base.WindowState; } }
		public new WindowStyle WindowStyle { get { return base.WindowStyle; } }
	}
}