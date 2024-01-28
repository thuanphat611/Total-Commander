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
        string leftCurrentPath;
        string rightCurrentPath;
        public MainWindow()
        {
            InitializeComponent();
            leftCurrentPath = "";
            rightCurrentPath = "";
        }

        private string GetFileSizeString(long sizeInBytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (sizeInBytes >= GB)
                return $"{sizeInBytes / GB} GB";
            else if (sizeInBytes >= MB)
                return $"{sizeInBytes / MB} MB";
            else if (sizeInBytes >= KB)
                return $"{sizeInBytes / KB} KB";
            else
                return $"{sizeInBytes} Bytes";
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

        private void LoadDirectory(int position,  string path)
        {
            Label pathLabel = (Label)rightPathLabel;
            if (position == 0)
                leftListView.Items.Clear();
            else
                rightListView.Items.Clear();

            try
            {
                DirectoryInfo folder = new DirectoryInfo(path);
                FileSystemInfo[] items = folder.GetFileSystemInfos();

                foreach (FileSystemInfo item in items)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Content = new
                    {
                        Name = item.Name,
                        Type = item is DirectoryInfo ? "Folder" : "File",
                        Size = (item is FileInfo file) ? GetFileSizeString(file.Length) : "",
                        Date = $"{item.CreationTime.Day}-{item.CreationTime.Month}-{item.CreationTime.Year}"
                    };
                    if (position == 0)
                        leftListView.Items.Add(listViewItem);
                    else
                        rightListView.Items.Add(listViewItem);
                }

                if (position == 0)
                    leftCurrentPath = path;
                else
                    rightCurrentPath = path;
                if (pathLabel != null)
                {
                    pathLabel.Content = path;
                }
            }
            catch (Exception ex)
            {
                //ignore
            }
        }

        private void OpenFile(string path)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                //ignore
            }
        }

        private void LeftComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            DriveInfo selectedItem = (DriveInfo)comboBox.SelectedItem;
            string path = selectedItem.Name;
            LoadDirectory(0, path);
        }

        private void RightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            DriveInfo selectedItem = (DriveInfo)comboBox.SelectedItem;
            string path = selectedItem.Name;
            LoadDirectory(1, path);
        }

        private void LeftListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (leftListView.SelectedItem != null)
            {
                dynamic selectedItem = ((ListViewItem)leftListView.SelectedItem).Content;
                string path = System.IO.Path.Combine(leftCurrentPath, selectedItem.Name);

                if (selectedItem.Type == "Folder")
                    LoadDirectory(0, path);
                else if (selectedItem.Type == "File") 
                    OpenFile(path);
            }
        }

        private void RightListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (rightListView.SelectedItem != null)
            {
                dynamic selectedItem = ((ListViewItem)rightListView.SelectedItem).Content;
                string path = System.IO.Path.Combine(rightCurrentPath, selectedItem.Name);

                if (selectedItem.Type == "Folder")
                    LoadDirectory(1, path);
                else if (selectedItem.Type == "File")
                    OpenFile(path);
            }
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid directoryViews = bodyGrid as Grid;

            if (directoryViews != null)
            {
                GridView leftGrid = leftGridView as GridView;
                GridView rightGrid = rightGridView as GridView;
                double width = directoryViews.ColumnDefinitions[0].ActualWidth;
                double defaultScrollbarWidth = SystemParameters.VerticalScrollBarWidth;
                double columnWidth = (width - defaultScrollbarWidth * 1.5) / 4;
                leftGrid.Columns[0].Width = columnWidth;
                leftGrid.Columns[1].Width = columnWidth;
                leftGrid.Columns[2].Width = columnWidth;
                leftGrid.Columns[3].Width = columnWidth;
                rightGrid.Columns[0].Width = columnWidth;
                rightGrid.Columns[1].Width = columnWidth;
                rightGrid.Columns[2].Width = columnWidth;
                rightGrid.Columns[3].Width = columnWidth;
            }
        }
    }
}