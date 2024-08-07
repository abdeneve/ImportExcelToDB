﻿using Domain.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Infrastructure.Context;
using IronXL;
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
        Data.IsBusy = true;

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
            foreach (var file in files)
            {
                WorkBook workBook = WorkBook.Load(file.Path);
                WorkSheet workSheet = workBook.WorkSheets.First();
                DataTable dataTable = workSheet.ToDataTable(true);
                file.RowCount = dataTable.Rows.Count;
                file.DataTable = dataTable;
                file.ValidateHasRows(dataTable);

                long index = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    index++;
                    file.ValidateRow(index, row);
                }
                file.ValidateIsReady();
            }
        }
        await Delay();
        Data.IsBusy = false;
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
