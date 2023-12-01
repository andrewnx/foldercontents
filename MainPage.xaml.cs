using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Storage;

namespace foldercontents
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<string> FolderContents { get; set; }
        private string? _currentPath;
        private const int MaxDepth = 5; // Maximum depth for directory traversal

        private bool _isBusy;
        public new bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private string? _folderContentsText;
        public string FolderContentsText
        {
            get => _folderContentsText;
            set
            {
                _folderContentsText = value;
                OnPropertyChanged(nameof(FolderContentsText));
            }
        }


        public MainPage()
        {
            InitializeComponent();
            FolderContents = new ObservableCollection<string>();
            // _currentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // UpdateFolderContents(_currentPath);
            BindingContext = this;
        }

        private async void OnSelectFolderClicked(object sender, EventArgs e)
        {
            try
            {
                string selectedFolderPath = await PickFolderAsync();
                if (!string.IsNullOrEmpty(selectedFolderPath))
                {
                    _currentPath = selectedFolderPath; // Ensure _currentPath is assigned before usage
                    UpdateFolderContents(_currentPath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error selecting folder: {ex.Message}", "OK");
            }
        }
        private static async Task<string> PickFolderAsync()
        {
            try
            {
                var folderResult = await FolderPicker.Default.PickAsync();
                if (folderResult != null && folderResult.Folder != null)
                {
                    return folderResult.Folder.Path;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error picking folder: {ex.Message}");
                return string.Empty;
            }
        }

        private void UpdateFolderContents(string path)
        {
            IsBusy = true;
            FolderContents.Clear();
            if (path != null) // Check for null before calling BuildFolderTree
            {
                BuildFolderTree(path, 0);
            }
            FolderContentsText = string.Join("\n", FolderContents);
            IsBusy = false;
        }

        private void BuildFolderTree(string path, int depth)
        {
            if (depth > MaxDepth) return; // Limit the recursion depth

            // Skip the "node_modules" and ".git" folders
            string currentDirName = Path.GetFileName(path);
            if (currentDirName.Equals("node_modules", StringComparison.OrdinalIgnoreCase) ||
                currentDirName.Equals(".git", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get directories
            foreach (var dir in Directory.GetDirectories(path))
            {
                string subDirName = Path.GetFileName(dir); // Use a different variable name
                FolderContents.Add(GetFormattedName("Dir: " + subDirName, depth));
                BuildFolderTree(dir, depth + 1); // Recursively build tree for subdirectories
            }

            // Get files in the current directory
            if (depth > 0) // Only add files if it's not the root directory
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    var fileName = Path.GetFileName(file);
                    FolderContents.Add(GetFormattedName("File: " + fileName, depth));
                }
            }
        }

        private static string GetFormattedName(string name, int depth)
        {
            var prefix = new String(' ', depth * 4); // 4 spaces for each level of depth
            return prefix + name;
        }

        private void OnCopyClicked(object sender, EventArgs e)
        {
            var textToCopy = string.Join("\n", FolderContents);
            Clipboard.SetTextAsync(textToCopy);
        }

    }
}
