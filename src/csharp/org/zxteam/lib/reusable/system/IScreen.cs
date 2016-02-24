namespace org.zxteam.lib.reusable.system
{
	using System;

	public interface IScreen
	{
		int BitsPerPixel { get; }
		System.Drawing.Rectangle Bounds { get; }
		string DeviceName { get; }
		bool IsActive { get; }
		bool IsConnected { get; }
		bool IsPrimary { get; }
		event EventHandler BoundsChanged;
	}

}