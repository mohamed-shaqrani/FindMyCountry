namespace Main.Feature.IpGeoLocation.Endpoints.Logs;

public interface IBlockedAttemptStore
{
    void Add(BlockedAttemptLog log);
    IReadOnlyList<BlockedAttemptLog> GetAll();
}