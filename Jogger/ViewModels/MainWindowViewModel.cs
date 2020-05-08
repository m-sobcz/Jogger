using Jogger.Localization;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Jogger.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ITesterService testerService;
        private ICommand closingCommand;
        private ICommand aboutCommand;
        private ICommand helpCommand;
        public MainWindowViewModel(ITesterService testerService)
        {
            this.testerService = testerService;
        }
        public ICommand ClosingCommand
        {
            get
            {
                if (closingCommand == null)
                {
                    closingCommand = new RelayCommand(
                        o =>
                        {
                            //Logger.SaveLogToFile(Logger.CommunicationLog);
                            testerService?.Dispose();
                            Environment.Exit(0);
                        },
                        o => true
                        );
                }
                return closingCommand;
            }
        }
        public ICommand AboutCommand
        {
            get
            {
                if (aboutCommand == null)
                {
                    aboutCommand = new RelayCommand(
                        o =>
                        {
                            string currentVersion = "?";
                            //if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                            //{
                            //    currentVersion = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                            //}
                            //else
                            //{
                            //    currentVersion = "???";
                            //}
                            string applicationInfo = $"-----\nKATester v.{currentVersion}\n-----\n\nProgram do automatycznego joggingu zaworów SMA.\n\nMichał Sobczak\n2020 Kongsberg Automotive";
                            showInfo.Show(applicationInfo, "Informacje o aplikacji");
                        },
                        o => true
                        );
                }
                return aboutCommand;
            }
        }
        public ICommand HelpCommand
        {
            get
            {
                if (helpCommand == null)
                {
                    helpCommand = new RelayCommand(
                        o =>
                        {
                            showInfo.Show("Brak pomocy dla aktualnej wersji aplikacji.", "Pomoc");
                        },
                        o => true
                        );
                }
                return helpCommand;
            }
        }
    }
}
