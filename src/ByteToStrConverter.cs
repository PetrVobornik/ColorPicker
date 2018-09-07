using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xam.Plugin.SimpleColorPicker
{
   /// <summary>
   /// Converts byte to string (e.g. for editing byte number in Entry/TextBox).
   /// When a value is invalid, default value (<see cref="DefaultValue"/>) is return.
   /// </summary>
   public class ByteToStrConverter : IValueConverter
   {
      #region Public

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
         catch (Exception ex)
         {
            Console.Write(ex?.Message);
            return DefaultValue;
         }
      }

      #endregion
   }
}