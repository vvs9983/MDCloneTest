using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Test.Interfaces;
using Test.Models;
using Test.Views;

namespace Test.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ITabResolver tabResolver;

        private ICommand addDataTabCommand;
        private ICommand addMailTabCommand;
        
        private ObservableCollection<ITab> tabs;
        private ITab selectedTab;
        private ICsvFileService csvFileService;
        private PopupView popup;

        private string email;

        private MailMessage mailMessage;
        private SmtpClient smtpClient;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            tabResolver = serviceProvider.GetRequiredService<ITabResolver>();

            tabs = new ObservableCollection<ITab>();
            tabs.CollectionChanged += TabsCollectionChanged;

            csvFileService = serviceProvider.GetRequiredService<ICsvFileService>();
            popup = serviceProvider.GetRequiredService<PopupView>();
        }

        public ICommand AddDataTabCommand => addDataTabCommand ??= new ActionCommand(AddDataTableTab);

        public ICommand AddMailTabCommand => addMailTabCommand ??= new ActionCommand(AddMailTab);

        private void AddMailTab()
        {
            tabs.Add(tabResolver.Resolve("Mail"));
            SelectedTab = tabs.Last();
        }

        private void AddDataTableTab()
        {
            tabs.Add(tabResolver.Resolve("DataTable"));
            SelectedTab = tabs.Last();
        }

        private void TabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ((ITab)e.NewItems[0]).CloseRequested += OnTabCloseRequested;

                    if (e.NewItems[0] is MailTab newMailTab)
                    {
                        newMailTab.SendRequested += OnSendRequested;
                    }
                    else if (e.NewItems[0] is DataTableTab newDataTableTab)
                    {
                        newDataTableTab.ImportRequested += OnImportRequested;
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    ((ITab)e.OldItems[0]).CloseRequested -= OnTabCloseRequested;

                    if (e.OldItems[0] is MailTab oldMailTab)
                    {
                        oldMailTab.SendRequested += OnSendRequested;
                    }
                    else if (e.OldItems[0] is DataTableTab oldDataTableTab)
                    {
                        oldDataTableTab.ImportRequested -= OnImportRequested;
                    }

                    break;
            }
        }

        public override void Dispose()
        {
            smtpClient?.Dispose();
            mailMessage?.Dispose();

            Tabs.CollectionChanged -= TabsCollectionChanged;
            Tabs.Clear();

            base.Dispose();
        }

        private void OnTabCloseRequested(object sender, EventArgs e) => Tabs.Remove((ITab)sender);

        private async void OnSendRequested(object sender, EventArgs e)
        {
            var mailTab = sender as MailTab;

            if (!await IsInputValid(mailTab)) return;

            Email = mailTab.Sender;

            var passwordBox = await DialogHost.Show(serviceProvider.GetRequiredService<CredentialsView>(), "RootDialog") as PasswordBox;

            string res = ValidateEmail(Email);
            if (res != null)
            {
                ((PopupViewModel)popup.DataContext).Message = res;
                await DialogHost.Show(serviceProvider.GetRequiredService<PopupView>(), "RootDialog");
                return;
            }

            mailMessage = new MailMessage(mailTab.Sender, mailTab.Recipient, mailTab.Subject, mailTab.Body);
            
            smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Email, passwordBox.Password)
            };

            smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            smtpClient.SendAsync(mailMessage, mailTab.GetHashCode());
        }

        private async void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            var token = (int)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            else if (e.Error != null)
            {
                ((PopupViewModel)popup.DataContext).Message = $"[{token}] {e.Error.Message}";
                await DialogHost.Show(serviceProvider.GetRequiredService<PopupView>(), "RootDialog");
            }
            else
            {
                ((PopupViewModel)popup.DataContext).Message = "Email sent successfully!";
                await DialogHost.Show(serviceProvider.GetRequiredService<PopupView>(), "RootDialog");
            }

            ((SmtpClient)sender).SendCompleted -= SendCompletedCallback;
        }

        private async Task<bool> IsInputValid(MailTab sender)
        {
            var res = ValidateEmail(sender.Sender);

            if (res != null)
            {
                ((PopupViewModel)popup.DataContext).Message = "Invalid sender email address entered.";
                await DialogHost.Show(serviceProvider.GetRequiredService<PopupView>(), "RootDialog");
                return false;
            }

            res = ValidateEmail(sender.Recipient);
            if (res != null)
            {
                ((PopupViewModel)popup.DataContext).Message = "Invalid recipient email address entered.";
                await DialogHost.Show(serviceProvider.GetRequiredService<PopupView>(), "RootDialog");
                return false;
            }

            return true;
        }

        private string ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace((email ?? "").ToString())
                || !Regex.IsMatch(email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                return "Invalid email address entered.";
            }

            return null;
        }

        private async void OnImportRequested(object sender, EventArgs e)
        {
            try
            {
                (sender as DataTableTab).FilePath = string.Empty;

                OpenFileDialog fileDialog = new();

                if (fileDialog.ShowDialog() == true)
                {
                    using var dataTable = await csvFileService.LoadCsvFileAsync(fileDialog.FileName);

                    if (dataTable != null)
                    {
                        (sender as DataTableTab).FilePath = fileDialog.FileName;
                        (sender as DataTableTab).DataTable = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                (sender as DataTableTab).FilePath = ex.Message;
            }
        }

        public ObservableCollection<ITab> Tabs
        {
            get => tabs;
            set
            {
                tabs = value;
                OnPropertyChanged(nameof(Tabs));
            }
        }

        public ITab SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
    }
}
