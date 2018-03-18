using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Amporis.Xamarin.Forms.ColorPicker
{
    public class ColorPickerDialog : Dialog
    {
        Color originalColor;
        ColorDialogSettings settings;
        ColorPickerMixer colorEditor;

        /// <summary>
        /// Show color dialog with <see cref="ColorPickerMixer"/> for picking a color.
        /// </summary>
        /// <param name="parent">Root container on the page, where a modal dialog will be temporarily placed</param>
        /// <param name="title">Caption in the header of the dialog</param>
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

        protected async override Task<View> BuildContent()
        {
            colorEditor = new ColorPickerMixer()
            {
                TextColor = settings.TextColor,
                EditorsColor = settings.EditorsColor,
                ColorPreviewBorderColor = settings.ColorPreviewBorderColor,
                SliderWidth = settings.SliderWidth,
                ARGBEditorsWidth = settings.ARGBEditorsWidth,
                ColorEditorWidth = settings.ColorEditorWidth,
                EditAlpha = settings.EditAlfa,
            };
            colorEditor.Color = originalColor;            
            return colorEditor; 
        }
    }

    public class DialogSettings
    {
        public Color BackgroundColor { get; set; } = Color.FromHex("#40000000");
        public Color DialogColor { get; set; } = Color.White;
        public Color TextColor { get; set; } = Color.Black;
        public string OkButtonText { get; set; } = "OK";
        public string CancelButtonText { get; set; } = "Cancel";
        public bool DialogAnimation { get; set; } = true;
    }

    public class ColorDialogSettings : DialogSettings
    {
        public Color EditorsColor { get; set; } = Color.White;
        public Color ColorPreviewBorderColor { get; set; }
        public double SliderWidth { get; set; } = 256;
        public double ARGBEditorsWidth { get; set; } = 65;
        public double ColorEditorWidth { get; set; } = 120;
        public bool EditAlfa { get; set; } = true;
    }


    public abstract class Dialog : Grid
    {
        protected DialogSettings Settings { get; set; }

        protected async Task<bool> ShowDialog(Layout<View> parent)
        {
            if (Settings == null) Settings = new DialogSettings(); // Use default values
            Parent = null; // Parent byl nastaven, kvůli hledání rootu, ale před přidáním Layoutu do něčeho musí být Parent null
            this.MinimumWidthRequest = parent.Width;

            // V gridu přes všechny řádky a sloupce 
            if (parent is Grid)
            {
                if (((Grid)parent).RowDefinitions.Count > 1)
                    Grid.SetRowSpan(this, ((Grid)parent).RowDefinitions.Count);
                if (((Grid)parent).ColumnDefinitions.Count > 1)
                    Grid.SetColumnSpan(this, ((Grid)parent).ColumnDefinitions.Count);
            }

            // Animace zobrazení
            uint animLength = 400;
            if (Settings.DialogAnimation)
            {
                this.Opacity = 0;
                MainFrame.Scale = 0.75;
                parent.Children.Add(this);
                this.FadeTo(1, animLength, Easing.SinOut);
                await MainFrame.ScaleTo(1, animLength, Easing.SinOut);
            } else 
                parent.Children.Add(this);
            // Čekání na odkliknutí
            string result = await this.WaitForClick();
            // Animace zmizení
            if (Settings.DialogAnimation)
            {
                this.FadeTo(0, animLength, Easing.SinIn);
                await MainFrame.ScaleTo(0.75, animLength, Easing.SinIn);
            }
            parent.Children.Remove(this);
            // Vrácení výsledku
            return result == btnOk?.Text;
        }

        protected virtual void OnShow() { }

        protected Layout<View> MainConteiner { get; set; }
        protected Frame MainFrame { get; set; }
        public string Title { get; set; }

        protected Label lblTitle;
        protected Button btnOk, btnCancel;
        protected StackLayout stlTitle, stlButtons;

        protected async Task Initialize()
        {
            Children.Clear();

            // Grid v pozadí (překryv)
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;
            BackgroundColor = Settings.BackgroundColor;

            // Grid s pozadím dialogu
            MainFrame = new Frame()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Settings.DialogColor,
                Padding = new Thickness(16),
                HasShadow = true,
            };
            Children.Add(MainFrame);

            // Grid pro umístění obsahu dialogu
            MainConteiner = new StackLayout() { Orientation = StackOrientation.Vertical };
            MainFrame.Content = MainConteiner;

            // Title
            stlTitle = new StackLayout() { Orientation = StackOrientation.Horizontal, Margin = new Thickness(0, 0, 0, 10), HorizontalOptions = LayoutOptions.Fill };
            MainConteiner.Children.Add(stlTitle);
            lblTitle = new Label();
            lblTitle.FontSize *= 1.5;
            lblTitle.VerticalOptions = LayoutOptions.Center;
            lblTitle.Text = Title;
            lblTitle.TextColor = Settings.TextColor;
            stlTitle.Children.Add(lblTitle);

            // Buttons (container)
            stlButtons = new StackLayout();
            stlButtons.HorizontalOptions = LayoutOptions.End;
            stlButtons.Orientation = StackOrientation.Horizontal;

            // Button OK
            if (!String.IsNullOrEmpty(Settings.OkButtonText))
            {
                btnOk = new Button();
                btnOk.Clicked += Btn_Clicked;
                btnOk.Text = Settings.OkButtonText;
                stlButtons.Children.Add(btnOk);
            }

            // Button Cancel
            if (!String.IsNullOrEmpty(Settings.CancelButtonText))
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

        protected abstract Task<View> BuildContent();


        private TaskCompletionSource<string> buttonClicked;        // Pomocná proměnná pro "čekač"

        private async Task<string> WaitForClick()
        {
            buttonClicked = new TaskCompletionSource<string>();    // Vytvoření "čekače"  
            return await buttonClicked.Task;                       // Po nastavení hodnoty "čekači" bude tato vrácena 
        }

        private void Btn_Clicked(object sender, EventArgs e)
        {
            if (buttonClicked != null)
                buttonClicked.TrySetResult(((Button)sender).Text);   // Nastavit "čekači" hodnotu z tagu tlačítka na které se kliklo 
        }

        protected void CloseDialog()
        {
            if (buttonClicked != null)
                buttonClicked.TrySetResult("");
        }
    }
}
