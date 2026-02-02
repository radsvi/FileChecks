namespace FileChecks.Models
{
    public interface IVersionManagerFactory
    {
        VersionManager Create();
    }
    public class VersionManagerFactory : IVersionManagerFactory
    {
        private readonly IHashStore _hashStore;

        public VersionManagerFactory(IHashStore fileStore)
        {
            _hashStore = fileStore;
        }

        public VersionManager Create()
        {
            return new VersionManager(_hashStore);
        }
    }
}
