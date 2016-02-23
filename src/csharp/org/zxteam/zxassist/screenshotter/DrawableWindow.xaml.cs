namespace org.zxteam.zxassist.ScreenShotter
{
	using System;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Input;

	public partial class DrawableWindow : Window
	{
		public DrawableWindow()
		{
			InitializeComponent();

			//this.ShowInTaskbar = false;

			var virtualScreenRect = System.Windows.Forms.SystemInformation.VirtualScreen;
	
			this.WindowState = System.Windows.WindowState.Maximized;
			this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
			this.WindowStyle = System.Windows.WindowStyle.None;

			this.Left = virtualScreenRect.Left;
			this.Top = virtualScreenRect.Top;
			this.Width = virtualScreenRect.Width;
			this.Height = virtualScreenRect.Height;

			//this.Topmost = true;

			this.BorderThickness = new Thickness(0);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		}
	}
}
