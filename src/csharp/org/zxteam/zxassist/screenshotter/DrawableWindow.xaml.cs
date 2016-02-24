namespace org.zxteam.zxassist.screenshotter
{
	using org.zxteam.lib.reusable.wpf;
	using System;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Input;

	public partial class DrawableWindow : FullScreenWindow
	{
		public DrawableWindow()
		{
			InitializeComponent();

			//this.ShowInTaskbar = false;
			//this.Topmost = true;
		}

		//protected override void OnClosed(EventArgs e)
		//{
		//    base.OnClosed(e);
		//}
	}
}
