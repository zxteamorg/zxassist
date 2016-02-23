using System.Windows;
using System.Windows.Controls;

namespace org.zxteam.zxassist.Settings
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).Visibility = System.Windows.Visibility.Collapsed;
		}
	}
}
