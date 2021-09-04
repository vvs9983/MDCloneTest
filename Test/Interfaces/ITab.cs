using MaterialDesignThemes.Wpf;
using System;
using System.Windows.Input;

namespace Test.Interfaces
{
    public interface ITab
    {
        public string Name { get; set; }
        public PackIconKind IconKind { get; }

        public ICommand CloseCommand { get; }

        public event EventHandler CloseRequested;
    }
}
