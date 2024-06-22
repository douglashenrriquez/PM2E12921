using PM0220242P.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace PM0220242P.Views
{
    public partial class PageListSitios : ContentPage
    {
        public ObservableCollection<Sitios> SitiosCollection { get; set; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand MapaCommand { get; }

        private int _clickCount = 0;
        private const int ClickDelay = 300;

        public PageListSitios()
        {
            InitializeComponent();

            DeleteCommand = new Command<Sitios>(async (sitio) => await DeleteSitio(sitio));
            EditCommand = new Command<Sitios>(async (sitio) => await EditSitio(sitio));
            MapaCommand = new Command<Sitios>(async (sitio) => await OpenMap(sitio));

            BindingContext = this;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Views.PageInit page = new PageInit();
            await Navigation.PushAsync(page);
        }

        private async void OnItemTapped(object sender, EventArgs e)
        {
            _clickCount++;
            if (_clickCount == 2)
            {
                _clickCount = 0;
                if (sender is VisualElement element && element.BindingContext is Sitios selectedSitio)
                {
                    bool answer = await DisplayAlert("Abrir Mapa", "¿Desea abrir el mapa para este sitio?", "Sí", "No");
                    if (answer)
                    {

                            await Navigation.PushAsync(new PageMap(selectedSitio.Latitud, selectedSitio.Longitud, selectedSitio.Descripcion, selectedSitio.Foto));
                      
                    }
                }
            }
            else
            {
                await Task.Delay(ClickDelay);
                _clickCount = 0;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var sitiosList = await App.DataBase.GetListSitios();
            SitiosCollection = new ObservableCollection<Sitios>(sitiosList);
            listSitios.ItemsSource = SitiosCollection;
        }

        private async Task DeleteSitio(Sitios sitio)
        {
            if (await DisplayAlert("Confirmación", $"¿Está seguro de que desea eliminar {sitio.Descripcion}?", "Sí", "No"))
            {
                await App.DataBase.DeleteSitio(sitio);
                SitiosCollection.Remove(sitio);
            }
        }

        private async Task EditSitio(Sitios sitio)
        {
            Views.PageInit page = new PageInit(sitio);
            await Navigation.PushAsync(page);
        }

        private async Task OpenMap(Sitios sitio)
        {
            await Navigation.PushAsync(new PageMap(sitio.Latitud, sitio.Longitud, sitio.Descripcion, sitio.Foto));
        }
    }
}
