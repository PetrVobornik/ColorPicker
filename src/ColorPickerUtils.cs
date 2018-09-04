using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace Amporis.Xamarin.Forms.ColorPicker
{
   public static class ColorPickerUtils
   {
      /// <summary>
      /// Convert Color to its hexadecimal string
      /// </summary>
      /// <param name="color">Converted color</param>
      /// <returns>Hexadecimal code of the color in ARGB format (e.g. FF123456)</returns>
      public static string ToHex(this Color color)
      {
         var red = (int)(color.R * 255);
         var green = (int)(color.G * 255);
         var blue = (int)(color.B * 255);
         var alpha = (int)(color.A * 255);
         var hex = $"{alpha:X2}{red:X2}{green:X2}{blue:X2}";
         return hex;
      }

      /// <summary>
      /// Transform byte to double by b/255 (0 = 0, 255 = 1)
      /// </summary>
      public static double ToDouble(this byte b) => b / 255.0;

      /// <summary>
      /// Transform double to byte by d*255 (0 = 0, 1 = 255)
      /// </summary>
      public static byte ToByte(this double d) => (byte)(d * 255);

      /// <summary>
      /// Create color from four byte values (0-255)
      /// </summary>
      /// <param name="a">Alpha</param>
      /// <param name="r">Red</param>
      /// <param name="g">Green</param>
      /// <param name="b">Blue</param>
      public static Color ColorFromARGB(byte a, byte r, byte g, byte b)
          => Color.FromRgba(r.ToDouble(), g.ToDouble(), b.ToDouble(), a.ToDouble());

      /// <summary>
      /// Try find the farthest parent container of an element which is a T type or its subtype
      /// </summary>
      /// <typeparam name="T">Searched type of the parent container (type Layout or its subtype)</typeparam>
      /// <param name="view">Start element for searching of the parent container</param>
      /// <returns>Farthest parent container of an element (T type or its subtype)</returns>
      public static T GetRootParent<T>(Element view) where T : Layout
      {
         var parent = view;
         T result = parent as T;
         while (parent.Parent?.GetType().GetTypeInfo().IsSubclassOf(typeof(Element)) == true)
         {
            parent = parent.Parent as Element;
            if (parent.GetType().GetTypeInfo().IsSubclassOf(typeof(T)))
               result = (T)parent;
         }
         return result;
      }

   }
}
