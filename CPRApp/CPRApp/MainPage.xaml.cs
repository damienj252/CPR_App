using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Calls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CPRApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Geolocator myGeo;
        Geoposition _pos;

        public MainPage()
        {
            this.InitializeComponent();
            addContent();

        }//MainPage()

        private void elInit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // get the locations services running
            // get a handle to the geolocator
            // once it's available, add the two events
            // status updates, position changes
            initialiseGeoLocation();

        }//Image tapped

        private async void initialiseGeoLocation()
        {
            var access = await Geolocator.RequestAccessAsync();
            switch (access)
            {
                case GeolocationAccessStatus.Allowed:
                    // set some stuff up now.
                    tblStatus.Text = "Setting up location services";
                    myGeo = new Geolocator();
                    myGeo.StatusChanged += MyGeo_StatusChanged;
                    //myGeo.PositionChanged not needed just now.
                    myGeo.DesiredAccuracy = PositionAccuracy.High;
                    break;
                case GeolocationAccessStatus.Denied:
                case GeolocationAccessStatus.Unspecified:
                    // ask the user for something here if things go wrong.
                    tblStatus.Text = "Can't access location services";
                    break;
                default:
                    break;
            }//switch
        }//initialise

        private async void MyGeo_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            // notify the user that something has happened.
            // event is raised in a background thread, so use CoreDispatcher
            // to update the ui
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    #region Switch(args.Status)

                    switch (args.Status)
                    {
                        case PositionStatus.Ready:
                            tblStatus.Text = "All good";
                            break;
                        case PositionStatus.Initializing:
                            tblStatus.Text = "Initialising";
                            break;
                        case PositionStatus.NoData:
                            tblStatus.Text = "No Data";
                            break;
                        case PositionStatus.Disabled:
                            tblStatus.Text = "disabled";
                            break;
                        case PositionStatus.NotInitialized:
                            tblStatus.Text = "Not Initalised";
                            break;
                        case PositionStatus.NotAvailable:
                            tblStatus.Text = "Not Available";
                            break;
                        default:
                            break;
                    } // end switch
                    #endregion
                });

        }//StatusCanged

        private async void elSavePosition_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // get the current location and show it in the stack panel
            // create three text boxes, put them in a stack panel and then 
            // add to the existing stack panel
            StackPanel sp;
            TextBlock tblLong, tbLat;

            // get the current locations
            try
            {
                _pos = await myGeo.GetGeopositionAsync();
            }//try
            catch (Exception ex)
            {
                tblStatus.Text = "Problem reading location" + ex.Message;
                return;

            }//catch

            sp = new StackPanel();
            sp.Margin = new Windows.UI.Xaml.Thickness(2);

            tbLat = new TextBlock();
            tbLat.FontSize = 32;

            tbLat.Text = "Lat:  " + _pos.Coordinate.Point.Position.Latitude.ToString();

            tblLong = new TextBlock();
            tblLong.FontSize = 32;
            tblLong.Text = "Long: " + _pos.Coordinate.Point.Position.Longitude.ToString();

            sp.Children.Add(tbLat);
            sp.Children.Add(tblLong);

            // add to the screen
            spLocations.Children.Add(sp);

        }//SavePosition


        private void addContent()
        {
           
            tblIntroduction.Text = "This app will show you how to use hands only CPR. CPR is a life saving tool that could actually save someone's life if they go into cardiac arrest." + System.Environment.NewLine +
                "Roughly 13 people every day die from cardiac arrest which adds up to some 5,000 people a year in Ireland alone yet if more people knew CPR some of these lives could have been saved." + System.Environment.NewLine +
                "Cardiac arrest can happen anywhere and to anyone from any age and if you are trained properly you could save someone’s life." + System.Environment.NewLine +
                "Citizen CPR can be taught in a total of 15 minutes yet you could save someone’s life in the future. These statistics were provided by the Irish Heart Foundation. ";

            tblTraining.Text = "Step 1: Check the surronding area for obstructions" + System.Environment.NewLine +
                "Step 2: Call emergency services" + System.Environment.NewLine + "" +
                "Step 3: Tap the persons shoulders and say 'Hello can you hear me', check to see if they are breathing by placing your ear above their mouth" + System.Environment.NewLine +
                "Step 4: If the person is unresponsive start doing compressions immediately, press hard and fast until emergency services arrive." + System.Environment.NewLine +
                "Press the play button to play the staying alive song and do compressions to the beat of the song";
        }//addContent()

        private void StayingAlive_Song_Click(object sender, RoutedEventArgs e)
        {
            stayingAlive.Play();
        }//StayingAlive_Play

        private void Stop_StayingAlive_Song_Click(object sender, RoutedEventArgs e)
        {
            stayingAlive.Stop();
        }//StayingAlive_Play

        private void callButton_Click(object sender, RoutedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.ApplicationModel.Calls.PhoneCallManager"))
            {
                PhoneCallManager.Equals("999", "Emergency Services");
            }
        }//call_Click()





    }//partial Class
}//CPRApp
