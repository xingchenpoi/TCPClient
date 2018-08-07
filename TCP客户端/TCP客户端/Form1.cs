using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP客户端
{
    public partial class Form1 : Form
    {
        private Socket socket;
        private Thread thread;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 发起连接请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_open_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(txt_IP.Text.Trim());
            IPEndPoint server = new IPEndPoint(ip,Convert.ToInt32(txt_port.Text));
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(server);
            }
            catch 
            {
                MessageBox.Show("服务器连接失败！","系统提示",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
                throw;
            }

            this.btn_open.Enabled = false;
            txt_state.Text += "服务器连接成功\n";
            thread = new Thread(new ThreadStart(AcceptMessage));
            thread.Start();
        }

        /// <summary>
        /// 开启一个线程，用于接收数据
        /// </summary>
        private void AcceptMessage()
        {
            while (true)
            {
                try
                {
                    byte[] message = new byte[1024];
                    int size = socket.Receive(message);
                    if (size == 0)
                    {
                        break;
                    }
                    this.txt_receive.Text += System.Text.Encoding.Unicode.GetString(message);
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        txt_state.Text += "与服务器断开\n";
                    }
                    ));
                    break;
                }  
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_send_Click(object sender, EventArgs e)
        {
            string str = txt_send.Text;
            int i = str.Length;
            if (i == 0)
            {
                return;
            }
            else
            {
                i *= 2;
            }
            byte[] sendbytes = System.Text.Encoding.Unicode.GetBytes(str);
            try
            {
                socket.Send(sendbytes,sendbytes.Length,SocketFlags.None);
                txt_send.Text = "";
            }
            catch (Exception)
            {
                MessageBox.Show("无法发送！","系统提示",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_close_Click(object sender, EventArgs e)
        {
            try
            {
                socket.Shutdown( SocketShutdown.Both);
                socket.Close();
                txt_state.Text += "与服务器断开连接\n";
                thread.Abort();
            }
            catch (Exception)
            {

                MessageBox.Show("尚未与主机连接！！","系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btn_open.Enabled = true;
        }
    }
}
