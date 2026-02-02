namespace FileChecks.Models
{
    public class FileVersionInfo
    {
        public FileVersionInfo(string fullName, string name, long size, DateTime lastModified, byte[] hash, bool isPresent, bool newEntry)
        {
            FullName = fullName;
            Name = name;
            Size = size;
            LastModified = lastModified;
            Hash = hash;
            IsPresent = isPresent;
            NewEntry = newEntry;
        }

        public string FullName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public Byte[] Hash { get; set; }
        public int Version { get; set; } = 1;
        public bool IsPresent { get; set; }
        public bool NewEntry { get; set; }
    }
}