namespace FileChecks.Models
{
    public class FileItemViewModel
    {
        public FileItemViewModel(string name, string fullName, bool isContainer, long size, DateTime lastModified, byte[] hash)
        {
            Name = name;
            FullName = fullName;
            IsContainer = isContainer;
            Size = size;
            LastModified = lastModified;
            Hash = hash;
        }

        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public bool IsContainer { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public Byte[] Hash { get; set; }
    }
}
