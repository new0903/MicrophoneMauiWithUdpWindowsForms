using MauiAppMicrophone.Services;
using MauiAppMicrophone.ViewModels;

namespace MauiAppMicrophone
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        //private readonly AudioService audioService;

        public MainPage(MainViewModel model)
        {
            InitializeComponent();
            this.BindingContext = model;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RequestMicrophonePermission();
            await RequestNotifyPermission(); 
        }

        public async Task<bool> RequestMicrophonePermission()
        {
            try
            {
                // 1. Проверяем статус
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

                // 2. Если нужно, запрашиваем
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Microphone>();
                    
                    // Для Android 13+ нужно дополнительное объяснение
                    if (
                        //DeviceInfo.Platform == DevicePlatform.Android &&
                   //     DeviceInfo.Version.Major >= 13 &&
                        status == PermissionStatus.Denied)
                    {
                        // Показываем объяснение, зачем нужен микрофон
                        await ShowPermissionExplanation();
                        status = await Permissions.RequestAsync<Permissions.Microphone>();
                    }
                }
                
                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Permission error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RequestNotifyPermission()
        {
            try
            {
                // 1. Проверяем статус
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

                // 2. Если нужно, запрашиваем
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.PostNotifications>();

                    // Для Android 13+ нужно дополнительное объяснение
                    if (
                        //DeviceInfo.Platform == DevicePlatform.Android &&
                        //     DeviceInfo.Version.Major >= 13 &&
                        status == PermissionStatus.Denied)
                    {
                        // Показываем объяснение, зачем нужен микрофон
                        await ShowPermissionExplanation();
                        status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                    }
                }

                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Permission error: {ex.Message}");
                return false;
            }
        }

        //public async Task<bool> RequestForegroundPermission()
        //{
        //    try
        //    {
        //        // 1. Проверяем статус
        //        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

        //        // 2. Если нужно, запрашиваем
        //        if (status != PermissionStatus.Granted)
        //        {
        //            status = await Permissions.RequestAsync<Permissions.fore>();

        //            // Для Android 13+ нужно дополнительное объяснение
        //            if (
        //                //DeviceInfo.Platform == DevicePlatform.Android &&
        //                //     DeviceInfo.Version.Major >= 13 &&
        //                status == PermissionStatus.Denied)
        //            {
        //                // Показываем объяснение, зачем нужен микрофон
        //                await ShowPermissionExplanation();
        //                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        //            }
        //        }

        //        return status == PermissionStatus.Granted;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Permission error: {ex.Message}");
        //        return false;
        //    }
        //}
        private async Task ShowPermissionExplanation()
        {
            await Application.Current.MainPage.DisplayAlert(
                "Доступ к микрофону",
                "Приложению нужен доступ к микрофону для захвата голоса. " +
                "Разрешение используется только для передачи голоса на компьютер.",
                "Понятно");
        }
        //private void OnCounterClicked(object? sender, EventArgs e)
        //{
        //    count++;

        //    if (count == 1)
        //        CounterBtn.Text = $"Clicked {count} time";
        //    else
        //        CounterBtn.Text = $"Clicked {count} times";

        //    SemanticScreenReader.Announce(CounterBtn.Text);
        //}
    }
}
