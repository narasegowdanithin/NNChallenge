using NNChallenge.Interfaces;
using NNChallenge.Services;

namespace NNChallenge.iOS
{
    public partial class ForecastViewController : UIViewController
    {
        private WeatherService _weatherService;
        private string _selectedCity;
        private List<IHourWeatherForecastVO> _allForecasts;
        private DateTime _currentTime;

        // UI Elements
        private UILabel _cityLabel;
        private UILabel _dateHeaderLabel;
        private UITableView _tableView;
        private UIActivityIndicatorView _loadingIndicator;
        private UILabel _errorLabel;

        public ForecastViewController() : base(null, null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = "Weather Forecast";
            
            _weatherService = new WeatherService();
            _currentTime = DateTime.Now;
            
            CreateUIProgrammatically();
            SetupTableView();
            LoadWeatherData();
        }

        private void CreateUIProgrammatically()
        {
            View.BackgroundColor = UIColor.FromWhiteAlpha(0.95f, 1.0f); // Light gray

            // Create navigation bar with back button
            var backButton = new UIBarButtonItem("← Back", UIBarButtonItemStyle.Plain, (s, e) => 
            {
                NavigationController?.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = backButton;

            // City Label
            _cityLabel = new UILabel
            {
                Text = "Loading...",
                Font = UIFont.BoldSystemFontOfSize(24),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Date Header Label
            _dateHeaderLabel = new UILabel
            {
                Text = "Loading...",
                Font = UIFont.BoldSystemFontOfSize(16),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.FromRGBA(0/255f, 122/255f, 255/255f, 1.0f), // iOS blue
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Table View
            _tableView = new UITableView
            {
                RowHeight = 120,
                EstimatedRowHeight = 120,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
                BackgroundColor = UIColor.Clear,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Loading Indicator
            _loadingIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Medium)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                HidesWhenStopped = true
            };

            // Error Label
            _errorLabel = new UILabel
            {
                Text = "",
                Font = UIFont.SystemFontOfSize(16),
                TextColor = UIColor.Red,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                Hidden = true,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Add to view
            View.AddSubview(_cityLabel);
            View.AddSubview(_dateHeaderLabel);
            View.AddSubview(_tableView);
            View.AddSubview(_loadingIndicator);
            View.AddSubview(_errorLabel);

            // Setup constraints
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                // City Label
                _cityLabel.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 16),
                _cityLabel.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 16),
                _cityLabel.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -16),
                
                // Date Header
                _dateHeaderLabel.TopAnchor.ConstraintEqualTo(_cityLabel.BottomAnchor, 16),
                _dateHeaderLabel.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                _dateHeaderLabel.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
                _dateHeaderLabel.HeightAnchor.ConstraintEqualTo(50),
                
                // Table View
                _tableView.TopAnchor.ConstraintEqualTo(_dateHeaderLabel.BottomAnchor),
                _tableView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                _tableView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
                _tableView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor),
                
                // Loading Indicator
                _loadingIndicator.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
                _loadingIndicator.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor),
                
