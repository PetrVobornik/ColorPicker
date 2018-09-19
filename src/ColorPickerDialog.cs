using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xam.Plugin.SimpleColorPicker
{
   public class ColorPickerDialog : Dialog
   {
      /// <summary>
      /// original color
      /// </summary>
      private Color originalColor;

      /// <summary>
      /// dialog settings
      /// </summary>
      private ColorDialogSettings settings;

      /// <summary>
      /// color mixer
      /// </summary>
      private ColorPickerMixer colorEditor;

      /// <summary>
      /// Show color dialog with <see cref="ColorPickerMixer"/> for picking a color.
      /// </summary>
      /// <param name="parent">Root container on the page, where a modal dialog will be temporarily placed</param>
      /// <param name="title">Caption in the header of the dialog</param>
      /// <param name="dialogColor">Dialog color</param>
      /// <param name="defaultColor">Preselected color</param>
      /// <param name="settings">Dialog settings</param>
      /// <returns>Color selected in dialog or default color, if cancel is clicked</returns>
      public async static Task<Color> Show(Layout<View> parent, string title, Color defaultColor, ColorDialogSettings settings = null)
      {
         // Creating a dialog
         var dlg = new ColorPickerDialog()
         {
            Parent = parent,
            Title = title,
            originalColor = defaultColor,
            settings = settings ?? new ColorDialogSettings(),
         };
         dlg.Settings = dlg.settings;

         // Initializing
         await dlg.Initialize();

         // Showing
         bool result = await dlg.ShowDialog(parent);

         // Result
         if (result)
            return dlg.colorEditor.Color;
         return defaultColor;
      }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
      /// <summary>
      /// Init the color mixer
      /// </summary>
      /// <returns>Color mixer view</returns>
      protected async override Task<View> BuildContent()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
      {
         colorEditor = new ColorPickerMixer()
         {
            TextColor = settings.TextColor,
            EditorsColor = settings.EditorsColor,
            ColorPreviewBorderColor = settings.ColorPreviewBorderColor,
            EditAlpha = settings.EditAlfa,
         };
         colorEditor.Color = originalColor;
         return colorEditor;
      }
   }

   /// <summary>
   /// Dialog settings holder base
   /// </summary>
   public class DialogSettings
   {
      public Color BackgroundColor { get; set; } = Color.FromHex("#40000000");
      public Color DialogColor { get; set; } = Color.White;
      public Color TextColor { get; set; } = Color.Black;
      public string OkButtonText { get; set; } = "OK";
      public string CancelButtonText { get; set; } = "Cancel";
      public bool DialogAnimation { get; set; } = true;
   }

   /// <summary>
   /// Dialog settings holder
   /// </summary>
   public class ColorDialogSettings : DialogSettings
   {
      public Color EditorsColor { get; set; } = Color.White;
      public Color ColorPreviewBorderColor { get; set; }
      public bool EditAlfa { get; set; } = true;
   }

   /// <summary>
   /// Dialog
   /// </summary>
   public abstract class Dialog : Grid
   {
      #region Variables

      protected DialogSettings Settings { get; set; }
      protected virtual void OnShow() { }
      protected Layout<View> MainConteiner { get; set; }
      protected Frame MainFrame { get; set; }
      public string Title { get; set; }
      protected Label lblTitle;
      protected Button btnOk, btnCancel;
      protected StackLayout stlTitle, stlButtons;
      protected abstract Task<View> BuildContent();
      private TaskCompletionSource<string> buttonClicked;

      #endregion

      #region Protected

      protected async Task<bool> ShowDialog(Layout<View> parent)
      {
         if (Settings == null)
            Settings = new DialogSettings(); // Use default values

         Parent = null; // Parent byl nastaven, kvůli hledání rootu, ale před přidáním Layoutu do něčeho musí být Parent null
         MinimumWidthRequest = parent.Width;

         // V gridu přes všechny řádky a sloupce 
         if (parent is Grid grid)
         {
            if (grid.RowDefinitions.Count > 1)
               SetRowSpan(this, grid.RowDefinitions.Count);
            if (grid.ColumnDefinitions.Count > 1)
               SetColumnSpan(this, grid.ColumnDefinitions.Count);
         }

         uint animLength = 400;
         if (Settings.DialogAnimation)
         {
            Opacity = 0;
            MainFrame.Scale = 0.75;
            parent.Children.Add(this);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            this.FadeTo(1, animLength, Easing.SinOut);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            await MainFrame.ScaleTo(1, animLength, Easing.SinOut);
         }
         else
            parent.Children.Add(this);
         string result = await WaitForClick();
         if (Settings.DialogAnimation)
         {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            this.FadeTo(0, animLength, Easing.SinIn);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            await MainFrame.ScaleTo(0.75, animLength, Easing.SinIn);
         }
         parent.Children.Remove(this);
         return result == btnOk?.Text;
      }

      /// <summary>
      /// init dialog grid
      /// </summary>
      protected async Task Initialize()
      {
         Children.Clear();

         HorizontalOptions = LayoutOptions.Fill;
         VerticalOptions = LayoutOptions.Fill;
         BackgroundColor = Settings.BackgroundColor;
         Margin = new Thickness(-20);
         Padding = new Thickness(20);

         MainFrame = new Frame()
         {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Settings.DialogColor,
            Padding = new Thickness(16),
            HasShadow = true,
         };

         switch(Device.Idiom)
         {
            case TargetIdiom.Tablet:
            case TargetIdiom.Desktop:
               MainFrame.WidthRequest = 300;
               break;
         }
            
         Children.Add(MainFrame);
         MainConteiner = new StackLayout() { Orientation = StackOrientation.Vertical };
         MainFrame.Content = MainConteiner;

         if (!string.IsNullOrEmpty(Title))
         {
            // Title
            stlTitle = new StackLayout() { Orientation = StackOrientation.Horizontal, Margin = new Thickness(0, 0, 0, 10), HorizontalOptions = LayoutOptions.Fill };
            MainConteiner.Children.Add(stlTitle);
            lblTitle = new Label();
            lblTitle.FontSize *= 1.5;
            lblTitle.VerticalOptions = LayoutOptions.Center;
            lblTitle.Text = Title;
            lblTitle.TextColor = Settings.TextColor;
            stlTitle.Children.Add(lblTitle);
         }

         // Buttons (container)
         stlButtons = new StackLayout
         {
            HorizontalOptions = LayoutOptions.End,
            Orientation = StackOrientation.Horizontal
         };

         // Button OK
         if (!string.IsNullOrEmpty(Settings.OkButtonText))
         {
            btnOk = new Button();
            btnOk.Clicked += Btn_Clicked;
            btnOk.Text = Settings.OkButtonText;
            stlButtons.Children.Add(btnOk);
         }

         // Button Cancel
         if (!string.IsNullOrEmpty(Settings.CancelButtonText))
         {
            if (btnOk != null)
               btnOk.Margin = new Thickness(0, 0, 10, 0);
            btnCancel = new Button();
            btnCancel.Clicked += Btn_Clicked;
            btnCancel.Text = Settings.CancelButtonText;
            stlButtons.Children.Add(btnCancel);
         }

         // Obsah dialogu
         MainConteiner.Children.Add(await BuildContent());
         MainConteiner.Children.Add(stlButtons);
      }

      /// <summary>
      /// Close the dialog
      /// </summary>
      protected void CloseDialog()
      {
         if (buttonClicked != null)
            buttonClicked.TrySetResult("");
      }

      #endregion

      #region Private

      /// <summary>
      /// Wait for click action on button
      /// </summary>
      private async Task<string> WaitForClick()
      {
         buttonClicked = new TaskCompletionSource<string>();
         return await buttonClicked.Task; 
      }

      /// <summary>
      /// Button click handler
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Btn_Clicked(object sender, EventArgs e)
      {
         if (buttonClicked != null)
            buttonClicked.TrySetResult(((Button)sender).Text); 
      }

      #endregion
   }
}
