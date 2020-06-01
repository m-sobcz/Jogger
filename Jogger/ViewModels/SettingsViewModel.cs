using Jogger.Models;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Jogger.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        ITesterService testerService;
        readonly TestSettings testSettings;
        readonly ConfigurationSettings configurationSettings;
        private ICommand selectBaudrate;
        int selectedBaudrate=9600;
        public SettingsViewModel(ITesterService testerService,TestSettings testSettings, ConfigurationSettings configurationSettings)
        {
            this.testerService = testerService;
            this.testSettings = testSettings;
            this.configurationSettings = configurationSettings;
            HardwareChannelCount = 4;
            Repetitions = 3;
            ValveMinInflateTime = 200;
            ValveMinDeflateTime = 200;
            ValveMaxDeflateTime = 30000;
            ValveMaxInflateTime = 30000;
            Baudrate = 9600;
            testerService.ProgramStateChanged += TesterService_ProgramStateEventHandler_Change;
            baudrateOptions.Add(9600);
            baudrateOptions.Add(10417);
            baudrateOptions.Add(19200);
        }

        ObservableCollection<int> baudrateOptions = new ObservableCollection<int>();
        public ObservableCollection<int> BaudrateOptions
        {
            get { return baudrateOptions; }
            set
            {
                baudrateOptions = value;
                OnPropertyChanged(nameof(BaudrateOptions));
            }
        }

        public int SelectedBaudrate
        {
            get { return selectedBaudrate; }
            set { selectedBaudrate = value; OnPropertyChanged(nameof(SelectedBaudrate)); }
        }
        private void TesterService_ProgramStateEventHandler_Change(object sender, ProgramState programState)
        {
            OnPropertyChanged(nameof(IsPreInitialization));
            OnPropertyChanged("IsInitialized");
        }

        public int Repetitions
        {
            get { return testSettings.Repetitions; }
            set
            {
                testSettings.Repetitions = getParameterInBounds(value, 1, 100) ?? testSettings.Repetitions;
                OnPropertyChanged(nameof(Repetitions));
            }
        }
        public int ValveMinInflateTime
        {
            get { return testSettings.ValveMinInflateTime; }
            set
            {
                testSettings.ValveMinInflateTime = getParameterInBounds(value, 50, 60000) ?? testSettings.ValveMinInflateTime;
                OnPropertyChanged(nameof(ValveMinInflateTime));
            }
        }
        public int ValveMinDeflateTime
        {
            get { return testSettings.ValveMinDeflateTime; }
            set
            {
                testSettings.ValveMinDeflateTime = getParameterInBounds(value, 50, 60000) ?? testSettings.ValveMinDeflateTime;
                OnPropertyChanged(nameof(ValveMinDeflateTime));
            }
        }
        public int ValveMaxInflateTime
        {
            get { return testSettings.ValveMaxInflateTime; }
            set
            {
                testSettings.ValveMaxInflateTime = getParameterInBounds(value, 50, 60000) ?? testSettings.ValveMaxInflateTime;
                OnPropertyChanged(nameof(ValveMaxInflateTime));
            }
        }
        public int ValveMaxDeflateTime
        {
            get { return testSettings.ValveMaxDeflateTime; }
            set
            {
                testSettings.ValveMaxDeflateTime = getParameterInBounds(value, 50, 60000) ?? testSettings.ValveMaxDeflateTime;
                OnPropertyChanged(nameof(ValveMaxDeflateTime));
            }
        }
        public int HardwareChannelCount
        {
            get
            {
                return configurationSettings.HardwareChannelCount;
            }
            set
            {
                configurationSettings.HardwareChannelCount = getParameterInBounds(value, 1, 4) ?? configurationSettings.HardwareChannelCount;
                OnPropertyChanged(nameof(HardwareChannelCount));
            }
        }
        public int Baudrate
        {
            get
            {
                return configurationSettings.Baudrate;
            }
            set
            {
                configurationSettings.Baudrate = getParameterInBounds(value, 1, 64000) ?? configurationSettings.HardwareChannelCount;
                OnPropertyChanged(nameof(Baudrate));
            }
        }
        public bool IsPreInitialization
        {
            get
            {
                return testerService.State == ProgramState.NotInitialized;
            }
        }
        int? getParameterInBounds(int? parameter, int minBound, int maxBound)
        {
            if (parameter > maxBound | parameter < minBound)
            {
                parameter = null;
                showInfo.Show($"Wpisano wartość parametru spoza zakresu ({minBound} - {maxBound})");
            }
            return parameter;
        }

        public ICommand SelectBaudrate
        {
            get
            {
                if (selectBaudrate is null)
                {
                    selectBaudrate = new RelayCommand(
                    o =>
                    {
                        Baudrate = selectedBaudrate;
                        
                    },
                    o => true//testerService.State != ProgramState.NotInitialized
                    );
                }
                return selectBaudrate;
            }
        }


    }
}
