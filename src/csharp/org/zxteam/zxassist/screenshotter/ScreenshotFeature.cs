namespace org.zxteam.zxassist.screenshotter
{
	using System;
	using System.IO;
	using System.Windows.Media.Imaging;
	using System.Windows.Threading;
	using System.Runtime.InteropServices;
	using System.Collections.Generic;
	using System.Windows.Input;
	using System.Windows.Ink;
	using org.zxteam.zxassist.screenshotter;
	using System.Windows;
	using org.zxteam.lib.reusable.wpf;
	using org.zxteam.lib.reusable.wpf.input;
	using org.zxteam.lib.reusable.system.hooks;
	using org.zxteam.lib.reusable.system;
	using System.Windows.Media;
	using System.Diagnostics;
	using System.Collections.Specialized;
	using System.Net;
	using System.Xml.Linq;
	using System.Drawing;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Configuration;

	internal class ScreenshotManager : ViewModelBase, IDisposable
	{
		private enum MODE
		{
			IDLE,
			ACTIVATION,
			EDIT
		}

		private readonly Dispatcher _dispatcher;
		private readonly object _sync = new object();
		private bool _disposed;
		private readonly LowLevelKeyboardHook _printScreenHook;

		public enum KeyEvent : uint
		{
			WM_KEYDOWN = 256,
			WM_KEYUP = 257,
			WM_SYSKEYUP = 261,
			WM_SYSKEYDOWN = 260
		}

		public ScreenshotManager()
		{
			this._dispatcher = Dispatcher.CurrentDispatcher;

			Screen.SettingsChanged += Screen_SettingsChanged;

			this._currentMode = MODE.IDLE;

			_printScreenHook = Hook.CreateLowLevelKeyboardHook();
			_printScreenHook.KeyDown += (s, e) =>
			{
				if (e.Key == System.Windows.Forms.Keys.PrintScreen)
				{
					Console.WriteLine("ScreenHook Activated");
					Activate();
				}
			};
		}

		private void Screen_SettingsChanged(object sender, EventArgs e) { this.Cancel(); }

		public void Dispose()
		{
			if (this._disposed) { return; }
			lock (this._sync)
			{
				if (this._disposed) { return; }
				this._disposed = true;

				// TODO
				_printScreenHook.Dispose();
			}
		}

		#region UI Binding
		private BitmapImage _screenshotImage;
		public BitmapImage ScreenshotImage
		{
			get { return this._screenshotImage; }
			set { base.SetPropertyHelper(ref this._screenshotImage, value); }
		}

		private int _defaultActionTimeout;
		public int DefaultActionTimeout
		{
			get { return this._defaultActionTimeout; }
			set { base.SetPropertyHelper(ref this._defaultActionTimeout, value); }
		}

		private ICommand _switchToEditModeCommand;
		public ICommand SwitchToEditModeCommand
		{
			get { return this._switchToEditModeCommand ?? (this._switchToEditModeCommand = new RelayCommand(this.SwitchToEditMode)); }
		}

		private ICommand _cancelCommand;
		public ICommand CancelCommand
		{
			get { return this._cancelCommand ?? (this._cancelCommand = new RelayCommand(this.Cancel)); }
		}

		private ICommand<byte[]> _shareCommand;
		public ICommand<byte[]> ShareCommand
		{
			get { return this._shareCommand ?? (this._shareCommand = new RelayCommand<byte[]>(this.Share)); }
		}

		private MODE _currentMode;
		private MODE CurrentMode
		{
			get { return this._currentMode; }
			set
			{
				if (this._currentMode != value)
				{
					this._currentMode = value;
					base.OnPropertyChanged("IsActivationMode");
					base.OnPropertyChanged("IsEditMode");
				}
			}
		}

		private bool _isChoosingTool;
		public bool IsChoosingTool { get { return this._isChoosingTool; } set { base.SetPropertyHelper(ref this._isChoosingTool, value); } }

		public bool IsActivationMode { get { return this._currentMode == MODE.ACTIVATION; } }
		public bool IsEditMode { get { return this._currentMode == MODE.EDIT; } }

		private ICommand _selectTool1Command;
		public ICommand SelectTool1Command
		{
			get { return this._selectTool1Command ?? (this._selectTool1Command = new RelayCommand(this.SelectTool1)); }
		}

		private ICommand _selectTool2Command;
		public ICommand SelectTool2Command
		{
			get { return this._selectTool2Command ?? (this._selectTool2Command = new RelayCommand(this.SelectTool2)); }
		}

		private ICommand _selectTool3Command;
		public ICommand SelectTool3Command
		{
			get { return this._selectTool3Command ?? (this._selectTool3Command = new RelayCommand(this.SelectTool3)); }
		}

		private ICommand _selectTool4Command;
		public ICommand SelectTool4Command
		{
			get { return this._selectTool4Command ?? (this._selectTool4Command = new RelayCommand(this.SelectTool4)); }
		}

		private DrawingAttributes _selectedToolDrawingAttributes = new DrawingAttributes() { Width = 6, Height = 6, IsHighlighter = true, Color = System.Windows.Media.Colors.Blue };
		public DrawingAttributes SelectedToolDrawingAttributes { get { return this._selectedToolDrawingAttributes; } }
		#endregion

		#region Workflow
		private DispatcherTimer _defaultActionTimer;
		private readonly List<FullScreenWindow> _windows = new List<FullScreenWindow>(System.Windows.Forms.SystemInformation.MonitorCount);
		private void Activate()
		{
			if (!this._dispatcher.CheckAccess()) { this._dispatcher.Invoke(new Action(this.Activate)); return; }

			this.Cancel();

			// capture entire screen, and save it to a file
			var img = ScreenCapture.Instance.CaptureScreen();

			// Save image as PNG stream
			MemoryStream ms = new MemoryStream();
			img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			ms.Position = 0;

			// Prepare BitmapImage for render in ScreenshotWindow
			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			ms.Seek(0, SeekOrigin.Begin);
			bi.StreamSource = ms;
			bi.EndInit();

			this.ScreenshotImage = bi;

			IScreen[] allScreens = Screen.AllScreens;
			if (allScreens == null) { return; }

			foreach (IScreen screen in allScreens)
			{
				this._windows.Add(new ScreenshotDrawableWindow()
				{
					BindScreen = screen,
					DataContext = this
				});
			}
			this._windows.ForEach(w => w.WindowState = org.zxteam.lib.reusable.wpf.WindowState.NORMAL);

			//if (this._toolWindow == null)
			//{
			//    this._toolWindow = new ToolWindowObsolete();
			//    this._toolWindow.DataContext = this;
			//}
			//this._toolWindow.Show();

			if (this._defaultActionTimer == null)
			{
				int cancelCount = 5;
				this.DefaultActionTimeout = cancelCount;
				this._defaultActionTimer = new DispatcherTimer();
				this._defaultActionTimer.Interval = TimeSpan.FromSeconds(1);
				this._defaultActionTimer.Tick += (s, e) =>
				{
					if (--cancelCount == 0)
					{
						this._defaultActionTimer.Stop();
						this._defaultActionTimer = null;
						this.ExecDefaultAction();
					}
					this.DefaultActionTimeout = cancelCount;
				};
			}
			//this._defaultActionTimer.Start();

			this.CurrentMode = MODE.ACTIVATION;
		}
		private void Cancel()
		{
			if (!this._dispatcher.CheckAccess()) { this._dispatcher.Invoke(new Action(this.Cancel)); return; }

			if (this._defaultActionTimer != null)
			{
				this._defaultActionTimer.Stop();
				this._defaultActionTimer = null;
			}
			//if (this._toolWindow != null)
			//{
			//	this._toolWindow.Close();
			//	this._toolWindow = null;
			//}

			this._windows.ForEach(w => w.Dispose());
			this._windows.Clear();

			this.CurrentMode = MODE.IDLE;
		}
		private void ExecDefaultAction()
		{
			if (this._defaultActionTimer != null)
			{
				this._defaultActionTimer.Stop();
				this._defaultActionTimer = null;
			}
			//if (this._toolWindow != null)
			//{
			//	this._toolWindow.Close();
			//	this._toolWindow = null;
			//}
			this._windows.ForEach(w => w.Dispose());
			this._windows.Clear();

			this.CurrentMode = MODE.IDLE;
		}
		private void Share(byte[] data)
		{
			this.Cancel();

			Task.Run<XDocument>(() =>
			{
				try
				{
					using (var w = new WebClient())
					{
						string clientID = ConfigurationManager.AppSettings["IMGUR_CLIENT_ID"];
						w.Headers.Add("Authorization", "Client-ID " + clientID);
						var values = new NameValueCollection { { "image", Convert.ToBase64String(data) } };

						byte[] response = w.UploadValues("https://api.imgur.com/3/upload.xml", values);

						var doc = XDocument.Load(new MemoryStream(response));
						return doc;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Fail to upload", MessageBoxButton.OK, MessageBoxImage.Error);
					return null;
				}
			}).ContinueWith((t) =>
			{
				if (t.Result != null)
				{
					MessageBox.Show(t.Result.ToString());
					var link = t.Result.Root.Element(XName.Get("link")).Value;
					Clipboard.SetText(link);
					Process.Start(link);
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}


		private void SwitchToEditMode()
		{
			if (this._defaultActionTimer != null)
			{
				this._defaultActionTimer.Stop();
				this._defaultActionTimer = null;
			}
			this.DefaultActionTimeout = 0;
			this.CurrentMode = MODE.EDIT;
		}
		private void SelectTool1()
		{
			SelectedToolDrawingAttributes.Color = System.Windows.Media.Colors.Green;
		}
		private void SelectTool2()
		{
			SelectedToolDrawingAttributes.Color = System.Windows.Media.Colors.Blue;
		}
		private void SelectTool3()
		{
			SelectedToolDrawingAttributes.Color = System.Windows.Media.Colors.Yellow;
		}
		private void SelectTool4()
		{
			SelectedToolDrawingAttributes.Color = System.Windows.Media.Colors.Red;
		}
		#endregion

		private void VerifyDisposed()
		{
			if (this._disposed) { throw new ObjectDisposedException(this.GetType().FullName); }
		}
	}
}