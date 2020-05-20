using Jogger.Models;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        ITesterService testerService;
        readonly TestSettings testSettings;
        readonly ConfigurationSettings configurationSettings;
        public SettingsViewModel(ITesterService testerService,TestSettings testSettings, ConfigurationSettings configurationSettings)
        {
            this.testerService = testerService;
            this.testSettings = testSettings;
            this.configurationSettings = configurationSettings;
            HardwareChannelCount = 2;
            Repetitions = 1;
            ValveMinInflateTime = 200;
            ValveMinDeflateTime = 200;
            ValveMaxDeflateTime = 30000;
            ValveMaxInflateTime = 30000;
            testerService.ProgramStateChanged += TesterService_ProgramStateEventHandler_Change;
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

    }
}
