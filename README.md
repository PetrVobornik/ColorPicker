# Color picker for Xamarin.Forms
Color picker control for Xamarin.Forms. It can be used as a large color mixer control (ColorPickerMixer) or as a dialog for select a color (ColorPickerDialog) or as an entry editor (ColorPickerEntry) of hexadecimal value with preview of a color and option of launch a dialog with color mixer.


## NuGet
* Available on NuGet: https://www.nuget.org/packages/Amporis.Xamarin.Forms.ColorPicker/ [![NuGet](https://img.shields.io/nuget/v/Amporis.Xamarin.Forms.ColorPicker.svg?label=NuGet)](https://www.nuget.org/packages/Amporis.Xamarin.Forms.ColorPicker/)


## Platform Support

Color picker control is written in C# (.NET 4.5) and uses standard Xamarin.Forms only.
Color picker was tested as NuGet in shared PCL library with these platforms:

|Platform|Version|
| ------------------- | :------------------: |
|Windows 10 UWP|10+|
|Xamarin.Android|API 14+|
|Xamarin.iOS|iOS 8+|


## Usage

### ColorPickerDialog
```csharp
var color = await ColorPickerDialog.Show(gMain, "Choose color", Color.White, null);
```

**Parameters**
* **parent** (gMain) - root container (Layout<View>) on the page, where a modal dialog will be temporarily placed
* **title** ("Choose color") - caption in the header of the dialog
* **defaultColor** (Color.White) - preselected color
* **settings** (null) - dialog settings - class **ColorDialogSettings** with these properties and its default values
  * BackgroundColor (#40000000) - color of the panel below dialog which temporarily covers other controls on the page (using partial transparency)
  * DialogColor (#FFFFFFFF)
  * TextColor (#FF000000)
  * OkButtonText ("OK")
  * CancelButtonText ("Cancel")
  * DialogAnimation (true)
  * EditorsColor (#FFFFFFFF)
  * ColorPreviewBorderColor (#00FFFFFF)
  * SliderWidth (256)
  * ARGBEditorsWidth (65)
  * ColorEditorWidth (120)
  * EditAlfa (true)


### ColorPickerEntry and ColorPickerMixer
```xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:ColorPicker.Sample"
             xmlns:cp="clr-namespace:Amporis.Xamarin.Forms.ColorPicker;assembly=Amporis.Xamarin.Forms.ColorPicker"
             x:Class="ColorPicker.Sample.MainPage">
    <Grid x:Name="gMain">
        <StackLayout Spacing="30">
            <cp:ColorPickerEntry x:Name="cpEntry" />
            <cp:ColorPickerMixer x:Name="cpMixer" />
        </StackLayout>
    </Grid>
</ContentPage>
```

There are also these properties:

**ColorPickerEntry**
* Color (BindableProperty) 
* EditAlfa
* ShowColorPreview
* ColorPreviewButtonWidth
* AllowPickerDialog
* DialogTitle
* DialogSettings (class ColorDialogSettings)
* RootContainer - parent container for dialog, if is null then will be found automatically
* Editor (read only) - reference for Entry where a hexadecimal color value is edited
* PreviewButtonClicked (event) - has argument class PreviewButtonClickedEventArgs with these properties:
  * Color - current color which you can change to another by the event
  * Handled - if you set it to true, dialog will not be shown

**ColorPickerMixer**
* Color (BindableProperty)
* TextColor
* EditorsColor
* ColorPreviewBorderColor
* SliderWidth
* ARGBEditorsWidth
* ColorEditorWidth
* EditAlfa
