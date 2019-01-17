using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TCP_palvelin {
    class Program {
        static void Main(string[] args) {
            
            Socket Palvelin = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Loopback,25000);

            Palvelin.Bind(ipe);
            
            Palvelin.Listen(5);

            Socket asiakas = Palvelin.Accept();
            IPEndPoint iap = (IPEndPoint)asiakas.RemoteEndPoint;

            Console.WriteLine($"Yhteys osoitteesta: {iap.Address} Portista {iap.Port}");

            NetworkStream ns = new NetworkStream(asiakas);

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            string vast = sr.ReadLine();

            sw.WriteLine($"Ville palvelin; {vast}");
            sw.Flush();
            asiakas.Close();

            Console.ReadKey();
            Palvelin.Close();
        }
    }
}
