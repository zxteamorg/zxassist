namespace org.zxteam.zxassist.ScreenShotter
{
	using org.zxteam.zxassist.Utils;
	using System;
	using System.Runtime.InteropServices;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Interop;
	using System.Windows.Media;
	using org.zxteam.lib.reusable.WindowsHooks;

	public partial class ToolWindowObsolete : Window
	{
		private readonly LowLevelMouseHook _mouseHook;

		public ToolWindowObsolete()
		{
			InitializeComponent();

			this.AllowsTransparency = true;
			this.Background = Brushes.Transparent;
			this.ShowActivated = false;
			this.ShowInTaskbar = false;
			this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
			this.Topmost = true;
			this.WindowStyle = System.Windows.WindowStyle.None;

			this.Width = 50;
			this.Height = 50;

			_mouseHook = Hook.CreateLowLevelMouseHook();
			_mouseHook.Move += _mouseHook_HookOccured;

			POINT point;
			WinApi.GetCursorPos(out point);
			double left = (double)point.x - this.Width;
			double top = (double)point.y - this.Height;
			this.Left = left;
			this.Top = top;
		}

		internal new ScreenshotManager DataContext
		{
			get { return (ScreenshotManager)base.DataContext; }
			set { base.DataContext = value; }
		}

		protected override void OnClosed(EventArgs e)
		{
			this._mouseHook.Dispose();

			base.OnClosed(e);
		}

		void _mouseHook_HookOccured(object sender, LowLevelMouseHookEventArgs e)
		{
			if (this.DataContext.IsActivationMode)
			{
				switch ((MouseMessages)e.wParam)
				{
					case MouseMessages.WM_RBUTTONDOWN:
					case MouseMessages.WM_LBUTTONDOWN:
						var cmd = this.DataContext.SwitchToEditModeCommand;
						if (cmd.CanExecute(null))
						{
							cmd.Execute(null);
							this.IsPassAllInputToUnderlayedWindow = true;
							return;
						}
						break;
				}
			}

			switch ((MouseMessages)e.wParam)
			{
				case MouseMessages.WM_MOUSEMOVE:
					{
						if (this.DataContext.IsActivationMode || this.IsPassAllInputToUnderlayedWindow)
						{
							MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(e.lParam, typeof(MSLLHOOKSTRUCT));
							var point = hookStruct.pt;
							double left = (double)point.x - this.Width / 2;
							double top = (double)point.y - this.Height / 2;
							this.Left = left;
							this.Top = top;
						}
					}
					break;
				case MouseMessages.WM_RBUTTONDOWN:
					{
						this.DataContext.IsChoosingTool = !this.DataContext.IsChoosingTool;
						this.IsPassAllInputToUnderlayedWindow = !this.DataContext.IsChoosingTool;
					}
					break;
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			//IsPassAllInputToUnderlayedWindow = true;
			//HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
			//source.AddHook(new HwndSourceHook(HandleMessages));
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
		}

		private bool IsPassAllInputToUnderlayedWindow
		{
			get
			{
				// Get this window's handle         
				IntPtr hwnd = new WindowInteropHelper(this).Handle;

				// Change the extended window style to include WS_EX_TRANSPARENT
				int extendedStyle = WinApi.GetWindowLong(hwnd, WinApi.GWL_EXSTYLE);

				return (extendedStyle & WinApi.WS_EX_TRANSPARENT) == WinApi.WS_EX_TRANSPARENT;
			}
			set
			{
				/* We need to set WS_EX_TRANSPARENT to window's GWL_EXSTYLE for pass all event to underlayed window */

				// Get this window's handle         
				IntPtr hwnd = new WindowInteropHelper(this).Handle;

				// Change the extended window style to include WS_EX_TRANSPARENT
				int extendedStyle = WinApi.GetWindowLong(hwnd, WinApi.GWL_EXSTYLE);
				int newextendedStyle = value ? (extendedStyle | WinApi.WS_EX_TRANSPARENT) : (extendedStyle & ~WinApi.WS_EX_TRANSPARENT);
				WinApi.SetWindowLong(hwnd, WinApi.GWL_EXSTYLE, newextendedStyle);

				this.Opacity = value ? 0.25 : 1.0;
			}
		}

		private const int WM_MOUSEMOVE = 0x0200;

		private enum MouseMessages
		{
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_MOUSEMOVE = 0x0200,
			WM_MOUSEWHEEL = 0x020A,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct POINT
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct MSLLHOOKSTRUCT
		{
			public POINT pt;
			public uint mouseData;
			public uint flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		private static class WinApi
		{
			public const int GWL_EXSTYLE = -20;
			public const int WS_EX_NOACTIVATE = 0x08000000;
			public const int WS_EX_TRANSPARENT = 0x00000020;


			//[StructLayout(LayoutKind.Sequential)]
			//public struct POINT
			//{
			//	public int X;
			//	public int Y;

			//	public POINT(int x, int y)
			//	{
			//		this.X = x;
			//		this.Y = y;
			//	}
			//}


			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetCursorPos(out POINT lpPoint);

			[DllImport("user32.dll")]
			public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

			[DllImport("user32.dll")]
			public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DataContext.IsChoosingTool = false;
			this.IsPassAllInputToUnderlayedWindow = true;
			POINT point;
			WinApi.GetCursorPos(out point);
			double left = (double)point.x - this.Width / 2;
			double top = (double)point.y - this.Height / 2;
			this.Left = left;
			this.Top = top;
		}
	}
}
