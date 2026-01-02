using AudioLibrary.Models;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsAppMicrophone
{
    public class StreamingAudioPlayer 
    {
        private BufferedWaveProvider bufferedProvider;
        public IWavePlayer waveOut {  get; private set; }


        public readonly AudioOption defaultAudioOption = new AudioOption(44100, 16, 1); //{ get; set; }

        private WasapiOut woMicOutput;

        public StreamingAudioPlayer()
        {
            InitWasapiLoopback(defaultAudioOption);
        }


        //private void InitWasapiLoopback()
        //{
        //    // Настройка формата (например, 44.1kHz, 16-bit, моно)

        //    //локальный проигрыватель звука
        //    //waveOut =new WaveOutEvent();//new DirectSoundOut();//
        //    //waveOut.Init(bufferedProvider);

        //    //waveOut = FindAndCreateWoMicWaveOut();

        //    //if (waveOut == null)
        //    //{
        //    //    waveOut = new WaveOutEvent();
        //    //}

        //    //waveOut.Init(bufferedProvider);


        //    WaveFormat waveFormat = new WaveFormat(audioOption.SampleRate, audioOption.BitDepth, audioOption.Chanells);

        //    bufferedProvider = new BufferedWaveProvider(waveFormat)
        //    {
        //        BufferDuration = TimeSpan.FromSeconds(5),
        //        DiscardOnBufferOverflow = true
        //    };


        //    // waveOut = FindAndCreateWoMicWaveOut();

        //    var device = FindMicDevice();
        //    //////////виртуальный миркофон
        //    waveOut = new WasapiOut(
        //               device,
        //               AudioClientShareMode.Shared,
        //               true,
        //               100);
        //    waveOut.Init(bufferedProvider);

        //}




        public void InitWasapiLoopback(AudioOption audioOption)
        {
            

            WaveFormat waveFormat = new WaveFormat(audioOption.SampleRate, audioOption.BitDepth, audioOption.Chanells);

            bufferedProvider = new BufferedWaveProvider(waveFormat)
            {
                BufferDuration = TimeSpan.FromSeconds(5), 
                DiscardOnBufferOverflow = true 
            };


            // waveOut = FindAndCreateWoMicWaveOut();

            var device = FindMicDevice();
            //////////виртуальный миркофон
            waveOut = new WasapiOut(
                       device,
                       AudioClientShareMode.Shared,
                       true,
                       100); 
            waveOut.Init(bufferedProvider);

        }
        private IWavePlayer FindAndCreateWoMicWaveOut()
        {
            // Проверяем все устройства WaveOut
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var caps = WaveOut.GetCapabilities(i);
                //Console.WriteLine($"Device {i}: {caps.ProductName}");
                if (caps.ProductName.ToLower().Contains("cable input"))
                {
                    return new WaveOutEvent()
                    {
                        DeviceNumber = i
                    };
                }

                //    // Создаем WaveOut для конкретного устройства
                
                
            }

            return null;
        }
        /*
        private IWavePlayer FindAndCreateWoMicSoundOut()
        {
            // Проверяем все устройства WaveOut

            foreach (var caps in DirectSoundOut.Devices)
            {
                if (caps.ModuleName.ToLower().Contains("wo mic")|| caps.Description.ToLower().Contains("wo mic"))
                {

                    return new DirectSoundOut(caps.Guid);
                }
            }
           

            return null;
        }
        private MMDevice FindWoMicDevice()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(
                DataFlow.Render, DeviceState.Active);

            // Ищем Wo Mic по названию
            foreach (var device in devices)
            {//(WO Mic Device)
                if (device.FriendlyName.Contains("Wo Mic", StringComparison.OrdinalIgnoreCase) ||
                    device.FriendlyName.Contains("Virtual Audio", StringComparison.OrdinalIgnoreCase))
                {
                    return device;
                }
            }

            return null;
        }

        */
        private MMDevice FindMicDevice()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(
                DataFlow.Render, DeviceState.Active);

            // Ищем Wo Mic по названию
            foreach (var device in devices)
            {
                
                    return device;
                
            }

            return null;
        }




        // Добавляем новые аудио данные
        public void AddAudioData(byte[] pcmData)
        {
            bufferedProvider.AddSamples(pcmData, 0, pcmData.Length);
            
        }

        public void Stop()
        {
            waveOut?.Stop();
            //waveOut?.Dispose();
        }
    }
}
