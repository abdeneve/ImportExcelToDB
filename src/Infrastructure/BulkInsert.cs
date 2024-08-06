using Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure;
public class BulkInsert : IBulkInsert
{
    public async Task BulkInsertAsync(string connectionString, DataTable dataTable)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var bulkCopy = new SqlBulkCopy(connection);

        bulkCopy.DestinationTableName = "Sale";
        bulkCopy.BatchSize = 10000;

        bulkCopy.ColumnMappings.Add("Id", "SaleId");
        bulkCopy.ColumnMappings.Add("CountryId", "CountryId");
        bulkCopy.ColumnMappings.Add("CustomerId", "CustomerId");
        bulkCopy.ColumnMappings.Add("StoreCustomerCode", "StoreCustomerCode");
        bulkCopy.ColumnMappings.Add("CustomerItemCode", "CustomerItemCode");
        bulkCopy.ColumnMappings.Add("Date", "Date");
        bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
        bulkCopy.ColumnMappings.Add("Amount", "Amount");

        try
        {
            await bulkCopy.WriteToServerAsync(dataTable);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
