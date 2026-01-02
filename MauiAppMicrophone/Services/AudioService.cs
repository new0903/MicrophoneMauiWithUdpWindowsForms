
using MauiAppMicrophone.Extensions;
using Microsoft.Maui.Dispatching;
using Plugin.Maui.Audio;
using Plugin.Maui.Audio.AudioListeners;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppMicrophone.Services
{
    public partial class AudioService : IAudioService
    {

        private readonly IAudioManager audioManager;
        private IAudioStreamer audioStreamer;


        private event Action<byte[]> callBackDelegate;







        //private CancellationTokenSource? tokenSource;
        private int sizAllChunks = 0;

        public bool IsPlaying { get; private set; }


        //private int sampleRate;
        //private BitDepth selectedBitDepth;
        //private ChannelType selectedChannelType;

        public AudioService(IAudioManager audioManager)
        {
            this.audioManager = audioManager;
            //sampleRate = AudioStreamConfig.sampleRate;
            //selectedBitDepth = AudioStreamConfig.selectedBitDepth;
            //selectedChannelType = AudioStreamConfig.selectedChannelType;


            PCMInit();
        }



        public void PCMInit()
        {
   
            audioStreamer = audioManager.CreateStreamer();
            audioStreamer.Options.BitDepth = AudioStreamConfig.selectedBitDepth;
            audioStreamer.Options.Channels = AudioStreamConfig.selectedChannelType;
            audioStreamer.Options.SampleRate = AudioStreamConfig.sampleRate;
            audioStreamer.OnAudioCaptured += ListenSound;
        }


 
        public string GetNameService() => "Audio";

        private void ListenSound(object? sender, AudioStreamEventArgs e)
        {
            callBackDelegate?.Invoke(e.Audio);
            //127.0.0.1 нельзя. Нужен ip wife
        }


        public async Task PlayeAudio()
        {
            
            if (!IsPlaying)
            {
                IsPlaying = true;

                audioStreamer.Options.BitDepth = AudioStreamConfig.selectedBitDepth;
                audioStreamer.Options.Channels = AudioStreamConfig.selectedChannelType;
                audioStreamer.Options.SampleRate = AudioStreamConfig.sampleRate;
                await audioStreamer.StartAsync();
            }

        }


        public async Task StopPlayeAudio()
        {

            if (IsPlaying)
            {
                await audioStreamer.StopAsync();
                IsPlaying = false;
            }
        }

        public async Task BackgroundPlayer(CancellationToken ct)
        {
            await Task.CompletedTask;
        }

        public void AddListnerAudio(Action<byte[]> action)
        {
            callBackDelegate += action;
        }

    }
}
