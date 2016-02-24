namespace org.zxteam.lib.reusable.system.hooks
{
	using System;
	using System.Runtime.InteropServices;

	public sealed class LowLevelMouseHookEventArgs : HookEventArgs
	{
		private readonly WinApi.MSLLHOOKSTRUCT _hookStruct;

		internal LowLevelMouseHookEventArgs(int nCode, UIntPtr wParam, IntPtr lParam)
			: base(nCode, wParam, lParam)
		{
			this._hookStruct = (WinApi.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.MSLLHOOKSTRUCT));
		}

		public int PixelX { get { return this._hookStruct.pt.x; } }
		public int PixelY { get { return this._hookStruct.pt.y; } }

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
			public struct MSLLHOOKSTRUCT
			{
				public POINT pt;
				public uint mouseData;
				public uint flags;
				public uint time;
				public UIntPtr dwExtraInfo;
			}
		}
	}
}