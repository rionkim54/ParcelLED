using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;

namespace ParcelLED
{
    public partial class Form1 : Form
    {
        public static string server_ip = "192.168.0.2";
        public static int port = 502;
        public static int data_addresss = 40000;


        public Form1()
        {

            InitializeComponent();

            Thread thread1 = new Thread(connect);  // Thread 객채 생성, Form과는 별도 쓰레드에서 connect 함수가 실행됨.
            thread1.IsBackground = true;  // Form이 종료되면 thread1도 종료.
            thread1.Start();  // thread1 시작.

            timer2.Start();

            this.Location = Properties.Settings.Default.Location;

            tbWinXY.Text = this.Location.X + "," + this.Location.Y;

            this.BringToFront();
            this.Focus();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);


            //textBox1.Font = new Font(textBox1.Font.FontFamily, 8);
            //textBox1.Text = "택배도착날짜\r\n12월31일\r\n화요일";

            textBox1.Font = new Font(textBox1.Font.FontFamily, 8);
            textBox1.Text = "남은\r\n보관시간\r\n23:30";
            textBox1.ForeColor = Color.Orange;

        }

        public static TcpClient client = new TcpClient();
        public static NetworkStream stream;

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private void connect()  // thread1에 연결된 함수. 메인폼과는 별도로 동작한다.
        {
            //TcpClient tc = new TcpClient(server_ip, 502);
            ////TcpClient tc = new TcpClient("localhost", 7000);

            ////string msg = "Hello World";
            ////byte[] buff = Encoding.ASCII.GetBytes(msg);
            //Byte[] sendbuf = new byte[] { 0, 6, 0, 0, 0, 6, 1, 1, 0x9c, 0x40, 0, 0x10 };


            //// (2) NetworkStream을 얻어옴 
            //NetworkStream stream = tc.GetStream();

            //// (3) 스트림에 바이트 데이타 전송
            //stream.Write(sendbuf, 0, sendbuf.Length);

            //// (4) 스트림으로부터 바이트 데이타 읽기
            //byte[] outbuf = new byte[1024];
            //int nbytes = stream.Read(outbuf, 0, outbuf.Length);
            //string output = Encoding.ASCII.GetString(outbuf, 0, nbytes);

            //// (5) 스트림과 TcpClient 객체 닫기
            //stream.Close();
            //tc.Close();

            //Console.WriteLine($"{nbytes} bytes: {output}");

            try
            {
                IPEndPoint clientAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 55555);
                IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(server_ip), port);

                Console.WriteLine("클라이언트: {0}, 서버: {1}", clientAddress.ToString(), serverAddress.ToString());

                client.Connect(serverAddress);

                //byte[] data = Encoding.Default.GetBytes(message);
                stream = client.GetStream();


                //Byte[] sendbuf = new byte[] { 0, 0, 0, 0, 0, 6, 0, 3, 0x9c, 0x40, 0, 0x0a };
                //stream.Write(sendbuf, 0, sendbuf.Length);

                //Console.WriteLine("송신: {0}", sendbuf);

                Byte [] data = new byte[256];

                string responseData = "";

                while (stream.CanRead)
                {
                    byte [] myReadBuffer = new byte[1024];
                    String myCompleteMessage = "";
                    int numberOfBytesRead = 0;
                   

                    // Incoming message may be larger than the buffer size.
                    do
                    {
                        numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        //myCompleteMessage =
                        //    String.Concat(myCompleteMessage, Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

                        byte[] byteBuffer = new byte[numberOfBytesRead]; //  myReadBuffer.Length];

                        Array.Copy(myReadBuffer, 0, byteBuffer, 0, numberOfBytesRead); //  myReadBuffer.Length);

                        myCompleteMessage = ByteArrayToString(byteBuffer);


                    }
                    while (stream.DataAvailable);

                    Console.WriteLine(myCompleteMessage);
                }
                //else
                //{
                //    Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
                //}

                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
        }

        private void writeRichTextbox(string data)  // richTextbox1 에 쓰기 함수
        {
            System.Console.WriteLine(data);
        }

        private int xpos = 0, ypos = 0;
        public string mode = "Left-to-Right";




        private void timer2_Tick(object sender, EventArgs e)
        {
            if (stream == null) return;
            //NetworkStream stream = tc.GetStream();
            Byte[] sendbuf = new byte[] { 0, 0, 0, 0, 0, 6, 0, 3, 0, 0, 0, 0x0a };
            stream.Write(sendbuf, 0, sendbuf.Length);

            Console.WriteLine("송신: {0}", sendbuf);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // (5) 스트림과 TcpClient 객체 닫기
            Properties.Settings.Default.Location = this.Location;
            Properties.Settings.Default.Save();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Left)
            {
                this.Location = new Point(this.Location.X - 1, this.Location.Y);
                Properties.Settings.Default.Location = this.Location;
                Properties.Settings.Default.Save();
                tbWinXY.Text = this.Location.X + "," + this.Location.Y;
            }
            else if (e.KeyCode == Keys.Right)
            {
                this.Location = new Point(this.Location.X + 1, this.Location.Y);
                Properties.Settings.Default.Location = this.Location;
                Properties.Settings.Default.Save();
                tbWinXY.Text = this.Location.X + "," + this.Location.Y;

            }
            else if (e.KeyCode == Keys.Up)
            {
                this.Location = new Point(this.Location.X, this.Location.Y-1);
                Properties.Settings.Default.Location = this.Location;
                Properties.Settings.Default.Save();
                tbWinXY.Text = this.Location.X + "," + this.Location.Y;

            }
            else if (e.KeyCode == Keys.Down)
            {
                this.Location = new Point(this.Location.X, this.Location.Y + 1);
                Properties.Settings.Default.Location = this.Location;
                Properties.Settings.Default.Save();
                tbWinXY.Text = this.Location.X + "," + this.Location.Y;

            }


        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            this.Location = new Point(this.Location.X, this.Location.Y + 1);
            Properties.Settings.Default.Location = this.Location;
            Properties.Settings.Default.Save();
            tbWinXY.Text = this.Location.X + "," + this.Location.Y;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }
    }
}
