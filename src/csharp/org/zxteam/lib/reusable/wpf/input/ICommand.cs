namespace org.zxteam.lib.reusable.wpf.input
{
	public interface ICommand<T> : System.Windows.Input.ICommand
	{
		bool CanExecute(T parameter);
		void Execute(T parameter);
	}
}
