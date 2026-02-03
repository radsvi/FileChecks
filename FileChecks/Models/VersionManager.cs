using FileChecks.ViewModels;
using System.Security.Cryptography;

namespace FileChecks.Models
{
    public class VersionManager
    {
        private readonly IHashStore hashStore;


        public string? SafePath { get; private set; }
        public DirectoryViewModel? Content { get; private set; }
        public string? SubFolderPath { get; set; }
        public IReadOnlyList<IVersionInfo>? StoredVersions { get; set; }
        public List<string?> CheckedFolders { get; private set; } = [];

        public VersionManager(IHashStore hashStore)
        {
            this.hashStore = hashStore;
        }

        public void Start(string? subFolderPath)
        {
            const string rootPath = "C:\\MyFolder";

            SubFolderPath = subFolderPath;

            SafePath = ResolveSafePath(rootPath, SubFolderPath);

            if (!Directory.Exists(SafePath))
                return;

            ScanFolder(SafePath);

            StoreVersions();

            //StoredVersions = hashStore.GetAll();
            StoredVersions = hashStore.GetFolder(SafePath);
        }

        private void StoreVersions()
        {
            if (Content == null) throw new NullReferenceException($"{nameof(Content)} is null");

            this.hashStore.UpdateAll(Content.Files, CheckedFolders);
        }

        public void ScanFolder(string? path)
        {
            if (path == null) throw new NullReferenceException($"{nameof(path)} is null");

            //List<string?> checkedFolders = Directory.GetDirectories(SafePath).Select(Path.GetFullPath).ToList();
            //checkedFolders.Add(SafePath);

            CheckedFolders.Add(path);
            Directory.GetDirectories(path, "*", SearchOption.AllDirectories).ToList().ForEach(f => CheckedFolders.Add(f));

            List<IFileSystemEntry> folders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
                .Select(f =>
                {
                    var info = new FileInfo(f);

                    return new FSFolder(
                        info.Name,
                        info.FullName,
                        info.DirectoryName ?? throw new InvalidOperationException("File has no directory"),
                        info.LastWriteTime,
                        true);
                })
                .Cast<IFileSystemEntry>().ToList();

            List<IFileSystemEntry> files = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                .Select(f =>
                {
                    var info = new FileInfo(f);

                    using var stream = File.OpenRead(info.FullName);
                    byte[] hash = SHA256.HashData(stream);

                    return new FSFile(
                        info.Name,
                        info.FullName,
                        info.DirectoryName ?? throw new InvalidOperationException("File has no directory"),
                        info.LastWriteTime,
                        false,
                        info.Length,
                        hash);
                })
                .Cast<IFileSystemEntry>().ToList();

            IEnumerable<IFileSystemEntry> joinedLists = folders.Concat(files);

            Content = new DirectoryViewModel
            {
                CurrentPath = path ?? "",
                ParentPath = Path.GetDirectoryName(path),

                Files = folders.Concat(files).ToList()


            };


        }
        private static string ResolveSafePath(string root, string? relativePath)
        {
            var combined = Path.Combine(root, relativePath ?? "");
            var fullPath = Path.GetFullPath(combined);
            var fullRoot = Path.GetFullPath(root);

            if (!fullPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Invalid path");

            return fullPath;
        }
    }
}
