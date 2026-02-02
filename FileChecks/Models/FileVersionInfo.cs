namespace FileChecks.Models
{
    public class FileVersionInfo
    {
        public string FullName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public int Version { get; set; } = 1;
        public Byte[]? Hash { get; set; }
        public bool Deleted { get; set; }
    }
}