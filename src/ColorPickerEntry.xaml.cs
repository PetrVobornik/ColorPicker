using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Amporis.Xamarin.Forms.ColorPicker
{
    #region PreviewButtonClicked (EventArgs + Delegate)

    public class PreviewButtonClickedEventArgs
    {
        /// <summary>
        /// Current color which you can change to another
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// A click to the color preview button was handled by an event (color dialog will not shown)
        /// </summary>
        public bool Handled { get; set; }
    }

    public delegate Task PreviewButtonClickedDelegate(object sender, PreviewButtonClickedEventArgs e);

    #endregion

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPickerEntry : ContentView
    {
        public ColorPickerEntry()
        {
            InitializeComponent();
            eColor.BindingContext = ColorVal;
            bEdit.BindingContext = ColorVal;
            ColorVal.PropertyChanged += ColorVal_PropertyChanged;
        }

        private ColorValue ColorVal = new ColorValue();

        /// <summary>
        /// Editor (Entry) where a color is edited as hexadecimal string
        /// </summary>
        public Entry Editor { get => eColor; }

        /// <summary>
        /// Caption in the header of the dialog
        /// </summary>
        public string DialogTitle { get; set; } = "";

        /// <summary>
        /// Root container on the page, where a modal dialog will be temporarily placed
        /// </summary>
        public Layout<View> RootContainer { get; set; }

        /// <summary>
        /// Display the color preview button
        /// </summary>
        public bool ShowColorPreview { get => bEdit.IsVisible; set => bEdit.IsVisible = value; }

        /// <summary>
        /// Width of the color preview button
        /// </summary>
        public double ColorPreviewButtonWidth { get => bEdit.WidthRequest; set => bEdit.WidthRequest = value; }

        /// <summary>
        /// Show modal dialog (<see cref="ColorPickerDialog"/>) with color mixer (<see cref="ColorPickerMixer"/>) when click to the color preview button
        /// </summary>
        public bool AllowPickerDialog { get; set; } = true;

        /// <summary>
        /// Also allow editing of the alpha channel?
        /// TRUE - edited are ARGB, FALSE - edited are only RGB.
        /// </summary>
        public bool EditAlfa { get => ColorVal.EditAlpha; set => ColorVal.EditAlpha = value; }
        public ColorDialogSettings DialogSettings { get; set; }

        /// <summary>
        /// Raised when color preview button is clicked
        /// </summary>
        public event PreviewButtonClickedDelegate PreviewButtonClicked;

        private void ColorVal_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
                Color = ColorVal.Value;
        }

        #region Color

        public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color", typeof(Color), typeof(ColorPickerEntry), Color.White, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorChanged);

        static void ColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if ((Color)oldValue != (Color)newValue)
                ((ColorPickerEntry)bindable).ColorVal.Value = (Color)newValue;
        }

        /// <summary>
        /// Edited color (bindable property)
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); } 
            set { if (Color != value) SetValue(ColorProperty, value); } 
        }

        #endregion

        private async void bEdit_Clicked(object sender, EventArgs e)
        {
            // Handling by event
            if (PreviewButtonClicked != null)
            {
                var eva = new PreviewButtonClickedEventArgs() { Color = Color, Handled = false };
                await PreviewButtonClicked(this, eva);
                Color = eva.Color;
                if (eva.Handled) return;
            }
            // Color dialog
            if (!AllowPickerDialog) return;
            if (RootContainer == null)
                RootContainer = ColorPickerUtils.GetRootParent<Layout<View>>(this);
            if (RootContainer != null)
            {
                var ds = DialogSettings ?? new ColorDialogSettings();
                ds.EditAlfa = EditAlfa;
                ColorVal.Value = await ColorPickerDialog.Show(RootContainer, DialogTitle, ColorVal.Value, ds);
            }
        }
    }
}