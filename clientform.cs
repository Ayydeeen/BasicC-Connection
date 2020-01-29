using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
namespace client
{
    public partial class Form1 : Form
    {
        string hostIP = "localhost";
        int port = 10000;

        public Form1()
        {
            InitializeComponent();
        }

        private void sendPacket(string host, string command)
        {
            try
            {
                //new Tcpclient client with localhost and 10000 as IP and port respectively
                TcpClient client = new TcpClient(host, port);

                //set length variable to create only the necessary buffer
                int byteCount = Encoding.ASCII.GetByteCount(message.Text);
                byte[] sendData = new byte[byteCount];
                //send data with a '`' on the end to notate the end of a script
                sendData = Encoding.ASCII.GetBytes(command + message.Text);
                NetworkStream stream = client.GetStream();
                status.Text = "Sending to" + host;
                stream.Write(sendData, 0, sendData.Length);
                responseBox.Text = command;
                if (command == "sys")
                {
                    responseBox.Text = "cmd";
                    //Receive response
                    byte[] receivedBuffer = new byte[1000];
                    stream.Read(receivedBuffer, 0, receivedBuffer.Length);

                    //String to store data
                    StringBuilder rawCharacters = new StringBuilder();
                    responseBox.Text = "received";

                    //Parse to string and break at end
                    foreach (byte b in receivedBuffer)
                    {
                        rawCharacters.Append(Convert.ToChar(b).ToString());
                    }

                    //Convert to string to make pooter happy
                    string text = rawCharacters.ToString();
                    responseBox.Text = text;
                }
                

                
                //close stream and client connection
                stream.Close();
                client.Close();
                status.Text = "sent";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //send text button
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            sendPacket(hostIP, "pnt");
        }
        //send command button
        private void btnCommand_Click(object sender, EventArgs e)
        {
            sendPacket(hostIP, "sys");
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            sendPacket(hostIP, "prc");
        }
    }
}
