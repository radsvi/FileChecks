namespace FileChecks.Models
{
    public class FileVersionInfo
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public int Version { get; set; } = 1;
        public HashCode? Hash { get; set; }
        public bool Deleted { get; set; }
    }
}