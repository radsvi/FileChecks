using System.Text.Json;

namespace FileChecks.Models
{
    public interface IHashStore
    {
        IReadOnlyList<FileVersionInfo> GetAll();
        //void Load();
        void Upsert(FileItemViewModel file);
    }

    public class HashStore : IHashStore
    {
        private readonly object _lock = new();
        private readonly string _filePath;

        private List<FileVersionInfo> _content = new();

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
            _content = JsonSerializer.Deserialize<List<FileVersionInfo>>(json) ?? new();

        }
        public IReadOnlyList<FileVersionInfo> GetAll()
        {
            lock (_lock)
            {
                return _content;
            }
        }
        public void Upsert(FileItemViewModel file)
        {
            lock (_lock)
            {
                Load();

                var existing = _content.FirstOrDefault(f => f.FullName == file.FullName);

                if (existing is null)
                {
                    var entry = new FileVersionInfo
                    {
                        FullName = file.FullName,
                        Name = file.Name,
                        Hash = file.Hash,
                        Size = file.Size,
                        LastModified = file.LastModified,
                    };

                    _content.Add(entry);
                }
                else
                {
                    existing.LastModified = file.LastModified;
                    existing.Hash = file.Hash;
                    existing.Size = file.Size;
                    existing.Version++;
                }

                SaveUnsafe();
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
