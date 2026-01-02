using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioLibrary.Models
{
    public class AudioOption
    {
        public int SampleRate { get; set; } = 44100;//16000 44000 ....
        public int Chanells { get; set; } = 1;//1 = mono 2 = stereo
        public int BitDepth { get; set; } = 16; // 16 8 32 
        public AudioOption()
        {

        }
        public AudioOption(int sampleRate, int bitDepth, int chanells )
        {
            SampleRate = sampleRate;
            Chanells = chanells;
            BitDepth = bitDepth;
        }
    }
}
