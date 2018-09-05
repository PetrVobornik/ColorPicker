using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xam.Plugin.SimpleColorPicker
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
         fEdit.BindingContext = ColorVal;
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
      public double ColorPreviewButtonWidth { get => fEdit.WidthRequest; set => fEdit.WidthRequest = value; }

      /// <summary>
      /// Color of the color preview button border
      /// </summary>
      public Color ColorPreviewButtonBorder { get => fEdit.OutlineColor; set { fEdit.OutlineColor = value; } }

      /// <summary>
      /// Space between editor and button
      /// </summary>
      public double SpaceBetweenEditorAndButton { get => sLayout.Spacing; set => sLayout.Spacing = value; }


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

      /// <summary>
      /// Color val changed handler
      /// </summary>
      private void ColorVal_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         if (e.PropertyName == "Value")
            Color = ColorVal.Value;
      }

      #region Color

      /// <summary>
      /// Set the current color for the picker
      /// </summary>
      public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color", typeof(Color), typeof(ColorPickerEntry), Color.White, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorChanged);

      /// <summary>
      /// Handler for Color changed
      /// </summary>
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

      /// <summary>
      /// Set the dialog color
      /// </summary>
      public static readonly BindableProperty DialogColorProperty = BindableProperty.Create("DialogColor", typeof(Color), typeof(ColorPickerEntry), Color.White, defaultBindingMode: BindingMode.TwoWay, propertyChanged: DialogColorChanged);

      /// <summary>
      /// Set the dialog color
      /// </summary>
      static void DialogColorChanged(BindableObject bindable, object oldValue, object newValue)
      {
         if ((Color)oldValue != (Color)newValue)
            ((ColorPickerEntry)bindable).DialogColor = (Color)newValue;
      }

      /// <summary>
      /// Dialog color (bindable property)
      /// </summary>
      public Color DialogColor
      {
         get { return (Color)GetValue(DialogColorProperty); }
         set { if (DialogColor != value) SetValue(DialogColorProperty, value); }
      }

      /// <summary>
      /// Set the entry text color
      /// </summary>
      public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(ColorPickerEntry), Color.Gray, defaultBindingMode: BindingMode.TwoWay, propertyChanged: TextColorChanged);

      /// <summary>
      /// Set the text color
      /// </summary>
      static void TextColorChanged(BindableObject bindable, object oldValue, object newValue)
      {
         if ((Color)oldValue != (Color)newValue)
            ((ColorPickerEntry)bindable).TextColor = (Color)newValue;
      }

      /// <summary>
      /// Text color (bindable property)
      /// </summary>
      public Color TextColor
      {
         get { return (Color)GetValue(TextColorProperty); }
         set { if (TextColor != value) SetValue(TextColorProperty, value); eColor.TextColor = value; }
      }

      #endregion

      /// <summary>
      /// Open dialog
      /// </summary>
      private async void OpenDialog_Clicked(object sender, EventArgs e)
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
            ColorVal.Value = await ColorPickerDialog.Show(RootContainer, DialogTitle, DialogColor, ColorVal.Value, TextColor, ds);
         }
      }
   }
}