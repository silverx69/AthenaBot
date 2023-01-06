using System.Windows.Input;

namespace AthenaBot
{
    /// <summary>
    /// Useful class for ICommand that allows supplying delegates for CanExecute and Execute instead of writing a new class for each Command.
    /// Used in UI's like WinForms and WPF.
    /// </summary>
    public sealed class ModelCommand : ICommand
    {
        readonly Action<object> m_execute;
        readonly Predicate<object> m_canExecute;

        public ModelCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            m_execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return m_canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            m_execute.Invoke(parameter);
        }

        public void Update()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}
