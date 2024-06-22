using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using PM0220242P.Models;
using PM0220242P.Controllers;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;

namespace PM0220242P.Views
{
    public partial class PageInit : ContentPage
    {
        private Microsoft.Maui.Storage.FileResult photo;
        private Sitios _sitioExistente;

        public PageInit()
        {
            InitializeComponent();
            CargarDatosIniciales();
        }

        public PageInit(Sitios sitio) : this()
        {
            _sitioExistente = sitio;
            LatitudLabel.Text = _sitioExistente.Latitud.ToString();
            LongitudLabel.Text = _sitioExistente.Longitud.ToString();
            Descripcion.Text = _sitioExistente.Descripcion;

            if (!string.IsNullOrEmpty(_sitioExistente.Foto))
            {
                FotoImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(_sitioExistente.Foto)));
            }
        }

        private async void CargarDatosIniciales()
        {
            if (_sitioExistente == null)
            {
                double latitud = await ObtenerLatitudAsync();
                double longitud = await ObtenerLongitudAsync();
                LatitudLabel.Text = latitud.ToString();
                LongitudLabel.Text = longitud.ToString();
            }
            else
            {
                LatitudLabel.Text = _sitioExistente.Latitud.ToString();
                LongitudLabel.Text = _sitioExistente.Longitud.ToString();
                Descripcion.Text = _sitioExistente.Descripcion;

                if (!string.IsNullOrEmpty(_sitioExistente.Foto))
                {
                    FotoImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(_sitioExistente.Foto)));
                }
            }
        }

        private async void btnaceptar_Clicked(object sender, EventArgs e)
        {
            double latitud = await ObtenerLatitudAsync();
            double longitud = await ObtenerLongitudAsync();

            var sitio = new Sitios
            {
                Id = _sitioExistente?.Id ?? 0,
                Latitud = latitud,
                Longitud = longitud,
                Descripcion = Descripcion.Text,
                Foto = GetImageBase64()
            };

            SitiosController sitiosController = new SitiosController();

            if (_sitioExistente == null)
            {
                if (await sitiosController.StoreSitio(sitio) > 0)
                {
                    await DisplayAlert("Aviso", "Registro ingresado con éxito!!", "OK");
                    await Navigation.PopAsync();
                }
            }
            else
            {
                if (await sitiosController.StoreSitio(sitio) > 0)
                {
                    await DisplayAlert("Aviso", "Registro actualizado con éxito!!", "OK");
                    await Navigation.PopAsync();
                }
            }
        }

        private async void btnfoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    string localizacion = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using Stream sourceStream = await photo.OpenReadAsync();
                    using FileStream imagenlocal = File.OpenWrite(localizacion);

                    FotoImage.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result);

                    await sourceStream.CopyToAsync(imagenlocal);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task<double> ObtenerLatitudAsync()
        {
            return await ObtenerCoordenadaAsync(coord => coord.Latitude);
        }

        private async Task<double> ObtenerLongitudAsync()
        {
            return await ObtenerCoordenadaAsync(coord => coord.Longitude);
        }

        private async Task<double> ObtenerCoordenadaAsync(Func<Location, double> obtenerCoordenada)
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });

                    if (location == null)
                    {
                        await MostrarAlertaUbicacionDesactivada();
                    }
                }

                return obtenerCoordenada(location);
            }
            catch (FeatureNotSupportedException)
            {
                Console.WriteLine("Geolocalización no soportada");
            }
            catch (PermissionException)
            {
                Console.WriteLine("Permiso denegado para la ubicación");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la ubicación: {ex.Message}");
                await MostrarAlertaUbicacionDesactivada();
            }

            return 0.0;
        }

        private async Task MostrarAlertaUbicacionDesactivada()
        {
            var activarUbicacion = await DisplayAlert("Ubicación desactivada", "La ubicación está desactivada o no se puede obtener la ubicación. ¿Desea activarla?", "OK", "Cancelar");
            if (activarUbicacion)
            {
                await Navigation.PopAsync();
            }
        }

        private string GetImageBase64()
        {
            string base64String = string.Empty;

            if (photo != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Stream stream = photo.OpenReadAsync().Result;
                    stream.CopyTo(ms);
                    byte[] data = ms.ToArray();

                    base64String = Convert.ToBase64String(data);
                }
            }

            return base64String;
        }

        private void SearchLocation_Clicked(object sender, EventArgs e)
        {
            var searchQuery = SearchBar.Text;
            var searchUrl = $"https://www.google.com/maps/search/?api=1&query={searchQuery}";

            Microsoft.Maui.ApplicationModel.Launcher.OpenAsync(new Uri(searchUrl));
        }
    }
}
