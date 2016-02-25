namespace org.zxteam.zxassist.screenshotter
{
	using org.zxteam.lib.reusable.wpf;
	using System;
	using System.IO;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;

	public partial class ScreenshotDrawableWindow : FullScreenWindow
	{
		public ScreenshotDrawableWindow()
		{
			InitializeComponent();
		}

		private void shareButton_Click(object sender, RoutedEventArgs e)
		{
			ScreenshotManager friendlyDataContext = this.DataContext as ScreenshotManager;
			if (friendlyDataContext != null)
			{
				//get the dimensions of the ink control
				int width = (int)this.inkCanvas.ActualWidth;
				int height = (int)this.inkCanvas.ActualHeight;
				//render ink to bitmap
				RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
				rtb.Render(this.inkCanvas);
				//save the ink to a memory stream
				BitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(rtb));
				using (MemoryStream ms = new MemoryStream())
				{
					encoder.Save(ms);
					//get the bitmap bytes from the memory stream
					ms.Position = 0;
					friendlyDataContext.ShareCommand.Execute(ms.ToArray());
				}
			}
		}
	}
}
