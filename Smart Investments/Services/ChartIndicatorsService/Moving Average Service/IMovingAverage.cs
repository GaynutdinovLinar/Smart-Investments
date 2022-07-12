using LiveCharts;
using Smart_Investments.ViewsModels;
using System.Collections.Generic;

namespace Smart_Investments.Services.ChartIndicatorsService.Moving_Average_Service
{
    interface IMovingAverage
    {
        string Title { get; set; }
        ChartValues<ChartPointXY> GetValues(int interval, ICollection<ChartPointXY> data);
    }
}
