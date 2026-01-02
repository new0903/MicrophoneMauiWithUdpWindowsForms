using AudioLibrary.Models;
using Microsoft.VisualBasic.Logging;
using NAudio.CoreAudioApi;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Windows.Forms;
using WinFormsAppMicrophone.Helper;

namespace WinFormsAppMicrophone
{
    public partial class Form1 : Form
    {

        UdpClient client;
        private static CancellationTokenSource? cts;
        int port = 4252;


        // НАСТРОЙКИ СЕТИ
        bool IsContected = false;
        bool IsWaitConnect = false;
        DateTime startWaitConnect = DateTime.UtcNow;


        public string NameDevice=string.Empty;
        string IpAddressProducer;

        int countGetPackeges = 0;
        int countGetPackegesNull = 0;

        // НАСТРОЙКИ АУДИО
        private const int TARGET_LATENCY_MS = 500;  // Целевая задержка 250 мс
        private const int CHUNK_SIZE_MS = 50;       // Размер чанка 50 мс

        private int sampleRate = 44100;
        public bool IsPlaying { get; private set; }
        private int chunkSizeBytes;
        private int bufferTargetBytes;

        //SoundPlayer soundPlayer;
        //MemoryStream audioStream;
        //MemoryStream audioCaptureStream;

        StreamingAudioPlayer audioPlayer;


        DateTime lastTimeIncomingMessage;

        public Form1()
        {
            InitializeComponent();
            client = new UdpClient(port);
            //soundPlayer=new SoundPlayer();
            //audioStream=new MemoryStream();
            //audioCaptureStream=new MemoryStream();
            audioPlayer = new StreamingAudioPlayer();
            /**/
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAdress = host.AddressList.FirstOrDefault(x=>x.AddressFamily==AddressFamily.InterNetwork);
            if (ipAdress!=null)
            {

                textBox2.Text = ipAdress.ToString();
            }
            /*все аудио устройства*/
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(
                DataFlow.Render, DeviceState.Active).Select(d => d.FriendlyName).ToArray();
            comboBox1.Items.AddRange(devices);
            comboBox1.SelectedIndex = 0;

            NameDevice = comboBox1.Text;
            button2.Enabled = false;
        }



        public void InitAudioSettings()
        {
            int bytesPerSecond = sampleRate * (16 / 8) * 1;
            chunkSizeBytes = (bytesPerSecond * CHUNK_SIZE_MS) / 1000;
            bufferTargetBytes = (bytesPerSecond * TARGET_LATENCY_MS) / 1000;
        }



        private async Task StartStream(CancellationToken token)
        {

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var data = await client.ReceiveAsync(token);
                    ProccessMessage(data);
                    Text = $"Bytes recieved: {data.Buffer.Length * sizeof(byte)}, packages={countGetPackeges},countGetPackegesNull={countGetPackegesNull}";

                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {

                MessageBox.Show($"StartStream  \r\n {ex.Message}");
            }
            finally
            {

                //client.Dispose();
                //// client.Close();

                //client = null;
                cts.Dispose();
                IsContected = false;
                cts = null;

            }

        }



        public void ProccessMessage(UdpReceiveResult result)
        {
            if (result.Buffer.Length > 0)
            {
                var message = MessageProtocol.UnpackMessage(result.Buffer);
                if (message != null)
                {
                    if (!IsContected)
                    {

                        //сообщение об успешном подключении
                        if (message.ServiceText == ServOption.Handshake)
                        {
                            label1.Text = $"состояние: подключено к {message.IpSender} ip={IpAddressProducer},res={result.RemoteEndPoint.ToString()}";
                            IsContected = true;
                            IsWaitConnect = false;
                            audioPlayer.Stop();
                            /*задать настройки*/
                            audioPlayer.InitWasapiLoopback(message.option??audioPlayer.defaultAudioOption);
                            _ = AudioBackgroundPlayer(cts.Token);
                        }
                    }
                    else
                    {
                        //сообщение о разрыве соединения
                        if (message.ServiceText == ServOption.Disconnected)
                        {
                            OverConnect();
                        }

                        //аудио фрагмент
                        if (message.AudioBytes != null && message.AudioBytes.Length > 0 && message.ServiceText == ServOption.AudioMessage)
                        {
                            lastTimeIncomingMessage = DateTime.UtcNow;
                            countGetPackeges++;
                            //label1.Text += $"\n получены данные";
                            audioPlayer.AddAudioData(message.AudioBytes);


                        }

                        //если сообщения не приходят длительное время
                        if (lastTimeIncomingMessage + TimeSpan.FromSeconds(30) < DateTime.UtcNow)
                        {
                            OverConnect();

                        }

                    }
                }
                else
                {
                    countGetPackegesNull++;
                }

            }

            if (IsWaitConnect && !IsContected && DateTime.UtcNow > startWaitConnect + TimeSpan.FromSeconds(30))
            {
                OverConnect();
            }

        }


        public void OverConnect()
        {
            cts.Cancel();
            IsWaitConnect = false;
            IsContected = false;
            button1.Enabled = true;
            button2.Enabled = false;
        }


        public async Task AudioBackgroundPlayer(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (audioPlayer.waveOut.PlaybackState != NAudio.Wave.PlaybackState.Playing)
                    {
                        audioPlayer.waveOut.Play();
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {

                MessageBox.Show($"StartStream  \r\n {ex.Message}");
            }
            finally
            {

            }

        }


        //void WriteAudioAsWavFile(byte[] audio, int sampleRate, int channels=1, int bitDepth=16)
        //{
        //    var header = AudioWavHellper.CreateWavFileHeader(audio.Length, sampleRate, channels,bitDepth);


        //    audioCaptureStream.SetLength(0);
        //    audioCaptureStream.Position = 0;
        //    audioCaptureStream.Write(header);
        //    audioCaptureStream.Write(audio);
        //    // capturedAudioStream.Close();
        //}



        private void button1_Click(object sender, EventArgs e)
        {

            if (!IsContected)
            {
                startWaitConnect = DateTime.UtcNow;
                IsWaitConnect = true;
                IpAddressProducer = textBox1.Text;


                button1.Enabled = false;
                button2.Enabled = true;
                NameDevice = comboBox1.Text;


                var messageHandshake = new MessageProtocol();
                messageHandshake.IpSender = textBox2.Text;
                messageHandshake.ServiceText = ServOption.Handshake;
                var bufferHandShake = MessageProtocol.PackMessage(messageHandshake);
                client.Send(bufferHandShake, bufferHandShake.Length, IpAddressProducer, port);



                cts = new CancellationTokenSource();
                _ = StartStream(cts.Token);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var messageHandshake = new MessageProtocol();
            messageHandshake.IpSender = textBox2.Text;
            messageHandshake.ServiceText = ServOption.Disconnected;
            var bufferHandShake = MessageProtocol.PackMessage(messageHandshake);
            client.Send(bufferHandShake, bufferHandShake.Length, IpAddressProducer, port);


            OverConnect();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
