NNChallenge - Weather Forecast App
==================================

A native .NET multiplatform weather forecast application for Android and iOS devices.

Features
--------

*   **3-day hourly weather forecast**
    
*   **Multiple city selection**
    
*   **Platform-specific native UIs**
    
*   **Shared business logic**
    
*   **Real weather data from WeatherAPI**
    

Platforms
---------

*   ✅ Android
    
*   ✅ iOS

**WeatherService** automatically fetches forecasts from WeatherAPI when you select a city. The service handles all API communication and data parsing behind the scenes.

Running on Android
------------------

*    Open NNChallenge.sln in Visual Studio

*    Set NNChallenge.Droid as startup project

*    Select Android emulator or device

*    Build and run (F5)

Running on iOS
---------------------

*    Requires Mac with Xcode

*    Connect to Mac agent from Visual Studio

*    Set NNChallenge.iOS as startup project

*    Select iOS simulator or device

*    Build and run (F5)



Outputs
------------
<img width="293" height="648" alt="image" src="https://github.com/user-attachments/assets/32107df5-2101-47a7-9724-72a271d9f71f" />
&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
<img width="289" height="648" alt="image" src="https://github.com/user-attachments/assets/9d64947a-92f4-488b-af30-e1d1be6b8c4c" />
</br></br></br>
<img width="304" height="648" alt="image" src="https://github.com/user-attachments/assets/6bf5d50f-49be-48fd-82aa-2c06a7765e54" />
&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
<img width="327" height="648" alt="image" src="https://github.com/user-attachments/assets/d812c84b-3cec-496c-a20f-8bc7e6e68607" />
    

Architecture
------------

*   **Shared Logic**: .NET Standard library
    
*   **Android**: Xamarin.Android
    
*   **iOS**: Xamarin.iOS
    
*   **No Xamarin.Forms or MAUI** - Pure native implementations
    

API
---

Uses [WeatherAPI.com](https://www.weatherapi.com/) for real-time weather data with 3-day forecasts.
