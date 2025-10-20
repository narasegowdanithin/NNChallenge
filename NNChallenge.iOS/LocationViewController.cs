using System;
using NNChallenge.Constants;
using NNChallenge.iOS.ViewModel;
using UIKit;

namespace NNChallenge.iOS
{
    public partial class LocationViewController : UIViewController
    {
        public LocationViewController() : base("LocationViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = "Location";
            _submitButton.TitleLabel.Text = "Submit";
            _contentLabel.Text = "Select your location.";
            _submitButton.TouchUpInside += SubmitButtonTouchUpInside;

            _picker.Model = new LocationPickerModel(LocationConstants.LOCATIONS);
        }

        private void SubmitButtonTouchUpInside(object sender, EventArgs e)
        {
            var selectedIndex = (int)_picker.SelectedRowInComponent(0);
    
            // Get the selected city from LocationConstants
            if (selectedIndex >= 0 && selectedIndex < LocationConstants.LOCATIONS.Length)
            {
                var selectedCity = LocationConstants.LOCATIONS[selectedIndex];

                // Create ForecastViewController and pass the selected city
                var forecastView = new ForecastViewController();
                forecastView.SetSelectedCity(selectedCity);
                
                // Navigate to forecast screenif (NavigationController != null)
                if (NavigationController != null)
                {
                    NavigationController.PushViewController(forecastView , true);
                }
            }
            else
            {
                // Show error if no city selected
                var alert = UIAlertController.Create("Error", "Please select a city", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);
            }
        }
        
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

