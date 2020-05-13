using Jogger.Services;
using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jogger.Converters
{
    public class ResultToImageSourceConverter : IValueConverter
    {
        readonly ImageSource activity;
        readonly ImageSource alert;
        readonly ImageSource check;
        readonly ImageSource cloud;
        readonly ImageSource slash;
        readonly ImageSource clock;

        public ResultToImageSourceConverter()
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri("\\Images\\activity.png", UriKind.Relative));
            activity = bitmapImage;
            bitmapImage = new BitmapImage(new Uri("\\Images\\alert.png", UriKind.Relative));
            alert = bitmapImage;
            bitmapImage = new BitmapImage(new Uri("\\Images\\check.png", UriKind.Relative));
            check = bitmapImage;
            bitmapImage = new BitmapImage(new Uri("\\Images\\cloud.png", UriKind.Relative));
            cloud = bitmapImage;
            bitmapImage = new BitmapImage(new Uri("\\Images\\slash.png", UriKind.Relative));
            slash = bitmapImage;
            bitmapImage = new BitmapImage(new Uri("\\Images\\clock.png", UriKind.Relative));
            clock = bitmapImage;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Result result = (Result)value;
            ImageSource imageSource;
            switch (result)
            {
                case Result.Unused:
                    imageSource = cloud;
                    break;
                case Result.Idle:
                    imageSource = cloud;
                    break;
                case Result.Testing:
                    imageSource = activity;
                    break;
                case Result.DoneOk:
                    imageSource = check;
                    break;
                case Result.DoneErrorCriticalCode:
                    imageSource = alert;
                    break;
                case Result.DoneErrorConnection:
                    imageSource = slash;
                    break;
                case Result.DoneErrorTimeout:
                    imageSource = clock;
                    break;
                case Result.Stopped:
                    imageSource = cloud;
                    break;
                default:
                    imageSource = cloud;
                    break;
            }
            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
