using System.Text.Json.Serialization;

namespace FileChecks.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")] // Optional: customizes the key name
    [JsonDerivedType(typeof(FileVersionInfo), typeDiscriminator: "file")]
    [JsonDerivedType(typeof(FolderVersionInfo), typeDiscriminator: "folder")]
    public interface IVersionInfo
    {
        string FullName { get; set; }
        string Name { get; set; }
        DateTime LastModified { get; set; }
        bool IsPresent { get; set; }
        bool IsNewEntry { get; set; }
        bool IsContainer { get; set; }
    }

    public abstract class VersionInfo : IVersionInfo
    {
        public VersionInfo(string fullName, string name, DateTime lastModified, bool isPresent, bool isNewEntry, bool isContainer)
        {
            FullName = fullName;
            Name = name;
            LastModified = lastModified;
            IsContainer = isContainer;
            IsPresent = isPresent;
            IsNewEntry = isNewEntry;
        }

        public string FullName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public bool IsContainer { get; set; }
        public bool IsPresent { get; set; }
        public bool IsNewEntry { get; set; }
    }
    public class FileVersionInfo : VersionInfo
    {
        public FileVersionInfo(string fullName, string name, DateTime lastModified, bool isPresent, bool isNewEntry, bool isContainer, long size, int version, byte[] hash)
            : base (fullName, name, lastModified, isPresent, isNewEntry, isContainer)
        {
            Size = size;
            Version = version;
            Hash = hash;
        }

        public long Size { get; set; }
        public int Version { get; set; } = 1;
        public Byte[] Hash { get; set; }
    }
    public class FolderVersionInfo : VersionInfo
    {
        public FolderVersionInfo(string fullName, string name, DateTime lastModified, bool isPresent, bool isNewEntry, bool isContainer)
            : base(fullName, name, lastModified, isPresent, isNewEntry, isContainer) {}
    }
}