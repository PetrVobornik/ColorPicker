using Xamarin.Forms;

namespace ColorPicker.Sample
{
   public partial class MainPage : ContentPage
   {
      /// <summary>
      /// Constructor
      /// </summary>
      public MainPage()
      {
         InitializeComponent();
         gMain.BindingContext = this;
      }

      /// <summary>
      /// edit color with default value
      /// </summary>
      private Color editedColor = Color.FromHex("#FFFF9800");//default value

      /// <summary>
      /// Edited color property
      /// </summary>
      public Color EditedColor
      {
         get => editedColor;
         set { editedColor = value; OnPropertyChanged(); }
      }
   }
}
