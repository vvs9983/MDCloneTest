using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Test.Interfaces;

namespace Test.Models
{
    public abstract class Tab : ITab, INotifyPropertyChanged
    {
        public Tab()
        {
            CloseCommand = new ActionCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; set; }

        public ICommand CloseCommand { get; }

        public abstract PackIconKind IconKind { get; }
        public event EventHandler CloseRequested;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
