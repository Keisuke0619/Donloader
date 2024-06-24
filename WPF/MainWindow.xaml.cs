using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Loader;
namespace DonderHiroba
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			if(File.Exists("user.dat") == false) { return; }
			var sr = new StreamReader("user.dat");
			this.MailBox.Text = sr.ReadLine();
			int levelBit;
			Int32.TryParse(sr.ReadLine(), out levelBit);
			sr.Close();
			this.levelBit0.IsChecked = (levelBit & 1) == 1;
			this.levelBit1.IsChecked = (levelBit & 2) == 2;
			this.levelBit2.IsChecked = (levelBit & 4) == 4;
			this.levelBit3.IsChecked = (levelBit & 8) == 8;
			this.levelBit4.IsChecked = (levelBit & 16) == 16;
		}


		private void Button_Click(object sender, RoutedEventArgs e)
		{
			int levelBit = 0;
			levelBit |= this.levelBit0.IsChecked.Value ? 1 : 0;
			levelBit |= this.levelBit1.IsChecked.Value ? 2 : 0;
			levelBit |= this.levelBit2.IsChecked.Value ? 4 : 0;
			levelBit |= this.levelBit3.IsChecked.Value ? 8 : 0;
			levelBit |= this.levelBit4.IsChecked.Value ? 16 : 0;

			string[] args =
			{
				this.MailBox.Text,
				this.PassBox.Password,
				levelBit.ToString(),
				this.FileBox.Text
			};
			Hide();
			var sw = new StreamWriter("user.dat", false, Encoding.UTF8);
			sw.WriteLine(this.MailBox.Text);
			sw.Write(levelBit.ToString());
			sw.Close();
			Loader.Load.Main(args);
			//Process.Start("Get.exe", args);
			Close();

		}

	}
}