using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Amporis.Xamarin.Forms.ColorPicker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPickerMixer : ContentView
    {
        public ColorPickerMixer()
        {
            InitializeComponent(); 
            eColor.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
            gMain.BindingContext = this;
            ColorVal.PropertyChanged += (s, e) => {
                switch (e.PropertyName)
                {
                    case "Value": Color = ((ColorValue)s).Value; break;
                    case "EditAlpha": ValueChanged(nameof(EditAlpha)); break;
                }
            };
        }


        /// <summary>
        /// Edited color value (for internal use)
        /// </summary>
        public ColorValue ColorVal { get; private set; } = new ColorValue();

        #region Settings 

        private Color editorsColor = Color.White;
        private Color colorPreviewBorderColor = Color.Black;
        private double sliderWidth = 256;
        private double aRGBEditorsWidth = 65;
        private double colorEditorWidth = 120;

        //public Color TextColor { get => textColor; set { textColor = value; ValueChanged(); } }
        public Color EditorsColor { get => editorsColor; set { editorsColor = value; ValueChanged(); } }
        public Color ColorPreviewBorderColor { get => colorPreviewBorderColor; set { colorPreviewBorderColor = value; ValueChanged(); } }
        public double SliderWidth { get => sliderWidth; set { sliderWidth = value; ValueChanged(); } }
        public double ARGBEditorsWidth { get => aRGBEditorsWidth; set { aRGBEditorsWidth = value; ValueChanged(); } }
        public double ColorEditorWidth { get => colorEditorWidth; set { colorEditorWidth = value; ValueChanged(); } }
        public bool EditAlpha { get => ColorVal.EditAlpha; set { ColorVal.EditAlpha = value; ColorVal.EditAlpha = value; ValueChanged(); } }

        #endregion

        #region Color

        /// <summary>
        /// Color property
        /// </summary>
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(ColorPickerEntry), Color.White, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorChanged);

        static void ColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if ((Color)oldValue != (Color)newValue)
                ((ColorPickerMixer)bindable).ColorVal.Value = (Color)newValue;
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

        #region TextColor
     
        /// <summary>
        /// Set the entry text color
        /// </summary>
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ColorPickerEntry), Color.Black, defaultBindingMode: BindingMode.TwoWay, propertyChanged: TextColorChanged);

        /// <summary>
        /// Set the text color
        /// </summary>
        static void TextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if ((Color)oldValue != (Color)newValue)
                ((ColorPickerMixer)bindable).TextColor = (Color)newValue;
        }

        /// <summary>
        /// Text color (bindable property)
        /// </summary>
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { if (TextColor != value) SetValue(TextColorProperty, value); ValueChanged(); }
        }

        #endregion


        private void ValueChanged([CallerMemberName] string propName = null) => OnPropertyChanged(propName);
    }
}