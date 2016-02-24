namespace org.zxteam.lib.reusable.system.hooks
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Diagnostics;

	public abstract class Hook : IDisposable
	{
		public static LowLevelMouseHook CreateLowLevelMouseHook() { return new LowLevelMouseHook(); }
		public static LowLevelKeyboardHook CreateLowLevelKeyboardHook() { return new LowLevelKeyboardHook(); }

		private readonly IntPtr _hookID;
		private readonly HookDelegate _hookCallbackDelegate;

		internal Hook(HookID hookId)
		{
			this._hookCallbackDelegate = new HookDelegate(this.HookCallback);
			this._hookID = SetWindowsHook(hookId, this._hookCallbackDelegate);
		}
		~Hook() { Dispose(false); }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract IntPtr OnHookCallback(int nCode, UIntPtr wParam, IntPtr lParam);

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				// release other disposable objects
			}

			// Release unmanaged resources
			WinApi.UnhookWindowsHookEx(_hookID);
		}

		private delegate IntPtr HookDelegate(int nCode, UIntPtr wParam, IntPtr lParam);

		private static IntPtr SetWindowsHook(HookID hookId, HookDelegate proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			{
				return WinApi.SetWindowsHookEx((int)hookId, proc, WinApi.GetModuleHandle(curProcess.MainModule.ModuleName), 0);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private IntPtr HookCallback(int nCode, UIntPtr wParam, IntPtr lParam)
		{
			IntPtr hookResult = this.OnHookCallback(nCode, wParam, lParam);
			if (hookResult != IntPtr.Zero) { return hookResult; }

			return WinApi.CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		private static class WinApi
		{
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr SetWindowsHookEx(int idHook, HookDelegate lpfn, IntPtr hMod, uint dwThreadId);

			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr CallNextHookEx(IntPtr hookHandle, int nCode, UIntPtr wParam, IntPtr lParam);

			[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr GetModuleHandle(string lpModuleName);
		}
	}
}