using System.Data;

namespace Domain.Interfaces;
public interface IBulkInsert
{
    Task BulkInsertAsync(string connectionString, DataTable dataTable);
}
