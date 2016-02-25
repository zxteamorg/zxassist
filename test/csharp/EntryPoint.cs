using System;
using System.Windows;

public static class EntryPoint
{
	[STAThread]
	static void Main()
	{
		var instance = new org.zxteam.lib.reusable.wpf.FullScreenWindowTest();
		instance.Run();
	}

}