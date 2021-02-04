using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RPLidarA1protocol;
using OpenTK;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RPLidarA1Reader
{
    public partial class RPLidarA1Reader : Form
    {
        SerialLidar Lidar;
        int f;
        GameWindow gwin;
        Wrender wrender;
        Task task;
        int TStart, TNow, posold;
        Point[] poly_points = new Point[400];
        int poly_pos;
        int[,] R_Quadro = new int[100, 4];

 
        public RPLidarA1Reader()
        {
            poly_pos = 0;
            string[] porte;
            InitializeComponent();
            bStart.Visible = false;
            tSample.ForeColor = Color.Black;
            cMode.SelectedIndex = 2;
            Lidar = new SerialLidar();
            porte = Lidar.cercaseriale();
            cSerialPort.Items.Add("select");
            foreach (string porta in porte)
            {
                cSerialPort.Items.Add(porta);
            }
            if (cSerialPort.Items.Count > 0) cSerialPort.SelectedIndex = 0;
            tBaudrate.Text = "115200";
            Save.Visible = false;
        }

        private void On_Connect(object sender, EventArgs e)
        {
            if (Lidar.IsConnected())
            {
                Lidar.Disconnect();
                bConnect.Text = "Connect";
                cSerialPort.Enabled = true;
                tBaudrate.Enabled = true;
                bStart.Visible = false;
                return;
            }
            if (tBaudrate.Text == "")
            {
                tserial.Text = "Serial port error!";
                return;
            }
            
            if (cSerialPort.SelectedItem.ToString() =="select")
            {
                for (int i = 0; i < cSerialPort.Items.Count; i++)
                {
                    cSerialPort.SelectedIndex = i;

                    if (Lidar.Connect(cSerialPort.SelectedItem.ToString(), Int32.Parse(tBaudrate.Text)))
                    {
                        if (Lidar.SerialNum() != "")
                        {
                            bConnect.Text = "Disconnect";
                            break;
                        }
  
                    }
                }
            }
            else
            {
                if (Lidar.Connect(cSerialPort.SelectedItem.ToString(), Int32.Parse(tBaudrate.Text)))
                {
                    bConnect.Text = "Disconnect";
                }
                else
                {
                    tserial.Text = "Connection Error ! Verify, please.";
                    return;
                }
            }
            
           
            if (bConnect.Text == "Disconnect")
            {
                cSerialPort.Enabled = false;
                tBaudrate.Enabled = false;
                bStart.Visible = true;
            }

            string dato;
            int d;
            byte dd;
            dato = Lidar.SerialNum();
            if (dato == "")
            {
                tserial.Text = "Retrive Serial Error ! Please disconnect";
                return;
            }
            d = (int)dato.ElementAt(0);
            tserial.Text = "Mod:" + d.ToString() + " Ver:";
            d = (int)dato.ElementAt(2);
            tserial.Text += d.ToString() + ".";
            d = (int)dato.ElementAt(1);
            tserial.Text += d.ToString() + " Hw:";
            d = (int)dato.ElementAt(3);
            tserial.Text += d.ToString() + " Serial:";
            for (int x = 4; x < dato.Length; x++)
            {
                dd = (byte)dato.ElementAt(x);
                tserial.Text += dd.ToString("X") + " ";
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            if (bStart.Text == "Start Scan")
            {
                task = new Task(Grafica);
                task.Start();
                int i;
                cMode.Enabled = false;
                switch(cMode.SelectedIndex)
                {
                    case 0:
                        i = 0;
                        break;
                    case 1:
                        i = 1;
                        break;
                    case 2:
                        i = 2;
                        break;
                    default:
                        i = 0;
                        break;
                }

                if (i != 0)
                {
                    if (!Lidar.Express_Scan(i))
                    {
                        MessageBox.Show("Start Scan Error");
                    }
                }
                else Lidar.Scan();

                TStart = Environment.TickCount;
                posold = 0;
                bStart.Text = "Stop Scan";
                f = 0;
                Save.Visible = true;

                data1.Rows.Clear();
                data1.Rows.Add(100);
                for (int j = 0; j < 100; j++)
                {
                    DataGridViewCell cell;
                    cell=data1.Rows[j].Cells[0];
                    cell.Value = j.ToString();
                }

                timer1.Start();
                return;
            }
            if (bStart.Text == "Stop Scan")
            {
                Save.Visible = false;
                Lidar.Stop_Express_Scan();
                cMode.Enabled = true;
                bStart.Text = "Start Scan";
                timer1.Stop();
                return;
            }
        }

        private void On_Save_Data(object sender, EventArgs e)
        {
            WriteToBinaryFile("saved_data.dat", wrender.polygon_points, false);
            WriteToBinaryFile("saved_data.dat", wrender.polygon_pos, true);
            WriteToBinaryFile("saved_data.dat", wrender.Rif_Quadro, true);
            WriteToBinaryFile("saved_data.dat", tBaudrate.Text, true);
            WriteToBinaryFile("saved_data.dat", cMode.SelectedIndex, true);
        }

        private void On_Load_Data(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("saved_data.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                poly_points = (Point[])formatter.Deserialize(fs);
                poly_pos = (int)formatter.Deserialize(fs);
                R_Quadro = (int[,])formatter.Deserialize(fs);
                tBaudrate.Text=(string)formatter.Deserialize(fs);
                cMode.SelectedIndex = (int)formatter.Deserialize(fs);
            }
            catch 
            {

            }
            finally
            {
                fs.Close();
            }
        }

        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : System.IO.FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                try
                {
                    binaryFormatter.Serialize(stream, objectToWrite);
                }
                catch
                {

                }
            }
        }


        //public static T ReadFromBinaryFile<T>(string filePath)
        //{
        //    using (Stream stream = File.Open(filePath, FileMode.Open))
        //    {
        //        var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //        return (T)binaryFormatter.Deserialize(stream);
        //    }
        //}


        private void OnTimer(object sender, EventArgs e)
        {
            int i;
            i = 0;

            if (Lidar.pos == 0)
            {
                return;
            }

            TNow = Environment.TickCount - TStart;
            try
            {
                i = (Lidar.pos) - posold;
                i =(i / (TNow))*1000;
            }
            catch
            {
                tSample.Text = "0000";
            }
            TNow = i;
            tSample.Text = TNow.ToString("0000");

            for (int x = posold; x <= Lidar.pos; x++)
            {
                try
                {
                    wrender.Vertici[x] = Lidar.PointData[x];//Prevent exception windows OpenGL not ready
                }
                catch
                {
                    break;
                }
            }
            posold = Lidar.pos;

            if (Lidar.pos >= 9998)
            {
                Lidar.ToClean = true;
                posold = 0;
            }

            for (int j = 0; j < 100; j++)
            {
                DataGridViewCell cell1, cell2, cell3;
                cell1 = data1.Rows[j].Cells[1];
                try { cell1.Value = wrender.Rif_Quadro[j, 0].ToString(); } //Prevent exception windows OpenGL not ready
                catch { break; }
                cell2 = data1.Rows[j].Cells[2];
                cell2.Value = wrender.Rif_Quadro[j, 1].ToString();
                cell3 = data1.Rows[j].Cells[3];
                cell3.Value = wrender.Rif_Quadro[j, 2].ToString();
            }

            TStart = Environment.TickCount;
        }


        private void OnCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell1;
            string s;
            cell1 = data1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            s = cell1.Value.ToString();
            wrender.Rif_Quadro[e.RowIndex, (e.ColumnIndex - 1)] = Int32.Parse(s);
        }

        private void OnCellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                wrender.Rif_Quadro[e.RowIndex, 3] = 0;
            }
            catch { }
        }

        private void OnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            wrender.Rif_Quadro[e.RowIndex, 3] = 1;
        }

        private void Grafica ()
        {
            gwin = new GameWindow(800, 600);
            wrender = new Wrender(gwin);
            if (poly_pos != 0)
            {
                wrender.polygon_points = poly_points;
                wrender.polygon_pos = poly_pos;
                wrender.Rif_Quadro = R_Quadro;
            }
            gwin.Run();
        }
    }
}
