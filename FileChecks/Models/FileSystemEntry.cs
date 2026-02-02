namespace FileChecks.Models
{
    public interface IFileSystemEntry
    {
        string Name { get; set; }
        string FullName { get; set; }
        string Path { get; set; }
        DateTime LastModified { get; set; }
        bool IsContainer { get; set; }
    }

    public abstract class FileSystemEntry : IFileSystemEntry
    {
        public FileSystemEntry(string name, string fullName, string path, DateTime lastModified, bool isContainer)
        {
            Name = name;
            FullName = fullName;
            Path = path;
            LastModified = lastModified;
            IsContainer = isContainer;
        }

        public string Name { get; set; }
        public string FullName { get; set; }
        public string Path { get; set; }
        public bool IsContainer { get; set; }
        public DateTime LastModified { get; set; }
        
    }
    public class FSFolder : FileSystemEntry
    {
        public FSFolder(string name, string fullName, string path, DateTime lastModified, bool isContainer) : base(name, fullName, path, lastModified, isContainer) {}
    }
    public class FSFile : FileSystemEntry
    {
        public FSFile(string name, string fullName, string path, DateTime lastModified, bool isContainer, long size, byte[] hash) : base(name, fullName, path, lastModified, isContainer)
        {
            Size = size;
            Hash = hash;
        }
        
        public long Size { get; set; }
        public Byte[] Hash { get; set; }
    }
}
