using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main(string[] args)
    {
        IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
        TcpListener server = new TcpListener(ip, 10000);
        TcpClient client = default(TcpClient);

        try
        {
            server.Start();
            Console.WriteLine("Sever started");

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.Read();
        }

        //Continuously read and parse data
        while (true)
        {
            client = server.AcceptTcpClient();
            //Console.WriteLine("Connected")

            //Receive data to receivedBuffer
            byte[] receivedBuffer = new byte[100];
            NetworkStream stream = client.GetStream();
            stream.Read(receivedBuffer, 0, receivedBuffer.Length);

            //String to store data
            StringBuilder rawCharacters = new StringBuilder();

            //Parse to string and break at end
            foreach (byte b in receivedBuffer)
            {
                //if (b.Equals(037)) break; //break if ® 
                rawCharacters.Append(Convert.ToChar(b).ToString());
            }
            
            //Convert to string to make pooter happy
            string text = rawCharacters.ToString();
            string command = text.Substring(0, 3);
            
            switch (command.ToString()) {
                case "sys": //system command
                    string systemcommand = text.Substring(3, text.Length - 3);
                    string resp = syscommand(systemcommand);
                    //response
                    byte[] data = Encoding.ASCII.GetBytes(resp);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("sent");
                    break;

                case "pnt": //print to console
                    Console.WriteLine(text.Substring(3, text.Length - 3));
                    break;

                case "prc": //start process
                    try
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo.Verb = "runas";
                        process.StartInfo.FileName = text.Substring(3, text.Length - 3);
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    break;
            }
        }
    }

    private static string syscommand(string text)
    {
        //Start CMD process using system.diagnostics and return output as string

        try
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C" + text;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return(result);
        }
        catch(Exception e) 
        {
            return(e.ToString());
        }
        
    }
}