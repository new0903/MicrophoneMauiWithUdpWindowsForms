using NAudio.CoreAudioApi;

namespace ConsoleAppAllMicrophonesDevice
{
    public class MicrophoneInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public bool IsDefault { get; set; }
        public string InterfaceName { get; set; }
    }

    internal class Program
    {
        public static List<MicrophoneInfo> GetMicrophones()
        {
            var microphones = new List<MicrophoneInfo>();

            // Используем MMDeviceEnumerator для получения устройств
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(
                DataFlow.Capture , // Захват звука = микрофоны
                DeviceState.Active); // Только активные устройства

            Console.WriteLine($"Найдено микрофонов: {devices.Count}");
            Console.WriteLine("=====================================");

            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                var info = new MicrophoneInfo
                {
                    Id = device.ID,
                    Name = device.FriendlyName,
                    State = device.State.ToString(),
                    IsDefault = device.ID.Contains("{0.0.1.00000000}"), // Проверка на устройство по умолчанию
                    InterfaceName = GetInterfaceName(device.ID)
                };

                microphones.Add(info);

                Console.WriteLine($"[{i + 1}] {info.Name}");
                Console.WriteLine($"    ID: {info.Id}");
                Console.WriteLine($"    Состояние: {info.State}");
                Console.WriteLine($"    По умолчанию: {info.IsDefault}");
                Console.WriteLine($"    Интерфейс: {info.InterfaceName}");


                //        Console.WriteLine($"    Громкость: {device.AudioEndpointVolume.MasterVolumeLevelScalar * 100}%");
                //        Console.WriteLine($"    Muted: {device.AudioEndpointVolume.Mute}");


                Console.WriteLine();
            }

           ;
            enumerator.Dispose();

            return microphones;
        }

        private static string GetInterfaceName(string deviceId)
        {
            if (deviceId.Contains("USB")) return "USB";
            if (deviceId.Contains("HDMI")) return "HDMI";
            if (deviceId.Contains("Bluetooth")) return "Bluetooth";
            if (deviceId.Contains("JACK")) return "3.5mm Jack";
            return "Встроенный/Другой";
        }
        static void Main(string[] args)
        {
            Console.WriteLine("=== Детектор микрофонов ===");
            Console.WriteLine();

            try
            {
                var microphones =GetMicrophones();

                Console.WriteLine("\n=== Краткая сводка ===");
                Console.WriteLine($"Всего найдено: {microphones.Count}");

                var defaultMic = microphones.Find(m => m.IsDefault);
                if (defaultMic != null)
                {
                    Console.WriteLine($"Микрофон по умолчанию: {defaultMic.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
