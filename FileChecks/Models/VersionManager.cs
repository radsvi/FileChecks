using FileChecks.ViewModels;

namespace FileChecks.Models
{
    public class VersionManager
    {
        string _rootPath;
        public string Vystup { get; private set; } = string.Empty;

        public VersionManager(string folderPath)
        {
#warning dodelat folderPath
            _rootPath = "C:\\MyFolder";
        }

        public void ScanFolder()
        {
            Vystup = "sample text";
        }

        public void Browse(string? path)
        {
            var root = _rootPath;
            var safePath = ResolveSafePath(root, path);

            var model = new DirectoryViewModel
            {
                CurrentPath = path ?? "",
                ParentPath = Path.GetDirectoryName(path),

                Directories = Directory.GetDirectories(safePath)
                    .Select(Path.GetFileName)
                    .ToList(),

                Files = Directory.GetFiles(safePath)
                    .Select(f =>
                    {
                        var info = new FileInfo(f);
                        return new FileItemViewModel
                        {
                            Name = info.Name,
                            Size = info.Length,
                            LastModified = info.LastWriteTime
                        };
                    })
                    .ToList()
            };

            
        }
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
