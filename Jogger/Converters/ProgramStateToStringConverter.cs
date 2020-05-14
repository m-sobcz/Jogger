using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Jogger.Converters
{
    class ProgramStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string msg = "Stan pracy: ";
            switch (((ProgramState)value))
            {
                case ProgramState.NotInitialized:
                    msg += "Nie zainicjalizowany";
                    break;
                case ProgramState.Initializing:
                    msg += "Inicjalizowanie...";
                    break;
                case ProgramState.Initialized:
                    msg += "Zainicjalizowany";
                    break;
                case ProgramState.Idle:
                    msg += "Gotowy";
                    break;
                case ProgramState.Starting:
                    msg += "Uruchamianie...";
                    break;
                case ProgramState.Started:
                    msg += "Uruchomiony";
                    break;
                case ProgramState.Stopping:
                    msg += "Zatrzymywanie...";
                    break;
                case ProgramState.Done:
                    msg += "Zakończono";
                    break;
                case ProgramState.Error:
                    msg += "Błąd !";
                    break;
            }
            return msg;
        }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
    }
}