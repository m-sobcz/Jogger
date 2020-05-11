using Jogger.IO;
using Jogger.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels
{
    public class DiagnosticsViewModel : ObservedObject
    {
        ITesterService testerService;
        IOView iOView = new IOView();
        public ShowInfo showInfo = new ShowInfo();
        BitArray bitArray;
        public DiagnosticsViewModel()
        {

        }
        public DiagnosticsViewModel(ITesterService testerService)
        {
            this.testerService = testerService;
            testerService.DigitalIOChanged += TesterService_DigitalIOChanged;
            for (int i = 0; i < 8; i++)
            {
                iOView[i] = DigitalState.Unknown;
            }
            testerService.ProgramStateChanged += TesterService_ProgramStateEventHandler_Change;
        }
        private void TesterService_ProgramStateEventHandler_Change(object sender, ProgramState programState)
        {
            OnPropertyChanged(nameof(IsPreInitialization));
        }
        public bool IsPreInitialization
        {
            get
            {
                return testerService.State == ProgramState.NotInitialized;
            }
        }
        private void TesterService_DigitalIOChanged(object sender, byte[] data)
        {
            bitArray = new BitArray(data);
            for (int i = 0; i < bitArray.Length; i++)
            {
                iOView[i] = bitArray[i] ? DigitalState.Active : DigitalState.Inactive;
            }
            OnPropertyChanged(nameof(DigitalIO));
        }
        public IOView DigitalIO
        {
            get
            {
                return iOView;
            }
            set
            {
                iOView = value;
                OnPropertyChanged(nameof(IOView));
            }
        }
    }
}
