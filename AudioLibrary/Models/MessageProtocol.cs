using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioLibrary.Models
{

    public enum ServOption 
    {
        Handshake,
        Disconnected,
        AudioMessage
    }


    public class MessageProtocol
    {

        public string IpSender { get; set; }
        public DateTime DispatchTime { get; set; } = DateTime.UtcNow;
        public string IdMessage { get; set; }
        public ServOption ServiceText { get; set; }//Handshake, Disconnected, AudioMessage

        public AudioOption? option { get; set; }


        //public bool? IsHandshake { get; set; } = true;
        public int size { get; set; }

        //реализовать функционал json
        //public string? DataType { get; set; }//тип object для as is и т.д.
        //public object? Data { get; set; }//данные json формата

        public byte[]? AudioBytes { get; set; }

        public static byte[] PackMessage(MessageProtocol message)
        {
            byte[]? fileBytes = null;
            if (message.AudioBytes != null)
            {

                fileBytes = message.AudioBytes;
                message.AudioBytes = null;
            }
            string text = JsonConvert.SerializeObject(message);

            //  text = text.Trim();
            string jsonString = text.Replace("\0", string.Empty);

            var jsonB = Encoding.UTF8.GetBytes(jsonString);
            var bytes = jsonB;

            if (fileBytes != null)
            {
                var buffer = fileBytes;
                var headerJson = $"Fj{jsonB.Length}j";
                var headerFile = $"j{buffer.Length}j";


                var headerJB = Encoding.UTF8.GetBytes(headerJson);
                var headerFB = Encoding.UTF8.GetBytes(headerFile);


                bytes = headerJB.Concat(jsonB).Concat(headerFB).Concat(buffer).ToArray();

            }


            return bytes;
        }

        public static byte[] PackMessage(string message)
        {


            // string text = JsonConvert.SerializeObject(message);

            //  text = text.Trim();
            string jsonString = message.Replace("\0", string.Empty);

            var bytes = Encoding.UTF8.GetBytes(jsonString);

            //шифрование


            return bytes;
        }
        public static MessageProtocol UnpackMessage(byte[] buffer)
        {
            byte b = buffer[0];

            string letter = Encoding.UTF8.GetString(new byte[1] { buffer[0] });
            string text = "";
            if (b == 70 || letter == "F")
            {
                try
                {
                    byte[] f = new byte[buffer.Length];

                    // форма с файлом
                    text = MessageProtocol.ProccessingBinaryDate(buffer, out f);
                    var message = JsonConvert.DeserializeObject<MessageProtocol>(text);
                    if (message != null)
                    {
                        message.AudioBytes = f;
                        return message;
                    }
                    return null;

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    return null;
                }


            }
            else
            {
                //дешифрование
                text = Encoding.UTF8.GetString(buffer);

                try
                {
                    text = text.Trim();
                    string jsonString = text.Replace("\0", string.Empty);
                    var message = JsonConvert.DeserializeObject<MessageProtocol>(jsonString);
                    return message;
                }
                catch (Exception ex)
                {
                    //var test2 = new MultipartReader("test", stream);
                    //var test=test2.ReadNextSectionAsync();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(text);
                    return null;

                    //  var test = new MultipartFormDataContent();
                    //    test.
                    //throw;
                }
            }

        }


        private static string ProccessingBinaryDate(byte[] buffer, out byte[] File)
        {
            File = null;
            //      bool[] jPos= new bool[4] { false, false, false, false};
            //106 == j

            int sizeJson = 0;
            int sizeLenght = 0;
            int sizeFile = 0;

            int counterJ = 0;



            int startPosLenght = 0;
            int endPosLenght = 0;

            int startPosJsonMessage = 0;
            int endPosJsonMessage = 0;

            string text = "";
            int startPosFileLenght = 0;
            int endPosFileLenght = 0;

            int lenjsonstr = 0;
            int lenfile = 0;// int.Parse(Encoding.UTF8.GetString(lenghtFile));
            /*
           0=F 1=j 2=2 3=5 4=8 5=j
            получаем инфу где что считывать
            */
            for (int i = 0; i < buffer.Length; i++)
            {
                var elem = buffer[i];
                if (elem == 106)
                {
                    if (counterJ == 0)
                    {
                        startPosLenght = i + 1;
                    }
                    if (counterJ == 1)
                    {
                        endPosLenght = i;
                        byte[] lenghtJSON = buffer //получаем массив байтов в которых указан размер текста в сообщении
                            .Skip(startPosLenght)
                            .Take(endPosLenght - startPosLenght)
                            .ToArray();
                        if (!int.TryParse(Encoding.UTF8.GetString(lenghtJSON), out lenjsonstr)) return "";//получаем размер сообщения

                        startPosJsonMessage = i + 1;
                        byte[] textByte = buffer.Skip(startPosJsonMessage).Take(lenjsonstr).ToArray();//endPosJsonMessage - startPosJsonMessage
                        text = Encoding.UTF8.GetString(textByte);
                        i = lenjsonstr + startPosJsonMessage - 1;
                    }
                    if (counterJ == 2)
                    {
                        endPosJsonMessage = i;
                        startPosFileLenght = i + 1;
                    }
                    if (counterJ == 3)
                    {
                        endPosFileLenght = i;
                        byte[] lenghtFile = buffer.Skip(startPosFileLenght).Take(endPosFileLenght - startPosFileLenght).ToArray();
                        if (!int.TryParse(Encoding.UTF8.GetString(lenghtFile), out lenfile)) return "";
                        break;
                    }
                    counterJ++;
                }
            }


            if (counterJ != 3)
            {
                return "";
            }
            //byte[] textByte = buffer.Skip(startPosJsonMessage).Take(endPosJsonMessage - startPosJsonMessage).ToArray();

            byte[] file = buffer.Skip(endPosFileLenght + 1).Take(endPosFileLenght - endPosFileLenght + 1).ToArray();





            var stF = endPosFileLenght + 1;
            File = buffer.Skip(stF).Take(lenfile).ToArray();

            return text;
        }


    }
}
