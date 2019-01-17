using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace JYU_tehtava_1 {
    class Program {
        static void Main(string[] args) {
            Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            s.Connect("localhost",25000);

            String send = "GET / HTTP/1.1\r\nHost: localhost\r\n\r\n";

            Byte[] Buffer = Encoding.ASCII.GetBytes(send);
            s.Send(Buffer);

            int count = 1;
            string sivu = "";
           
            while (count > 0) {
                byte[] reciveBytes = new byte[2048];
                count = s.Receive(reciveBytes);
                sivu += Encoding.ASCII.GetString(reciveBytes,0,count);
                Console.WriteLine($"Bytejä tuli {count}");
            }
            
            Console.Write(sivu);
            

            Console.ReadKey();
            s.Close();
        }
    }
}
