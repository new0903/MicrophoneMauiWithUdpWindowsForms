
using MauiAppMicrophone.Extensions;
using MauiAppMicrophone.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppMicrophone.Services.Web
{

    public enum UdpClientState
    {
        Connected,
        Disconnected,
        StopRun,
        WaitConnect,
    }
    public class UdpSocketService
    {
        private CancellationTokenSource? tokenSource;

        public string ipAddressDevice { get; private set; }//это мы
        public string ipAddressConsumer { get; private set; }//это мы


        public event Func<UdpReceiveResult,Task> OnMessage;





        public event Action<UdpClientState> EventStateUdp;
        /*сюда добавить ещё делегаты
         1) ожидание подключения
        2) подключено и проигрывается
        3) остановлено
         */
        private UdpClient client;

        public UdpSocketService()
        {
            client = new UdpClient(MyConfig.PORT);
        }
        public void SetConsumerIp( string ipConsumer)
        {
            ipAddressConsumer=ipConsumer;
            if (!string.IsNullOrEmpty(ipAddressConsumer))
            {

                EventStateUdp?.Invoke(UdpClientState.Connected);
            }
            else
            {

                EventStateUdp?.Invoke(UdpClientState.Disconnected);
            }
        }
        public async Task Start()
        {
                var address = await WiFiHelper.GetCurrentWifiIPAsync();
                ipAddressDevice = address;
                tokenSource= new CancellationTokenSource();
                _ = StartRecieving(tokenSource.Token);
                EventStateUdp?.Invoke(UdpClientState.WaitConnect);
        }

        public void Stop()
        {
            tokenSource?.Cancel();
            EventStateUdp?.Invoke(UdpClientState.StopRun);
        }


        public async Task Send(byte[] bufferResponse, int port= MyConfig.PORT)
        {

            await client.SendAsync(bufferResponse, bufferResponse.Length, ipAddressConsumer, port);
        }


        public async Task StartRecieving(CancellationToken ct)
        {
            try
            {
                int countGetPackeges = 0;
                while (!ct.IsCancellationRequested)
                {
                    var res = await client.ReceiveAsync(ct);
                    OnMessage?.Invoke(res);
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                tokenSource.Dispose();
                tokenSource = null;
            }

        }

    }
}
