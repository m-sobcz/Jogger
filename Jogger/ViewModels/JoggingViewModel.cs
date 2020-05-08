using Jogger.Models;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Jogger.ViewModels
{
    class JoggingViewModel : ObservedObject
    {
        readonly ITesterService testerService;
        readonly JoggingModel model = new JoggingModel();
        readonly TestSettings testSettings;
        readonly ConfigurationSettings configurationSettings;
        public ShowInfo showInfo = new ShowInfo();
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
        public JoggingViewModel(ITesterService testerService)
        {
            this.testerService = testerService;
            valveTypes.Add(new ValveModel("", "Sprawdzanie obecności"));
            valveTypes.Add(new ValveModel("2Up", "GM MBM 2UP LIN"));
            //valveTypes.Add(new ValveModel("3_5Up", "GM 3,5Up"));
            valveTypes.Add(new ValveModel("6Up", "GM MBM 6UP LIN"));
            //valveTypes.Add(new ValveModel("11Up", "GM 11Up"));
            SelectedType = valveTypes[0];
            testSettings = TestSettings.GetActual();
            configurationSettings = ConfigurationSettings.GetActual();
            IsLogInDataSelected = true;
            testerService.ResultChanged += TesterService_ResultEventHandler_Change;
            testerService.ActiveErrorsChanged += TesterService_ActiveErrorsEventHandler_Change;
            testerService.OccuredErrorsChanged += TesterService_OccuredErrorsEventHandler_Change;
            testerService.CommunicationLogChanged += TesterService_CommunicationLogEventHandler_Change;
            testerService.ProgramStateChanged += TesterService_ProgramStateEventHandler_Change;
        }

        public bool IsLogInDataSelected
        {
            get { return testSettings.IsLogInDataSelected; }
            set { testSettings.IsLogInDataSelected = value; }
        }
        public bool IsLogOutDataSelected
        {
            get { return testSettings.IsLogOutDataSelected; }
            set { testSettings.IsLogOutDataSelected = value; }
        }
        public bool IsLogTimeoutSelected
        {
            get { return testSettings.IsLogTimeoutSelected; }
            set { testSettings.IsLogTimeoutSelected = value; }
        }

        private void TesterService_ProgramStateEventHandler_Change(object sender, ProgramState programState)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private void TesterService_CommunicationLogEventHandler_Change(object sender, string log)
        {
            CommunicationLog = log;
        }

        private void TesterService_OccuredErrorsEventHandler_Change(object sender, string errors, int channelNumber)
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

        private void TesterService_ActiveErrorsEventHandler_Change(object sender, string errors, int channelNumber)
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

        private void TesterService_ResultEventHandler_Change(object sender, Result result, int channelNumber)
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

        public ICommand InitializeCommand
        {
            get
            {
                if (initializeCommand == null)
                {
                    initializeCommand = new RelayCommand(
                        o =>
                        {
                            ActionStatus actionStatus = testerService.Initialize(configurationSettings);
                            switch (actionStatus)
                            {
                                case ActionStatus.OK:
                                    break;
                                case ActionStatus.WarningWrongParameter:
                                    showInfo.Show("Zły parametr inicjalizacji", "Ostrzeżenie");
                                    break;
                                case ActionStatus.Error:
                                    showInfo.Show("Inicjalizacja drivera zakończona niepowodzeniem!\n\nSprawdź czy Vector jest podłączony i poprawnie skonfigurowany.\nUruchom aplikację ponownie.", "Błąd");
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
                        ActionStatus actionStatus = ActionStatus.OK;
                        //ActionStatus actionStatus = testerService.Start(testSettings);
                        switch (actionStatus)
                        {
                            case ActionStatus.Error:
                                showInfo.Show("Wystąpił problem z załadowaniem wybranego typu !", "Błąd");
                                break;
                            default:
                                break;
                        }
                    },
                    o => (testerService.State == ProgramState.Initialized | testerService.State == ProgramState.Idle | testerService.State == ProgramState.Done)
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
                        //testerService.Stop();
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
                if (selectValveType == null)
                {
                    selectValveType = new RelayCommand(
                    o =>
                    {
                        testerService.ValveType = ((string)SelectedType.Code);
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
            set { model.communicationLog = value; OnPropertyChanged("CommunicationLog"); }
        }
    }
}
