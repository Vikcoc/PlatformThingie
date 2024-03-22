namespace PlatformInterfaces
{
    public interface IMigrationProvider
    {
        public string Name { get; }
        public Task Migrate(string connectionString);
    }
}
