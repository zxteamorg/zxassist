namespace org.zxteam.lib.reusable.wpf
{
	using System;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;
#if MAX_NETFX_40
	using System.Diagnostics;
	using System.Reflection;
#endif

	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Multicast event for property change notifications.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Get value and check for not null
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		protected T GetPropertyHelper<T>(ref T storage,
#if MIN_NETFX_45
			[CallerMemberName] 
#endif
 string propertyName = null)
		{
#if MAX_NETFX_40
			if (propertyName == null) { propertyName = this.ResolvePropertyName(); }
#endif
			var value = storage;
			if (value == null)
			{
				throw new InvalidOperationException(string.Format("The '{0}' propery value is not set in instance of class '{1}'", propertyName, this.GetType().FullName));
			}

			return value;
		}

		/// <summary>
		/// Checks if a property already matches a desired value.  Sets the property and
		/// notifies listeners only when necessary.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="storage">Reference to a property with both getter and setter.</param>
		/// <param name="value">Desired value for the property.</param>
		/// <param name="propertyName">Name of the property used to notify listeners.  This
		/// value is optional and can be provided automatically when invoked from compilers that
		/// support CallerMemberName.</param>
		/// <returns>True if the value was changed, false if the existing value matched the
		/// desired value.</returns>
#if MAX_NETFX_40
		[MethodImpl(MethodImplOptions.NoInlining)]
#endif
		protected bool SetPropertyHelper<T>(ref T storage, T value,
#if MIN_NETFX_45
			[CallerMemberName] 
#endif
 string propertyName = null)
		{
#if MAX_NETFX_40
			if (propertyName == null) { propertyName = this.ResolvePropertyName(); }
#endif
			if (object.Equals(storage, value)) return false;

			storage = value;
			this.OnPropertyChanged(propertyName);

			return true;
		}

		/// <summary>
		/// Notifies listeners that a property value has changed.
		/// </summary>
		/// <param name="propertyName">Name of the property used to notify listeners.  This
		/// value is optional and can be provided automatically when invoked from compilers
		/// that support <see cref="CallerMemberNameAttribute"/>.</param>
#if MAX_NETFX_40
		[MethodImpl(MethodImplOptions.NoInlining)]
#endif
		protected void OnPropertyChanged(
#if MIN_NETFX_45
			[CallerMemberName] 
#endif
string propertyName = null)
		{
#if MAX_NETFX_40
			if (propertyName == null) { propertyName = this.ResolvePropertyName(); }
#endif
			var eventHandler = this.PropertyChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

#if MIN_NETFX_40
		/// <summary>
		/// Delayed notifies listeners that a property value has changed.
		/// </summary>
		/// <param name="propertyName">Name of the property used to notify listeners.  This
		/// value is optional and can be provided automatically when invoked from compilers
		/// that support <see cref="CallerMemberNameAttribute"/>.</param>
#if MAX_NETFX_40
		[MethodImpl(MethodImplOptions.NoInlining)]
#endif
		protected void OnDelayedPropertyChanged(
#if MIN_NETFX_45
			[CallerMemberName] 
#endif
string propertyName = null)
		{
#if MAX_NETFX_40
			if (propertyName == null) { propertyName = this.ResolvePropertyName(); }
#endif
			new Task(() => OnPropertyChanged(propertyName))
				.Start(TaskScheduler.FromCurrentSynchronizationContext());
		}
#endif

#if MAX_NETFX_40
		[MethodImpl(MethodImplOptions.NoInlining)]
		private string ResolvePropertyName()
		{
			StackTrace stackTrace = new StackTrace();
			StackFrame frame = stackTrace.GetFrame(2);
			MethodBase method = frame.GetMethod();
			return method.Name.Replace("set_", "");
		}
#endif
	}

	public abstract class ViewModelBase<TModel> : ViewModelBase
	{
		private readonly TModel _model;

		/// <summary>
		/// The Model related to this ViewModel. Cannot be <c>null</c>.
		/// </summary>
		public TModel Model { get { return _model; } }

		protected ViewModelBase() { /* for design time */ }

		protected ViewModelBase(TModel model)
		{
			if (model == null) throw new ArgumentNullException();

			_model = model;
		}
	}
}
