namespace org.zxteam.zxassist.Utils
{
	using System;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;

	public enum HotKeyModifier : int
	{
		/// <summary>
		/// Either ALT key must be held down.
		/// </summary>
		MOD_ALT = 0x0001,

		/// <summary>
		/// Either CTRL key must be held down.
		/// </summary>
		MOD_CONTROL = 0x0002,

		/// <summary>
		/// Either SHIFT key must be held down.
		/// </summary>
		MOD_SHIFT = 0x0004,

		/// <summary>
		/// Either WINDOWS key was held down. These keys are labeled with the Windows logo. Keyboard shortcuts that involve the WINDOWS key are reserved for use by the operating system.
		/// </summary>
		MOD_WIN = 0x0008,

		/// <summary>
		/// Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications.
		/// </summary>
		MOD_NOREPEAT = 0x4000
	}

	public sealed class HotKey : IDisposable
	{
		// TODO

		private static int __idCount = 10001;
		private readonly HotKeyWindow _hotKeyWnd;
		private readonly IntPtr _wndHandle;
		private readonly int _id;

		public HotKey(IntPtr wndHandle, HotKeyModifier mod, Keys vk)
		{
			this._id = __idCount++;
			this._wndHandle = wndHandle;
			if (RegisterHotKey(wndHandle, this._id, mod, vk))
			{
				throw new Exception("");
			}
			this._hotKeyWnd = new HotKeyWindow(this._id);
			this._hotKeyWnd.AssignHandle(wndHandle);
		}

		~HotKey() { Dispose(false); }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}



		private void Dispose(bool disposing)
		{
			if (disposing)
			{ // release other disposable objects
			}

			// Release unmanaged resources
			this._hotKeyWnd.ReleaseHandle();
			UnregisterHotKey(this._wndHandle, this._id);
		}


		private class HotKeyWindow : NativeWindow
		{
			private readonly int _id;
			public HotKeyWindow(int id)
			{
				this._id = id;
			}

			protected override void WndProc(ref Message m)
			{
				if (m.Msg == 0x0312 && m.WParam.ToInt32() == this._id)
				{
				}

				base.WndProc(ref m);
			}
		}


		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr wnd, int id, HotKeyModifier mode, Keys vk);

		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr wnd, int id);
	}

}