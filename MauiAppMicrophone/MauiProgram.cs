using MauiAppMicrophone.Services;
using MauiAppMicrophone.Services.Background;
using MauiAppMicrophone.Services.Web;
using MauiAppMicrophone.ViewModels;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace MauiAppMicrophone
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .AddAudio();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<UdpSocketService>();

#if ANDROID
            builder.Services.AddSingleton<IAudioService,AudioService>();
            builder.Services.AddSingleton<IBackgroundService, MauiAppMicrophone.Platforms.Android.AudioBackgroundService>();
  #elif WINDOWS
  /*заглушка */
            builder.Services.AddSingleton<IBackgroundService, BackgroundService>();
         
#endif


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
