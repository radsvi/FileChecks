using FileChecks.Models;

namespace FileChecks.ViewModels
{
    public class DirectoryViewModel
    {
        public string CurrentPath { get; set; } = "";
        public string? ParentPath { get; set; }

        public IReadOnlyList<string?> Directories { get; set; }
            = [];

        public IReadOnlyList<IFileSystemEntry> Files { get; set; }
            = Array.Empty<IFileSystemEntry>();
    }
}
