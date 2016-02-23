using System;
using System.Windows;
using System.Windows.Input;
using org.zxteam.zxassist.Settings;
using org.zxteam.zxassist.ScreenShotter;

namespace org.zxteam.zxassist.TrayIcon
{
	/// <summary>
	/// Provides bindable properties and commands for the NotifyIcon. In this sample, the
	/// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
	/// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
	/// </summary>
	public class TrayIconViewModel
	{
		private ToolWindow _demoToolWnd;

		private static Window GetOrCreateMainWindow()
		{
			if (Application.Current.MainWindow == null)
			{
				Application.Current.MainWindow = new SettingsWindow();
				Application.Current.MainWindow.Show();
			}
			return Application.Current.MainWindow;
		}

		/// <summary>
		/// Shows a window, if none is already open.
		/// </summary>
		public ICommand ActivateWindowCommand
		{
			get
			{
				return new DelegateCommand
				{
					CanExecuteFunc = () => true,
					CommandAction = () =>
					{
						var wnd = GetOrCreateMainWindow();
						wnd.Activate();
						wnd.Topmost = true;  // important
						wnd.Topmost = false; // important
						wnd.Focus();         // important
					}
				};
			}
		}

		/// <summary>
		/// Shows a window, if none is already open.
		/// </summary>
		public ICommand ShowWindowCommand
		{
			get
			{
				return new DelegateCommand
				{
					CanExecuteFunc = () => Application.Current.MainWindow == null,
					CommandAction = () => GetOrCreateMainWindow()
				};
			}
		}

		/// <summary>
		/// Hides the main window. This command is only enabled if a window is open.
		/// </summary>
		public ICommand HideWindowCommand
		{
			get
			{
				return new DelegateCommand
				{
					CommandAction = () => Application.Current.MainWindow.Close(),
					CanExecuteFunc = () => Application.Current.MainWindow != null
				};
			}
		}


		public ICommand ShowDemoToolWindowCommand
		{
			get
			{
				return new DelegateCommand
				{
					CanExecuteFunc = () => this._demoToolWnd == null,
					CommandAction = () =>
					{
						if (this._demoToolWnd == null) { this._demoToolWnd = new ToolWindow(); }
					}
				};
			}
		}

		public ICommand HideDemoToolWindowCommand
		{
			get
			{
				return new DelegateCommand
				{
					CommandAction = () => { this._demoToolWnd.Close(); this._demoToolWnd = null; },
					CanExecuteFunc = () => this._demoToolWnd != null
				};
			}
		}


		/// <summary>
		/// Shuts down the application.
		/// </summary>
		public ICommand ExitApplicationCommand
		{
			get
			{
				return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
			}
		}



		/// <summary>
		/// Simplistic delegate command for the demo.
		/// </summary>
		class DelegateCommand : ICommand
		{
			public Action CommandAction { get; set; }
			public Func<bool> CanExecuteFunc { get; set; }

			public void Execute(object parameter)
			{
				CommandAction();
			}

			public bool CanExecute(object parameter)
			{
				return CanExecuteFunc == null || CanExecuteFunc();
			}

			public event EventHandler CanExecuteChanged
			{
				add { CommandManager.RequerySuggested += value; }
				remove { CommandManager.RequerySuggested -= value; }
			}
		}
	}
}
