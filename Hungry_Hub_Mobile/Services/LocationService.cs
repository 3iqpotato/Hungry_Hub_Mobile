using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.Services;

public class LocationService : ILocationService
{
    public async Task<string> GetCurrentAddressAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("👉 Проверка на permissions...");

            // Провери дали имаме permission
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                System.Diagnostics.Debug.WriteLine("👉 Няма permission, искаме...");
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status != PermissionStatus.Granted)
            {
                System.Diagnostics.Debug.WriteLine("❌ Потребителят отказа permission");
                return null;
            }

            System.Diagnostics.Debug.WriteLine("✅ Имаме permission, взимаме локация...");

            // Вземи текущата локация
            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Best,
                Timeout = TimeSpan.FromSeconds(30)
            });   

            if (location == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ Не може да вземе локация");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"📍 Координати: {location.Latitude}, {location.Longitude}");

            // Превърни координатите в адрес
            var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
            var placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                var address = string.Join(", ", new[]
                {
                    placemark.Thoroughfare,
                    placemark.SubThoroughfare,
                    placemark.Locality,
                    placemark.AdminArea,
                    placemark.CountryName
                }.Where(s => !string.IsNullOrWhiteSpace(s)));

                System.Diagnostics.Debug.WriteLine($"🏠 Адрес: {address}");
                return address;
            }

            // Ако няма адрес, върни координатите като текст
            return $"{location.Latitude}, {location.Longitude}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Грешка при взимане на локация: {ex.Message}");
            return null;
        }
    }

    public async Task<(double Latitude, double Longitude)?> GetCurrentCoordinatesAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status != PermissionStatus.Granted)
                return null;

            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(30)
            });

            if (location == null)
                return null;

            return (location.Latitude, location.Longitude);
        }
        catch
        {
            return null;
        }
    }
}