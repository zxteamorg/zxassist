namespace org.zxteam.lib.reusable.system
{
	using System;
	using System.Linq;
	using System.Collections.Generic;

	public sealed class Screen : IScreen
	{
		public static IScreen[] GetScreens() { return __screens.ToArray(); }

		private int? _bitsPerPixel;
		public int BitsPerPixel { get { return _bitsPerPixel.HasValue ? _bitsPerPixel.Value : (_bitsPerPixel = this._wrap.BitsPerPixel).Value; } }

		System.Drawing.Rectangle? _bounds;
		public System.Drawing.Rectangle Bounds { get { return _bounds.HasValue ? _bounds.Value : (_bounds = this._wrap.Bounds).Value; } }

		private string _deviveName;
		public string DeviceName { get { return _deviveName ?? (_deviveName = this._wrap.DeviceName); } }

		private bool _isActive;
		public bool IsActive { get { return this._isActive; }
			set
			{
				if (this._isActive != value)
				{
					this._isActive = true;
					this.OnActiveChanged();
				}
			}
		}

		public bool IsConnected
		{
			get
			{
#warning TODO
				return true;
			}
		}

		public bool IsPrimary { get { return this._wrap.Primary; } }

		private void OnActiveChanged()
		{
		}

		private void OnBitsPerPixelChanged()
		{
		}

		public event EventHandler BoundsChanged;
		private void OnBoundsChanged()
		{
			var handler = this.BoundsChanged;
			if (handler != null) { handler(this, EventArgs.Empty); }
		}

		private static readonly List<Screen> __screens;
		static Screen()
		{
			__screens = new List<Screen>();
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
			Invalidate();
		}

		private static void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e) { Invalidate(); }

		private static void Invalidate()
		{
			//var wrappedSystemScreens = __screens.Select(s=>s._wrap).ToList();
			var systemScreens = System.Windows.Forms.Screen.AllScreens.ToList();

			//var obloseteScreens = __screens.Where(w => !systemScreens.Contains(w._wrap));
			//var notWrappedSystemScreens = systemScreens.Except(__screens.Select(s => s._wrap)).ToList();

			#region Wrap new screens
			systemScreens
				.Except(__screens.Select(s => s._wrap))
				.ToList()
				.ForEach(toWrap => __screens.Add(new Screen(toWrap)));
			#endregion

			__screens.ForEach(screen =>
			{
				if (!systemScreens.Contains(screen._wrap))
				{
					screen._bitsPerPixel = 0;
					screen.OnBitsPerPixelChanged();

					screen._bounds = System.Drawing.Rectangle.Empty;
					screen.OnBoundsChanged();

					screen.IsActive = false;
				}
				else
				{
					if (!System.Drawing.Rectangle.Equals(screen._bounds, screen._wrap.Bounds))
					{
						screen._bounds = null;
						screen.OnBoundsChanged();
					}

					screen.IsActive = true;
				}
			});
		}

		private readonly System.Windows.Forms.Screen _wrap;
		private Screen(System.Windows.Forms.Screen wrap) { this._wrap = wrap; }
	}

}