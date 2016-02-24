namespace org.zxteam.lib.reusable.wpf
{
	using System;

	public enum WindowState
	{
		NORMAL,
		MINIMIZED,
		MAXIMIZED,
		HIDDEN,
	}

	public interface IWindow : IDisposable
	{
		WindowState WindowState { get; set; }
	}
}