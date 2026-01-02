using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppMicrophone.Services
{
    public static class AudioStreamConfig
    {

        public static int sampleRate = 44100;
        public static BitDepth selectedBitDepth = BitDepth.Pcm16bit;
        public static ChannelType selectedChannelType = ChannelType.Mono;

    }
}
