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
        public List<FileVersionInfo>? StoredVersions { get; set; }

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

            hashStore.GetAll();
        }

        private void StoreVersions()
        {

            // Content
            if (Content == null) throw new NullReferenceException($"{nameof(Content)} is null");

            foreach (var entry in Content.Files)
            {
                this.hashStore.Upsert(entry);
            }
        }

        public void ScanFolder(string? path)
        {
            if (_rootPath == null) throw new NullReferenceException($"{nameof(_rootPath)} is null");

            SafePath = ResolveSafePath(_rootPath, path);

            Content = new DirectoryViewModel
            {
                CurrentPath = path ?? "",
                ParentPath = Path.GetDirectoryName(path),

                Directories = Directory.GetDirectories(SafePath, "*", SearchOption.AllDirectories)
                    .ToList(),

                Files = Directory.GetFiles(SafePath, "*", SearchOption.AllDirectories)
                    .Select(f =>
                    {
                        var info = new FileInfo(f);
                        return new FileItemViewModel
                        {
                            Name = info.Name,
                            FullName = info.FullName,
                            Size = info.Length,
                            LastModified = info.LastWriteTime,
                            Hash = SHA256.HashData(File.OpenRead(info.FullName))
                        };
                    })
                    .ToList()
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
