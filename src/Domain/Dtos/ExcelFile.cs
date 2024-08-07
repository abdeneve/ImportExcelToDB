using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using System.Data;
using System.Globalization;

namespace Domain.Dtos;
public class ExcelFile
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public Status Status { get; set; }
    public string Path { get; set; }
    public Int64 RowCount { get; set; }

    public List<string> Erros { get; set; } = new();
    public DataTable DataTable { get; set; }

    public ExcelFile(string name, string path)
    {
        Name = name;
        Path = path;
        Status = Status.Error;
    }

    public void ValidateDateInFileName()
    {
        var dateString = Name.ExtractDate();
        if (DateTime.TryParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data))
            Date = data;
        else
            Erros.Add("El nombre del archivo no tiene fecha válida!");
    }

    public void ValidateIsImported(IQueryable<Sale> sales, Country country, Customer customer)
    {
        var exists = sales.Any(s =>
            s.CountryId == country.Id &&
            s.CustomerId == customer.Id &&
            s.Date.Date == Date.Date);

        if (exists)
        {
            Status = Status.Imported;
            Erros.Add($"Ya existen ventas para País: {country.Name}, Cliente: {customer.DisplayName}, Fecha: {Date.Date.ToString("dd/MM/yyyy")}");
        }
    }

    public void ValidateHasRows(DataTable dataTable)
    {
        if (dataTable.Rows.Count == 0)
            Erros.Add("El archivo no tiene filas!");
    }

    public async void ValidateRow(long index, DataRow row)
    {
        ValidateSKU(index, row);
        ValidateQuantity(index, row);
        ValidateAmount(index, row);
    }

    private void ValidateSKU(long index, DataRow row)
    {
        if (string.IsNullOrEmpty(row[0].ToString()))
            Erros.Add($"Fila: {index}, SKU no puede ser vacio!");
    }

    private void ValidateQuantity(long index, DataRow row)
    {
        if (string.IsNullOrEmpty(row[2].ToString()))
            Erros.Add($"Fila: {index}, Venta Unidad no puede ser vacio!");

        if (!Int64.TryParse(row[2].ToString(), out Int64 quantity))
            Erros.Add($"Fila: {index}, Venta Unidad no es numérico!");
    }

    private void ValidateAmount(long index, DataRow row)
    {
        if (string.IsNullOrEmpty(row[3].ToString()))
            Erros.Add($"Fila: {index}, Venta Neta no puede ser vacio!");

        if (!decimal.TryParse(row[3].ToString(), out decimal amount))
            Erros.Add($"Fila: {index}, Venta Neta no es numérico!");
    }

    public void ValidateIsReady()
    {
        if (Erros.Count() == 0)
            Status = Status.Ready;
    }

    public bool IsValid() => Erros.Count() == 0 && Status != Status.Imported;
    public bool IsInvalid() => !IsValid();
}
