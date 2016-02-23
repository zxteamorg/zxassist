namespace org.zxteam.zxassist
{
	using org.zxteam.zxassist.Utils;
	using System;
	using System.Runtime.InteropServices;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Interop;
	using System.Windows.Media;
	using System.Windows.Threading;
	using System.Collections.Generic;
	using org.zxteam.lib.reusable.WindowsHooks;

	sealed class ExtendedLowLevelMouseHook
	{
		private readonly LowLevelMouseHook _mouseHook;
		private readonly DispatcherTimeout _idleTimeout;

		public ExtendedLowLevelMouseHook(TimeSpan idleTimeout, Dispatcher dispatcher)
		{
			this._mouseHook = Hook.CreateLowLevelMouseHook();
			this._mouseHook.Move += new EventHandler<LowLevelMouseHookEventArgs>(_hook_Move);
			this._idleTimeout = new DispatcherTimeout(idleTimeout, DispatcherPriority.Input, dispatcher);
			this._idleTimeout.Timeout += new EventHandler(_idleTimeout_Timeout);
		}

		~ExtendedLowLevelMouseHook() { Dispose(false); }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				// release other disposable objects
				this._idleTimeout.Timeout -= new EventHandler(_idleTimeout_Timeout);
				this._mouseHook.Dispose();
			}

			// Release unmanaged resources
		}

		public LowLevelMouseHookEventArgs LastMouseMoveEventArgs { get { return this._lastMouseMoveEventArgs; } }
		private LowLevelMouseHookEventArgs _lastMouseMoveEventArgs;

		private void _hook_Move(object sender, LowLevelMouseHookEventArgs e)
		{
			this._lastMouseMoveEventArgs = e;
			this._idleTimeout.Reset();
			this.IsMoving = true;
		}
		private void _idleTimeout_Timeout(object sender, EventArgs e) { this.IsMoving = false; }

		private bool _isMoving;
		public bool IsMoving
		{
			get { return this._isMoving; }
			private set
			{
				if (this._isMoving != value)
				{
					this._isMoving = value;
					if (value)
					{
						this.OnMoveStart();
					}
					else
					{
						this.OnMoveStop();
					}
				}
			}
		}

		public event EventHandler<LowLevelMouseHookEventArgs> MoveStart;
		private void OnMoveStart()
		{
			var handler = this.MoveStart;
			if (handler != null)
			{
				handler(this, this._lastMouseMoveEventArgs);
			}
		}

		public event EventHandler<LowLevelMouseHookEventArgs> MoveStop;
		private void OnMoveStop()
		{
			var handler = this.MoveStop;
			if (handler != null)
			{
				handler(this, this._lastMouseMoveEventArgs);
			}
		}

		public event EventHandler<LowLevelMouseHookEventArgs> Move { add { this._mouseHook.Move += value; } remove { this._mouseHook.Move -= value; } }
		public event EventHandler<LowLevelMouseHookEventArgs> LeftButtonDown { add { this._mouseHook.LeftButtonDown += value; } remove { this._mouseHook.LeftButtonDown -= value; } }
		public event EventHandler<LowLevelMouseHookEventArgs> LeftButtonUp { add { this._mouseHook.LeftButtonUp += value; } remove { this._mouseHook.LeftButtonUp -= value; } }
		public event EventHandler<LowLevelMouseHookEventArgs> MiddleButtonDown { add { this._mouseHook.MiddleButtonDown += value; } remove { this._mouseHook.MiddleButtonDown -= value; } }
		public event EventHandler<LowLevelMouseHookEventArgs> MiddleButtonUp { add { this._mouseHook.MiddleButtonUp += value; } remove { this._mouseHook.MiddleButtonUp -= value; } }
		public event EventHandler<LowLevelMouseHookEventArgs> RightButtonDown { add { this._mouseHook.RightButtonDown += value; } remove { this._mouseHook.RightButtonDown -= value; } }
		public event EventHandler<LowLevelMouseHookEventArgs> RightButtonUp { add { this._mouseHook.RightButtonUp += value; } remove { this._mouseHook.RightButtonUp -= value; } }
		public event EventHandler<LowLevelMouseHookEventArgs> HorizontalWhell { add { this._mouseHook.HorizontalWhell += value; } remove { this._mouseHook.HorizontalWhell -= value; } }
		public event EventHandler<LowLevelMouseHookEventArgs> VerticalWhell { add { this._mouseHook.VerticalWhell += value; } remove { this._mouseHook.VerticalWhell -= value; } }
	}

	public sealed class DispatcherTimeout
	{
		private readonly DispatcherTimer _timer;

		public DispatcherTimeout(TimeSpan timeout, DispatcherPriority priority, Dispatcher dispatcher)
		{
			this._timer = new DispatcherTimer(priority, dispatcher);
			this._timer.Interval = timeout;
			this._timer.Tick += new EventHandler(_timer_Tick);

			this._isTimeout = true;
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			this._timer.Stop();
			this._isTimeout = true;
			this.OnTimeout();
		}

		private bool _isTimeout;
		public bool IsTimeout
		{
			get { return _isTimeout; }
		}

		public void Reset()
		{
			this._isTimeout = false;
			this._timer.Stop();
			this._timer.Start();
		}

		public bool YeildTimeout()
		{
			if (!this._isTimeout)
			{
				this._isTimeout = true;
				this._timer.Stop();
				OnTimeout();
				return true;
			}
			return false;
		}

		public event EventHandler Timeout;
		private void OnTimeout()
		{
			var handler = this.Timeout;
			if (handler != null) { handler(this, EventArgs.Empty); }
		}
	}

	public enum WINDOW_MODE
	{
		/// <summary>
		/// Window inactive because mouse is moving beyong HitTestBounds
		/// </summary>
		INACTIVE,

		/// <summary>
		/// Window shown as hint (when mouse stop moving)
		/// </summary>
		ACTIVE_HINT,

		/// <summary>
		/// Window was catched by mouse pointer and activate
		/// </summary>
		ACTIVE,
	}

	public class MouseHauntingWindow : Window
	{
		private static readonly DependencyPropertyKey __modePropertyKey = DependencyProperty.RegisterReadOnly(
			"Mode",
			typeof(WINDOW_MODE),
			typeof(MouseHauntingWindow),
			new PropertyMetadata(WINDOW_MODE.INACTIVE, OnModePropertyChanged));
		public static readonly DependencyProperty ModeProperty = __modePropertyKey.DependencyProperty;
		private static void OnModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MouseHauntingWindow _this = (MouseHauntingWindow)d;
			WINDOW_MODE value = (WINDOW_MODE)e.NewValue;
			switch (value)
			{
				case WINDOW_MODE.ACTIVE:
					//_this.Opacity = ACTIVE_OPACITY;
					_this.Show();
					break;
				case WINDOW_MODE.ACTIVE_HINT:
					//_this.Opacity = HINT_OPACITY;
					_this.Show();
					break;
				case WINDOW_MODE.INACTIVE:
					//_this.Hide();
					break;
			}
			VisualStateManager.GoToElementState(_this, value.ToString(), true);
		}
		public WINDOW_MODE Mode
		{
			get { return (WINDOW_MODE)base.GetValue(ModeProperty); }
			private set { base.SetValue(__modePropertyKey, value); }
		}


		private const int MOUSE_MOVING_TIMEOUT = 150;
		private const int MOUSE_IDLE_TIMEOUT = 250;
		private const double ACTIVE_OPACITY = 1;
		private const double HINT_OPACITY = 0.25;

		private readonly ExtendedLowLevelMouseHook _mouseHook;
		private readonly DispatcherTimeout _startMouseMoveTimeout;

		public MouseHauntingWindow()
		{
			this.AllowsTransparency = true;
			this.Background = Brushes.Transparent;
			this.ShowActivated = false;
			this.ShowInTaskbar = false;
			this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
			this.Topmost = true;
			this.WindowStyle = System.Windows.WindowStyle.None;

			this._mouseHook = new ExtendedLowLevelMouseHook(TimeSpan.FromMilliseconds(MOUSE_IDLE_TIMEOUT), this.Dispatcher);
			this._mouseHook.Move += this._mouseHook_Move;
			this._mouseHook.MoveStart += this._mouseHook_MoveStart;
			this._mouseHook.MoveStop += this._mouseHook_MoveStop;

			this._startMouseMoveTimeout = new DispatcherTimeout(TimeSpan.FromMilliseconds(MOUSE_MOVING_TIMEOUT), DispatcherPriority.Input, this.Dispatcher);
			this._startMouseMoveTimeout.Timeout += new EventHandler(_startMouseMoveTimeout_Timeout);
		}


		void _startMouseMoveTimeout_Timeout(object sender, EventArgs e)
		{
			Console.WriteLine("MOVING LONG");

			if (this.HitTestBounds())
			{
				this.Mode = WINDOW_MODE.ACTIVE;
			}
			else
			{
				this.MoveToCursorLocation();
				this.Mode = WINDOW_MODE.ACTIVE_HINT;
			}
		}

		void _mouseHook_MoveStart(object sender, LowLevelMouseHookEventArgs e)
		{
			Console.WriteLine("MOVING START");
			this._startMouseMoveTimeout.Reset();
		}

		void _mouseHook_MoveStop(object sender, LowLevelMouseHookEventArgs e)
		{
			Console.WriteLine("MOVING STOP");
			if (!this._startMouseMoveTimeout.YeildTimeout())
			{
				if (this.Mode == WINDOW_MODE.ACTIVE_HINT)
				{
					if (this.HitTestBounds())
					{
						this.Mode = WINDOW_MODE.ACTIVE;
						return;
					}
				}

				if (this.Mode != WINDOW_MODE.ACTIVE)
				{
					this.MoveToCursorLocation();
					this.Mode = WINDOW_MODE.ACTIVE_HINT;
				}
			}
		}

		void _mouseHook_Move(object sender, LowLevelMouseHookEventArgs e)
		{
			this._lastMovePoint = ConvertPixelsToUnits(e.PixelX, e.PixelY);
			if (this.Mode != WINDOW_MODE.INACTIVE)
			{
				if (this._startMouseMoveTimeout.IsTimeout)
				{
					if (!HitTestBounds())
					{
						this.Mode = WINDOW_MODE.INACTIVE;
					}
				}
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			this._mouseHook.Dispose();

			base.OnClosed(e);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			/* We need to set WS_EX_TRANSPARENT to window's GWL_EXSTYLE for pass all event to underlayed window */

			// Get this window's handle         
			IntPtr hwnd = new WindowInteropHelper(this).Handle;

			// Change the extended window style to include WS_EX_TRANSPARENT
			int extendedStyle = WinApi.GetWindowLong(hwnd, WinApi.GWL_EXSTYLE);
			int newextendedStyle = (extendedStyle | WinApi.WS_EX_TRANSPARENT);
			//WinApi.SetWindowLong(hwnd, WinApi.GWL_EXSTYLE, newextendedStyle);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
		}

		private bool HitTestBounds() { return this.HitTestBounds(this._lastMovePoint); }
		protected virtual bool HitTestBounds(Point hitTestPoint)
		{
			Rect r = new Rect(this.Left + 5, this.Top + 5, this.Width - 10, this.Height - 10);
			return r.Contains(hitTestPoint);
		}

		private Point _lastMovePoint;
		private void MoveToCursorLocation()
		{
			Vector cursorOffset = this.CursorOffset;
			double left = this._lastMovePoint.X + cursorOffset.X;
			double top = this._lastMovePoint.Y + cursorOffset.Y;
			this.Left = left;
			this.Top = top;
		}

		private readonly Vector _cursorOffset = new Vector(5, 5);
		protected virtual Vector CursorOffset { get { return this._cursorOffset; } }

		private static Point ConvertPixelsToUnits(int x, int y)
		{
			// get the system DPI
			IntPtr dDC = WinApi.GetDC(IntPtr.Zero); // Get desktop DC
			int dpi = WinApi.GetDeviceCaps(dDC, 88);
			bool rv = WinApi.ReleaseDC(IntPtr.Zero, dDC);

			// WPF's physical unit size is calculated by taking the 
			// "Device-Independant Unit Size" (always 1/96)
			// and scaling it by the system DPI
			double physicalUnitSize = (1d / 96d) * (double)dpi;
			Point wpfUnits = new Point((double)x / physicalUnitSize, (double)y / physicalUnitSize);

			return wpfUnits;
		}

		private static class WinApi
		{
			public const int GWL_EXSTYLE = -20;
			public const int WS_EX_NOACTIVATE = 0x08000000;
			public const int WS_EX_TRANSPARENT = 0x00000020;


			[StructLayout(LayoutKind.Sequential)]
			public struct POINT
			{
				public int x;
				public int y;

				public POINT(int x, int y)
				{
					this.x = x;
					this.y = y;
				}
			}

			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetCursorPos(out POINT lpPoint);

			[DllImport("user32.dll")]
			public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

			[DllImport("user32.dll")]
			public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

			[DllImport("User32.dll")]
			public static extern IntPtr GetDC(IntPtr hwnd);

			[DllImport("gdi32.dll")]
			public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

			[DllImport("user32.dll")]
			public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
		}
	}
}