                // Error Label
                _errorLabel.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
                _errorLabel.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor),
                _errorLabel.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 20),
                _errorLabel.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -20)
            });
        }

        private void SetupTableView()
        {
            _tableView.RegisterClassForCellReuse(typeof(HourlyForecastCell), "HourlyForecastCell");
            _tableView.Source = new ForecastTableViewSource(this);
        }

        public void SetSelectedCity(string city)
        {
            _selectedCity = city;
        }

        private async void LoadWeatherData()
        {
            ShowLoading(true);
            HideError();

            try
            {
                var forecast = await _weatherService.GetForecastAsync(_selectedCity ?? "Berlin");
                
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
            _cityLabel.Text = forecast.City.ToUpper();
            
            // Get current weather (closest to current time)
            var currentWeather = GetCurrentWeather(forecast.HourForecast);
            
            // Get hourly forecasts for next 3 days (excluding current time)
            var hourlyForecasts = GetHourlyForecasts(forecast.HourForecast);
            
            // Combine: first item is "Now", then hourly forecasts
            _allForecasts = new List<IHourWeatherForecastVO>();
            if (currentWeather != null)
            {
                _allForecasts.Add(currentWeather);
            }
            _allForecasts.AddRange(hourlyForecasts);

            // Set initial header date
            if (_allForecasts.Count > 0)
            {
                UpdateHeaderDate(_allForecasts[0].Date);
            }

            _tableView.ReloadData();
        }

        private IHourWeatherForecastVO? GetCurrentWeather(IHourWeatherForecastVO[] allForecasts)
        {
            return allForecasts
                .OrderBy(f => Math.Abs((f.Date - _currentTime).TotalHours))
                .FirstOrDefault();
        }

        private List<IHourWeatherForecastVO> GetHourlyForecasts(IHourWeatherForecastVO[] allForecasts)
        {
            var futureForecasts = allForecasts
                .Where(h => h.Date > _currentTime)
                .OrderBy(h => h.Date)
                .Take(72)
                .ToList();

            return futureForecasts;
        }

        public void UpdateHeaderDate(DateTime date)
        {
            InvokeOnMainThread(() =>
            {
                _dateHeaderLabel.Text = date.ToString("MMMM dd, yyyy");
            });
        }

        private void ShowLoading(bool show)
        {
            InvokeOnMainThread(() =>
            {
                if (show)
                {
                    _loadingIndicator.StartAnimating();
                    _tableView.Hidden = true;
                }
                else
                {
                    _loadingIndicator.StopAnimating();
                    _tableView.Hidden = false;
                }
            });
        }

        private void ShowError(string message)
        {
            InvokeOnMainThread(() =>
            {
                _errorLabel.Text = message;
                _errorLabel.Hidden = false;
                _tableView.Hidden = true;
            });
        }

        private void HideError()
        {
            InvokeOnMainThread(() =>
            {
                _errorLabel.Hidden = true;
            });
        }

        // Table View Source
        private class ForecastTableViewSource : UITableViewSource
        {
            private readonly ForecastViewController _controller;

            public ForecastTableViewSource(ForecastViewController controller)
            {
                _controller = controller;
            }

            public override nint RowsInSection(UITableView tableView, nint section)
            {
                return _controller._allForecasts?.Count ?? 0;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell("HourlyForecastCell", indexPath) as HourlyForecastCell;
                var forecast = _controller._allForecasts[indexPath.Row];
                
                cell?.Configure(forecast, indexPath.Row);
                
                return cell;
            }

            public override void Scrolled(UIScrollView scrollView)
            {
                var tableView = scrollView as UITableView;
                if (tableView?.IndexPathsForVisibleRows?.Length > 0)
                {
                    var firstVisibleIndex = tableView.IndexPathsForVisibleRows[0];
                    if (firstVisibleIndex.Row < _controller._allForecasts.Count)
                    {
                        var firstVisibleDate = _controller._allForecasts[firstVisibleIndex.Row].Date;
                        _controller.UpdateHeaderDate(firstVisibleDate);
                    }
                }
            }
        }
    }

    // Custom Table View Cell
    public class HourlyForecastCell : UITableViewCell
    {
        private UIImageView _weatherIcon;
        private UILabel _temperatureLabel;
        private UILabel _timeLabel;

        public HourlyForecastCell(IntPtr handle) : base(handle)
        {
            SetupView();
        }

        private void SetupView()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            BackgroundColor = UIColor.FromWhiteAlpha(0.95f, 1.0f);

            // Content View
            var containerView = new UIView
            {
                BackgroundColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Layer =
                {
                    CornerRadius = 12,
                    BorderColor = UIColor.LightGray.CGColor,
                    BorderWidth = 1
                }
            };

            ContentView.AddSubview(containerView);

            // Weather Icon
            _weatherIcon = new UIImageView
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Temperature Label
            _temperatureLabel = new UILabel
            {
                Font = UIFont.BoldSystemFontOfSize(22),
                TextColor = UIColor.FromRGBA(0/255f, 122/255f, 255/255f, 1.0f), // iOS blue
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Time Label
            _timeLabel = new UILabel
            {
                Font = UIFont.SystemFontOfSize(16),
                TextColor = UIColor.Gray,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            containerView.AddSubview(_weatherIcon);
            containerView.AddSubview(_temperatureLabel);
            containerView.AddSubview(_timeLabel);

            // Constraints
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                // Container
                containerView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 8),
                containerView.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 16),
                containerView.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -16),
                containerView.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, 8),
                
                // Weather Icon
                _weatherIcon.LeadingAnchor.ConstraintEqualTo(containerView.LeadingAnchor, 20),
                _weatherIcon.CenterYAnchor.ConstraintEqualTo(containerView.CenterYAnchor),
                _weatherIcon.WidthAnchor.ConstraintEqualTo(80),
                _weatherIcon.HeightAnchor.ConstraintEqualTo(80),
                
                // Temperature Label
                _temperatureLabel.LeadingAnchor.ConstraintEqualTo(_weatherIcon.TrailingAnchor, 20),
                _temperatureLabel.TopAnchor.ConstraintEqualTo(containerView.TopAnchor, 20),
                
                // Time Label
                _timeLabel.LeadingAnchor.ConstraintEqualTo(_weatherIcon.TrailingAnchor, 20),
                _timeLabel.TopAnchor.ConstraintEqualTo(_temperatureLabel.BottomAnchor, 8)
            });
        }

        public void Configure(IHourWeatherForecastVO forecast, int position)
        {
            // Temperature format: "28.90C / 84.02F"
            _temperatureLabel.Text = $"{forecast.TeperatureCelcius:0.00}C / {forecast.TeperatureFahrenheit:0.00}F";
            
            // First position is always "Now", others show time
            _timeLabel.Text = position == 0 ? "Now" : forecast.Date.ToString("HH:mm");

            // Load weather icon
            if (!string.IsNullOrEmpty(forecast.ForecastPitureURL))
            {
                LoadImageFromUrl(forecast.ForecastPitureURL);
            }
            else
            {
                // Use basic UIImage instead of system image
                _weatherIcon.Image = UIImage.FromBundle("WeatherIcon") ?? CreateDefaultWeatherIcon();
            }
        }

        private UIImage CreateDefaultWeatherIcon()
        {
            // Create a simple placeholder image
            UIGraphics.BeginImageContextWithOptions(new CGSize(80, 80), false, 0);
            var context = UIGraphics.GetCurrentContext();
            
            // Draw a simple sun
            context.SetFillColor(UIColor.Yellow.CGColor);
            context.FillEllipseInRect(new CGRect(20, 20, 40, 40));
            
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            
            return image;
        }

        private async void LoadImageFromUrl(string imageUrl)
        {
            try
            {
                if (!imageUrl.StartsWith("http"))
                {
                    imageUrl = "https:" + imageUrl;
                }

                // Use HttpClient instead of WebClient
                using var httpClient = new System.Net.Http.HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    var imageData = NSData.FromArray(imageBytes);
                    var image = UIImage.LoadFromData(imageData);
                    
                    InvokeOnMainThread(() =>
                    {
                        _weatherIcon.Image = image;
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load image: {ex.Message}");
                InvokeOnMainThread(() =>
                {
                    _weatherIcon.Image = CreateDefaultWeatherIcon();
                });
            }
        }
    }
}