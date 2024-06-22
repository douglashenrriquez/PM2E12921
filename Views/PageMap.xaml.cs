using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PM0220242P.Views
{
    public partial class PageMap : ContentPage
    {
        private double latitude;
        private double longitude;
        private string description;
        private string fotoBase64;

        public PageMap(double latitude, double longitude, string description, string fotoBase64)
        {
            InitializeComponent();

            this.latitude = latitude;
            this.longitude = longitude;
            this.description = description;
            this.fotoBase64 = fotoBase64;

            var pin = new Pin
            {
                Label = description,
                Address = description,
                Type = PinType.Place,
                Location = new Location(latitude, longitude)
            };


            pin.MarkerClicked += OnPinClicked;

            map.Pins.Add(pin);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(latitude, longitude), Distance.FromMiles(1)));
        }

        private async void OnPinClicked(object sender, PinClickedEventArgs e)
        {
            e.HideInfoWindow = true;
            var pin = sender as Pin;
            if (pin != null)
            {
                await DisplayAlert("Pin Information", $"{pin.Label}", "OK");
            }
        }

        private async void ShareButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var imageSource = ConvertBase64ToImageSource(fotoBase64);

                if (imageSource != null)
                {
                    var imagePath = await SaveImageAsync(imageSource);

                    var shareTitle = $"Foto de {description} en ({latitude}, {longitude})";
                    var shareMessage = $"Compartiendo foto de {description} en la ubicación ({latitude}, {longitude})";

                    await DisplayAlert("Compartir Foto", shareMessage, "OK");

                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = shareTitle,
                        File = new ShareFile(imagePath),
                    });
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo convertir la imagen para compartir.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al compartir: {ex.Message}", "OK");
            }
        }

        private async Task<string> SaveImageAsync(ImageSource imageSource)
        {
            var fileName = $"temp_image_{Guid.NewGuid()}.png";

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                var stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
                await stream.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            var imagePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            File.WriteAllBytes(imagePath, imageBytes);

            return imagePath;
        }

        private ImageSource ConvertBase64ToImageSource(string base64String)
        {
            ImageSource imageSource = null;

            if (!string.IsNullOrEmpty(base64String))
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                var stream = new MemoryStream(imageBytes);
                imageSource = ImageSource.FromStream(() => stream);
            }

            return imageSource;
        }

        private void NavigateButton_Clicked(object sender, EventArgs e)
        {
            var searchQuery = $"{latitude},{longitude}";
            var searchUrl = $"https://www.google.com/maps/search/?api=1&query={searchQuery}";

            Launcher.OpenAsync(new Uri(searchUrl));
        }
    }
}