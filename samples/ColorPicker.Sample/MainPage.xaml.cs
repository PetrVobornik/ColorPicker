using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Amporis.Xamarin.Forms.ColorPicker;

namespace ColorPicker.Sample
{
    public partial class MainPage : ContentPage
    {
	    public MainPage()
	    {
		    InitializeComponent();
            gMain.BindingContext = this;
        }


        private Color editedColor = Color.FromHex("#FFFF8000"); //default value;

        public Color EditedColor {
            get => editedColor;
            set { editedColor = value; OnPropertyChanged(); }
        }


        private async void BInputDialog_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Text = await InputDialog.Show(gMain, "Caption", "New caption", btn.Text);
        }
    }
}
