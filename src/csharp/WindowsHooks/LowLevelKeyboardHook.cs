namespace org.zxteam.lib.reusable.WindowsHooks
{
	using System;

	public sealed class LowLevelKeyboardHook : Hook
	{
		internal LowLevelKeyboardHook() : base(HookID.WH_KEYBOARD_LL) { }

		/// <summary>
		/// WM_KEYDOWN
		/// </summary>
		public event EventHandler<LowLevelKeyboardHookEventArgs> KeyDown;
		/// <summary>
		/// WM_KEYUP
		/// </summary>
		public event EventHandler<LowLevelKeyboardHookEventArgs> KeyUp;
		/// <summary>
		/// WM_SYSKEYDOWN
		/// </summary>
		public event EventHandler<LowLevelKeyboardHookEventArgs> SysKeyDown;
		/// <summary>
		/// WM_SYSKEYUP
		/// </summary>
		public event EventHandler<LowLevelKeyboardHookEventArgs> SysKeyUp;

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

			EventHandler<LowLevelKeyboardHookEventArgs> handler = null;
			LowLevelKeyboardHookEventArgs args = new LowLevelKeyboardHookEventArgs(nCode, wParam, lParam);

			switch ((WinApi.KeyboardMessages)wParam)
			{
				case WinApi.KeyboardMessages.WM_KEYDOWN: handler = this.KeyDown; break;
				case WinApi.KeyboardMessages.WM_KEYUP: handler = this.KeyUp; break;
				case WinApi.KeyboardMessages.WM_SYSKEYDOWN: handler = this.SysKeyDown; break;
				case WinApi.KeyboardMessages.WM_SYSKEYUP: handler = this.SysKeyUp; break;
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
			public enum KeyboardMessages : int
			{
				WM_KEYDOWN = 0x0100,
				WM_KEYUP = 0x0101,
				WM_SYSKEYDOWN = 0x0104,
				WM_SYSKEYUP = 0x0105,
			}
		}
	}
}
