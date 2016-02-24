namespace org.zxteam.lib.reusable.wpf.input
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows.Input;

	public class RelayCommand : ICommand
	{
		readonly Func<bool> _canExecute;
		readonly Action _action;

		#region Implement interface ICommand
		public event EventHandler CanExecuteChanged;
		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute();
		}
		public void Execute(object parameter)
		{
			_action();
        }
        #endregion

		public RelayCommand(Action action)
		{
			if (action == null)
				throw new ArgumentNullException();

			_action = action;
		}

		public RelayCommand(Action action, Func<bool> canExecute)
			: this(action)
		{
			_canExecute = canExecute;
		}

		/// <summary>
		/// Raises the CanExecuteChaged event
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged();
		}

		void OnCanExecuteChanged()
		{
			var handler = CanExecuteChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}

	public class RelayCommand<T> : ICommand<T>, ICommand
	{
		readonly Func<T, bool> _canExecute;
		readonly Action<T> _action;

		#region Implement interface ICommand
		public event EventHandler CanExecuteChanged;
		bool ICommand.CanExecute(object parameter) { return this.CanExecute((T)parameter); }
		void ICommand.Execute(object parameter) { Execute((T)parameter); }

		public bool CanExecute(T parameter)
		{
			return _canExecute == null || _canExecute((T)parameter);
		}
		public void Execute(T parameter)
		{
			_action((T)parameter);
		}
		#endregion

		public RelayCommand(Action<T> action)
		{
			if (action == null)
				throw new ArgumentNullException();

			_action = action;
		}

		public RelayCommand(Action<T> action, Func<T, bool> canExecute)
			: this(action)
		{
			_canExecute = canExecute;
		}

		/// <summary>
		/// Raises the CanExecuteChaged event
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged();
		}

		void OnCanExecuteChanged()
		{
			var handler = CanExecuteChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}
