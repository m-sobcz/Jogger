using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Jogger.Converters
{
    public class ResultToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Result result = (Result)value;
            String text;
            switch (result)
            {
                case Result.Unused:
                    text = "Nieaktywny";
                    break;
                case Result.Idle:
                    text = "Bezczynny";
                    break;
                case Result.Testing:
                    text = "Testowanie...";
                    break;
                case Result.DoneOk:
                    text = "OK!";
                    break;
                case Result.DoneErrorCriticalCode:
                    text = "Błąd: Krytyczny kod PCB";
                    break;
                case Result.DoneErrorConnection:
                    text = "Błąd: Brak połączenia";
                    break;
                case Result.DoneErrorTimeout:
                    text = "Błąd: Timeout";
                    break;
                case Result.Stopped:
                    text = "Zatrzymany";
                    break;
                default:
                    text = "???";
                    break;
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
