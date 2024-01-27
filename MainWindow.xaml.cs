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

        private void LeftComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            DriveInfo selectedItem = (DriveInfo)comboBox.SelectedItem;
            string path = selectedItem.Name;
            Label pathLabel = (Label)leftPathLabel;
            ListView directoryView = (ListView)leftListView;

            if (selectedItem != null && selectedItem.IsReady)
            {
                directoryView.Items.Clear();
                DirectoryInfo rootDirectory = selectedItem.RootDirectory;
                FileSystemInfo[] items = rootDirectory.GetFileSystemInfos();

                foreach (FileSystemInfo item in items)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Content = new
                    {
                        Name = item.Name,
                        Type = item is DirectoryInfo ? "Folder" : "File",
                        Size = (item is FileInfo file) ? GetFileSizeString(file.Length) : ""
                    };
                    directoryView.Items.Add(listViewItem);
                }
            }

            if (pathLabel != null)
            {
                leftCurrentPath = path;
                pathLabel.Content = leftCurrentPath;
                //add to queue
            }
        }

        private void RightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            DriveInfo selectedItem = (DriveInfo)comboBox.SelectedItem;
            string path = selectedItem.Name;
            Label pathLabel = (Label)rightPathLabel;
            ListView directoryView = (ListView)rightListView;

            if (selectedItem != null && selectedItem.IsReady)
            {
                directoryView.Items.Clear();
                DirectoryInfo rootDirectory = selectedItem.RootDirectory;
                FileSystemInfo[] items = rootDirectory.GetFileSystemInfos();

                foreach (FileSystemInfo item in items)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Content = new
                    {
                        Name = item.Name,
                        Type = item is DirectoryInfo ? "Folder" : "File",
                        Size = (item is FileInfo file) ? GetFileSizeString(file.Length) : ""
                    };
                    directoryView.Items.Add(listViewItem);
                }
            }

            if (pathLabel != null)
            {
                rightCurrentPath = path;
                pathLabel.Content = rightCurrentPath;
                //add to queue
            }
        }

        private void LeftListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Label pathLabel = (Label)leftPathLabel;

            if (leftListView.SelectedItem != null)
            {
                dynamic selectedItem = ((ListViewItem)leftListView.SelectedItem).Content;

                if (selectedItem.Type == "Folder")
                {
                    string subFolderPath = System.IO.Path.Combine(leftCurrentPath, selectedItem.Name);

                    leftListView.Items.Clear();
                    try
                    {
                        DirectoryInfo folder = new DirectoryInfo(subFolderPath);
                        FileSystemInfo[] items = folder.GetFileSystemInfos();

                        foreach (FileSystemInfo item in items)
                        {
                            ListViewItem listViewItem = new ListViewItem();
                            listViewItem.Content = new
                            {
                                Name = item.Name,
                                Type = item is DirectoryInfo ? "Folder" : "File",
                                Size = (item is FileInfo file) ? GetFileSizeString(file.Length) : ""
                            };
                            leftListView.Items.Add(listViewItem);
                        }

                        leftCurrentPath = subFolderPath;
                        if (pathLabel != null)
                        {
                            pathLabel.Content = subFolderPath;
                        }
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                }
                else if (selectedItem.Type == "File")
                {
                    string filePath = System.IO.Path.Combine(leftCurrentPath, selectedItem.Name);
                    try
                    {
                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(psi);
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                }
            }
        }

        private void RightListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Label pathLabel = (Label)rightPathLabel;

            if (rightListView.SelectedItem != null)
            {
                dynamic selectedItem = ((ListViewItem)rightListView.SelectedItem).Content;

                if (selectedItem.Type == "Folder")
                {
                    string subFolderPath = System.IO.Path.Combine(rightCurrentPath, selectedItem.Name);

                    rightListView.Items.Clear();
                    try
                    {
                        DirectoryInfo folder = new DirectoryInfo(subFolderPath);
                        FileSystemInfo[] items = folder.GetFileSystemInfos();

                        foreach (FileSystemInfo item in items)
                        {
                            ListViewItem listViewItem = new ListViewItem();
                            listViewItem.Content = new
                            {
                                Name = item.Name,
                                Type = item is DirectoryInfo ? "Folder" : "File",
                                Size = (item is FileInfo file) ? GetFileSizeString(file.Length) : ""
                            };
                            rightListView.Items.Add(listViewItem);
                        }

                        rightCurrentPath = subFolderPath;
                        if (pathLabel != null)
                        {
                            pathLabel.Content = subFolderPath;
                        }
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                }
                else if (selectedItem.Type == "File")
                {
                    string filePath = System.IO.Path.Combine(rightCurrentPath, selectedItem.Name);
                    try
                    {
                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(psi);
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                }
            }
        }

    }
}