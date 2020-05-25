using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Jogger.Converters
{
    class IntOCToStringOCConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Trace.WriteLine($"targetType is {targetType}");          
            ObservableCollection<int> intCollection = value as ObservableCollection<int>;
            ObservableCollection<string> stringCollection = new ObservableCollection<string>();
            foreach (int i in intCollection) 
            {
                stringCollection.Add(i.ToString());
            }
            return stringCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
  
}
