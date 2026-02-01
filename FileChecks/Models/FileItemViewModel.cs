namespace FileChecks.Models
{
    public class FileItemViewModel
    {
        public string Name { get; set; } = "";
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
    }
}
