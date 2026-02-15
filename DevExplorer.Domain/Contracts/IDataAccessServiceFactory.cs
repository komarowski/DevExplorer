namespace DevExplorer.Domain.Contracts;

public interface IDataAccessServiceFactory
{
    IDataAccessService CreateService(string connectionString);
}
