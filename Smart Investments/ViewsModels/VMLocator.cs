using Microsoft.Extensions.DependencyInjection;
using Smart_Investments.Models;
using Smart_Investments.Services;
using Smart_Investments.Services.DataMarket;
using Smart_Investments.Services.LocationDataBaseService;
using Smart_Investments.Services.PageService;
using Smart_Investments.Views.DragDropFile;
using Smart_Investments.Views.Pages;

namespace Smart_Investments.ViewsModels
{
    internal class VMLocator
    {
        private static ServiceProvider _provider;

        public static void Init()
        {
            var services = new ServiceCollection();

            services.AddTransient<BackPageService>();

            services.AddScoped<BagPagesStack>();
            services.AddScoped<MarketPagesStack>();

            services.AddSingleton<MainViewModel>();
            services.AddTransient<StockPageModel>();

            services.AddScoped<MarketPageViewModel>();
            services.AddScoped<MarketPageModel>();
            services.AddScoped<BagPageModel>();
            services.AddScoped<BagPageViewModel>();

            services.AddTransient<StockPageViewModel>();

            services.AddScoped<StocksCollection>();

            services.AddTransient<DragDropDepositsAndDebits>();
            services.AddTransient<DragDropSdelki>();

            services.AddSingleton<ExcelReaderService>();
            services.AddSingleton<TableOperations>();
            services.AddSingleton<TableDepositsAndDebits>();
            services.AddSingleton<LocationDataBase>();
            services.AddSingleton<ExceptionService>();
            services.AddSingleton<BoardsCollection>();
            services.AddSingleton<PagesService>();
            services.AddSingleton<ThemeSwitchService>();
            services.AddSingleton<LoadService>();
            services.AddSingleton<LoadPage>();


            _provider = services.BuildServiceProvider();

            //foreach (var item in services)
            //{
            //    _provider.GetRequiredService(item.ServiceType);
            //}
        }

        public MainViewModel MainViewModel => _provider.GetRequiredService<MainViewModel>();

        public MarketPageViewModel MarketPageViewModel => _provider.GetRequiredService<MarketPageViewModel>();

        public BagPageViewModel BagPageViewModel => _provider.GetRequiredService<BagPageViewModel>();

        public BackPageService BackPageService => _provider.GetRequiredService<BackPageService>();

        public StockPageViewModel StockPageViewModel => _provider.GetRequiredService<StockPageViewModel>();

        public DragDropSdelki DragDropSdelki => _provider.GetRequiredService<DragDropSdelki>();

        public DragDropDepositsAndDebits DragDropDepositsAndDebits => _provider.GetRequiredService<DragDropDepositsAndDebits>();
    }
}
