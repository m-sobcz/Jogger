﻿using Jogger.Drivers;
using Jogger.IO;
using Jogger.Models;
using Jogger.Services;
using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Jogger.ViewModels
{
    public class JoggingViewModel : ViewModelBase
    {
        private readonly IDigitalIO digitalIO;
        private readonly IDriver driver;
        readonly ITesterService testerService;
        readonly JoggingModel model = new JoggingModel();
        readonly TestSettings testSettings;
        readonly ConfigurationSettings configurationSettings;
        private readonly IValveManager valveManager;
        ObservableCollection<ValveModel> valveTypes = new ObservableCollection<ValveModel>();
        ValveModel selectedType;
        public ObservableCollection<ValveModel> ValveTypes
        {
            get { return valveTypes; }
            set
            {
                valveTypes = value;
                OnPropertyChanged(nameof(ValveTypes));
            }
        }
        public ValveModel SelectedType
        {
            get { return selectedType; }
            set { selectedType = value; }
        }
        //public string ValveTypes{get;set;}
        private ICommand initializeCommand;
        private ICommand startCommand;
        private ICommand stopCommand;
        private ICommand selectValveType;
        private ProgramState programState;

        public JoggingViewModel(ITesterService testerService, IValveManager valveManager, IDriver driver, TestSettings testSettings, ConfigurationSettings configurationSettings, IDigitalIO digitalIO)
        {
            this.digitalIO = digitalIO;
            this.driver = driver;
            this.testerService = testerService;
            this.testSettings = testSettings;
            this.configurationSettings = configurationSettings;
            this.valveManager = valveManager;
            valveTypes.Add(new ValveModel("", "Sprawdzanie obecności"));
            valveTypes.Add(new ValveModel("2Up", "GM MBM 2UP LIN"));
            valveTypes.Add(new ValveModel("3_5Up", "JLR 3,5UP LIN"));
            valveTypes.Add(new ValveModel("6Up", "GM MBM 6UP LIN"));
            SelectedType = valveTypes[0];
            IsLogInDataSelected = true;
            valveManager.ActiveErrorsChanged += ValveManager_ActiveErrorsChanged;
            valveManager.OccuredErrorsChanged += ValveManager_OccuredErrorsChanged;
            valveManager.ResultChanged += ValveManager_ResultChanged;
            valveManager.CommunicationLogChanged += CommunicationLogChanged;
            testerService.ProgramStateChanged += TesterService_ProgramStateEventHandler_Change;
            driver.CommunicationLogChanged += CommunicationLogChanged;
            digitalIO.CommunicationLogChanged += CommunicationLogChanged;
        }


        public bool IsLogInDataSelected
        {
            get { return testSettings.IsLogInDataSelected; }
            set { testSettings.IsLogInDataSelected = value; valveManager.TestSettings=testSettings; }
        }
        public bool IsLogOutDataSelected
        {
            get { return testSettings.IsLogOutDataSelected; }
            set { testSettings.IsLogOutDataSelected = value; valveManager.TestSettings=testSettings; }
        }
        public bool IsLogTimeoutSelected
        {
            get { return testSettings.IsLogTimeoutSelected; }
            set { testSettings.IsLogTimeoutSelected = value; valveManager.TestSettings=testSettings; }
        }
        public ICommand InitializeCommand
        {
            get
            {
                if (initializeCommand == null)
                {
                    initializeCommand = new RelayCommand(
                        o =>
                        {
                            ActionStatus actionStatus = testerService.Initialize(configurationSettings.HardwareChannelCount);
                            switch (actionStatus)
                            {
                                case ActionStatus.OK:
                                    _=testerService.CommunicationLoop();
                                    break;
                                case ActionStatus.WarningWrongParameter:
                                    showInfo.Show("Zły parametr inicjalizacji", "Ostrzeżenie");
                                    break;
                                case ActionStatus.Error:
                                    showInfo.Show("Inicjalizacja zakończona niepowodzeniem!\n\n" +
                                        "Sprawdź podłączenie urządzeń oraz informacje zawarte w logu na dole ekranu.\n" +
                                        "Aplikacja wymaga ponownego uruchomienia.", "Błąd inicjalizacji");
                                    break;
                            }
                        },
                        o => (testerService.State == ProgramState.NotInitialized)
                        );
                }
                return initializeCommand;
            }
        }
        public ICommand StartCommand
        {
            get
            {
                if (startCommand == null)
                {
                    startCommand = new RelayCommand(
                    o =>
                    {
                        ActionStatus actionStatus = testerService.Start(testSettings, testerService.ValveType);
                        switch (actionStatus)
                        {
                            case ActionStatus.Error:
                                showInfo.Show("Wystąpił problem z załadowaniem wybranego typu zaworu!\n\n" +
                                    "Start testu został zatrzymany.", "Błąd");
                                break;
                            default:
                                break;
                        }
                    },
                    o => (testerService.State == ProgramState.Initialized |
                    testerService.State == ProgramState.Idle |
                    testerService.State == ProgramState.Done)
                    );
                }
                return startCommand;
            }
        }
        public ICommand StopCommand
        {
            get
            {
                if (stopCommand == null)
                {
                    stopCommand = new RelayCommand(
                    o =>
                    {
                        testerService.Stop();
                    },
                    o => testerService.State == ProgramState.Started
                    );
                }
                return stopCommand;
            }
        }
        public ICommand SelectValveType
        {
            get
            {
                if (selectValveType is null)
                {
                    selectValveType = new RelayCommand(
                    o =>
                    {
                        testerService.ValveType = (string)SelectedType.Code;
                    },
                    o => true//testerService.State != ProgramState.NotInitialized
                    );
                }
                return selectValveType;
            }
        }
        public Result Result1
        {
            get { return model.results[0]; }
            set { model.results[0] = value; OnPropertyChanged("Result1"); }
        }
        public Result Result2
        {
            get { return model.results[1]; }
            set { model.results[1] = value; OnPropertyChanged("Result2"); }
        }
        public Result Result3
        {
            get { return model.results[2]; }
            set { model.results[2] = value; OnPropertyChanged("Result3"); }
        }
        public Result Result4
        {
            get { return model.results[3]; }
            set { model.results[3] = value; OnPropertyChanged("Result4"); }
        }
        public string ActiveErrors1
        {
            get { return model.activeErrors[0] ?? "?"; }
            set { model.activeErrors[0] = value; OnPropertyChanged("ActiveErrors1"); }
        }
        public string ActiveErrors2
        {
            get { return model.activeErrors[1] ?? "?"; }
            set { model.activeErrors[1] = value; OnPropertyChanged("ActiveErrors2"); }
        }
        public string ActiveErrors3
        {
            get { return model.activeErrors[2] ?? "?"; }
            set { model.activeErrors[2] = value; OnPropertyChanged("ActiveErrors3"); }
        }
        public string ActiveErrors4
        {
            get { return model.activeErrors[3] ?? "?"; }
            set { model.activeErrors[3] = value; OnPropertyChanged("ActiveErrors4"); }
        }
        public string OccuredErrors1
        {
            get { return model.occuredErrors[0] ?? "?"; }
            set { model.occuredErrors[0] = value; OnPropertyChanged("OccuredErrors1"); }
        }
        public string OccuredErrors2
        {
            get { return model.occuredErrors[1] ?? "?"; }
            set { model.occuredErrors[1] = value; OnPropertyChanged("OccuredErrors2"); }
        }
        public string OccuredErrors3
        {
            get { return model.occuredErrors[2] ?? "?"; }
            set { model.occuredErrors[2] = value; OnPropertyChanged("OccuredErrors3"); }
        }
        public string OccuredErrors4
        {
            get { return model.occuredErrors[3] ?? "?"; }
            set { model.occuredErrors[3] = value; OnPropertyChanged("OccuredErrors4"); }
        }
        public string CommunicationLog
        {
            get { return model.communicationLog ?? "?"; }
            set
            {
                if (model.communicationLog.Length > 10000) model.communicationLog = model.communicationLog.Substring(2000);
                model.communicationLog += value;
                OnPropertyChanged("CommunicationLog");
            }
        }

        private void ValveManager_ResultChanged(int channelNumber, Result result)
        {
            switch (channelNumber)
            {
                case 0:
                    Result1 = result;
                    break;
                case 1:
                    Result2 = result;
                    break;
                case 2:
                    Result3 = result;
                    break;
                case 3:
                    Result4 = result;
                    break;
                default:
                    break;
            }
        }

        private void ValveManager_OccuredErrorsChanged(int channelNumber, string errors)
        {
            switch (channelNumber)
            {
                case 0:
                    OccuredErrors1 = errors;
                    break;
                case 1:
                    OccuredErrors2 = errors;
                    break;
                case 2:
                    OccuredErrors3 = errors;
                    break;
                case 3:
                    OccuredErrors4 = errors;
                    break;
                default:
                    break;
            }
        }

        private void ValveManager_ActiveErrorsChanged(int channelNumber, string errors)
        {
            switch (channelNumber)
            {
                case 0:
                    ActiveErrors1 = errors;
                    break;
                case 1:
                    ActiveErrors2 = errors;
                    break;
                case 2:
                    ActiveErrors3 = errors;
                    break;
                case 3:
                    ActiveErrors4 = errors;
                    break;
                default:
                    break;
            }
        }

        private void CommunicationLogChanged(string log)
        {
            CommunicationLog = log;
        }

        private void TesterService_ProgramStateEventHandler_Change(ProgramState programState)
        {
            ProgramState = programState;
            CommandManager.InvalidateRequerySuggested();
        }
        public ProgramState ProgramState
        {
            get { return programState; }
            set { programState = value; OnPropertyChanged(nameof(ProgramState)); }
        }
    }
}
