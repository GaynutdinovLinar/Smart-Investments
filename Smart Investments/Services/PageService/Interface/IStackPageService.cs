using System.Windows.Controls;

namespace Smart_Investments.Services.PageService.Interface
{
    interface IStackPageService
    {
        public Page GetPage();

        public void AddPage(Page page);

        public void RemovePage();
    }
}
