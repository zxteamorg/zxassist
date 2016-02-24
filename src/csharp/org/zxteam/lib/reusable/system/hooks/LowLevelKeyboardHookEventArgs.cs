namespace org.zxteam.lib.reusable.system.hooks
{
	using System;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;

	public sealed class LowLevelKeyboardHookEventArgs : HookEventArgs
	{
		private readonly WinApi.KBDLLHOOKSTRUCT _hookStruct;

		internal LowLevelKeyboardHookEventArgs(int nCode, UIntPtr wParam, IntPtr lParam)
			: base(nCode, wParam, lParam)
		{
			this._hookStruct = (WinApi.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.KBDLLHOOKSTRUCT));
		}

		public Keys Key { get { return (Keys)this._hookStruct.vkCode; } }
		//public int PixelY { get { return this._hookStruct.pt.y; } }

		private static class WinApi
		{
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

			[StructLayout(LayoutKind.Sequential)]
			public struct KBDLLHOOKSTRUCT
			{
				public uint vkCode;
				public uint scanCode;
				public uint flags;
				public uint time;
				public UIntPtr dwExtraInfo;
			}
		}
	}
}