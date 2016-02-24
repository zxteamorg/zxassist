namespace org.zxteam.zxassist.screenshotter
{
	using System;
	using System.IO;
	using System.Windows.Media.Imaging;
	using System.Windows.Threading;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;
	using System.Collections.Generic;
	using System.Windows.Input;
	using System.Windows.Ink;
	using org.zxteam.zxassist.screenshotter;
	using System.Windows;
	using org.zxteam.lib.reusable.wpf;
	using org.zxteam.lib.reusable.wpf.input;
	using org.zxteam.lib.reusable.system.hooks;
	using org.zxteam.lib.reusable.system;

	internal class ScreenshotManager : ViewModelBase, IDisposable
	{
		private enum MODE
		{
			IDLE,
			ACTIVATION,
			EDIT
		}

		private readonly object _sync = new object();
		private bool _disposed;
		//private readonly KeyboardListener _keyboardListener = new KeyboardListener();
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
			this._currentMode = MODE.IDLE;

			_printScreenHook = Hook.CreateLowLevelKeyboardHook();
			_printScreenHook.KeyDown += (s, e) =>
			{
					if (e.Key == Keys.PrintScreen)
					{
						Console.WriteLine("ScreenHook Activated");
						Activation();
					}
			};
		}

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

		private DrawingAttributes _selectedToolDrawingAttributes = new DrawingAttributes() { Width = 30 };
		public DrawingAttributes SelectedToolDrawingAttributes { get { return this._selectedToolDrawingAttributes; } }
		#endregion

		#region Workflow
		private ToolWindowObsolete _toolWindow;
		private DrawableWindow _screenshotWindow;
		private DispatcherTimer _defaultActionTimer;
		private void Activation()
		{
			if (!App.Current.Dispatcher.CheckAccess()) { App.Current.Dispatcher.Invoke(new Action(this.Activation)); }

			if (this._screenshotWindow == null)
			{
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

				//this._screenshotWindow = new DrawableWindow();
				//this._screenshotWindow.DataContext = this;
				//this._screenshotWindow.Show();
				MultiScreenWindowsManager<DrawableWindow> mgr = new MultiScreenWindowsManager<DrawableWindow>();
				mgr.Show();
			}

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
			this._defaultActionTimer.Start();

			this.CurrentMode = MODE.ACTIVATION;
		}
		private void Cancel()
		{
			if (this._defaultActionTimer != null)
			{
				this._defaultActionTimer.Stop();
				this._defaultActionTimer = null;
			}
			if (this._toolWindow != null)
			{
				this._toolWindow.Close();
				this._toolWindow = null;
			}
			if (this._screenshotWindow != null)
			{
				this._screenshotWindow.Close();
				this._screenshotWindow = null;
			}

			this.CurrentMode = MODE.IDLE;
		}
		private void ExecDefaultAction()
		{
			if (this._defaultActionTimer != null)
			{
				this._defaultActionTimer.Stop();
				this._defaultActionTimer = null;
			}
			if (this._toolWindow != null)
			{
				this._toolWindow.Close();
				this._toolWindow = null;
			}
			if (this._screenshotWindow != null)
			{
				this._screenshotWindow.Close();
				this._screenshotWindow = null;
			}

			this.CurrentMode = MODE.IDLE;
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