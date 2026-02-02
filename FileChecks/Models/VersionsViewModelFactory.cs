namespace FileChecks.Models
{
    public interface IVersionsViewModelFactory
    {
        VersionsViewModel Create();
    }
    public class VersionsViewModelFactory : IVersionsViewModelFactory
    {
        private readonly IHashStore _hashStore;

        public VersionsViewModelFactory(IHashStore fileStore)
        {
            _hashStore = fileStore;
        }

        public VersionsViewModel Create()
        {
            return new VersionsViewModel(_hashStore);
        }
    }
}
