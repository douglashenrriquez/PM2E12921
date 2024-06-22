using Microsoft.Maui.Controls;

namespace PM0220242P
{
    public partial class App : Application
    {
        static Controllers.SitiosController dbSitios;

        public static Controllers.SitiosController DataBase
        {
            get
            {
                if (dbSitios == null)
                {
                    dbSitios = new Controllers.SitiosController();
                }
                return dbSitios;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Views.PageListSitios());
        }
    }
}
