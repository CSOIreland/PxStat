

using System.Configuration;
using System.Net.Sockets;
using System.Net;
using AngleSharp.Dom;
using ImpromptuInterface;
using API;
using PxStatCore.Test;
using PxStat.Security;

namespace PxStatXUnit.Tests
{

    [Collection("PxStatXUnit")]
    public class HttpClient_Test
    {
        [Fact]
        public void TestHttpClientString()
        {
            Helper.SetupTests();
            var socketHandler = new SocketsHttpHandler()
            {
                ConnectCallback = async (context, cancellationToken) =>
                {
                    // Use DNS to look up the IP addresses of the target host:
                    // - IP v4: AddressFamily.InterNetwork
                    // - IP v6: AddressFamily.InterNetworkV6
                    // - IP v4 or IP v6: AddressFamily.Unspecified
                    // note: this method throws a SocketException when there is no IP address for the host
                    var entry = await Dns.GetHostEntryAsync(context.DnsEndPoint.Host, AddressFamily.InterNetwork, cancellationToken);

                    // Open the connection to the target host/port
                    var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                    // Turn off Nagle's algorithm since it degrades performance in most HttpClient scenarios.
                    socket.NoDelay = true;

                    try
                    {
                        await socket.ConnectAsync(entry.AddressList, context.DnsEndPoint.Port, cancellationToken);

                        // If you want to choose a specific IP address to connect to the server
                        // await socket.ConnectAsync(
                        //    entry.AddressList[Random.Shared.Next(0, entry.AddressList.Length)],
                        //    context.DnsEndPoint.Port, cancellationToken);

                        // Return the NetworkStream to the caller
                        return new NetworkStream(socket, ownsSocket: true);
                    }
                    catch
                    {
                        socket.Dispose();
                        throw;
                    }
                }

            };
            string response;
            using (var httpClient = new HttpClient(socketHandler))
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", Configuration_BSO.GetStaticConfig("APP_USER_AGENT"));
                response = GetDownloadString(httpClient, "https://www.google.com").Result;
            }
            Assert.True(response != null);
        }

        [Fact]
        public void TestHttpClientData()
        {
            Helper.SetupTests();
            var socketHandler = new SocketsHttpHandler()
            {
                ConnectCallback = async (context, cancellationToken) =>
                {
                    // Use DNS to look up the IP addresses of the target host:
                    // - IP v4: AddressFamily.InterNetwork
                    // - IP v6: AddressFamily.InterNetworkV6
                    // - IP v4 or IP v6: AddressFamily.Unspecified
                    // note: this method throws a SocketException when there is no IP address for the host
                    var entry = await Dns.GetHostEntryAsync(context.DnsEndPoint.Host, AddressFamily.InterNetwork, cancellationToken);

                    // Open the connection to the target host/port
                    var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                    // Turn off Nagle's algorithm since it degrades performance in most HttpClient scenarios.
                    socket.NoDelay = true;

                    try
                    {
                        await socket.ConnectAsync(entry.AddressList, context.DnsEndPoint.Port, cancellationToken);

                        // If you want to choose a specific IP address to connect to the server
                        // await socket.ConnectAsync(
                        //    entry.AddressList[Random.Shared.Next(0, entry.AddressList.Length)],
                        //    context.DnsEndPoint.Port, cancellationToken);

                        // Return the NetworkStream to the caller
                        return new NetworkStream(socket, ownsSocket: true);
                    }
                    catch
                    {
                        socket.Dispose();
                        throw;
                    }
                }

            };
            byte[] response;
            using (var httpClient = new HttpClient(socketHandler))
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", Configuration_BSO.GetStaticConfig("APP_USER_AGENT"));
                response = GetImageData(httpClient, "https://cdn.jsdelivr.net/gh/CSOIreland/CSOStatic@1.0.0/static/img/cso_logo.png").Result;
            }
            Assert.True(response != null);
        }

        public static async Task<string> GetDownloadString(HttpClient httpClient, string url)
        {
            return await httpClient.GetStringAsync(url);
        }

        public static async Task<byte[]> GetImageData(HttpClient httpClient, string url)
        {
            byte[] b;

            var httpResult = await httpClient.GetAsync(url);
            using var resultStream = await httpResult.Content.ReadAsStreamAsync();

            using (BinaryReader br = new BinaryReader(resultStream))
            {
                b = br.ReadBytes((int)resultStream.Length);
            }
            return b;

        }

    }
}
