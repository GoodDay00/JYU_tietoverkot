using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Peliprotokolla_palvelin {
    class Program {
        static char[] erotin = { ' ' };
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

            string STATE = "WAIT";

            bool on = true;
            int vuoro = -1;
            int ala_raja = 0;
            int ylä_raja = 10;
            int pelaajat = 0;
            int Quit_ACK = 0;
            int luku = -1;
            EndPoint[] Pelaaja = new EndPoint[2];
            string[] nimi = new string[2];

            while (on) {
                IPEndPoint client = new IPEndPoint(IPAddress.Any,0);
                EndPoint remote = (EndPoint)(client);
                string[] kehys = Vastaanota(s, ref remote);

                switch (STATE) {

                    case "WAIT":
                        switch (kehys[0]) {
                            case "JOIN":
                                try {
                                    Pelaaja[pelaajat] = remote;
                                    nimi[pelaajat] = kehys[1];
                                    pelaajat++;
                                    Console.WriteLine($"{remote} Liity peliin");
                                    if (pelaajat == 1) {
                                        Laheta(s,remote,"ACK 201 JOIN OK");
                                    }
                                    else if (pelaajat == 2) {
                                        Laheta(s,remote,"ACK 201 JOIN OK");
                                        Random rand = new Random();
                                        int Aloittaja = rand.Next(0,1);
                                        vuoro = Aloittaja;
                                        luku = rand.Next(ala_raja,ylä_raja);
                                        Console.WriteLine($"Vastausluku: {luku}");
                                        Laheta(s,Pelaaja[vuoro],$"ACK 202 {nimi[Flip(vuoro)]} {ala_raja} {ylä_raja}");
                                        Laheta(s,Pelaaja[Flip(vuoro)],$"ACK 203 {nimi[vuoro]} {ala_raja} {ylä_raja}");
                                        STATE = "GAME";
                                    }
                                }
                                catch {
                                    Laheta(s,Pelaaja[Flip(vuoro)],$"ACK 401 JOIN ei jostain syystä onnistu");
                                }
                                
                                    
                                break;
                            default:
                                break;
                        }
                        break;

                    case "GAME":
                        switch (kehys[0]) {
                            case "DATA":
                                if(remote.Equals(Pelaaja[vuoro])) {
                                    try {
                                        int arvaus = int.Parse(kehys[1]);
                                        if (arvaus != luku) {
                                            Laheta(s, Pelaaja[vuoro],"ACK 300 DATA OK");
                                            Laheta(s,Pelaaja[Flip(vuoro)],$"DATA {arvaus}");
                                            vuoro = Flip(vuoro);
                                            STATE = "WAIT_ACK";
                                        }
                                        else {
                                            Laheta(s,Pelaaja[vuoro],$"ACK 501 {luku}");
                                            Laheta(s,Pelaaja[Flip(vuoro)],$"ACK 502 {luku}");
                                            STATE = "END";
                                        }
                                    }
                                    catch {
                                        Laheta(s,Pelaaja[vuoro],"ACK 407 Arvaus ei ollut numero");
                                    }
                                }
                                else {
                                    Laheta(s,Pelaaja[Flip(vuoro)], $"ACK 402 Pelaajan {nimi[vuoro]} vuoro");
                                }
                                break;
                            default:
                                
                                Console.WriteLine("Virhe datassa");
                                break;
                        }
                        break;

                    case "WAIT_ACK":
                        if (remote.Equals(Pelaaja[vuoro])) {
                            switch (kehys[0]) {
                                case "ACK":
                                    switch (kehys[1]) {
                                        case "300":
                                            
                                            STATE = "GAME";
                                            break;
                                        default:
                                            Laheta(s,remote,"ACK 403 ACK viesti virheellinen");
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;

                    case "END":
                        switch (kehys[0]) {
                            case "ACK":
                                switch (kehys[1]) {
                                    case "500":
                                        if (remote.Equals(Pelaaja[0])) {
                                            Pelaaja[0] = null;
                                            Quit_ACK++;
                                        }
                                        else if (remote.Equals(Pelaaja[1])) {
                                            Pelaaja[1] = null;
                                            Quit_ACK++;
                                        }

                                        if (Quit_ACK >= pelaajat) {
                                            on = false;
                                        }
                                        break;
                                    default:
                                        Console.WriteLine("Virhe STATESSä");
                                        break;
                                }
                                break;
                            default:
                                Laheta(s,remote,"ACK 403 ACK viesti virheellinen");
                                break;
                        }

                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("Peli loppui");
            STATE = "CLOSED";
            s.Close();
            Console.ReadLine();
        }

        private static string[] Vastaanota(Socket s,ref EndPoint ep) {
            try {
                byte[] rec = new byte[248];
                int count = s.ReceiveFrom(rec,ref ep);
                string vastaus = System.Text.Encoding.ASCII.GetString(rec,0,count);
                string[] vast = vastaus.Split(' ');
                return vast;
            }
            catch (Exception ex){
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }

        }
        static void Laheta(Socket s,EndPoint ep,string msg) {
            try {
                s.SendTo(Encoding.ASCII.GetBytes(msg),(IPEndPoint)ep);
            }
            catch {

            }
            
        }
        static int Flip(int i) {
            return 1 - i;
        }
    }
}
