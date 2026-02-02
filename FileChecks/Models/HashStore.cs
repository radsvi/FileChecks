using System.Text.Json;

namespace FileChecks.Models
{
    public interface IHashStore
    {
        void Add(FileVersionInfo file);
        IReadOnlyList<FileVersionInfo> GetAll();
        void Load();
        void Update(FileVersionInfo file);
    }

    public class HashStore : IHashStore
    {
        private readonly object _lock = new();
        private readonly string _filePath;

        private List<FileVersionInfo> Content { get; set; } = new();

        public HashStore(IHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);

            _filePath = Path.Combine(dataDir, "file-hashes.json");
        }
        private void Save()
        {
            var json = JsonSerializer.Serialize(Content);
            File.WriteAllText(_filePath, json);
        }
        public void Load()
        {
            if (!File.Exists(_filePath))
                return;

            var json = File.ReadAllText(_filePath);
            Content = JsonSerializer.Deserialize<List<FileVersionInfo>>(json) ?? new();

        }
        public IReadOnlyList<FileVersionInfo> GetAll()
        {
            lock (_lock)
            {
                return Content.ToList();
            }
        }
        public void Add(FileVersionInfo file)
        {
            lock (_lock)
            {
                Content.Add(file);
                Save();
            }
        }
        public void Update(FileVersionInfo file)
        {
            lock (_lock)
            {
                Content.Add(file);
                Save();
            }
        }
    }
}
