using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Windows.Input;

namespace Test.Models
{
    public class MailTab : Tab
    {
        public MailTab()
        {
            SendCommand = new ActionCommand(() => SendRequested?.Invoke(this, EventArgs.Empty));
        }

        public override PackIconKind IconKind => PackIconKind.EmailEditOutline;
        
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public ICommand SendCommand { get; }
        public EventHandler SendRequested;
    }
}
