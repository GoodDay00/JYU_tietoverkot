using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JYU_harjoitus_4 {
    class Program {
        static void Main(string[] args) {
            int port = 9999;
            Console.WriteLine("Anna yksisanainen käyttäjänimesi pelin liitymiseksi!");
            string name = Console.ReadLine();
            bool päällä = true;

            Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback,port);
            EndPoint ipe = (IPEndPoint)ep;
            s.ReceiveTimeout = 1000;
            Console.WriteLine("Yhdistetään...");
            Laheta(s, ipe, $"JOIN {name}");
            string TILA = "JOIN";
            while (päällä) {
                string[] osat = Vastaanotot(s,ipe);
                if(osat != null) {
                    switch (TILA) {
                        
                        case "JOIN":
                            switch (osat[0]) {

                                case "ACK":
                                    switch (osat[1]) {

                                        case "201":
                                            Console.WriteLine("Odotetaan toista pelaajaa...");
                                            break;

                                        case "202":
                                            Console.WriteLine($"Peli alkaa! Vastustajasi on {osat[2]} \r\nLuku on {osat[3]} ja {osat[4]} välillä");
                                            Kysynumero(s,ep);
                                            TILA = "GAME";
                                            break;

                                        case "203":
                                            Console.WriteLine($"Peli alkaa! Vastustajasi on {osat[2]} \r\nLuku on {osat[3]} ja {osat[4]} välillä");
                                            Console.WriteLine($"Vastustajan {osat[2]} vuoro");
                                            TILA = "GAME";
                                            break;
                                        
                                        default:
                                            string error = string.Join(" ",osat);
                                            Console.WriteLine(error);
                                            break;
                                    }
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case "GAME":
                            switch (osat[0]) {

                                case "ACK":
                                    switch (osat[1]) {
                                        case "300":
                                            Console.WriteLine("Arvasit väärän numeron");
                                            break;
                                        case "501":
                                            Console.WriteLine($"Voitit pelin! Oikea numero oli {osat[2]}");
                                            Laheta(s,ep,"ACK 500");
                                            TILA = "CLOSED";
                                            päällä = false;
                                            break;

                                        case "502":
                                            Console.WriteLine($"Hävisit pelin! Oikea numero oli {osat[2]}");
                                            Laheta(s,ep,"ACK 500");
                                            TILA = "CLOSED";
                                            päällä = false;
                                            break;
                                        case "407":
                                            Console.WriteLine("Antamasi vastaus ei ollut numero");
                                            Kysynumero(s,ep);
                                            break;
                                        default:
                                            Console.WriteLine(string.Join(" ",osat));
                                            break;
                                    }
                                    break;

                                case "DATA":
                                    Console.WriteLine($"Vastustajasi arvasi numeron {osat[1]}");
                                    Laheta(s,ep,"ACK 300");
                                    Kysynumero(s,ep);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            s.Close();
            Console.ReadKey();

        }

        private static void Kysynumero(Socket s,IPEndPoint ep) {
            string arvaus = "";
            while (true) {
                Console.WriteLine("Anna numero veikkauksesi: ");
                try {
                    arvaus = Console.ReadLine();
                    break;
                }
                catch {
                    Console.WriteLine("Kirjoita vain veikkaamasi numero! ");
                    throw;
                }
            }
            Laheta(s,ep,$"DATA {arvaus}");
        }

        private static string[] Vastaanotot(Socket s, EndPoint ep) {
            try {
                byte[] rec = new byte[248];
                int count = s.ReceiveFrom(rec,ref ep);
                string vastaus = System.Text.Encoding.ASCII.GetString(rec,0,count);
                string[] vast = vastaus.Split(' ');
                return vast;
            }
            catch {
                return null;
            }
            
        }

        static void Laheta(Socket s,EndPoint ep, string msg) {
            s.SendTo(Encoding.ASCII.GetBytes(msg),ep);
        }
    }
}
