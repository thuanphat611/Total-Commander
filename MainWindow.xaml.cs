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
        List<string> leftHistory;
        List<string> rightHistory;
        int leftHistoryIndex;
        int rightHistoryIndex;
        int lastClicked;

        public MainWindow()
        {
            InitializeComponent();
            leftCurrentPath = "";
            rightCurrentPath = "";
            leftHistory = new List<string>();
            rightHistory = new List<string>();
            leftHistoryIndex = 0;
            rightHistoryIndex = 0;
            lastClicked = 0;
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
            //position: 0:left ListView, 1:right ListView
            Label pathLabel;
            if (position == 0)
            {
                pathLabel = (Label)leftPathLabel;
                leftListView.Items.Clear();
            }
            else
            {
                pathLabel = (Label)rightPathLabel;
                rightListView.Items.Clear();
            }

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

        private void AddToHistory(int position, string path)
        {
            if (position == 0)
                if (leftHistory.Count == 0)
                {
                    leftHistoryIndex = 0;
                }
                else
                {
                    if (leftHistoryIndex == leftHistory.Count - 1)
                    {
                        leftHistoryIndex = leftHistory.Count;
                        leftHistory.Add(path);
                    }
                    else
                    {
                        leftHistory.RemoveRange(leftHistoryIndex + 1, leftHistory.Count - leftHistoryIndex - 1);
                        leftHistory.Add(path);
                        leftHistoryIndex = leftHistory.Count - 1;
                    }
                }
            else
                if (rightHistory.Count == 0)
                {
                    leftHistoryIndex = 0;
                }
                else
                {
                    if (rightHistoryIndex == rightHistory.Count - 1)
                    {
                        rightHistoryIndex = rightHistory.Count;
                        rightHistory.Add(path);
                    }
                    else
                    {
                        rightHistory.RemoveRange(rightHistoryIndex + 1, rightHistory.Count - rightHistoryIndex - 1);
                        rightHistory.Add(path);
                        rightHistoryIndex = rightHistory.Count - 1;
                    }
                }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            DriveInfo selectedItem = (DriveInfo)comboBox.SelectedItem;
            string path = selectedItem.Name;
            if (comboBox.Name == "leftComboBox")
            {
                LoadDirectory(0, path);
                AddToHistory(0, path);
            }
            else
            {
                LoadDirectory(1, path);
                AddToHistory(1, path);
            }
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListView directoryList = (ListView)sender;
            if (directoryList.SelectedItem != null)
            {
                dynamic selectedItem = ((ListViewItem)directoryList.SelectedItem).Content;
                string path;

                if (directoryList.Name == "leftListView")
                    path = System.IO.Path.Combine(leftCurrentPath, selectedItem.Name);
                else
                    path = System.IO.Path.Combine(rightCurrentPath, selectedItem.Name);

                if (selectedItem.Type == "Folder")
                    if (directoryList.Name == "leftListView")
                    {
                        LoadDirectory(0, path);
                        AddToHistory (0, path);
                        lastClicked = 0;
                    }
                    else
                    {
                        LoadDirectory(1, path);
                        AddToHistory(1, path);
                        lastClicked = 1;
                    }
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

        private void ToParent(object sender, RoutedEventArgs e)
        {
            string path;
            if (lastClicked == 0)
                path = leftCurrentPath;
            else 
                path = rightCurrentPath;
            string parentPath = System.IO.Path.GetDirectoryName(path);
            if (parentPath != null)
            {
                LoadDirectory(lastClicked, parentPath);
                AddToHistory(lastClicked, parentPath);
            }  
        }



    }
}