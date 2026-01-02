using AudioLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAppMicrophone.helpers;
using MauiAppMicrophone.Services;
using MauiAppMicrophone.Services.Background;
using MauiAppMicrophone.Services.Web;
using Plugin.Maui.Audio;
using System.Net.Sockets;

namespace MauiAppMicrophone.ViewModels
{
    public partial class MainViewModel :ObservableObject
    {
        private readonly IBackgroundService backgroundService;
        private readonly UdpSocketService udpService;

        //private readonly IAudioService audioService;
        //private readonly UdpSocketService udpService;



        [ObservableProperty]
        private string _textMessageAudio = "stop";

        [ObservableProperty]
        private string _logIp = "нет соединений";


        [ObservableProperty]
        private bool _isPlaying = false;

        [ObservableProperty]
        private int _selectedSampleRate = 16000;
        [ObservableProperty]
        private BitDepth _selectedBitDepth = BitDepth.Pcm16bit;
        [ObservableProperty]
        private ChannelType _selectedChannelType = ChannelType.Mono;


        public List<ChannelType> ChannelTypes { get; set; } = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>().ToList();
        public List<BitDepth> BitDepths { get; set; } = Enum.GetValues(typeof(BitDepth)).Cast<BitDepth>().ToList();

        public int[] SampleRates { get; set; } =
        [
            8000,
            16000,
            44100,
            48000
        ];


        [ObservableProperty]
        private string _ipEndpoint = "stop";//это же мы сюда выводится наш ip

 

        public MainViewModel(IBackgroundService backgroundService,UdpSocketService udpService)
        {
            this.backgroundService = backgroundService;
            this.udpService = udpService;
            udpService.EventStateUdp += BackgroundService_EventPlaying;
            //backgroundService.EventPlaying += BackgroundService_EventPlaying;
            //backgroundService.EventConsumer += BackgroundService_EventConsumer;


            LogIp = "ждем указание backgroundService";
        }


        [RelayCommand]
        public async Task StartAudio()
        {
            IsPlaying = !IsPlaying;
            if (IsPlaying)
            {




                IpEndpoint = await WiFiHelper.GetCurrentWifiIPAsync();
                LogIp = "ожидание подключения, фоновый сервис запущен";


                AudioStreamConfig.sampleRate = SelectedSampleRate;
                AudioStreamConfig.selectedBitDepth = SelectedBitDepth;
                AudioStreamConfig.selectedChannelType = SelectedChannelType;

                backgroundService.Start();


            }
            else
            {
                LogIp = "отключение...";
                await StopConnect();
            }
        }

        //private void BackgroundService_EventConsumer(string obj)
        //{
        //    LogIp = $"подключен {obj}";
        //    //LogIp = $"подключен {obj}";
        //}



        private void BackgroundService_EventPlaying(UdpClientState state)
        {

            switch (state)
            {
                case UdpClientState.Connected:
                    TextMessageAudio = $"пользователь подключился";
                    break;
                case UdpClientState.Disconnected:
                    TextMessageAudio = $"пользователь отсоеденился";
                    break;
                case UdpClientState.StopRun:
                    TextMessageAudio = $"остановлено";
                    break;
                case UdpClientState.WaitConnect:
                    TextMessageAudio = $"Ждем подключение";
                    break;
                default:
                    break;
            }
        }

      

        private async Task StopConnect()
        {

            backgroundService.Stop();
            //backgroundService.EventPlaying -= BackgroundService_EventPlaying;
            //backgroundService.EventConsumer -= BackgroundService_EventConsumer;
            IpEndpoint = "stop";
            LogIp = "выключено";
            TextMessageAudio = "аудио трансляция приостановлена";
            // await client.SendAsync(bufferResponse, bufferResponse.Length, ipAddressConsumer, MyConfig.PORT);
        }

 

    }
}
