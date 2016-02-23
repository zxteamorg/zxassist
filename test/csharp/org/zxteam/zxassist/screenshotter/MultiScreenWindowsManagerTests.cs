namespace org.zxteam.zxassist.screenshotter
{
	using System;
	using System.Windows;

	public sealed class MultiScreenWindowsManagerTests : Application
	{
		[STAThread]
		static void Main()
		{
			var instance = new MultiScreenWindowsManagerTests();
			instance.Run();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this.ChangeResolutionTest();
		}

		public void ChangeResolutionTest()
		{
			MultiScreenWindowsManager mgr = new MultiScreenWindowsManager()
			{
				Content = 1,
				ContentStringFormat = "Number {0}"
			};
		}
	}
}