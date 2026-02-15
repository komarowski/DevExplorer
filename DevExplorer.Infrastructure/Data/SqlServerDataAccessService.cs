using System.Data;
using Dapper;
using DevExplorer.Domain.Contracts;
using Microsoft.Data.SqlClient;

namespace DevExplorer.Infrastructure.Data;

/// <summary>
/// SQL Server data access service implementation using Dapper.
/// </summary>
public class SqlServerDataAccessService : IDataAccessService
{
    private readonly string _connectionString;

    public SqlServerDataAccessService(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public async Task<List<dynamic>> ExecuteSelectAsync(
        string query,
        object? parameters = null,
        CancellationToken ct = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        var result = await connection.QueryAsync(
            query,
            parameters,
            commandTimeout: 300,
            commandType: CommandType.Text);

        return [.. result];
    }

    public async Task<T?> ExecuteScalarAsync<T>(
        string query,
        object? parameters = null,
        CancellationToken ct = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        var result = await connection.ExecuteScalarAsync<T>(
            query,
            parameters,
            commandTimeout: 300,
            commandType: CommandType.Text);

        return result;
    }
}
