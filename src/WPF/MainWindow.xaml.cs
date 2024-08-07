using Domain.Dtos;
using Domain.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF;
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