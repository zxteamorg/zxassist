﻿#if MANUAL_TEST
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ZXAssist-ManualTest")]
#endif

namespace org.zxteam.zxassist
{
	using System;

	public static class EntryPoint
	{
		[STAThread]
		public static void Main(string[] args)
		{
			System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			try
			{
				using (var guard = new ProcessSingletonGuard(ProcessSingletonGuard.SCOPE.USER))
				{
					if (guard.IsProcessSingleInstance)
					{
						new App().Run();
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject;
		}
	}
}