namespace FileChecks.Models
{
    public class FileVersionInfo
    {
        public FileVersionInfo(string fullName, string name, long size, DateTime lastModified, bool isContainer, byte[] hash, bool isPresent, bool isNewEntry)
        {
            FullName = fullName;
            Name = name;
            Size = size;
            LastModified = lastModified;
            IsContainer = isContainer;
            Hash = hash;
            IsPresent = isPresent;
            IsNewEntry = isNewEntry;
        }

        public string FullName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsContainer { get; set; }
        public Byte[] Hash { get; set; }
        public int Version { get; set; } = 1;
        public bool IsPresent { get; set; }
        public bool IsNewEntry { get; set; }
    }
}