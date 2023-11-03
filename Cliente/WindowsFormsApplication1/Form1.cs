using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }


        private void button2_Click(object sender, EventArgs e)
        {

            if (Longitud.Checked)
            {
                // Quiere saber la longitud
                string mensaje = "1/" + nombre.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show("La longitud de tu nombre es: " + mensaje);
            }
            else if (Bonito.Checked)
            {
                // Quiere saber si el nombre es bonito
                string mensaje = "2/" + nombre.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];


                if (mensaje == "SI")
                    MessageBox.Show("Tu nombre ES bonito.");
                else
                    MessageBox.Show("Tu nombre NO es bonito. Lo siento.");


            }
            else if (Alto.Checked)
            {
                string mensaje = "3/" + nombre.Text + "/" + Altura.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                MessageBox.Show(mensaje);
            }
            else if (Palindromo.Checked)
            {
                string mensaje = "4/" + nombre.Text;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] response = new byte[80];
                server.Receive(response);
                string respuesta = Encoding.ASCII.GetString(response).Split('\0')[0];
                MessageBox.Show(respuesta == "SI" ? "Tu nombre es palíndromo" : "Tu nombre no es palíndromo");
            }
            else if(Mayusculas.Checked){
                string mensaje = "5/" + nombre.Text;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] response = new byte[80];
                server.Receive(response);
                string respuesta = Encoding.ASCII.GetString(response).Split('\0')[0];
                MessageBox.Show("Tu nombre en mayúsculas: " + respuesta);
            }

        }


        private void Conectar_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("172.18.10.220");
            IPEndPoint ipep = new IPEndPoint(direc, 9050);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
        }

        private void Desconectar_Click(object sender, EventArgs e)
        {
            string message = "0/";
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            server.Send(bytes);

            // Se terminó el servicio. 
            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        private void servicios_Click(object sender, EventArgs e)
        {
            // obtiene el número de servicios atendidos por el servidor
            string message = "6/";
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            server.Send(bytes);

            byte[] response = new byte[80];
            server.Receive(response);
            string respuesta = Encoding.ASCII.GetString(response).Split('\0')[0];
            serviciosLabel.Text = respuesta;
        }
    }
}
