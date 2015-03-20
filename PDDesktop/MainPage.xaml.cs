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

using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.Storage;
using Windows.Storage.Pickers;

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

        private List<double> dAcc;

        Dictionary<double, int> histogramX;
        Dictionary<double, int> histogramY;
        Dictionary<double, int> histogramZ;

        Dictionary<double, int> histogram;

        public MainPage()
        {
            this.InitializeComponent();

            accX = new List<double>();
            accY = new List<double>();
            accZ = new List<double>();

            dAccX = new List<double>();
            dAccY = new List<double>();
            dAccZ = new List<double>();

            dAcc = new List<double>();

            histogramX = new Dictionary<double, int>();
            histogramY = new Dictionary<double, int>();
            histogramZ = new Dictionary<double, int>();
            histogram = new Dictionary<double, int>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HideGraphElements();
        }

        private void HideGraphElements()
        {
            XLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            YLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AxisLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            AxisSelector.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            WindowLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            WindowSelector.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            XAxisStart.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            XAxisEnd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            YAxisStart.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            YAxisEnd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            WindowSizeSelector.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            WindowSizeLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            GraphSelector.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            GraphSelectorBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ShowGraphElements()
        {
            XLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            YLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AxisSelector.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AxisLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            WindowSelector.Visibility = Windows.UI.Xaml.Visibility.Visible;
            WindowLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            XAxisStart.Visibility = Windows.UI.Xaml.Visibility.Visible;
            XAxisEnd.Visibility = Windows.UI.Xaml.Visibility.Visible;
            YAxisStart.Visibility = Windows.UI.Xaml.Visibility.Visible;
            YAxisEnd.Visibility = Windows.UI.Xaml.Visibility.Visible;
            WindowSizeSelector.Visibility = Windows.UI.Xaml.Visibility.Visible;
            WindowSizeLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            GraphSelector.Visibility = Windows.UI.Xaml.Visibility.Visible;
            GraphSelectorBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async void ReadDataFile(StorageFile dataFile)
        {
            accX.Clear();
            accY.Clear();
            accZ.Clear();

            IList<string> lines = await FileIO.ReadLinesAsync(dataFile);

            foreach (var line in lines)
            {

                string data = line.ToString();
                string[] cols = data.Split(';');

                double x = double.Parse(cols[0], System.Globalization.CultureInfo.InvariantCulture);
                double y = double.Parse(cols[1], System.Globalization.CultureInfo.InvariantCulture);
                double z = double.Parse(cols[2], System.Globalization.CultureInfo.InvariantCulture);

                if (x > 1.0 || x < -1.0 || y > 1.0 || y < -1.0 || z > 1.0 || z < -1.0)
                {
                    continue;
                }
                
                accX.Add(x);
                accY.Add(y);
                accZ.Add(z);
            }

            CalculateDelta();
            CalculateHistogram();

            CreateWindows();

            RefreshGraph();
        }

        private void CalculateDelta()
        {
            int length = accX.Count();

            dAcc.Clear();
            dAccX.Clear();
            dAccY.Clear();
            dAccZ.Clear();

            dAccX.Add(0);
            dAccY.Add(0);
            dAccZ.Add(0);
            dAcc.Add(0);

            for (int i = 1; i < length; i++)
            {
                double dx = Math.Abs(accX[i] - accX[i - 1]);
                double dy = Math.Abs(accY[i] - accY[i - 1]);
                double dz = Math.Abs(accZ[i] - accZ[i - 1]);

                dAccX.Add(dx);
                dAccY.Add(dy);
                dAccZ.Add(dz);

                dAcc.Add((dx + dy + dz) / 3.0);
            }
        }

        private void CalculateHistogram()
        {
            histogramX.Clear();
            histogramY.Clear();
            histogramZ.Clear();
            histogram.Clear();

            int length = dAccX.Count();

            for (int i = 0; i < length; i++)
            {
                double dx = double.Parse(dAccX[i].ToString("0.000"));
                double dy = double.Parse(dAccY[i].ToString("0.000"));
                double dz = double.Parse(dAccZ[i].ToString("0.000"));
                double d = double.Parse(dAcc[i].ToString("0.000"));

                if (histogramX.ContainsKey(dx))
                {
                    histogramX[dx]++;
                }
                else
                {
                    histogramX[dx] = 1;
                }

                if (histogramY.ContainsKey(dy))
                {
                    histogramY[dy]++;
                }
                else
                {
                    histogramY[dy] = 1;
                }

                if (histogramZ.ContainsKey(dz))
                {
                    histogramZ[dz]++;
                }
                else
                {
                    histogramZ[dz] = 1;
                }

                if (histogram.ContainsKey(d))
                {
                    histogram[d]++;
                }
                else
                {
                    histogram[d] = 1;
                }
            }
        }

        private void CreateWindows()
        {
            WindowSelector.Items.Clear();

            int windowSize = (WindowSizeSelector.SelectedIndex + 1) * 100;

            int numWindows = (dAccX.Count() / windowSize) + 1;

            for (int i = 0; i < numWindows; i++)
            {
                int begin = i * windowSize / 10;
                int end = begin + windowSize / 10;
                WindowSelector.Items.Add(begin.ToString() + " - " + end.ToString());
            }

            WindowSelector.SelectedIndex = 0;
        }

        private void DrawGraph()
        {
            ShowGraphElements();

            int axis = AxisSelector.SelectedIndex;

            int windowSize = (WindowSizeSelector.SelectedIndex + 1) * 100;

            List<double> readings;

            switch (axis)
            {
                case 0:
                    readings = dAccX;
                    break;
                case 1:
                    readings = dAccY;
                    break;
                case 2:
                    readings = dAccZ;
                    break;
                case 3:
                    readings = dAcc;
                    break;
                default:
                    return;
            }

            double x1, x2, y1, y2;
            y1 = GraphBox.Height;

            double maxReading = readings.Max();

            double yScale = GraphBox.Height / maxReading;
            double xScale = GraphBox.Width / windowSize;

            int length = readings.Count();

            GraphBox.Children.Clear();

            int start = WindowSelector.SelectedIndex * windowSize;
            int cnt = 0;

            XAxisStart.Text = start.ToString();
            XAxisEnd.Text = ((start + windowSize) / 10).ToString();
            YAxisStart.Text = "0";
            YAxisEnd.Text = maxReading.ToString();

            XLabel.Text = "Time (in seconds)";
            YLabel.Text = "Change in Sensor Reading";

            for (int i = start; i < length; i++)
            {
                if (cnt >= windowSize)
                {
                    break;
                }

                double reading = readings[i];

                y2 = y1 - reading * yScale;
                x1 = cnt * xScale;
                x2 = cnt * xScale;

                Color graphColor;

                if (reading >= 0 && reading < maxReading / 10)
                {
                    graphColor = Colors.Red;
                }
                else if (reading >= maxReading / 10 && reading < 2 * maxReading / 10)
                {
                    graphColor = Colors.OrangeRed;
                }
                else if (reading >= 2 * maxReading / 10 && reading < 3 * maxReading / 10)
                {
                    graphColor = Colors.Yellow;
                }
                else if (reading >= 3 * maxReading / 10 && reading < 4 * maxReading / 10)
                {
                    graphColor = Colors.LightYellow;
                }
                else if (reading >= 4 * maxReading / 10 && reading <= 5 * maxReading / 10)
                {
                    graphColor = Colors.YellowGreen;
                }
                else if (reading >= 5 * maxReading / 10 && reading <= 6 * maxReading / 10)
                {
                    graphColor = Colors.Lime;
                }
                else if (reading >= 6 * maxReading / 10 && reading <= 7 * maxReading / 10)
                {
                    graphColor = Colors.Green;
                }
                else if (reading >= 7 * maxReading / 10 && reading <= 8 * maxReading / 10)
                {
                    graphColor = Colors.DarkGreen;
                }
                else if (reading >= 8 * maxReading / 10 && reading <= 9 * maxReading / 10)
                {
                    graphColor = Colors.Blue;
                }
                else if (reading >= 9 * maxReading / 10 && reading <= maxReading)
                {
                    graphColor = Colors.DarkBlue;
                }

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

        private void DrawHistogram()
        {
            ShowGraphElements();

            int axis = AxisSelector.SelectedIndex;

            Dictionary<double, int> readings;

            Color graphColor;

            switch (axis)
            {
                case 0:
                    readings = histogramX;
                    graphColor = Colors.Red;
                    break;
                case 1:
                    readings = histogramY;
                    graphColor = Colors.Green;
                    break;
                case 2:
                    readings = histogramZ;
                    graphColor = Colors.Blue;
                    break;
                case 3:
                    readings = histogram;
                    graphColor = Colors.Yellow;
                    break;
                default:
                    return;
            }

            double x1, x2, y1, y2;
            y1 = GraphBox.Height;
            
            double maxKey = 1.0;

            if (readings.Keys.Max() > 1.0)
            {
                maxKey = 2.0;
            }
            
            double maxValue = readings.Values.Max();

            double yScale = GraphBox.Height / maxValue;
            double xScale = GraphBox.Width / maxKey;

            GraphBox.Children.Clear();

            XAxisStart.Text = "0";
            YAxisStart.Text = "0";
            XAxisEnd.Text = maxKey.ToString();
            YAxisEnd.Text = maxValue.ToString();

            XLabel.Text = "Change in Sensor Reading";
            YLabel.Text = "Frequency";

            foreach (KeyValuePair<double, int> reading in readings)
            {
                x1 = reading.Key * xScale;
                x2 = reading.Key * xScale;
                y2 = y1 - reading.Value * yScale;

                Line line = new Line();
                line.Stroke = new SolidColorBrush(graphColor);

                line.X1 = x1;
                line.X2 = x2;
                line.Y1 = y1;
                line.Y2 = y2;

                GraphBox.Children.Add(line);
            }
        }

        private void DrawCumulativeHistogram()
        {
            ShowGraphElements();

            int axis = AxisSelector.SelectedIndex;

            Dictionary<double, int> readings;

            Color graphColor;

            switch (axis)
            {
                case 0:
                    readings = histogramX;
                    graphColor = Colors.Red;
                    break;

                case 1:
                    readings = histogramY;
                    graphColor = Colors.Green;
                    break;

                case 2:
                    readings = histogramZ;
                    graphColor = Colors.Blue;
                    break;

                case 3:
                    readings = histogram;
                    graphColor = Colors.Yellow;
                    break;

                default:
                    return;
            }

            double x1, x2, y1, y2;
            y1 = GraphBox.Height;

            double maxValue = accX.Count();

            double maxKey = 1.0;

            if (readings.Keys.Max() > 1.0) 
            {
                maxKey = 2.0;
            }

            double yScale = GraphBox.Height / maxValue;
            double xScale = GraphBox.Width / maxKey;

            GraphBox.Children.Clear();

            XAxisStart.Text = "0";
            YAxisStart.Text = "0";
            XAxisEnd.Text = maxKey.ToString();
            YAxisEnd.Text = maxValue.ToString();

            XLabel.Text = "Change in Sensor Reading";
            YLabel.Text = "Density";

            int sum = 0;
            int end = (int)(maxKey * 1000);

            for (int c = 0; c <= end; c++)
            {
                double i = (double)c / 1000.0;
                
                if (readings.ContainsKey(i))
                {
                    sum += readings[i];
                }

                x1 = i * xScale;
                x2 = i * xScale;
                y2 = y1 - sum * yScale;

                Line line = new Line();
                line.Stroke = new SolidColorBrush(graphColor);

                line.X1 = x1;
                line.X2 = x2;
                line.Y1 = y1;
                line.Y2 = y2;

                GraphBox.Children.Add(line);
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

        private void RefreshGraph()
        {
            try
            {
                switch (GraphSelectorBox.SelectedIndex)
                {
                    case 0:
                        DrawGraph();
                        break;
                    case 1:
                        DrawHistogram();
                        break;
                    case 2:
                        DrawCumulativeHistogram();
                        break;
                    default:
                        return;
                }
            }
            catch (Exception) { };
        }

        private void AxisSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshGraph();
        }

        private void WindowSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshGraph();
        }

        private void WindowSize_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CreateWindows();
            }
            catch (Exception) { }

            RefreshGraph();
        }

        private void GraphSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshGraph();
        }
    }
}
