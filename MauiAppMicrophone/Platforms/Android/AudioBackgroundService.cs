using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using AndroidX.Core.App;
using AudioLibrary.Models;
using MauiAppMicrophone.Services;
using MauiAppMicrophone.Services.Background;
using MauiAppMicrophone.Services.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppMicrophone.Platforms.Android
{
    [Service(Exported = false, ForegroundServiceType = ForegroundService.TypeMicrophone)]
    public class AudioBackgroundService : Service, IBackgroundService
    {

        private int NOTIFICATION_ID = new object().GetHashCode();
        private const string CHANNEL_ID = "audio_service_channel";


        IAudioService DeviceAudioService; 
        UdpSocketService UdpServerService;
        private bool _isInitialized = false;

        private CancellationTokenSource? tokenSource;
        private readonly object _lock = new object();
        private bool _isPlaying = false;
        private string _ipAddressConsumer = string.Empty;

        // Свойства с thread-safe доступом
        public bool IsPlaying
        {
            get { lock (_lock) return _isPlaying; }
            private set { 
                lock (_lock)
                {
                    _isPlaying = value;
                }
                //MainThread.BeginInvokeOnMainThread(() =>
                //{
                //});
            }
        }

        public string ipAddressConsumer
        {
            get { lock (_lock) return _ipAddressConsumer; }
            private set { 
                lock (_lock)
                {
                    _ipAddressConsumer = value;
                }
                
            }
        }


        private void InitializeServices()
        {
            if (_isInitialized) return;

            UdpServerService = IPlatformApplication.Current.Services.GetService<UdpSocketService>();
            DeviceAudioService = IPlatformApplication.Current.Services.GetService<IAudioService>();

            if (UdpServerService==null|| DeviceAudioService==null)
            {

                throw new NotImplementedException();
            }
            DeviceAudioService.AddListnerAudio(AudioCapture);
            _isInitialized = true;
        }


        public override IBinder? OnBind(Intent? intent)
        {
            return null;
        }


        [return: GeneratedEnum]
        public override  StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {

            try
            {
                InitializeServices();
                if (intent.Action == "START_SERVICE")
                {

                    RegisterNotification();//Proceed to notify

                    Task.Run(async () => await StartAudio());

                }
                else if (intent.Action == "STOP_SERVICE")
                {

                    Task.Run(async () =>
                    {
                        await StopConnect();
                    });
                    StopSelfResult(startId);
                    StopForeground(true);
                }

            }
            catch (Exception ex)
            {
                Log.Error("AudioService", $"OnStartCommand error: {ex}");
            }
           
            return StartCommandResult.Sticky;
        }

        public void Start()
        {
            //ipAddressConsumer=ipConsumer;

            
            Intent startService = new Intent(MainActivity.ActivityCurrent, typeof(AudioBackgroundService)); //new Intent(MainActivity.ActivityCurrent, typeof(MainActivity));
            startService.SetAction("START_SERVICE");
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                
                MainActivity.ActivityCurrent.StartForegroundService(startService);

            }
            else
            {
                MainActivity.ActivityCurrent.StartService(startService);
            }

            //Shell.Current.DisplayAlert("Start", "Фоновый сервис работает", "ok");
        }

        public void Stop()
        {
            Intent stopIntent = new Intent(MainActivity.ActivityCurrent, typeof(AudioBackgroundService));//MainActivity.ActivityCurrent
            stopIntent.SetAction("STOP_SERVICE");
            MainActivity.ActivityCurrent.StartService(stopIntent);

            //Shell.Current.DisplayAlert("Stop", "Фоновый сервис приостановлен", "ok");
        }

        private void RegisterNotification()
        {

            var notificationManager = GetSystemService(NotificationService) as NotificationManager;

            // Создаем канал уведомлений
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(CHANNEL_ID, "Audio Service",
                    NotificationImportance.Low);
                notificationManager.CreateNotificationChannel(channel);
            }

            // Создаем уведомление
            var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle("Audio Recording")
                .SetContentText("Recording in progress...")
                .SetSmallIcon(Resource.Drawable.abc_ab_share_pack_mtrl_alpha) // Ваша иконка
                .SetOngoing(true)
                .Build();

            // Запускаем как foreground сервис
            if ((int)Build.VERSION.SdkInt >= 34) // Android 14+
            {
                StartForeground(NOTIFICATION_ID, notification,
                    ForegroundService.TypeMicrophone);
            }
            else
            {
                StartForeground(NOTIFICATION_ID, notification);
            }
        }


        private async Task StopConnect()
        {

            await DeviceAudioService.StopPlayeAudio();
            var messageResponse = new MessageProtocol();
            messageResponse.IpSender = UdpServerService.ipAddressDevice;
            messageResponse.ServiceText = ServOption.Disconnected;
            var bufferResponse = MessageProtocol.PackMessage(messageResponse);


            if (tokenSource != null)
            {

                tokenSource.Cancel();
            }

            await UdpServerService.Send(bufferResponse);
            UdpServerService.OnMessage -= ProcessMessage;

            ipAddressConsumer = string.Empty;
            UdpServerService.SetConsumerIp(ipAddressConsumer);
            UdpServerService.Stop();
            IsPlaying = false;
            //EventPlaying?.Invoke(false);

            //EventConsumer?.Invoke(string.Empty);
        }

        public async Task StartAudio()
        {
            if (!IsPlaying)
            {
                tokenSource = new CancellationTokenSource();
                await UdpServerService.Start();
               // await DeviceAudioService.PlayeAudio();
                UdpServerService.OnMessage += ProcessMessage;
                IsPlaying = true;
            }
           
        }


        public async Task ProcessMessage(UdpReceiveResult result)
        {
            if (result.Buffer.Length > 0)
            {
                var message = MessageProtocol.UnpackMessage(result.Buffer);
                if (message is not null)
                {
                    if (string.IsNullOrEmpty(ipAddressConsumer))
                    {

                        if (message.ServiceText == ServOption.Handshake)
                        {

                            ipAddressConsumer = message.IpSender;
                            UdpServerService.SetConsumerIp(ipAddressConsumer);

                            await DeviceAudioService.PlayeAudio();

                            /*отправляем данные рукопожатия свой ip 
                             * хотя он итак известен получателю,
                             * но чисто что бы подтвердить что мы его услышали и
                             * отправляем получателю настройки */
                            var messageResponse = new MessageProtocol();
                            messageResponse.IpSender = UdpServerService.ipAddressDevice;
                            messageResponse.ServiceText = ServOption.Handshake;
                            messageResponse.option = new AudioOption()
                            {
                                BitDepth = (int)AudioStreamConfig.selectedBitDepth,
                                Chanells = (int)AudioStreamConfig.selectedChannelType,
                                SampleRate = AudioStreamConfig.sampleRate
                            };

                            var bufferResponse = MessageProtocol.PackMessage(messageResponse);
                            await UdpServerService.Send(bufferResponse);
                        }
                    }
                    else
                    {
                        if (message.ServiceText == ServOption.Disconnected)
                        {
                            /* вот здесь функционал надо доработать 
                             разорванные соединения 
                            например пользователь перестал отвечать надо периодические сообщения
                            спустя какое то время
                            тогда прекращаем вещание с микрофона
                            но пока просто сделаем если пользоваетль отправил сообщение и это отключение то прерываем трансляцию
                             */

                            await DeviceAudioService.StopPlayeAudio();
                        }

                    }
                }

            }
        }

        public async void AudioCapture(byte[] bytes)
        {
            var messageStream = new MessageProtocol();
            messageStream.IpSender = UdpServerService.ipAddressDevice;
            messageStream.AudioBytes = bytes;
            messageStream.ServiceText = ServOption.AudioMessage;


            var messageBytes = MessageProtocol.PackMessage(messageStream);

            await UdpServerService.Send(messageBytes);
        }
    }
}