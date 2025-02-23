using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace priv2ppk
{
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {
        private readonly nint hwnd;
        private readonly ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public MainWindow()
        {
            this.InitializeComponent();
            hwnd = WindowNative.GetWindowHandle(this);

            // Fix being able to double click titlebar to maximize even when disabled
            NativeWindowHelper.ForceDisableMaximize(this);
        }

        private async void Button_WinSCPLocation_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new();
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add(".exe");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                TextBox_WinSCPLocation.Text = file.Path;
            }
        }

        private async void Button_PrivateKeyLocation_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new();
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add("*");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                TextBox_PrivateKeyLocation.Text = file.Path;
            }
        }

        private async void Button_PuTTYKeyLocation_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new();
            InitializeWithWindow.Initialize(savePicker, hwnd);

            savePicker.FileTypeChoices.Add("PuTTY Key File", [".ppk"]);

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                TextBox_PuTTYKeyLocation.Text = file.Path;
            }
        }

        private void TextBox_WinSCPLocation_Loaded(object sender, RoutedEventArgs e)
        {
            if (localSettings.Values["WinSCPLocation"] is string location)
            {
                TextBox_WinSCPLocation.Text = location;
            }
        }

        private void TextBox_WinSCPLocation_Unloaded(object sender, RoutedEventArgs e)
        {
            localSettings.Values["WinSCPLocation"] = TextBox_WinSCPLocation.Text;
        }

        private void Button_Convert_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo info = new()
            {
                FileName = TextBox_WinSCPLocation.Text,
                Arguments = $"/keygen {TextBox_PrivateKeyLocation.Text} /output={TextBox_PuTTYKeyLocation.Text}",
            };
            using Process? process = Process.Start(info);
        }
    }
}
