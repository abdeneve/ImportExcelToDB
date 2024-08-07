using System.Windows.Input;

namespace WPF;
class RelayCommand<T> : ICommand
{
    Action<T?> _action;
    Func<T?, bool> _canExecute;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(T? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(T? parameter)
    {
        _action(parameter);
    }

    public bool CanExecute(object? parameter)
    {
        if (TryGetCommandArg(parameter, out T? res))
        {
            return CanExecute(res);
        }
        return false;
    }

    public void Execute(object? parameter)
    {
        if (TryGetCommandArg(parameter, out T? res))
        {
            Execute(res);
        }
    }

    public RelayCommand(Action<T> action, Func<T, bool> canExecute = null)
    {
        _action = action;
        _canExecute = canExecute;
    }

    private static bool TryGetCommandArg(object? parameter, out T? arg)
    {
        if (parameter is null && default(T) is null)
        {
            arg = default;
            return true;
        }

        if (parameter is T argument)
        {
            arg = argument;
            return true;
        }

        arg = default;
        return false;
    }

    public void RaiseCanExecuteChanged()
    {
        if (CanExecuteChanged != null)
            CanExecuteChanged(this, new EventArgs());
    }
}