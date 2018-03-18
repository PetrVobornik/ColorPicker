using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Amporis.Xamarin.Forms.ColorPicker
{
    /// <summary>
    /// Converts byte to string (e.g. for editing byte number in Entry/TextBox).
    /// When a value is invalid, default value (<see cref="DefaultValue"/>) is return.
    /// </summary>
    public class ByteToStrConverter : IValueConverter
    {
        public byte DefaultValue { get; set; } = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => System.Convert.ToString(value ?? DefaultValue);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToByte(value);
            }
            catch
            {
                return DefaultValue;
            }
        }
    }


}
