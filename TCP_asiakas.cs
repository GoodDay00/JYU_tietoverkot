using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TCP_asaikas {
    class Program {
        static void Main(string[] args) {
            Socket asiakas = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            asiakas.Connect("localhost",25000);

            string sendmsg = "Hei!\r\n\r\n";

            byte[] buffer = Encoding.ASCII.GetBytes(sendmsg);
            
            asiakas.Send(buffer);
            int count = 1;
            string sivu = "";

            while (count > 0) {
                byte[] reciveBytes = new byte[2048];
                count = asiakas.Receive(reciveBytes);
                sivu += Encoding.ASCII.GetString(reciveBytes,0,count);
                Console.WriteLine($"Bytejä tuli {count}");
            }
            string[] tiedot = sivu.Split(';');
            Console.WriteLine("Palvelin: " + tiedot[0]);
            Console.WriteLine("Teksti: " + tiedot[1]);
            Console.ReadKey();
            asiakas.Close();
        }
    }
}
