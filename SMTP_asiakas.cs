using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;


namespace SMTP_asiakas {
    class Program {
        static void Main(string[] args) {
            Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            try {
                s.Connect("localhost",25000);

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                throw;
            }

            NetworkStream ns = new NetworkStream(s);

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            string email = "Terve";
            string viesti = "";
            bool päällä = true;
            while (päällä) {
                viesti = sr.ReadLine();
                string[] viestit = viesti.Split(' ');

                switch (viestit[0]) {
                    case "220":
                        sw.WriteLine("HELO gmail.com");
                        break;
                    case "250":
                        switch (viestit[1]) {
                            case "ITKP104":
                                sw.WriteLine("MAIL FROM: ville@gmail.com");
                                break;
                            case "2.0.0":
                                sw.WriteLine("QUIT");
                                Console.WriteLine("Viesti lähettetty!");
                                päällä = false;
                                break;
                            case "2.1.0":
                                sw.WriteLine("RCPT TO: pekka@hotmail.com");
                                break;
                            case "2.1.5":
                                sw.WriteLine("DATA");
                                break;
                            default:
                                Console.WriteLine("Virhe.. " + viestit[0] + " " + viesti);
                                break;
                        }
                        break;
                    case "221":
                        päällä = false;
                        break;
                    case "354":
                        sw.WriteLine("Hei\r\nmiten menee\r\n.\r\n");
                        break;
                    default:
                        Console.WriteLine("Virhe.. " + viestit[0] + " " + viesti);
                        sw.WriteLine("QUIT");
                        break;
                }
                sw.Flush();
            }
            Console.ReadKey();

            sw.Close();
            sr.Close();
            ns.Close();
            s.Close();
        }
    }
}
