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
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atenderServidor;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

        }

        private void AtenderServidor()
        {
            while (true)
            {
                byte[] msg2 = new byte[200];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string respuesta = trozos[1].Split('\0')[0];

                switch (codigo){
                    case 1:
                        MessageBox.Show("La longitud de tu nombre es: " + respuesta);
                        break;
                    case 2:
                        if (respuesta == "SI")
                            MessageBox.Show("Tu nombre ES bonito.");
                        else
                            MessageBox.Show("Tu nombre NO es bonito. Lo siento.");
                        break;
                    case 3:
                        MessageBox.Show(respuesta);
                        break;
                    case 4:
                        MessageBox.Show(respuesta == "SI" ? "Tu nombre es palíndromo" : "Tu nombre no es palíndromo");
                        break;
                    case 5:
                        MessageBox.Show("Tu nombre en mayúsculas: " + respuesta);
                        break;
                    case 6: //notificación de actualización de contador de peticiones.
                        serviciosLabel.Text = "Número de peticiones: " + respuesta;
                        break;

                }
            }
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
               
            }
            else if (Bonito.Checked)
            {
                // Quiere saber si el nombre es bonito
                string mensaje = "2/" + nombre.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            else if (Alto.Checked)
            {
                string mensaje = "3/" + nombre.Text + "/" + Altura.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            else if (Palindromo.Checked)
            {
                string mensaje = "4/" + nombre.Text;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            else if(Mayusculas.Checked){
                string mensaje = "5/" + nombre.Text;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

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

            ThreadStart ts = delegate { AtenderServidor(); };
            atenderServidor = new Thread(ts);
            atenderServidor.Start();
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

            atenderServidor.Abort();
        }

    }
}
