using System.Text.Json;

namespace FileChecks.Models
{
    public interface IHashStore
    {
        IReadOnlyList<IVersionInfo> GetAll();
        IReadOnlyList<IVersionInfo> GetFolder(string path);
        //void Load();
        void UpdateAll(IReadOnlyList<IFileSystemEntry> files, List<string?> checkedFolders);
    }

    public class HashStore : IHashStore
    {
        private readonly object _lock = new();
        private readonly string _filePath;

        private List<IVersionInfo> _content = new();

        public HashStore(IHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);

            _filePath = Path.Combine(dataDir, "file-hashes.json");
        }
        private void SaveUnsafe()
        {
            var json = JsonSerializer.Serialize(_content);
            File.WriteAllText(_filePath, json);
        }
        
        private void Load()
        {
            if (!File.Exists(_filePath))
                return;

            var json = File.ReadAllText(_filePath);
            _content = JsonSerializer.Deserialize<List<IVersionInfo>>(json) ?? new();

        }
        public IReadOnlyList<IVersionInfo> GetAll()
        {
            lock (_lock)
            {
                return _content.ToList();
            }
        }
        public IReadOnlyList<IVersionInfo> GetFolder(string path)
        {
            lock (_lock)
            {
                // tato cast je samozrejme neefektivni, ale predpokladame ze pracujeme maximalne s 100 soubory

                var fullList = _content.Where(entry => entry.Path.Contains(path, StringComparison.OrdinalIgnoreCase));
                var folders = fullList.Where(entry => entry is FolderVersionInfo);
                var files = fullList.Where(entry => entry is FileVersionInfo).OrderBy(e => e.Name);
                var finalList = new List<IVersionInfo>();

                if (VersionManager.RootPath == path)
                    AddFiles(files, finalList, path);

                foreach (var folder in folders)
                {
                    finalList.Add(folder);
                    AddFiles(files, finalList, folder.FullName);
                }

                return finalList.ToList();
            }

            static void AddFiles(IEnumerable<IVersionInfo> files, List<IVersionInfo> finalList, string folderPath)
            {
                foreach (var file in files)
                {
                    if (folderPath == file.Path)
                        finalList.Add(file);
                }
            }
        }
        public void UpdateAll(IReadOnlyList<IFileSystemEntry> files, List<string?> checkedFolders)
        {
            lock (_lock)
            {
                Load();

                foreach (var entry in _content)
                {
                    if (checkedFolders.Contains(entry.Path))
                    {
                        entry.IsPresent = false;
                        entry.IsChanged = false;
                    }
                    entry.IsNewEntry = false;
                }

                foreach (var file in files)
                {
                    UpsertUnsafe(file);
                }

                SaveUnsafe();
            }
        }
        private void UpsertUnsafe(IFileSystemEntry entity)
        {
            var existing = _content.FirstOrDefault(f => f.FullName == entity.FullName);

            if (existing is null)
            {
                IVersionInfo entry;

                if (entity is FSFile file)
                {

                    entry = new FileVersionInfo(
                        file.FullName,
                        file.Name,
                        file.Path,
                        file.LastModified,
                        true,
                        true,
                        file.Size,
                        1,
                        file.Hash);
                }
                else
                {
                    entry = new FolderVersionInfo(
                        entity.FullName,
                        entity.Name,
                        entity.Path,
                        entity.LastModified,
                        true,
                        true);
                }

                _content.Add(entry);
            }
            else
            {
                existing.LastModified = entity.LastModified;
                //existing.Size = entity.Size;
                existing.IsPresent = true;

                if (existing is FileVersionInfo existingEntry && entity is FSFile file)
                {
                    existingEntry.Size = file.Size;

                    if (!existingEntry.Hash.SequenceEqual(file.Hash))
                    {
                        existingEntry.Hash = file.Hash;
                        existingEntry.Version++;
                        existingEntry.IsChanged = true;
                    }
                }

            }
        }
        //public void Update(FileVersionInfo file)
        //{
        //    lock (_lock)
        //    {
        //        Content.Add(file);
        //        Save();
        //    }
        //}
    }
}
