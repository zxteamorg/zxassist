using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Threading;
using org.zxteam.zxassist.screenshotter;

namespace org.zxteam.zxassist
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private TaskbarIcon _trayIcon;
		private ScreenshotManager _screenshotManager;

		public App()
		{
			this.InitializeComponent();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			//create the notifyicon (it's a resource declared in NotifyIconResources.xaml
			this._trayIcon = (TaskbarIcon)FindResource("AppTrayIcon");
			if (this._trayIcon != null)
			{
				this._trayIcon.DataContext = new TrayIconViewModel();
			}

			this._screenshotManager = new ScreenshotManager();

//#if DEBUG
//			if (System.Diagnostics.Debugger.IsAttached)
//			{
//				((TrayIconViewModel)this._trayIcon.DataContext).ShowDemoToolWindowCommand.Execute(null);
//			}
//#endif
		}
		protected override void OnExit(ExitEventArgs e)
		{
			if (this._trayIcon != null)
			{
				this._trayIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
			}

			if (this._screenshotManager != null)
			{
				this._screenshotManager.Dispose();
			}

			base.OnExit(e);
		}
	}
}
