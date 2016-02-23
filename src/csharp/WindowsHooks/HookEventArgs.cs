namespace org.zxteam.lib.reusable.WindowsHooks
{
	using System;

	public abstract class HookEventArgs : EventArgs
	{
		protected HookEventArgs(int nCode, UIntPtr wParam, IntPtr lParam)
		{
			this._nCode = nCode;
			this._wParam = wParam;
			this._lParam = lParam;
			this.Result = IntPtr.Zero;
		}

		public virtual IntPtr Result { get; set; }

		private readonly int _nCode;
		internal int nCode { get { return _nCode; } }

		private readonly UIntPtr _wParam;
		public UIntPtr wParam { get { return _wParam; } }

		private readonly IntPtr _lParam;
		public IntPtr lParam { get { return _lParam; } }
	}
}