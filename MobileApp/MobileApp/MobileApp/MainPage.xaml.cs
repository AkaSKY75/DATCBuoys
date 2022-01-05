using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using static Android.Provider.Settings;

namespace MobileApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var map = new Xamarin.Forms.Maps.Map(MapSpan.FromCenterAndRadius(new Position(45.9442858, 25.0094303), Distance.FromMiles(200)));
            string id = Android.OS.Build.Serial;
            try
            {
                var context = Android.App.Application.Context;
                id = Secure.GetString(context.ContentResolver, Secure.AndroidId);
            }
            catch (Exception ex)
            {
                Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
            }

            /*var pin = new Pin()
            {
                Position = new Position(37, -122),
                Label = "Some Pin!"
            };
            map.Pins.Add(pin);*/

            var cp = this.FindByName<ContentPage>("ContentPage");
            cp.Content = map;
            try
            {

                var location = Task.Run(async () => { return await Geolocation.GetLastKnownLocationAsync(); }).GetAwaiter().GetResult();

                if (location != null)
                {
                    System.Diagnostics.Debug.WriteLine($"\n\n\n\n\nLatitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}\n\n\n\n\n");
                }
            }
            /*catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }*/
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("\n\n\n\n\n"+ex+"\n\n\n\n\n");
                // Unable to get location
            }
        }
    }
}
