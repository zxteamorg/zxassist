namespace org.zxteam.lib.reusable.system
{
	using System;

	public interface IScreen
	{
		System.Drawing.Rectangle Bounds { get; }
		event EventHandler BoundsChanged;
	}

	public interface IPhysicalScreen : IScreen
	{
		int BitsPerPixel { get; }
		string DeviceName { get; }
		bool IsActive { get; }
		bool IsConnected { get; }
		bool IsPrimary { get; }
	}
}