namespace org.zxteam.lib.reusable.wpf
{
	using System.Windows;
	using System;
	using org.zxteam.lib.reusable.system;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Media;

	public class FullScreenWindow : ContentControl, IWindow
	{
		private readonly Window _wrap;
		private bool _disposed = false;

		private IScreen _bindScreen;
		public IScreen BindScreen
		{
			get { return this._bindScreen; }
			set
			{
				if (this._bindScreen != value)
				{
					if (this._bindScreen != null)
					{
						// detach from old screen
						this._bindScreen.BoundsChanged -= new EventHandler(BindScreen_Changed);
					}

					this._bindScreen = value;

					if (this._bindScreen != null)
					{
						// Attach to new screen
						this._bindScreen.BoundsChanged += new EventHandler(BindScreen_Changed);
					}

					this.InvalidateScreen();
				}
			}
		}

		private void BindScreen_Changed(object sender, EventArgs e)
		{
			this.Dispatcher.Invoke(new Action(this.InvalidateScreen));
		}

		private WindowState _windowState = WindowState.HIDDEN;
		public WindowState WindowState
		{
			get { return this._windowState; }
			set
			{
				if (value == wpf.WindowState.MAXIMIZED) { throw new ArgumentException("MAXIMIZED state is not supported by " + this.GetType().FullName); }

				if (value != this._windowState)
				{
					this._windowState = value;
					if (this._bindScreen != null) { ApplyStateToWrap(value); }
				}
			}
		}
		private void ApplyStateToWrap(wpf.WindowState ourState)
		{
			switch (ourState)
			{
				case wpf.WindowState.HIDDEN:
					this._wrap.Hide();
					break;
				case wpf.WindowState.MAXIMIZED:
					this._wrap.WindowState = System.Windows.WindowState.Maximized;
					this._wrap.Show();
					break;
				case wpf.WindowState.MINIMIZED:
					this._wrap.WindowState = System.Windows.WindowState.Minimized;
					this._wrap.Show();
					break;
				case wpf.WindowState.NORMAL:
					this._wrap.WindowState = System.Windows.WindowState.Normal;
					this._wrap.Show();
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public FullScreenWindow()
		{
			this._wrap = new Window();

			this._wrap.WindowState = System.Windows.WindowState.Normal;
			this._wrap.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
			this._wrap.WindowStyle = System.Windows.WindowStyle.None;
			this._wrap.ResizeMode = System.Windows.ResizeMode.NoResize;
			this._wrap.SetBinding(Window.ContentProperty, new Binding() { Source = this });
			this._wrap.SetBinding(Window.BackgroundProperty, new Binding("Background") { Source = this });
		}

		public void Dispose()
		{
			if (!this._disposed)
			{
				this._wrap.Close();

				this._disposed = true;
			}
		}

		private void InvalidateScreen()
		{
			var screen = this._bindScreen;
			System.Windows.Rect windowBounds;
			wpf.WindowState windowState;
			if (screen != null && screen.IsActive)
			{
				windowBounds = screen.Bounds.ToUnits();
				windowState = this._windowState;
			}
			else
			{
				windowBounds = Rect.Empty;
				windowState = wpf.WindowState.HIDDEN;
			}

			this._wrap.Left = windowBounds.Left;
			this._wrap.Top = windowBounds.Top;
			this._wrap.Width = windowBounds.Width;
			this._wrap.Height = windowBounds.Height;

			this.ApplyStateToWrap(windowState);
		}
	}
}