using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace UPD_palvelin {
    class Program {
        static void Main(string[] args) {
            Socket s = null;
            int port = 9999;
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback,port);
            List<IPEndPoint> asiakkaat = new List<IPEndPoint>();

            try {
                s = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
                s.Bind(iep);
            }
            catch (Exception ex) {
                Console.WriteLine("VIRHE: " + ex.Message);
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Odotetaan asiakasta...");

            while (!Console.KeyAvailable) {
                try {
                    byte[] rec = new byte[256];
                    IPEndPoint asiakas = new IPEndPoint(IPAddress.Any,0);
                    EndPoint remote = (EndPoint)asiakas;
                    int recived = s.ReceiveFrom(rec,ref remote);

                    string rec_string = Encoding.ASCII.GetString(rec);
                    char[] delim = { ';' };
                    string[] palat = rec_string.Split(delim,2);
                    if (palat.Length < 2) {

                    }
                    else {
                        if (!asiakkaat.Contains(remote)) {
                            asiakkaat.Add((IPEndPoint)remote);
                            Console.WriteLine($"Uusi asiakas: {((IPEndPoint)remote).Address}: { ((IPEndPoint)remote).Port }");
                        }
                        foreach (EndPoint client in asiakkaat) {
                            s.SendTo(Encoding.ASCII.GetBytes(rec_string),client);
                        }
                    }
                }
                catch {

                }
                
            }

            Console.ReadKey();
            s.Close();
        }
    }
}
