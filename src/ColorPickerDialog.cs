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
                SliderWidth = settings.SliderWidth,
                ARGBEditorsWidth = settings.ARGBEditorsWidth,
                ColorEditorWidth = settings.ColorEditorWidth,
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
        public bool HideNavigationBar { get; set; } = false;
    }

    /// <summary>
    /// Dialog settings holder
    /// </summary>
    public class ColorDialogSettings : DialogSettings
    {
        public Color EditorsColor { get; set; } = Color.White;
        public Color ColorPreviewBorderColor { get; set; }
        public double SliderWidth { get; set; } = 256;
        public double ARGBEditorsWidth { get; set; } = 65;
        public double ColorEditorWidth { get; set; } = 120;
        public bool EditAlfa { get; set; } = true;
    }


    /// <summary>
    /// Dialog
    /// </summary>
    public abstract class Dialog : Grid
    {
        protected DialogSettings Settings { get; set; }

        protected async Task<bool> ShowDialog(Layout<View> parent)
        {
            if (Settings == null) Settings = new DialogSettings(); // Use default values
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

            var page = Settings.HideNavigationBar ? ColorPickerUtils.GetRootParent<ContentPage>(parent) : null;
            bool hasNavBar = page == null ? false : NavigationPage.GetHasNavigationBar(page);
            if (hasNavBar)
                NavigationPage.SetHasNavigationBar(page, false);

            // Display animation
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
            } else 
                parent.Children.Add(this);
            // Waiting for click
            string result = await WaitForClick();
            // The disappearance animation
            if (Settings.DialogAnimation)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.FadeTo(0, animLength, Easing.SinIn);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await MainFrame.ScaleTo(0.75, animLength, Easing.SinIn);
            }
            parent.Children.Remove(this);

            if (hasNavBar)
                NavigationPage.SetHasNavigationBar(page, true);

            // Returning the result
            return result == btnOk?.Text;
        }

        protected virtual void OnShow() { }

        protected Layout<View> MainConteiner { get; set; }
        protected Frame MainFrame { get; set; }
        public string Title { get; set; }

        protected Label lblTitle;
        protected Button btnOk, btnCancel;
        protected StackLayout stlTitle, stlButtons;

        /// <summary>
        /// init dialog grid
        /// </summary>
        protected async Task Initialize()
        {
            Children.Clear();

            // Background Grid (Overlay)
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;
            BackgroundColor = Settings.BackgroundColor;
            Margin = new Thickness(-20);
            Padding = new Thickness(20);

            // Grid with dialogue background
            MainFrame = new Frame()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Settings.DialogColor,
                Padding = new Thickness(16),
                HasShadow = true,
            };
            Children.Add(MainFrame);

            // Container for the contents of the dialog
            MainConteiner = new StackLayout() { Orientation = StackOrientation.Vertical };
            MainFrame.Content = new ScrollView() { Content = MainConteiner };

            // Title
            if (!String.IsNullOrEmpty(Title))
            {
                stlTitle = new StackLayout() { Orientation = StackOrientation.Horizontal, Margin = new Thickness(0, 0, 0, 10), HorizontalOptions = LayoutOptions.Fill };
                MainConteiner.Children.Add(stlTitle);
                lblTitle = new Label()
                {
                    VerticalOptions = LayoutOptions.Center,
                    Text = Title,
                    TextColor = Settings.TextColor,
                };
                lblTitle.FontSize *= 1.5;
                stlTitle.Children.Add(lblTitle);
            }

            // Buttons (container)
            stlButtons = new StackLayout() {
                HorizontalOptions = LayoutOptions.End,
                Orientation = StackOrientation.Horizontal,
            };

            // Button OK
            if (!String.IsNullOrEmpty(Settings.OkButtonText))
            {
                btnOk = new Button() { Text = Settings.OkButtonText };
                stlButtons.Children.Add(btnOk);
                btnOk.Clicked += Btn_Clicked;
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

            // Content of the dialogue
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

        /// <summary>
        /// Close the dialog by Cancel
        /// </summary>
        protected void CloseDialog()
        {
            if (buttonClicked != null)
                buttonClicked.TrySetResult("");
        }

        /// <summary>
        /// Close the dialog by OK
        /// </summary>
        protected void CloseDialogOk()
        {
            if (buttonClicked != null)
                buttonClicked.TrySetResult(btnOk.Text);
        }
    }


    /// <summary>
    /// Input dialog (bonus)
    /// </summary>
    public class InputDialog : Dialog
    {
        ColorDialogSettings settings;

        public async static Task<string> Show(Layout<View> parent, string title, string header, string defaultText, string ext = "", ColorDialogSettings settings = null)
        {
            // Creating a dialog
            var dlg = new InputDialog()
            {
                Parent = parent,
                Title = title,
                ext = ext,
                settings = settings ?? new ColorDialogSettings(),                
            };
            dlg.Settings = dlg.settings;

            await dlg.Initialize();

            dlg.lblHeader.Text = header;
            dlg.edtEditor.Text = defaultText;

            // Zobrazení
            bool result = await dlg.ShowDialog(parent);
            if (result)
                return dlg.edtEditor.Text;
            return null;
        }

        protected override void OnShow()
        {
            base.OnShow();
            edtEditor.Focus();
        }

        Label lblHeader;
        Entry edtEditor;
        string ext = "";


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected async override Task<View> BuildContent()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var stlContent = new StackLayout();
            stlContent.Orientation = StackOrientation.Vertical;

            // Header
            lblHeader = new Label() { TextColor = settings.TextColor, Margin = new Thickness(0, 0, 0, 2) };
            stlContent.Children.Add(lblHeader);

            // Editor
            edtEditor = new Entry() { TextColor = settings.TextColor, BackgroundColor = settings.EditorsColor };
            edtEditor.Completed += (s, e) => CloseDialogOk();
            if (String.IsNullOrEmpty(ext))
            {
                edtEditor.Margin = new Thickness(0, 0, 0, 10);
                stlContent.Children.Add(edtEditor);
            }
            else
            {
                edtEditor.WidthRequest = 160;
                var grd = new Grid();
                grd.Margin = new Thickness(0, 0, 0, 10);
                grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                grd.AddChild(edtEditor, 0, 0).SetLayoutOption(LO.S, LO.C);
                grd.AddChild(new Label() { Text = ext, Margin = new Thickness(5, 0, 0, 0), TextColor = settings.TextColor }, 0, 1)
                    .SetLayoutOption(LO.S, LO.C);
                stlContent.Children.Add(grd);
            }

            return stlContent;
        }

    }
}
