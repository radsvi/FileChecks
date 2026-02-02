using System.Text.Json;

namespace FileChecks.Models
{
    public interface IHashStore
    {
        IReadOnlyList<IVersionInfo> GetAll();
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

                if (existing is FileVersionInfo existingEntry && entity is FileVersionInfo file)
                {
                    existingEntry.Size = file.Size;

                    if (!existingEntry.Hash.SequenceEqual(file.Hash))
                    {
                        existingEntry.Hash = file.Hash;
                        existingEntry.Version++;
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
