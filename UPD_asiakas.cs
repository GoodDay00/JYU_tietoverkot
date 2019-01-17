using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace UPD_asiakas {
    class Program {
        static void Main(string[] args) {
            Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp);
            int port = 9999;

            IPEndPoint ipe = new IPEndPoint(IPAddress.Loopback,port);
            byte[] rec = new byte[256];

            EndPoint ep = (EndPoint)ipe;
            s.ReceiveTimeout = 1000;
            string msg;
            bool on = true;
            do {
                Console.Write(">");
                msg = Console.ReadLine();
                if (msg.Equals("q")) {
                    on = false;
                }
                else {
                    s.SendTo(Encoding.ASCII.GetBytes(msg),ep);

                    while (!Console.KeyAvailable) {
                        IPEndPoint remote = new IPEndPoint(IPAddress.Any,0);
                        EndPoint Palvelinep = (EndPoint)remote;
                        int paljon = 0;
                        try {
                            s.ReceiveFrom(rec,ref Palvelinep);
                            //Split
                            byte[] vastbyte = rec.Where(val => val != 00).ToArray();
                            string rec_string = Encoding.ASCII.GetString(vastbyte);
                            char[] delim = { ';' };
                            string[] palat = rec_string.Split(delim,2);
                            if (palat.Length < 2) {

                            }
                            else {
                                Console.WriteLine($"{palat[0]}: {palat[1]}");
                            }
                        }
                        catch {

                        }
                    }
                }
            } while (on);

            s.Close();
        }
    }
}
