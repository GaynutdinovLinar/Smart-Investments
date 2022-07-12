using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Smart_Investments.ViewsModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Smart_Investments.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для StockPage.xaml
    /// </summary>
    public partial class StockPage : Page
    {
        public StockPage()
        {
            InitializeComponent();
        }

        private StockPageViewModel _dataContext;
        private CartesianChart _chart;
        private Canvas _graphPlottingArea;
        private FrameworkElement _cursorXToolTip;
        private Line _cursorX;

        private bool firstEnter = true;

        private void ListBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(ListBox, (DependencyObject)e.OriginalSource) as ListBoxItem;
            if (item == null) return;

            var series = (Series)item.Content;
            series.Visibility = series.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        private void LineChart_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_dataContext.SelectedChartPoint != null)
            {
                Point chartMousePosition = e.GetPosition(_chart); //Получаем точку курсора

                Point chartPoint = _chart.ConvertToChartValues(chartMousePosition);

                for (int i = 3, j = 0; i < _chart.Series.Count && j < _dataContext.SelectedChartPoint.Count; i++)
                {
                    if (_chart.Series[i].Values?.Count > 0)
                    {
                        if (_chart.Series[i].Title == _dataContext.SelectedChartPoint[j].SeriesView.Title)
                        {
                            _dataContext.SelectedChartPoint?[j].View?.OnHoverLeave(_dataContext.SelectedChartPoint[j]);
                            _dataContext.SelectedChartPoint[j] = _chart.Series[i].ClosestPointTo(chartPoint.X, AxisOrientation.X);
                            _dataContext.SelectedChartPoint?[j].View?.OnHover(_dataContext.SelectedChartPoint[j]);
                            j++;
                        }
                    }
                }


                Point canvasMousePosition = e.GetPosition(_graphPlottingArea);

                if (canvasMousePosition.X >= _graphPlottingArea.ActualWidth - _cursorXToolTip.ActualWidth) Canvas.SetLeft(_cursorXToolTip, canvasMousePosition.X - _cursorXToolTip.ActualWidth);
                else Canvas.SetLeft(_cursorXToolTip, canvasMousePosition.X);

                Canvas.SetTop(_cursorXToolTip, canvasMousePosition.Y);
            }
        }

        private bool TryFindVisualChildElement<TChild>(DependencyObject parent, out TChild resultElement)
              where TChild : DependencyObject
        {
            resultElement = null;
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(parent); childIndex++)
            {
                DependencyObject childElement = VisualTreeHelper.GetChild(parent, childIndex);

                if (childElement is Popup popup)
                {
                    childElement = popup.Child;
                }

                if (childElement is TChild)
                {
                    resultElement = childElement as TChild;
                    return true;
                }

                if (TryFindVisualChildElement(childElement, out resultElement))
                {
                    return true;
                }
            }

            return false;
        }

        private void LineChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _cursorX.Visibility = Visibility.Hidden;
            _cursorXToolTip.Visibility = Visibility.Hidden;
        }

        private void LineChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _dataContext = (StockPageViewModel) DataContext;

            _chart = sender as CartesianChart;

            if (firstEnter)
            {
                if (!TryFindVisualChildElement(_chart, out Canvas outerCanvas) ||
                !TryFindVisualChildElement(outerCanvas, out Canvas graphPlottingArea))
                {
                    return;
                }

                _graphPlottingArea = graphPlottingArea;

                if (_chart.TryFindResource("CursorX") is Line cursorX && !_graphPlottingArea.Children.Contains(cursorX))
                {
                    _cursorX = cursorX;
                    _graphPlottingArea.Children.Add(_cursorX);
                    _cursorX.Visibility = Visibility.Visible;
                }

                if (!(_chart.TryFindResource("CursorXToolTip") is FrameworkElement cursorXToolTip))
                {
                    return;
                }

                _cursorXToolTip = cursorXToolTip;

                if (!_graphPlottingArea.Children.Contains(_cursorXToolTip))
                {
                    _graphPlottingArea.Children.Add(_cursorXToolTip);
                    _cursorXToolTip.Visibility = Visibility.Visible;
                }

                firstEnter = false;
            }
            else
            {
                _cursorX.Visibility = Visibility.Visible;
                _cursorXToolTip.Visibility = Visibility.Visible;
            }

            _dataContext.SelectedChartPoint = new System.Collections.ObjectModel.ObservableCollection<ChartPoint>();

            Point chartMousePosition = e.GetPosition(_chart); //Получаем точку курсора

            Point chartPoint = _chart.ConvertToChartValues(chartMousePosition);

            for (int i = 3; i < _chart.Series.Count; i++)
            {
                if (_chart.Series[i].Values?.Count > 0 && _chart.Series[i].IsSeriesVisible) _dataContext.SelectedChartPoint.Add(_chart.Series[i].ClosestPointTo(chartPoint.X, AxisOrientation.X));
            }
        }
    }
}
