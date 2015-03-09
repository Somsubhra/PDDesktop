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

        private List<double> accX;
        private List<double> accY;
        private List<double> accZ;

        public MainPage()
        {
            this.InitializeComponent();

            accX = new List<double>();
            accY = new List<double>();
            accZ = new List<double>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private async void readDataFile(StorageFile dataFile) {
            // Parse the data file
            IList<string> lines = await FileIO.ReadLinesAsync(dataFile);

            foreach(var line in lines) {

                string data = line.ToString();
                string[] cols = data.Split(';');

                accX.Add(double.Parse(cols[0], System.Globalization.CultureInfo.InvariantCulture));
                accY.Add(double.Parse(cols[1], System.Globalization.CultureInfo.InvariantCulture));
                accZ.Add(double.Parse(cols[2], System.Globalization.CultureInfo.InvariantCulture));
            }

            var length = accX.Count();

            string content = "";

            for (int i = 0; i < length; i++)
            {
                if (i > 40)
                {
                    break;
                }

                content += "X:" + accX[i].ToString() + " Y:" + accY[i].ToString() + " Z:" + accZ[i].ToString() + "\n";
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
