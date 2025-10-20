using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using NNChallenge.Interfaces;
using NNChallenge.Services;
using System;
using System.Threading.Tasks;
using Android.Graphics;
using System.Net;
using Android.Views;
using Android.Content;
using System.Linq;
using System.Collections.Generic;

namespace NNChallenge.Droid
{
    [Activity(Label = "Weather Forecast")]
    public class ForecastActivity : Activity
    {
        private RecyclerView _recyclerView;
        private ProgressBar _progressBar;
        private TextView _cityTextView;
        private TextView _cityNameTextView;
        private TextView _errorTextView;
        private Button _backButton;
        private TextView _headerDateTextView;
        private WeatherService _weatherService;
        private string _selectedCity;
        private List<IHourWeatherForecastVO> _allForecasts;
        private DateTime _currentTime;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_forecast);

            _currentTime = DateTime.Now;
            InitializeViews();
            _selectedCity = Intent.GetStringExtra("SELECTED_CITY") ?? "Berlin";
            _weatherService = new WeatherService();

            SetupEventHandlers();
            LoadWeatherData();
        }

        private void InitializeViews()
        {
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.forecastRecyclerView);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            _cityTextView = FindViewById<TextView>(Resource.Id.cityTextView);
            _cityNameTextView = FindViewById<TextView>(Resource.Id.cityNameTextView);
            _errorTextView = FindViewById<TextView>(Resource.Id.errorTextView);
            _backButton = FindViewById<Button>(Resource.Id.backButton);
            _headerDateTextView = FindViewById<TextView>(Resource.Id.headerDateTextView);

            var layoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(layoutManager);
            _recyclerView.AddOnScrollListener(new ForecastScrollListener(this));
        }

        private void SetupEventHandlers()
        {
            _backButton.Click += (sender, e) =>
            {
                Finish();
            };
        }

        private async void LoadWeatherData()
        {
            ShowLoading(true);
            HideError();

            try
            {
                var forecast = await _weatherService.GetForecastAsync(_selectedCity);
                
                if (forecast?.HourForecast != null && forecast.HourForecast.Length > 0)
                {
                    DisplayForecast(forecast);
                }
                else
                {
                    ShowError("No forecast data available");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Failed to load weather data: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void DisplayForecast(IWeatherForcastVO forecast)
        {
            _cityTextView.Text = "Weather Forecast";
            _cityNameTextView.Text = forecast.City.ToUpper();
            
            // Get current weather (closest to current time)
            var currentWeather = GetCurrentWeather(forecast.HourForecast);
            
            // Get hourly forecasts for next 3 days (excluding current time)
            var hourlyForecasts = GetHourlyForecasts(forecast.HourForecast);
            
            // Combine: first item is "Now", then hourly forecasts
            var allForecasts = new List<IHourWeatherForecastVO>();
            if (currentWeather != null)
            {
                allForecasts.Add(currentWeather);
            }
            allForecasts.AddRange(hourlyForecasts);

            _allForecasts = allForecasts;

            // Set initial header date
            if (_allForecasts.Count > 0)
            {
                UpdateHeaderDate(_allForecasts[0].Date);
            }

            var adapter = new HourlyForecastAdapter(allForecasts.ToArray(), this);
            _recyclerView.SetAdapter(adapter);
        }

        private IHourWeatherForecastVO GetCurrentWeather(IHourWeatherForecastVO[] allForecasts)
        {
            // Find the forecast closest to current time
            return allForecasts
                .OrderBy(f => Math.Abs((f.Date - _currentTime).TotalHours))
                .FirstOrDefault();
        }

        private List<IHourWeatherForecastVO> GetHourlyForecasts(IHourWeatherForecastVO[] allForecasts)
        {
            // Get forecasts starting from current time, skip the very first one (we use it for "Now")
            // and take hourly forecasts for next 72 hours (73 total including "Now")
            var futureForecasts = allForecasts
                .Where(h => h.Date > _currentTime)
                .OrderBy(h => h.Date)
                .Take(72)
                .ToList();

            return futureForecasts;
        }

        public void UpdateHeaderDate(DateTime date)
        {
            RunOnUiThread(() =>
            {
                _headerDateTextView.Text = date.ToString("MMMM dd, yyyy");
            });
        }

        public void OnScrollChanged()
        {
            if (_allForecasts == null || _allForecasts.Count == 0) return;

            var layoutManager = _recyclerView.GetLayoutManager() as LinearLayoutManager;
            if (layoutManager == null) return;

            int firstVisiblePosition = layoutManager.FindFirstVisibleItemPosition();
            if (firstVisiblePosition >= 0 && firstVisiblePosition < _allForecasts.Count)
            {
                var firstVisibleDate = _allForecasts[firstVisiblePosition].Date;
                UpdateHeaderDate(firstVisibleDate);
            }
        }

        private void ShowLoading(bool show)
        {
            _progressBar.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
            _recyclerView.Visibility = show ? ViewStates.Gone : ViewStates.Visible;
        }

        private void ShowError(string message)
        {
            _errorTextView.Text = message;
            _errorTextView.Visibility = ViewStates.Visible;
            _recyclerView.Visibility = ViewStates.Gone;
        }

        private void HideError()
        {
            _errorTextView.Visibility = ViewStates.Gone;
        }
    }

    // Scroll listener
    public class ForecastScrollListener : RecyclerView.OnScrollListener
    {
        private readonly ForecastActivity _activity;

        public ForecastScrollListener(ForecastActivity activity)
        {
            _activity = activity;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            _activity.OnScrollChanged();
        }
    }

    // Adapter
    public class HourlyForecastAdapter : RecyclerView.Adapter
    {
        private readonly IHourWeatherForecastVO[] _hourlyForecasts;
        private readonly Context _context;

        public HourlyForecastAdapter(IHourWeatherForecastVO[] hourlyForecasts, Context context)
        {
            _hourlyForecasts = hourlyForecasts;
            _context = context;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_hour_forecast, parent, false);
            return new HourlyForecastViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is HourlyForecastViewHolder viewHolder)
            {
                viewHolder.Bind(_hourlyForecasts[position], position);
            }
        }

        public override int ItemCount => _hourlyForecasts?.Length ?? 0;
    }

    // ViewHolder
    public class HourlyForecastViewHolder : RecyclerView.ViewHolder
    {
        private readonly TextView _timeTextView;
        private readonly TextView _temperatureTextView;
        private readonly ImageView _weatherIconImageView;

        public HourlyForecastViewHolder(View itemView) : base(itemView)
        {
            _timeTextView = itemView.FindViewById<TextView>(Resource.Id.timeTextView);
            _temperatureTextView = itemView.FindViewById<TextView>(Resource.Id.temperatureTextView);
            _weatherIconImageView = itemView.FindViewById<ImageView>(Resource.Id.weatherIconImageView);
        }

        public void Bind(IHourWeatherForecastVO forecast, int position)
        {
            // Temperature format: "28.90C / 84.02F"
            _temperatureTextView.Text = $"{forecast.TeperatureCelcius:0.00}C / {forecast.TeperatureFahrenheit:0.00}F";
            
            // First position is always "Now", others show time
            var timeDisplay = (position == 0) ? "Now" : forecast.Date.ToString("HH:mm");
            _timeTextView.Text = timeDisplay;

            // Load weather icon
            if (!string.IsNullOrEmpty(forecast.ForecastPitureURL))
            {
                LoadImageFromUrl(forecast.ForecastPitureURL);
            }
            else
            {
                _weatherIconImageView.SetImageResource(Android.Resource.Drawable.IcMenuView);
            }
        }

        private async void LoadImageFromUrl(string imageUrl)
        {
            try
            {
                if (!imageUrl.StartsWith("http"))
                {
                    imageUrl = "https:" + imageUrl;
                }

                using var webClient = new WebClient();
                var imageBytes = await webClient.DownloadDataTaskAsync(imageUrl);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    var bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    var resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, 120, 120, true);
                    
                    ItemView.Post(() => 
                    {
                        _weatherIconImageView.SetImageBitmap(resizedBitmap);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load image: {ex.Message}");
                ItemView.Post(() => 
                {
                    _weatherIconImageView.SetImageResource(Android.Resource.Drawable.IcMenuView);
                });
            }
        }
    }
}