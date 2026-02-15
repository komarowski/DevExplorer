using DevExplorer.Domain.Contracts;

namespace DevExplorer.Infrastructure.Data;

public class DataAccessServiceFactory : IDataAccessServiceFactory
{
    public IDataAccessService CreateService(string connectionString)
    {
        return new SqlServerDataAccessService(connectionString);
    }
}
