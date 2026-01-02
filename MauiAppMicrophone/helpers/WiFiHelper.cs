using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppMicrophone.helpers
{
    public class WiFiHelper
    {
        public static async Task<string> GetCurrentWifiIPAsync()
        {
            try
            {
                // Проверяем, подключены ли к WiFi
                if (!IsConnectedToWifi())
                    return "Not connected to WiFi";

                // Получаем все IP-адреса
                var ipAddresses = GetLocalIPAddresses();

                // Фильтруем WiFi адреса (обычно это 192.168.x.x, 10.x.x.x)
                foreach (var ip in ipAddresses)
                {
                    if (IsWifiIPAddress(ip))
                        return ip;
                }

                // Если не нашли специфичный WiFi адрес, возвращаем первый локальный
                return ipAddresses.FirstOrDefault() ?? "No IP address";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private static bool IsConnectedToWifi()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                return interfaces.Any(ni =>
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                     ni.Name.Contains("wlan", StringComparison.OrdinalIgnoreCase) ||
                     ni.Name.Contains("wifi", StringComparison.OrdinalIgnoreCase)) &&
                    ni.OperationalStatus == OperationalStatus.Up &&
                    ni.GetIPProperties().UnicastAddresses
                        .Any(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork));
            }
            catch
            {
                return false;
            }
        }

        private static List<string> GetLocalIPAddresses()
        {
            var addresses = new List<string>();

            try
            {
                // Через сетевые интерфейсы
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                addresses.Add(ip.Address.ToString());
                            }
                        }
                    }
                }

                // Через DNS (резервный способ)
                if (addresses.Count == 0)
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if (!addresses.Contains(ip.ToString()))
                                addresses.Add(ip.ToString());
                        }
                    }
                }
            }
            catch { }

            return addresses;
        }

        private static bool IsWifiIPAddress(string ipAddress)
        {
            try
            {
                var ip = IPAddress.Parse(ipAddress);
                var bytes = ip.GetAddressBytes();

                // WiFi адреса обычно в частных диапазонах:
                // 192.168.x.x, 10.x.x.x, 172.16.x.x - 172.31.x.x

                if (bytes[0] == 192 && bytes[1] == 168)
                    return true;

                if (bytes[0] == 10)
                    return true;

                if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                    return true;
            }
            catch { }

            return false;
        }
    }
}
