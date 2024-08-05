using Domain.Dtos;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImportExcelToDB;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    public MainWindow()
    {
    }

    private void FilesDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        var row = e.Row;
        var file = row.DataContext as ExcelFile;
        switch (file.Status)
        {
            case Status.Error:
                row.Background = Brushes.Red;
                row.Foreground = Brushes.White;
                break;
            case Status.Ready:
                row.Background = Brushes.Yellow;
                break;
            case Status.Imported:
                row.Background = Brushes.Green;
                row.Foreground = Brushes.White;
                break;
        }
    }
}