using System.Reflection;
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
                parent = parent.Parent;
                if (parent.GetType().GetTypeInfo().IsSubclassOf(typeof(T)))
                    result = (T)parent;
            }
            return result;
        }

        public static T SetGridCoords<T>(this T v, int row = 0, int col = 0, int rowSpan = 1, int colSpan = 1) where T : View
        {
            Grid.SetRow(v, row);
            Grid.SetColumn(v, col);
            Grid.SetRowSpan(v, rowSpan);
            Grid.SetColumnSpan(v, colSpan);
            return v;
        }

        public static T AddChild<T>(this Grid grd, T child, int row = 0, int col = 0, int rowSpan = 1, int colSpan = 1) where T : View
        {
            grd.Children.Add(child.SetGridCoords(row, col, rowSpan, colSpan));
            return child;
        }

        public static LayoutOptions LOtoLayoutOptions(LO layoutOptions)
        {
            switch (layoutOptions)
            {
                case LO.S: return LayoutOptions.Start;
                case LO.C: return LayoutOptions.Center;
                case LO.E: return LayoutOptions.End;
                case LO.F: return LayoutOptions.Fill;
                case LO.SE: return LayoutOptions.StartAndExpand;
                case LO.CE: return LayoutOptions.CenterAndExpand;
                case LO.EE: return LayoutOptions.EndAndExpand;
                case LO.FE: return LayoutOptions.FillAndExpand;
                default: return LayoutOptions.Start;
            }
        }

        public static T SetLayoutOption<T>(this T v, LO? horizontalOptions = null, LO? verticalOptions = null, double? marginDefault = null,
            double? marginLeft = null, double? marginTop = null, double? marginRight = null, double? marginBottom = null) where T : View
        {
            if (horizontalOptions != null)
                v.HorizontalOptions = LOtoLayoutOptions((LO)horizontalOptions);
            if (verticalOptions != null)
                v.VerticalOptions = LOtoLayoutOptions((LO)verticalOptions);
            if (marginDefault != null || marginLeft != null || marginTop != null || marginRight != null || marginBottom != null)
                v.Margin = new Thickness(
                    marginLeft ?? marginDefault ?? v.Margin.Left,
                    marginTop ?? marginDefault ?? v.Margin.Top,
                    marginRight ?? marginDefault ?? v.Margin.Right,
                    marginBottom ?? marginDefault ?? v.Margin.Bottom);
            return v;
        }

    }

    public enum LO
    {
        S,  // Start
        C,  // Center
        E,  // End
        F,  // Fill
        SE, // StartAndExpand
        CE, // CenterAndExpand
        EE, // EndAndExpand
        FE, // FillAndExpand
    }
}
