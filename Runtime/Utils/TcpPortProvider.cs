using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Izhguzin.GoogleIdentity.Utils
{
    internal class TcpPortProvider
    {
        public static int GetAvailablePortFromCollection(ICollection<int> portCollection)
        {
            return GetUnusedPortFromArray(portCollection);
        }

        private static int GetRandomUnusedPort()
        {
            try
            {
                TcpListener listener = new(IPAddress.Loopback, 0);
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                listener.Stop();
                return port;
            }
            catch (SocketException)
            {
                throw new Exception("Failed to get a random unused port.");
            }
        }

        private static int GetUnusedPortFromArray(ICollection<int> readOnlyCollection)
        {
            if (readOnlyCollection.Count == 0) return GetRandomUnusedPort();

            List<int> ports = new(readOnlyCollection);

            foreach (IPEndPoint activeTcpListener in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners())
                if (ports.Contains(activeTcpListener.Port))
                    ports.Remove(activeTcpListener.Port);

            if (ports.Count != 0) return ports.First();

            throw new NotSupportedException("All specified TCP ports are already busy.");
        }
    }
}