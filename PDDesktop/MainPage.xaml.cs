using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Storage.Pickers;
using Windows.Storage;

namespace PDDesktop
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private async void readDataFile(StorageFile dataFile) {
            // Parse the data file
            IList<string> lines = await FileIO.ReadLinesAsync(dataFile);

            string content = "";
            var itr = 0;
            foreach(var line in lines) {

                if (itr > 38) {
                    break;
                }

                content += line.ToString() + "\n";
                itr++;
            }

            DataFileContent.Text = content;
        }

        private async void ChooseDataFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".csv");
            filePicker.SuggestedStartLocation = PickerLocationId.Desktop;

            var selectedFiles = await filePicker.PickSingleFileAsync();

            if (selectedFiles != null) {
                readDataFile(selectedFiles);
            }
        }
    }
}
