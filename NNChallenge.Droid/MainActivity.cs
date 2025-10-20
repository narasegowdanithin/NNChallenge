using _Microsoft.Android.Resource.Designer;
using Android.Content;
using AndroidX.AppCompat.App;
using NNChallenge.Constants;

namespace NNChallenge.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(ResourceConstant.Layout.activity_location);

            Button buttonForecst = FindViewById<Button>(Resource.Id.button_forecast);
            buttonForecst.Click += OnForecastClick;

            Spinner spinnerLocations = FindViewById<Spinner>(Resource.Id.spinner_location);

            ArrayAdapter<String> adapter = new ArrayAdapter<String>(
                this,
                Android.Resource.Layout.SimpleSpinnerDropDownItem,
                LocationConstants.LOCATIONS
            );

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            spinnerLocations.Adapter = adapter;
        }

        private void OnForecastClick(object sender, EventArgs e)
        {
            //this.StartActivity(new Intent(this, typeof(ForecastActivity)));

            var spinnerLocations = FindViewById<Spinner>(Resource.Id.spinner_location);
            var selectedCity = spinnerLocations?.SelectedItem?.ToString() ?? LocationConstants.LOCATIONS[0];
    
            var intent = new Intent(this, typeof(ForecastActivity));
            intent.PutExtra("SELECTED_CITY", selectedCity);
            this.StartActivity(intent);
        }
    }
}
