using FileChecks.ViewModels;
using System.Security.Cryptography;

namespace FileChecks.Models
{
    public class VersionManager
    {
        private readonly IHashStore hashStore;

        string? _rootPath;

        public string? SafePath { get; private set; }
        public DirectoryViewModel? Content { get; private set; }
        public IReadOnlyList<IVersionInfo>? StoredVersions { get; set; }

        public VersionManager(IHashStore hashStore)
        {
            this.hashStore = hashStore;
        }

        public void Start(string folderPath)
        {
#warning dodelat folderPath
            _rootPath = "C:\\MyFolder";

            ScanFolder(string.Empty);

            StoreVersions();

            StoredVersions = hashStore.GetAll();
        }

        private void StoreVersions()
        {
            if (Content == null) throw new NullReferenceException($"{nameof(Content)} is null");

            this.hashStore.UpdateAll(Content.Files);
        }

        public void ScanFolder(string? path)
        {
            if (_rootPath == null) throw new NullReferenceException($"{nameof(_rootPath)} is null");

            SafePath = ResolveSafePath(_rootPath, path);

            List<IFileSystemEntry> folders = Directory.GetDirectories(SafePath, "*", SearchOption.AllDirectories)
                .Select(f =>
                {
                    var info = new FileInfo(f);

                    return new FSFolder(
                        info.Name,
                        info.FullName,
                        info.LastWriteTime,
                        true);
                })
                .Cast<IFileSystemEntry>().ToList();

            List<IFileSystemEntry> files = Directory.GetFiles(SafePath, "*", SearchOption.AllDirectories)
                .Select(f =>
                {
                    var info = new FileInfo(f);

                    using var stream = File.OpenRead(info.FullName);
                    byte[] hash = SHA256.HashData(stream);

                    return new FSFile(
                        info.Name,
                        info.FullName,
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

        //public void Browse(string? path)
        //{
        //    var root = _rootPath;
        //    var safePath = ResolveSafePath(root, path);

        //    var model = new DirectoryViewModel
        //    {
        //        CurrentPath = path ?? "",
        //        ParentPath = Path.GetDirectoryName(path),

        //        Directories = Directory.GetDirectories(safePath)
        //            .Select(Path.GetFileName)
        //            .ToList(),

        //        Files = Directory.GetFiles(safePath)
        //            .Select(f =>
        //            {
        //                var info = new FileInfo(f);
        //                return new FileItemViewModel
        //                {
        //                    Name = info.Name,
        //                    Size = info.Length,
        //                    LastModified = info.LastWriteTime
        //                };
        //            })
        //            .ToList()
        //    };


        //}
        //public IActionResult Browse(string? path)
        //{
        //    var root = _rootPath;
        //    var safePath = ResolveSafePath(root, path);

        //    var model = new DirectoryViewModel
        //    {
        //        CurrentPath = path ?? "",
        //        ParentPath = Path.GetDirectoryName(path),

        //        Directories = Directory.GetDirectories(safePath)
        //            .Select(Path.GetFileName)
        //            .ToList(),

        //        Files = Directory.GetFiles(safePath)
        //            .Select(f =>
        //            {
        //                var info = new FileInfo(f);
        //                return new FileItemViewModel
        //                {
        //                    Name = info.Name,
        //                    Size = info.Length,
        //                    LastModified = info.LastWriteTime
        //                };
        //            })
        //            .ToList()
        //    };

        //    return View(model);
        //}
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
