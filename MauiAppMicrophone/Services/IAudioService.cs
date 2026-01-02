
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppMicrophone.Services
{
    public interface IAudioService
    {



        Task PlayeAudio();
        Task StopPlayeAudio();




        string GetNameService();

        Task BackgroundPlayer(CancellationToken ct);

        void AddListnerAudio(Action<byte[]> action);//
    }
}
