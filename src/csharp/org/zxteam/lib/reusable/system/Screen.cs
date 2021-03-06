﻿namespace org.zxteam.lib.reusable.system
{
	using System;
	using System.Linq;
	using System.Collections.Generic;

	public abstract class Screen
	{
		public static IScreen VirtualScreen { get { return __virtualScreen.Value; } }
		public static IPhysicalScreen[] PhysicalScreens { get { return PhysicalScreen.PhysicalScreens; } }

		public static event EventHandler SettingsChanged;

		private static readonly Lazy<VirtualScreen> __virtualScreen = new Lazy<VirtualScreen>(
			() => new VirtualScreen(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

		static Screen()
		{
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
		}

		private static void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			if(__virtualScreen.IsValueCreated) 
			{
				__virtualScreen.Value.Invalidate();
			}

			var handler = SettingsChanged;
			if (handler != null) { handler(typeof(Screen), EventArgs.Empty); }
		}
	}

	public sealed class PhysicalScreen : Screen, IPhysicalScreen
	{
		public static new PhysicalScreen[] PhysicalScreens { get { return __screens.ToArray(); } }
		private static readonly List<PhysicalScreen> __screens;

		private int? _bitsPerPixel;
		public int BitsPerPixel { get { return _bitsPerPixel.HasValue ? _bitsPerPixel.Value : (_bitsPerPixel = this._wrap.BitsPerPixel).Value; } }

		System.Drawing.Rectangle? _bounds;
		public System.Drawing.Rectangle Bounds { get { return _bounds.HasValue ? _bounds.Value : (_bounds = this._wrap.Bounds).Value; } }

		private string _deviveName;
		public string DeviceName { get { return _deviveName ?? (_deviveName = this._wrap.DeviceName); } }

		private bool _isActive;
		public bool IsActive
		{
			get { return this._isActive; }
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

		private void Invalidate() 
		{
		}
		
		private static void UpdateWrapBinding()
		{
			var systemScreens = System.Windows.Forms.Screen.AllScreens.ToList();

			#region Wrap new screens
			{
				// I think Systtem Screen euqals by DisplayName
				systemScreens
					.Except(__screens.Select(s => s._wrap))
					.ToList()
					.ForEach(toWrap =>
				{
					__screens.Add(new PhysicalScreen(toWrap));
				});
			}
			#endregion

			__screens.ForEach(screen =>
			{
				// Update _wrap (new setting for system screen)
				screen._wrap = systemScreens.Where(w => w.Equals(screen._wrap)).First();

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

		private System.Windows.Forms.Screen _wrap;
		private PhysicalScreen(System.Windows.Forms.Screen wrap) { this._wrap = wrap; }

		static PhysicalScreen()
		{
			__screens = new List<PhysicalScreen>();
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
		}
		private static void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			PhysicalScreen.UpdateWrapBinding();
		}
	}

	internal class VirtualScreen : Screen, IScreen
	{
		public VirtualScreen()
		{
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += delegate { this.Invalidate(); };
			Invalidate();
		}

		#region Bounds stuff
		System.Drawing.Rectangle _bounds;
		public System.Drawing.Rectangle Bounds
		{
			get { return _bounds; }
			set
			{
				if (this._bounds != value)
				{
					this._bounds = value;
					this.OnBoundsChanged();
				}
			}
		}
		public event EventHandler BoundsChanged;
		private void OnBoundsChanged()
		{
			var handler = this.BoundsChanged;
			if (handler != null) { handler(this, EventArgs.Empty); }
		}
		#endregion

		public void Invalidate()
		{
			this.Bounds = System.Windows.Forms.SystemInformation.VirtualScreen;
		}
	}
}