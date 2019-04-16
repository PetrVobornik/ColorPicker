using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using Xamarin.Forms;

namespace Amporis.Xamarin.Forms.ColorPicker
{
    public class ColorValue : INotifyPropertyChanged
    {
        private byte a = 255, r = 255, g = 255, b = 255;

        #region ARGB

        /// <summary>
        /// Alpha
        /// </summary>
        public byte A
        {
            get => a;
            set { if (value != a) { a = value; ValuesChanged(nameof(A), nameof(Value), nameof(Hexa)); } }
        }

        /// <summary>
        /// Red
        /// </summary>
        public byte R
        {
            get => r;
            set { if (value != r) { r = value; ValuesChanged(nameof(R), nameof(Value), nameof(Hexa)); } }
        }

        /// <summary>
        /// Green
        /// </summary>
        public byte G
        {
            get => g;
            set { if (value != g) { g = value; ValuesChanged(nameof(G), nameof(Value), nameof(Hexa)); } }
        }

        /// <summary>
        /// Blue
        /// </summary>
        public byte B
        {
            get => b;
            set { if (value != b) { b = value; ValuesChanged(nameof(B), nameof(Value), nameof(Hexa)); } }
        }

        #endregion

        #region Value

        /// <summary>
        /// Result value
        /// </summary>
        public Color Value
        {
            get { return ColorPickerUtils.ColorFromARGB(a, r, g, b); }
            set
            {
                valueIsChanging = true;
                A = value.A.ToByte();
                R = value.R.ToByte();
                G = value.G.ToByte();
                B = value.B.ToByte();
                valueIsChanging = false;
                ValuesChanged(nameof(Value), nameof(Hexa));
            }
        }

        #endregion

        #region Hexa

        /// <summary>
        /// Hex value
        /// </summary>
        public string Hexa
        {
            get
            {
                string hex = Value.ToHex().TrimStart('#');
                if (!EditAlpha && hex.Length > 6)
                    hex = hex.Substring(2);
                return hex;
            }
            set
            {
                string hex = value.TrimStart('#');
                if (EditAlpha && hex.Length != 8 || !EditAlpha && hex.Length != 6)
                    return;
                try
                {
                    var clr = Color.FromHex(hex);
                    if (Value != clr)
                        Value = clr;
                }
                catch { }
            }
        }

        #endregion

        #region EditAlpha

        private bool editAlpha = true;

        /// <summary>
        /// Enable edit alpha
        /// </summary>
        public bool EditAlpha
        {
            get => editAlpha;
            set { if (value != editAlpha) { editAlpha = value; ValuesChanged(nameof(EditAlpha), nameof(Hexa)); } }
        }

        #endregion

        #region PropertyChanged

        bool valueIsChanging = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private void ValuesChanged(params string[] propNames)
        {
            if (valueIsChanging) // If a change is launched by changing in a values, it does not announce its change - when the individual items are changed, it will eventually announce when all changes are finished
                propNames = propNames.Where(p => p != nameof(Value) && p != nameof(Hexa)).ToArray();
            if (propNames != null && propNames.Length > 0)
                foreach (string prop in propNames)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void ValueChanged([CallerMemberName] string propName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        #endregion
    }
}
