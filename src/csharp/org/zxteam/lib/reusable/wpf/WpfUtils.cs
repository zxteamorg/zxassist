namespace org.zxteam.lib.reusable.wpf
{
	using System;
	using System.Runtime.InteropServices;

	public static class WpfUtils
	{
		public static System.Windows.Rect ToUnits(this System.Drawing.Rectangle _this)
		{
			// get the system DPI
			IntPtr dDC = WinApi.GetDC(IntPtr.Zero); // Get desktop DC
			int dpi = WinApi.GetDeviceCaps(dDC, 88);
			bool rv = WinApi.ReleaseDC(IntPtr.Zero, dDC);
			
			// WPF's physical unit size is calculated by taking the 
			// "Device-Independant Unit Size" (always 1/96)
			// and scaling it by the system DPI
			double physicalUnitSize = (1d / 96d) * (double)dpi;
			System.Windows.Rect wpfUnits = new System.Windows.Rect(
				(double)_this.Left / physicalUnitSize,
				(double)_this.Top / physicalUnitSize,
				(double)_this.Width / physicalUnitSize,
				(double)_this.Height / physicalUnitSize
				);

			return wpfUnits;
		}
		public static System.Drawing.Rectangle ToPixels(this System.Windows.Rect _this)
		{
			throw new NotImplementedException();
		}

		private static class WinApi
		{
			[DllImport("User32.dll")]
			public static extern IntPtr GetDC(IntPtr hwnd);

			[DllImport("gdi32.dll")]
			public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

			[DllImport("user32.dll")]
			public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
		}
	}
}