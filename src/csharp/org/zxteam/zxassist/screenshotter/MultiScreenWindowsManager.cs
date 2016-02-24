namespace org.zxteam.zxassist.screenshotter
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Threading;

	using org.zxteam.lib.reusable.wpf;
	using org.zxteam.lib.reusable.system;

	internal class MultiScreenWindowsManager<TWnd> : IDisposable where TWnd : FullScreenWindow, new()
	{
		private readonly List<FullScreenWindow> _windows = new List<FullScreenWindow>(System.Windows.Forms.SystemInformation.MonitorCount);
		private readonly Dispatcher _dispatcher;

		public MultiScreenWindowsManager()
		{
			this._dispatcher = Dispatcher.CurrentDispatcher;
			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

			this.Invalidate();
		}

		public void Dispose()
		{
			var windows = this._windows.ToArray();
			this._windows.Clear();
			foreach (var wnd in windows)
			{
				wnd.Dispose();
			}
		}

		public void Show()
		{
			this._windows.ForEach(w => w.WindowState = lib.reusable.wpf.WindowState.NORMAL);
		}

		public void Hide()
		{
			this._windows.ForEach(w => w.WindowState = lib.reusable.wpf.WindowState.HIDDEN);
		}

		public event EventHandler WindowsRearranged;
		private void OnWindowsRearranged() { var handler = this.WindowsRearranged; if (handler != null) { handler(this, EventArgs.Empty); } }



		private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			this._dispatcher.Invoke(new Action(this.Invalidate));
		}

		private void Invalidate()
		{
			IScreen[] allScreens = Screen.GetScreens();
			if (allScreens == null) { return; }

			#region Remove windows associated with not existing screens
			{
				FullScreenWindow[] windowsToRemove = this._windows.Where(w => !allScreens.Contains(w.BindScreen)).ToArray();
				foreach (FullScreenWindow windowToRemove in windowsToRemove) { this._windows.Remove(windowToRemove); }
			}
			#endregion

			#region Make a window for each new screen
			{
				IEnumerable<IScreen> screensWithoutWindow = allScreens.Except(this._windows.Select(w => w.BindScreen));
				foreach (IScreen screenWithoutWindow in screensWithoutWindow)
				{
					this._windows.Add(new TWnd()
					{
						Background = new SolidColorBrush(Colors.Aqua),
						Content = this._windows.Count,
						BindScreen = screenWithoutWindow
					});
				}
			}
			#endregion
		}
	}
}
