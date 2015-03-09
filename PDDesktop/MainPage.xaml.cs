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
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace PDDesktop
{
    public sealed partial class MainPage : Page
    {

        private List<double> accX;
        private List<double> accY;
        private List<double> accZ;

        private List<double> dAccX;
        private List<double> dAccY;
        private List<double> dAccZ;

        public MainPage()
        {
            this.InitializeComponent();

            accX = new List<double>();
            accY = new List<double>();
            accZ = new List<double>();

            dAccX = new List<double>();
            dAccY = new List<double>();
            dAccZ = new List<double>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            XLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            YLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AxisLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AxisSelector.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void ReadDataFile(StorageFile dataFile)
        {
            // Parse the data file
            IList<string> lines = await FileIO.ReadLinesAsync(dataFile);

            foreach (var line in lines)
            {

                string data = line.ToString();
                string[] cols = data.Split(';');

                accX.Add(double.Parse(cols[0], System.Globalization.CultureInfo.InvariantCulture));
                accY.Add(double.Parse(cols[1], System.Globalization.CultureInfo.InvariantCulture));
                accZ.Add(double.Parse(cols[2], System.Globalization.CultureInfo.InvariantCulture));
            }

            CalculateDelta();
            DisplayData();
            DrawGraph();
        }

        private void CalculateDelta()
        {
            int length = accX.Count();

            dAccX.Add(0);
            dAccY.Add(0);
            dAccZ.Add(0);

            for (int i = 1; i < length; i++)
            {
                dAccX.Add(Math.Abs(accX[i] - accX[i - 1]));
                dAccY.Add(Math.Abs(accY[i] - accY[i - 1]));
                dAccZ.Add(Math.Abs(accZ[i] - accZ[i - 1]));
            }
        }

        private void DisplayData()
        {
            int length = accX.Count();

            string content = "";

            for (int i = 0; i < length && i < 40; i++)
            {
                content += "dX:" + dAccX[i].ToString() + " dY:" + dAccY[i].ToString() + " dZ:" + dAccZ[i].ToString() + "\n";
            }

            DataFileContent.Text = content;
        }

        private void DrawGraph()
        {
            XLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            YLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AxisSelector.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AxisLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;

            int axis = AxisSelector.SelectedIndex;

            Color graphColor;
            List<double> readings;

            switch (axis)
            {
                case 0:
                    readings = dAccX;
                    graphColor = Colors.Red;
                    break;
                case 1:
                    readings = dAccY;
                    graphColor = Colors.Green;
                    break;
                case 2:
                    readings = dAccZ;
                    graphColor = Colors.Blue;
                    break;
                default:
                    return;
            }

            double x1, x2, y1, y2;
            y1 = GraphBox.Height;

            double yScale = GraphBox.Height / readings.Max();
            double xScale = GraphBox.Width / 300;

            int length = readings.Count();

            int start = 0;
            int cnt = 0;

            GraphBox.Children.Clear();

            for (int i = start; i < length; i++)
            {

                if (cnt >= 300)
                {
                    break;
                }

                y2 = y1 - readings[i] * yScale;
                x1 = i * xScale;
                x2 = i * xScale;

                Line line = new Line();
                line.Stroke = new SolidColorBrush(graphColor);

                line.X1 = x1;
                line.X2 = x2;
                line.Y1 = y1;
                line.Y2 = y2;

                GraphBox.Children.Add(line);
                cnt++;
            }
        }

        private async void ChooseDataFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".csv");
            filePicker.SuggestedStartLocation = PickerLocationId.Desktop;

            var selectedFile = await filePicker.PickSingleFileAsync();

            if (selectedFile != null)
            {
                ReadDataFile(selectedFile);
            }
        }

        private void AxisSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DrawGraph();
            }
            catch (Exception ex) { }
        }
    }
}
