using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Data;
using System.Windows.Input;

namespace Test.Models
{
    public class DataTableTab : Tab
    {
        private DataTable dataTable;
        private string filePath;

        public DataTableTab()
        {
            ImportCommand = new ActionCommand(() => ImportRequested?.Invoke(this, EventArgs.Empty));
        }

        public override PackIconKind IconKind => PackIconKind.Table;

        public DataTable DataTable
        {
            get => dataTable;
            set
            {
                dataTable = value;
                OnPropertyChanged(nameof(DataTable));
            }
        }

        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        public ICommand ImportCommand { get; }

        public EventHandler ImportRequested;
    }
}
