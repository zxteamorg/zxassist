namespace org.zxteam.lib.reusable.WindowsHooks
{
	using System;

	public sealed class LowLevelMouseHook : Hook
	{
		internal LowLevelMouseHook() : base(HookID.WH_MOUSE_LL) { }

		/// <summary>
		/// WM_MOUSEMOVE
		/// </summary>
		public event EventHandler<LowLevelMouseHookEventArgs> Move;
		/// <summary>
		/// WM_LBUTTONDOWN
		/// </summary>
		public event EventHandler<LowLevelMouseHookEventArgs> LeftButtonDown;
		/// <summary>
		/// WM_LBUTTONUP
		/// </summary>
		public event EventHandler<LowLevelMouseHookEventArgs> LeftButtonUp;
		/// <summary>
		/// MBUTTONDOWN
		/// </summary>
		public event EventHandler<LowLevelMouseHookEventArgs> MiddleButtonDown;
		/// <summary>
		/// WM_MBUTTONUP
		/// </summary>
		public event EventHandler<LowLevelMouseHookEventArgs> MiddleButtonUp;
		/// <summary>
		/// WM_RBUTTONDOWN
		/// </summary>
		public event EventHandler<LowLevelMouseHookEventArgs> RightButtonDown;
		/// <summary>
		/// WM_RBUTTONUP
		/// </summary>
		public event EventHandler<LowLevelMouseHookEventArgs> RightButtonUp;
		/// <summary>
		/// WM_MOUSEHWHEEL
		/// </summary>
		public event EventHandler<LowLevelMouseHookEventArgs> HorizontalWhell;
		/// <summary>
		/// WM_MOUSEWHEEL
		/// </summary>
		public event EventHandler<LowLevelMouseHookEventArgs> VerticalWhell;

		protected override IntPtr OnHookCallback(int nCode, UIntPtr wParam, IntPtr lParam)
		{
			// MSDN says: A code the hook procedure uses to determine how to process the message.
			// If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx
			// function without further processing and should return the value returned by CallNextHookEx.

			if (nCode < 0)
			{
				// Return IntPtr.Zero for run CallNextHookEx in base class
				return IntPtr.Zero;
			}

			EventHandler<LowLevelMouseHookEventArgs> handler = null;
			LowLevelMouseHookEventArgs args = new LowLevelMouseHookEventArgs(nCode, wParam, lParam);

			switch ((WinApi.MouseMessages)wParam)
			{
				case WinApi.MouseMessages.WM_LBUTTONDOWN: handler = this.LeftButtonDown; break;
				case WinApi.MouseMessages.WM_LBUTTONUP: handler = this.LeftButtonUp; break;
				case WinApi.MouseMessages.WM_MBUTTONDOWN: handler = this.MiddleButtonDown; break;
				case WinApi.MouseMessages.WM_MBUTTONUP: handler = this.MiddleButtonUp; break;
				case WinApi.MouseMessages.WM_MOUSEHWHEEL: handler = this.HorizontalWhell; break;
				case WinApi.MouseMessages.WM_MOUSEMOVE: handler = this.Move; break;
				case WinApi.MouseMessages.WM_MOUSEWHEEL: handler = this.VerticalWhell; break;
				case WinApi.MouseMessages.WM_RBUTTONDOWN: handler = this.RightButtonDown; break;
				case WinApi.MouseMessages.WM_RBUTTONUP: handler = this.RightButtonUp; break;
				default:
#if DEBUG
					System.Diagnostics.Debugger.Break();
#endif
					break;
			}

			if (handler != null)
			{
				handler(this, args);
			}

			return args.Result;
		}

		private static class WinApi
		{
			public enum MouseMessages : int
			{
				WM_MOUSEMOVE = 0x0200,
				WM_LBUTTONDOWN = 0x0201,
				WM_LBUTTONUP = 0x0202,
				WM_RBUTTONDOWN = 0x0204,
				WM_RBUTTONUP = 0x0205,
				WM_MBUTTONDOWN = 0x207,
				WM_MBUTTONUP = 0x208,
				WM_MOUSEWHEEL = 0x020A,
				WM_MOUSEHWHEEL = 0x020E,
			}
		}
	}
}
