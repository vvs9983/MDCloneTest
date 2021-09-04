using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Test.Interfaces;
using Test.Models;
using Test.Services;
using Test.ViewModels;
using Test.Views;

namespace Test
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IServiceProvider serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<PopupViewModel>();

            services.AddSingleton(s => new MainWindow
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            services.AddTransient<ICsvFileService, CsvFileService>();

            services.AddTransient(s => new MailTab { Name = "Mail" });
            services.AddTransient(s => new DataTableTab { Name = "DataTable" });
            services.AddTransient<ITabResolver, TabResolver>();

            services.AddTransient(s => new PopupView { DataContext = s.GetRequiredService<PopupViewModel>() });
            services.AddTransient(s => new CredentialsView { DataContext = s.GetRequiredService<MainViewModel>() });

            serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = serviceProvider.GetRequiredService<MainWindow>();

            MainWindow.Show();
        }
    }
}
