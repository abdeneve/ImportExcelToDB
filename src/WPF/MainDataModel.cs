using Domain.Dtos;
using Domain.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WPF;
class MainDataModel : INotifyPropertyChanged
{

    public RelayCommand<object> OpenFolderCommand { get; set; }
    public RelayCommand<object> ImportCommand { get; set; }
    public RelayCommand<object> ShowErrorsCommand { get; set; }

    #region Data Fields and Properties

    private Country selectedCountry = new();
    public Country SelectedCountry
    {
        get => selectedCountry;
        set
        {
            selectedCountry = value;
            OnPropertyChanged(nameof(SelectedCountry));
            OpenFolderCommand.RaiseCanExecuteChanged();
        }
    }
    public ObservableCollection<Country> Countries { get; set; } = new();

    private Customer selectedCustomer = new();
    public Customer SelectedCustomer
    {
        get => selectedCustomer;
        set
        {
            selectedCustomer = value;
            OnPropertyChanged(nameof(SelectedCustomer));
            OpenFolderCommand.RaiseCanExecuteChanged();
        }
    }
    public ObservableCollection<Customer> Customers { get; set; } = new();

    private string folderPath = "";
    public string FolderPath
    {
        get => folderPath;
        set
        {
            folderPath = value;
            OnPropertyChanged(nameof(FolderPath));
        }
    }

    public ExcelFile SelectedFile { get; set; }

    private ObservableCollection<ExcelFile> _files;
    public ObservableCollection<ExcelFile> Files
    {
        get => _files;
        set
        {
            _files = value;
            OnPropertyChanged(nameof(Files));
        }
    }


    public bool IsInvalid { get; set; } = false;

    private bool isBusy = false;
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            isBusy = value;
            OnPropertyChanged(nameof(IsBusy));
            ImportCommand.RaiseCanExecuteChanged();
        }
    }

    public bool ImportFailed { get; set; } = false;

    #endregion

    public MainDataModel() => _files = new();

    internal bool CanOpenFolder()
    {
        return SelectedCountry?.Id > 0 && SelectedCustomer?.Id > 0;
    }

    internal bool CanImport()
    {
        return Files.Any(p => p.Status == Domain.Enums.Status.Ready);
    }

    #region OnPropertyChanged

    private void OnPropertyChanged(string v)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));    
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion
}
