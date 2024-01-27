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

namespace Assignment01
{
    public partial class MainWindow : Window
    {
        string currentPath;
        public MainWindow()
        {
            InitializeComponent();
            currentPath = "";
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.IsLoaded)
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                comboBox.SelectedIndex = 0;
                foreach (DriveInfo d in allDrives)
                {
                    comboBox.Items.Add(d);
                }
            }
        }


    }
}