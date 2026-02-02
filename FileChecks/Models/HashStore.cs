using System.Text.Json;

namespace FileChecks.Models
{
    public interface IHashStore
    {
        IReadOnlyList<VersionInfo> GetAll();
        //void Load();
        void UpdateAll(IReadOnlyList<FileSystemEntry> files);
    }

    public class HashStore : IHashStore
    {
        private readonly object _lock = new();
        private readonly string _filePath;

        private List<VersionInfo> _content = new();

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
            _content = JsonSerializer.Deserialize<List<VersionInfo>>(json) ?? new();

        }
        public IReadOnlyList<VersionInfo> GetAll()
        {
            lock (_lock)
            {
                return _content.ToList();
            }
        }
        public void UpdateAll(IReadOnlyList<FileSystemEntry> files)
        {
            lock (_lock)
            {
                Load();

                foreach (var entry in _content)
                {
                    entry.IsPresent = false;
                    entry.IsNewEntry = false;
                }

                foreach (var file in files)
                {
                    UpsertUnsafe(file);
                }

                SaveUnsafe();
            }
        }
        private void UpsertUnsafe(IFSEntry file)
        {
            var existing = _content.FirstOrDefault(f => f.FullName == file.FullName);

            if (existing is null)
            {
                var entry = new FileVersionInfo(
                    file.FullName,
                    file.Name,
                    file.Size,
                    file.LastModified,
                    file.isContainer,
                    file.Hash,
                    true,
                    true
                    );

                _content.Add(entry);
            }
            else
            {
                existing.LastModified = file.LastModified;
                existing.Size = file.Size;
                existing.IsPresent = true;

                if (!existing.Hash.SequenceEqual(file.Hash))
                {
                    existing.Hash = file.Hash;
                    existing.Version++;
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
