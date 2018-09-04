using System;
using System.Globalization;
using Xamarin.Forms;

namespace Amporis.Xamarin.Forms.ColorPicker
{
   /// <summary>
   /// Converts byte to string (e.g. for editing byte number in Entry/TextBox).
   /// When a value is invalid, default value (<see cref="DefaultValue"/>) is return.
   /// </summary>
   public class ByteToStrConverter : IValueConverter
   {
      /// <summary>
      /// Default value of the converter
      /// </summary>
      public byte DefaultValue { get; set; } = 0;

      /// <summary>
      /// Convert byte to string 
      /// </summary>
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
          => System.Convert.ToString(value ?? DefaultValue);

      /// <summary>
      /// Convert back to string
      /// </summary>
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