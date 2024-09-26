using Domain.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Windows;
using WPF.Extensions;

namespace WPF;
class MainWindowViewModel
{
    public MainDataModel Data { get; set; }

    internal static ApplicationDbContext _context;
    internal static IConfiguration _configuration { get; private set; }
    internal static string _connectionString { get; private set; }

    private const int BatchSize = 1000;

    #region Commands

    public RelayCommand<object> OpenFolderCommand { get; }
    public RelayCommand<object> ShowErrorsCommand { get; }
    public RelayCommand<object> ImportCommand { get; }

    #endregion

    public MainWindowViewModel()
    {
        Data = new();
        Data.OpenFolderCommand = new RelayCommand<object>(OpenFolderCommandExecute, CanOpenFolderCommandExecute);
        Data.ImportCommand = new RelayCommand<object>(ImportCommandExecute, CanImportCommandExecute);
        Data.ShowErrorsCommand = new RelayCommand<object>(ShowErrorsCommandExecute);

        InitDataContext();
        InitData();
    }

    private void InitDataContext()
    {
        _configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

        _connectionString = _configuration.GetConnectionString("App");

        if (!string.IsNullOrEmpty(_connectionString))
            _context = new ApplicationDbContext(_connectionString);
    }

    private void InitData()
    {
        Data.Countries = new(_context.Countries.ToList());
        Data.Customers = new(_context.Customers.ToList());
    }

    private async void OpenFolderCommandExecute(object obj)
    {
        try
        {
            Data.IsBusy = true;
            await OpenFileAsync();
            Data.IsBusy = false;
        }
        catch (Exception ex)
        {
            Data.IsBusy = false;
            if (!ex.Message.Contains("La solicitud fue abortada: La solicitud fue cancelada."))
            {
                MessageBox.Show("Error inesperado: " + ex.Message,
                    "ImportExcelToDB", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
    }

    private async Task OpenFileAsync()
    {
        OpenFolderDialog openFolderDialog = new OpenFolderDialog()
        {
            Title = "Select folder to open ...",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };

        openFolderDialog.Multiselect = false;

        if (openFolderDialog.ShowDialog() == true)
        {
            Data.FolderPath = openFolderDialog.FolderName;

            List<string> xlsxFiles = Directory.GetFiles(openFolderDialog.FolderName, "*.xlsx", SearchOption.TopDirectoryOnly).ToList();

            Data.IsInvalid = false;
            Data.Files.Clear();
            foreach (string fileName in xlsxFiles)
            {
                var name = fileName.Replace($"{openFolderDialog.FolderName}\\", "");
                var file = new ExcelFile(name, fileName);
                file.ValidateDateInFileName();
                file.ValidateIsImported(_context.Sales.AsQueryable(), Data.SelectedCountry, Data.SelectedCustomer);
                Data.Files.Add(file);
            }

            var files = Data.Files.Where(p => p.IsValid());
            ProcessFiles(files);
        }
        await Delay();
    }

    private void ProcessFiles(IEnumerable<ExcelFile> files)
    {
        foreach (var file in files)
        {
            var dataTable = ExcelExtension.ReadExcelFile(file.Path);
            file.RowCount = dataTable.Rows.Count;
            file.DataTable = new DataTable();
            file.DataTable = dataTable;

            for (int startRow = 1; startRow < file.RowCount; startRow += BatchSize)
            {
                int endRow = Math.Min(startRow + BatchSize, file.RowCount);
                ValidateBatch(file, startRow, endRow);
            }

            file.ValidateHasRows();
            file.ValidateIsReady();
        }
    }

    private void ValidateBatch(ExcelFile file, int startRow, int endRow)
    {
        Parallel.For(startRow + 1, endRow, index =>
        {
            DataRow row = file.DataTable.Rows[index - 1];
            file.ValidateRow(index, row);
        });
    }

    private bool CanOpenFolderCommandExecute(object obj)
    {
        return Data.CanOpenFolder();
    }

    private static async Task Delay()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(300));
    }

    private void ShowErrorsCommandExecute(object obj)
    {
        var messages = string.Join("\n", Data.SelectedFile.Erros);

        if (string.IsNullOrEmpty(messages))
            return;

        MessageBox.Show($"{Data.SelectedFile.Name}\n\n" + messages, "Info");
    }

    private async void ImportCommandExecute(object obj)
    {
        string question = "¿Desea importar con los siguientes datos?\n\n";
        question += $"País: {Data.SelectedCountry.Name}\nCliente: {Data.SelectedCustomer.DisplayName}";

        if (MessageBox.Show(question, "Import",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            Data.IsBusy = true;

            var files = Data.Files.Where(p => p.Status == Status.Ready);
            foreach (var file in files)
            {
                var sales = new List<Sale>();
                foreach (DataRow row in file.DataTable.Rows)
                {
                    var sale = new Sale();
                    sale.CountryId = Data.SelectedCountry.Id;
                    sale.CustomerId = Data.SelectedCustomer.Id;
                    sale.StoreCustomerCode = "";
                    sale.CustomerItemCode = row[0].ToString();
                    sale.Date = file.Date;
                    sale.Quantity = int.Parse(row[2].ToString());
                    sale.Amount = decimal.Parse(row[3].ToString());

                    sales.Add(sale);
                }

                var dataTable = sales.ToDataTable();
                await BulkInsertAsync(dataTable);
            }

            Data.IsBusy = false;

            if (!Data.ImportFailed)
                MessageBox.Show("¡La importación se ejecutó exitosamente!", "Import", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Ocurrio un error, no se pudo realizar la importación.", "Import error", MessageBoxButton.OK, MessageBoxImage.Error);

        }
    }

    private bool CanImportCommandExecute(object obj)
    {
        return Data.CanImport();
    }

    private async Task BulkInsertAsync(DataTable dataTable)
    {
        try
        {
            var _bulkInsert = new BulkInsert();
            await _bulkInsert.BulkInsertAsync(_connectionString, dataTable);
        }
        catch (Exception)
        {
            Data.ImportFailed = true;
        }
    }
}
