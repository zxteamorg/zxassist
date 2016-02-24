namespace org.zxteam.zxassist.preferences
{
	using System.Windows;
	using System.Windows.Controls;

	public partial class PreferencesWindow : Window
	{
		public PreferencesWindow()
		{
			InitializeComponent();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Visibility = System.Windows.Visibility.Collapsed;
		}
	}
}
