using Jogger.IO;
using Jogger.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels
{
    public class DiagnosticsViewModel : ViewModelBase
    {
        ITesterService testerService;
        IDigitalIO digitalIO;
        IOView iOView = new IOView();
        BitArray bitArray;
        public bool IsPreInitialization
        {
            get
            {
                return testerService.State == ProgramState.NotInitialized;
            }
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
        public DiagnosticsViewModel()
        {

        }
        public DiagnosticsViewModel(ITesterService testerService, IDigitalIO digitalIO)
        {
            this.testerService = testerService;
            testerService.ProgramStateChanged += TesterService_ProgramStateEventHandler_Change;
            this.digitalIO = digitalIO;
            for (int i = 0; i < 8; i++)
            {
                iOView[i] = DigitalState.Unknown;
            }
            digitalIO.InputsRead += DigitalIO_InputsRead;
        }
        private void DigitalIO_InputsRead(object sender, string errorCode, byte[] buffer)
        {
            bitArray = new BitArray(buffer);
            for (int i = 0; i < bitArray.Length; i++)
            {
                iOView[i] = bitArray[i] ? DigitalState.Active : DigitalState.Inactive;
            }
            OnPropertyChanged(nameof(DigitalIO));
        }
        private void TesterService_ProgramStateEventHandler_Change(object sender, ProgramState programState)
        {
            OnPropertyChanged(nameof(IsPreInitialization));
        }
    }
}
