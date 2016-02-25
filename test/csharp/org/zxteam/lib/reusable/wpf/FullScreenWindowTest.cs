namespace org.zxteam.lib.reusable.wpf
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows;
	using System.Windows.Media;
	using System.Windows.Threading;

	using org.zxteam.lib.reusable.wpf;
	using org.zxteam.lib.reusable.system;
	using System.Windows.Controls;

	internal class FullScreenWindowTest : Application
	{
		private readonly List<FullScreenWindow> _windows = new List<FullScreenWindow>(System.Windows.Forms.SystemInformation.MonitorCount);
		private readonly Dispatcher _dispatcher;
		private readonly DispatcherTimer _testTimer;
		private readonly Queue<Action> _tests;

		public FullScreenWindowTest()
		{
			this._dispatcher = Dispatcher.CurrentDispatcher;

			this._testTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };
			this._testTimer.Tick += _testTimer_Tick;

			this._tests = new Queue<Action>();
			this.GetType()
				.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
				.Where(w => w.Name.StartsWith("Test"))
				.ToList()
				.ForEach(m => this._tests.Enqueue(() => { m.Invoke(this, new object[0]); }));

			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (_, __) => this.Invalidate();

			this.Invalidate();
		}

		void Test1()
		{
		}



		void _testTimer_Tick(object sender, EventArgs e)
		{
			if (this._tests.Count == 0)
			{
				this._testTimer.Stop();
				Application.Current.Shutdown();
				return;
			}

			var testAction = this._tests.Dequeue();
			testAction();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this._windows.ForEach(w => w.WindowState = WindowState.NORMAL);

			//this._testTimer.Start();
		}

		private void Invalidate()
		{
			IScreen[] allScreens = Screen.AllScreens;
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
					this._windows.Add(new FullScreenWindow()
					{
						Background = new SolidColorBrush(Colors.Aqua),
						Content = new FullScreenWindowTest_TestUI(),
						BindScreen = screenWithoutWindow,
						DataContext = new BindScreenViewModel(screenWithoutWindow)
					});
				}
			}
			#endregion
		}

		private class BindScreenViewModel : System.ComponentModel.INotifyPropertyChanged
		{
			private readonly IScreen _screen;

			public BindScreenViewModel(IScreen screen)
			{
				_screen = screen;
				screen.BoundsChanged += screen_BoundsChanged;
			}

			public string Bounds
			{
				get
				{
					return string.Format("{0} x {1}",
						this._screen.Bounds.Width, this._screen.Bounds.Height);
				}
			}

			void screen_BoundsChanged(object sender, EventArgs e)
			{
				var handler = this.PropertyChanged;
				if (handler != null)
				{
					handler(this, new System.ComponentModel.PropertyChangedEventArgs("Bounds"));
				}
			}

			public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		}
	}

	internal partial class FullScreenWindowTest_TestUI : UserControl
	{
		public FullScreenWindowTest_TestUI()
		{
			this.InitializeComponent();
		}

		private void finishButton_Clicked(object sender, EventArgs e)
		{

		}
	}
}