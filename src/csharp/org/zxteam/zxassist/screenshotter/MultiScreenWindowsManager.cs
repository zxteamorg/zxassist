namespace org.zxteam.zxassist.screenshotter
{
	using System;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Input;
	using System.Collections.Generic;
	using System.Windows.Threading;
	using System.Windows.Media;

	using org.zxteam.lib.reusable.wpf;

	internal class MultiScreenWindowsManager<TWnd> : IDisposable where TWnd : Window, new()
	{
		private readonly List<Window> _windows = new List<Window>(System.Windows.Forms.SystemInformation.MonitorCount);
		private readonly Dispatcher _dispatcher;

		public MultiScreenWindowsManager()
		{
			this._dispatcher = Dispatcher.CurrentDispatcher;
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

			//this.WindowState = System.Windows.WindowState.Maximized;
			//this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
			//this.WindowStyle = System.Windows.WindowStyle.None;

			//this.Left = virtualScreenRect.Left;
			//this.Top = virtualScreenRect.Top;
			//this.Width = virtualScreenRect.Width;
			//this.Height = virtualScreenRect.Height;

			//this.Topmost = true;

			//this.BorderThickness = new Thickness(0);
			this.Invalidate();
		}

		public void Dispose()
		{
			var windows = this._windows.ToArray();
			this._windows.Clear();
			foreach (var wnd in windows)
			{
				if (wnd.IsActive)
				{
					wnd.Close();
				}
			}
		}

		public void Show()
		{
			this._windows.ForEach(w => w.Show());
		}

		public void Hide()
		{
			this._windows.ForEach(w => w.Hide());
		}

		public event EventHandler WindowsRearranged;
		private void OnWindowsRearranged() { var handler = this.WindowsRearranged; if (handler != null) { handler(this, EventArgs.Empty); } }



		private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			this._dispatcher.Invoke(new Action(this.Invalidate));
		}

		private void Invalidate()
		{
			this.Hide();

			#region Make a window for each monitor
			var allScreens = Screen.AllScreens;
			var screenCount = allScreens.Length;
			if (this._windows.Count != screenCount)
			{
				while (this._windows.Count > 0 && this._windows.Count > screenCount)
				{
					int lastWndIndex = this._windows.Count - 1;
					var wnd = this._windows[lastWndIndex];
					wnd.Close();
					this._windows.RemoveAt(lastWndIndex);
				}

				if (screenCount > 0)
				{
					while (this._windows.Count < screenCount)
					{
						this._windows.Add(new TWnd() { Background = new SolidColorBrush(Colors.Aqua), Content = this._windows.Count });
					}
				}
			}
			#endregion

			#region Bind windows to monitors
			for (int screenIndex = 0; screenIndex < screenCount; ++screenIndex)
			{
				Screen screen = allScreens[screenIndex];
				Window wnd = this._windows[screenIndex];

				var screenBounds = screen.Bounds.ToUnits();
				
				wnd.Left = screenBounds.Left;
				wnd.Top = screenBounds.Top;
				wnd.Width = screenBounds.Width;
				wnd.Height = screenBounds.Height;
			}
			#endregion

			this.OnWindowsRearranged();

		}
	}
}
