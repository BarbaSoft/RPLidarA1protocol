using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Drawing;

using System.IO;


namespace RPLidarA1protocol
{
    public class SerialLidar
    {
        SerialPort seriale;
        Encoding e;
        Thread m_Thread;
        Thread m_SimpleScanThread;
        static string SYNC = "" + (char)0xA5 + (char)0x5A;
        private string m_ricevuto;
        private int lbyte;
        private bool m_STOP;
        public float[,] Data = new float[10000,2];
        public int pos;
        public bool ToClean;
        public Point[] PointData = new Point[10000];
        bool iniziato;
        int tipo;
        int scalelvl;

        public string[] cercaseriale()
        {
            string[] porte = SerialPort.GetPortNames(); //Cerca le porte seriali
            return porte;
        }

        public bool Connect(string com, int b)
        {
            seriale = new SerialPort(com, b, Parity.None, 8, StopBits.One);
            seriale.ReadTimeout = 1000;
            seriale.ReadBufferSize = 10000;
            e = System.Text.ASCIIEncoding.Default;
            seriale.Encoding = e;
            try
            {
                seriale.Open();

            }
            catch
            {
                return false;
            }
            seriale.DtrEnable = true; //Stop Motor
            m_STOP = false;
            tipo = 0;
            return true;
        }

        public bool IsConnected ()
        {
            bool b;
            if (seriale == null) return false;
            b=seriale.IsOpen;
            return b;
        }

        public void Disconnect()
        {
            if (seriale.IsOpen == true) seriale.Close();
        }

        public bool Express_Scan(int t)
        {
            tipo = t;
            seriale.DtrEnable = false; //Start Motor
            if (!GetHealt())
            {
                Reset(); //Try Reset
                if (!GetHealt())
                {
                    Disconnect(); //Bad
                    return false;
                }
            }
            string inviare;
            if (tipo == 1)
            {
                inviare = "" + (char)0xA5 + (char)0x82 + (char)0x05 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0X22;
            }
            else
            {
                for (int j = 0; j < 5; j++)
                {
                    //GET_RPLIDAR_CONFIG
                    inviare = "" + (char)0xA5 + (char)0x84 + (char)0x24 + (char)0x75 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x02;
                    for (int x = 0; x < 31; x++) inviare += (char)0x00;
                    inviare += (char)0x72;
                    Writeserial(inviare);
                    ReadSerial1Shot();

                    inviare = "" + (char)0xA5 + (char)0x5A + (char)0x05 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x20 + (char)0x75
                        + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x84;

                    if (m_ricevuto.IndexOf(inviare) >= 0)
                    {
                        break;
                    }
                }
            }
            inviare = "" + (char)0xA5 + (char)0x82 + (char)0x05 + (char)0x02 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0X20;

            Writeserial(inviare);
            pos = 0; //posizione matrice
            if (m_Thread == null) m_Thread = new Thread(ReadSerial);
            while (m_Thread.IsAlive)
            {
                m_STOP = true;
            }
            m_STOP = false;
            pos = 0;
            m_Thread.Start();
            return true;
        }

        public bool Scan ()
        {
            if (!IsConnected()) return false;
            seriale.DtrEnable = false; //Start Motor
            if (!GetHealt())
            {
                Reset(); //Try Reset
                if (!GetHealt())
                {
                    Disconnect(); //Bad
                    return false;
                }
            }
            string inviare;
            inviare = "" + (char)0xA5 + (char)0x20;
            Writeserial(inviare);
            if (m_SimpleScanThread == null) m_SimpleScanThread = new Thread(ReadSerialSimple);
            while (m_SimpleScanThread.IsAlive)
            {
                m_STOP = true;
            }
            m_STOP = false;
            pos = 0; //posizione matrice
            m_SimpleScanThread.Start();
            return true;
        }

        private void Stop_Restart (int t)
        {
            string inviare;
            if (t==0)
            {
                m_SimpleScanThread.Abort();
            }
            if (t!=0)
            {
                m_Thread.Abort();
            }

            inviare = "" + (char)0xA5 + (char)0x25;
            Writeserial(inviare);
            ReadSerial1Shot(); // clear read serial buffer

            if (t==0) //Simple
            {
                inviare = "" + (char)0xA5 + (char)0x20;
                Writeserial(inviare);
                if (m_SimpleScanThread == null) m_SimpleScanThread = new Thread(ReadSerialSimple);
                while (m_SimpleScanThread.IsAlive)
                {
                    m_STOP = true;
                }
                m_STOP = false;
                pos = 0; //posizione matrice
                m_SimpleScanThread.Start();
                return;
            }
            if (t == 1) //Express Scan
            {
                inviare = "" + (char)0xA5 + (char)0x82 + (char)0x05 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0X22;
            }
            else //Boost Scan
            {
                //GET_RPLIDAR_CONFIG
                inviare = "" + (char)0xA5 + (char)0x84 + (char)0x24 + (char)0x75 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x02;
                for (int x = 0; x < 31; x++) inviare += (char)0x00;
                inviare += (char)0x72;
                Writeserial(inviare);
                ReadSerial1Shot();

                inviare = "" + (char)0xA5 + (char)0x5A + (char)0x05 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x20 + (char)0x75
                    + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x84;

                if (m_ricevuto.IndexOf(inviare) < 0)
                {
                    return;
                }
                inviare = "" + (char)0xA5 + (char)0x82 + (char)0x05 + (char)0x02 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0X20;
            }
            Writeserial(inviare);
            pos = 0; //posizione matrice
            if (m_Thread == null) m_Thread = new Thread(ReadSerial);
            while (m_Thread.IsAlive)
            {
                m_STOP = true;
            }
            m_STOP = false;
            pos = 0;
            m_Thread.Start();
        }

        public void Stop_Express_Scan ()
        {
            string inviare;
            if (m_SimpleScanThread != null)
            {
                while (m_SimpleScanThread.IsAlive)
                {
                    m_STOP = true;
                }
                m_SimpleScanThread = null;
            }
            if (m_Thread != null)
            {
                while (m_Thread.IsAlive)
                {
                    m_STOP = true;
                }
                m_Thread = null;
            }
            inviare = "" + (char)0xA5 + (char)0x25;
            Writeserial(inviare);
            ReadSerial1Shot(); // clear read serial buffer
            seriale.DtrEnable = true; //Stop Motor
        }

        public void Reset ()
        {
            string inviare;
            inviare = "" + (char)0xA5 + (char)0x40;
            Writeserial(inviare);
            Thread.Sleep(5000);
        }
        public bool GetHealt ()
        {
            string inviare;
            int pos;
            inviare = "" + (char)0xA5 + (char)0x52;
            Writeserial(inviare);
            if (!ReadSerial1Shot()) return false;
            pos = m_ricevuto.IndexOf(SYNC);
            if (pos < 0) return false;
            if (m_ricevuto.Substring((pos + 3), 3) != "\0\0\0") return false;
            return true;
        }

        public string SerialNum ()
        {
            string inviare;
            int pos;
            inviare = "" + (char)0xA5 + (char)0x50;
            Writeserial(inviare);
            if (!ReadSerial1Shot()) return "";
            pos = m_ricevuto.IndexOf(SYNC);
            if (pos < 0) return "";
            inviare = "" + m_ricevuto.Substring(pos + 7);
            return inviare;
        }

        private bool ReadSerial1Shot()
        {
            lbyte = 0;
            m_ricevuto = "";

            for (int x = 0; (x < 700) && (lbyte == 0); x++)
            {
                System.Threading.Thread.Sleep(10);
                try
                {
                    lbyte = seriale.BytesToRead;
                }
                catch
                {
                    return false;
                }
            }

            try
            {
                lbyte = seriale.BytesToRead;
            }
            catch
            {
                return false;
            }
            for (int z = 0; z < lbyte; z++)
            {
                m_ricevuto += (char)seriale.ReadByte();
            }
            return true;
        }
        private bool Writeserial(string s)
        {
            byte[] ss = new byte[100];
            for (int x = 0; x < s.Length; x++)
            {
                ss[x] = (byte)s.ElementAt(x);
            }

            try
            {
                seriale.Write(ss, 0, s.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void ReadSerialSimple()
        {
            if (!IsConnected()) return;
            for (int x = 0; x < 10000; x++)
            {
                Data[x, 0] = 0;
                Data[x, 1] = 0;
                PointData[x].X = 0;
                PointData[x].Y = 0;
            }
            ToClean = false;
            lbyte = 0;
            int p;
            m_ricevuto = "";

            ReadSerial1Shot();

            p = m_ricevuto.IndexOf(SYNC);
            if (p < 0) Stop_Express_Scan();
            m_ricevuto = m_ricevuto.Remove(p, 7);
            
            while (true)
            {
                if (ToClean == true)
                {
                    for (int x = 0; x < 10000; x++)
                    {
                        Data[x, 0] = 0;
                        Data[x, 1] = 0;
                        PointData[x].X = 0;
                        PointData[x].Y = 0;
                    }
                    pos = 0;
                    ToClean = false;
                }

                try
                {
                    lbyte = seriale.BytesToRead;
                }
                catch
                {
                    Stop_Express_Scan();
                }
                if (lbyte == 0) continue;
                if (pos >= 9998) continue;

                for (int z = 0; (z < lbyte) && (z < 0x100); z++)
                {
                    m_ricevuto += (char)seriale.ReadByte();
                }
                if (m_ricevuto.Length>=5)
                {
                    while (m_ricevuto.Length >= 5)
                    {
                        DecodeSimple(m_ricevuto.Substring(0, 5));
                        m_ricevuto = m_ricevuto.Remove(0, 5);
                    }
                }
                if (m_STOP == true) break;
            }
        }

        private void DecodeSimple (string data)
        {
            float fstartangle;
            int startangle;
            startangle = 0;
            int d1,quality;
            
            quality = RotateRight((byte)data.ElementAt(0), 2);
            startangle = RotateRight((byte)data.ElementAt(1),1);
            startangle = startangle + (RotateLeft((byte)data.ElementAt(2), 7));
            fstartangle = (float)startangle / 64;

            d1 = RotateLeft((byte)data.ElementAt(4), 8);
            d1 = d1 + (byte)data.ElementAt(4);


            if (pos >= 9998) return;

            if (d1 != 0)
            {
                Data[pos, 0] = fstartangle;
                Data[pos, 1] = (float)d1 / 4;
                PointData[pos].X = (int)(Data[pos, 1] * System.Math.Sin((0.01745328)* Data[pos, 0]));
                PointData[pos].Y = (int)(Data[pos, 1] * System.Math.Cos((0.01745328)* Data[pos, 0]));
                pos++;
            }
        }

        private void ReadSerial()
        {
            for (int x=0;x<10000;x++)
            {
                Data[x,0] = 0;
                Data[x, 1] = 0;
                PointData[x].X = 0;
                PointData[x].Y = 0;
            }
            ToClean = false;
            lbyte = 0;
            int p;
            iniziato = false;
            char c;
            m_ricevuto = "";
            p = 0;
           
            ReadSerial1Shot();

            p = m_ricevuto.IndexOf(SYNC);
            if (p < 0) Stop_Express_Scan();
            m_ricevuto=m_ricevuto.Remove(p, 7);
    
            while (true)
            {
                if (ToClean == true)
                {
                    for (int x = 0; x < 10000; x++)
                    {
                        Data[x, 0] = 0;
                        Data[x, 1] = 0;
                        PointData[x].X = 0;
                        PointData[x].Y = 0;
                    }
                    pos = 0;
                    ToClean = false;
                }

                try
                {
                    lbyte = seriale.BytesToRead;
                }
                catch
                {
                    Stop_Express_Scan();
                }
                if (lbyte == 0) continue;
                if (pos >= 9998) continue;

                for (int z = 0; z < lbyte; z++)
                {
                    c = (char)seriale.ReadByte();
                    m_ricevuto += c;
                    if ((tipo==1)&&(m_ricevuto.Length == 88))
                    {
                        DecodeCabin(m_ricevuto);
                        m_ricevuto = m_ricevuto.Substring(84);
                        break;
                    }
                    if ((tipo == 2) && (m_ricevuto.Length == 136))
                    {
                        DecodeCabinBest(m_ricevuto);
                        m_ricevuto = m_ricevuto.Substring(132);
                        break;
                    }
                }

                if (m_STOP == true)
                {
                    break;
                }
                
            }

        }

        private void DecodeCabin (string data)
        {
            float fstartangle,fstartanglenext,delta;
            int startangle,startanglenext;
            startangle = 0;
            int a, b,d1,d2,da1,da2,k;
            byte mask;
            float f1,f2;

            a = (int)data.ElementAt(0);
            b = (int)data.ElementAt(1);
            a >>= 4;
            b >>= 4;
            if ((a != 0x0A) || (b != 0x05)) //SYNC PRESENT ?
            {
                Stop_Restart(1);
            }

            if (!iniziato)
            {
                mask = 0b10000000;
                d1 = RotateRight((byte)data.ElementAt(3) & (mask),7);//WAIT NEW 360 SCAN
                if (d1 != 0)
                {
                    iniziato = true;
                }
                else
                {
                    return;
                }
            }

            startangle = (byte)data.ElementAt(2);
            mask = 0b01111111;
            d1 = (byte)data.ElementAt(3) & (mask);
            startangle =startangle+ (RotateLeft(d1, 8));
            fstartangle = (float)startangle/64;

            startanglenext = (byte)data.ElementAt(86);
            mask = 0b01111111;
            d1 = (byte)data.ElementAt(87) & (mask);
            startanglenext = startanglenext + (RotateLeft(d1, 8));
            fstartanglenext = (float)startanglenext / 64;
            if (fstartanglenext < fstartangle) fstartanglenext = fstartanglenext + 360;
            delta = (fstartanglenext - fstartangle) / 32;

            k = 0;

            for (int x=4;x< data.Length; x=x+5) 
            {
                if (x + 5 > data.Length)
                {
                    break;
                }

                mask = 0b11111100;
                d1 = (byte)data.ElementAt(x) & (mask);
                d1 = RotateRight(d1, 2);
                d1 = d1+(RotateLeft(data.ElementAt(x+1),6));

                mask = 0b11111100;
                d2 = (byte)data.ElementAt(x+2) & (mask);
                d2 = RotateRight(d2, 2);
                d2 = d2 + (RotateLeft(data.ElementAt(x + 3), 6));

                mask = 0b00001111;
                da1 = ((byte)data.ElementAt(x + 4));
                da1 = da1 & (mask);
                mask = 0b00000011;
                da1 = da1 + RotateLeft(((byte)data.ElementAt(x)) & (mask), 4);
                if (da1 >= 32)
                {
                    da1 = (da1 - 32);
                    da1 = -da1;
                }
                f1 = (fstartangle + (delta * k)) - ((float)da1 / 8);
                k++;
                if (f1 > 360)
                {
                    f1 = f1 - 360;
                }

                mask = 0b11110000;
                da2 = ((byte)data.ElementAt(x + 4));
                da2 = da2 & (mask);
                da2 = RotateRight(da2, 4);
                mask = 0b00000011;
                da2 = da2 + RotateLeft(((byte)data.ElementAt(x + 2)) & (mask), 4);
                if (da2 >= 32)
                {
                    da2 = (da2 - 32);
                    da2 = -da2;
                }
                f2 = (fstartangle + (delta * k)) - ((float)da2 / 8);
                k++;
                if (f2 > 360)
                {
                    f2 = f2 - 360;
                }

                if (pos >= 9998) continue;

                if (d1 != 0)
                {
                    PointData[pos].X = (int)(d1 * (System.Math.Sin((Math.PI / 180) * f1)));
                    PointData[pos].Y = (int)(d1 * (System.Math.Cos((Math.PI / 180) * f1)));
                    Data[pos, 0] = f1;
                    Data[pos, 1] = d1;
                    pos++;
                }
                if (d2 != 0)
                {
                    PointData[pos].X = (int)(d2 * (System.Math.Sin((Math.PI / 180) * f2)));
                    PointData[pos].Y = (int)(d2 * (System.Math.Cos((Math.PI / 180) * f2)));
                    Data[pos, 0] = f2;
                    Data[pos, 1] = d2;
                    pos++;
                }

            }
        }

        private void DecodeCabinBest(string data)
        {
            float fstartangle, fstartanglenext, delta;
            int startangle, startanglenext;
            startangle = 0;
            int a, b, predict1,predict2,major, major2, k, dist_base, dist_base2, scalelvl2;
            byte mask;
            float f1;

            a = (int)data.ElementAt(0);
            b = (int)data.ElementAt(1);
            a >>= 4;
            b >>= 4;
            if ((a != 0x0A) || (b != 0x05)) //SYNC PRESENT ?
            {
                Stop_Restart(2);
            }


            startangle = (byte)data.ElementAt(2);
            startangle =startangle+ RotateLeft((byte)data.ElementAt(3),8);
            fstartangle = (float)startangle / 64;

            startanglenext = (byte)data.ElementAt(134);
            startanglenext =startanglenext+RotateLeft((byte)data.ElementAt(135),8);
            fstartanglenext = (float)startanglenext / 64;
            if (fstartanglenext < fstartangle) fstartanglenext = fstartanglenext + 360;
            delta = (fstartanglenext - fstartangle) / 95;

            k = 0;

            data=data.Substring(0,132);
            for (int x = 4; x < data.Length; x = x + 4)
            {
                if (x + 4 > data.Length)
                {
                    break;
                }

                if ((x + 4) < data.Length)
                {
                    mask = 0b00001111;
                    major2 = ((byte)data.ElementAt(x + 4 + 1)) & mask;
                    major2 = RotateLeft(major2, 8);
                    major2 = major2 + (byte)data.ElementAt(x + 4);
                }
                else major2 = 0; //// NON E' PROPRIO COSI'

                major2 = varbitscale_decode(major2);
                dist_base2 = major2;
                scalelvl2 = scalelvl;

                mask = 0b00001111;
                major= ((byte)data.ElementAt(x + 1))&mask;
                major = RotateLeft(major, 8);
                major=major+ (byte)data.ElementAt(x);

                mask = 0b00111111;
                predict1 = ((byte)data.ElementAt(x + 2)) & mask;
                predict1 = RotateLeft(predict1, 4);
                mask = 0b11110000;
                predict1 = predict1 + (RotateRight((((byte)data.ElementAt(x + 1)) & mask), 4));
                

                predict2 = RotateLeft((byte)data.ElementAt(x+3),2);
                mask = 0b11000000;
                predict2 = predict2 + RotateRight(((byte)data.ElementAt(x + 2)) & mask, 6);


                major = varbitscale_decode(major);
                dist_base = major;

                if ((major==0)&(major2!=0))
                {
                    major = major2;
                    dist_base = major2;
                    scalelvl = scalelvl2;
                }

                major = RotateLeft(major, 2);

                mask = 0b11111111;
                predict1 = predict1 & mask;
                if (predict1>=0xF0) predict1 = RotateLeft((dist_base), 2);
                else
                {
                    predict1 = RotateLeft(predict1, scalelvl);
                    predict1 = RotateLeft((predict1+dist_base), 2);
                }

                mask = 0b11111111;
                predict2 = predict2 & mask;
                if (predict2 >= 0xF0) predict2 = RotateLeft((dist_base2), 2);
                else
                {
                    predict2 = RotateLeft(predict2, scalelvl2);
                    predict2 = RotateLeft((predict2 + dist_base2), 2);
                }
                
                f1 = (fstartangle + (delta * k));
                k++;
                if (f1 > 360)
                {
                    f1 = f1 - 360;
                }

                if (pos >= 9998) continue;

                if (major != 0)
                {
                    PointData[pos].X = (int)(major * (System.Math.Sin((Math.PI / 180) * f1)));
                    PointData[pos].Y = (int)(major * (System.Math.Cos((Math.PI / 180) * f1)));
                    Data[pos, 0] = f1;
                    Data[pos, 1] = major;
                    pos++;
                }

                f1 = (fstartangle + (delta * k));
                k++;
                if (f1 > 360)
                {
                    f1 = f1 - 360;
                }

                if (pos >= 9998) continue;

                if (predict1 != 0)
                {
                    PointData[pos].X = (int)(predict1 * (System.Math.Sin((Math.PI / 180) * f1)));
                    PointData[pos].Y = (int)(predict1 * (System.Math.Cos((Math.PI / 180) * f1)));
                    Data[pos, 0] = f1;
                    Data[pos, 1] = predict1;
                    pos++;
                }

                f1 = (fstartangle + (delta * k));
                k++;
                if (f1 > 360)
                {
                    f1 = f1 - 360;
                }

                if (pos >= 9998) continue;

                if (predict2 != 0)
                {
                    PointData[pos].X = (int)(predict2 * (System.Math.Sin((Math.PI / 180) * f1)));
                    PointData[pos].Y = (int)(predict2 * (System.Math.Cos((Math.PI / 180) * f1)));
                    Data[pos, 0] = f1;
                    Data[pos, 1] = predict2;
                    pos++;
                }
            }
        }

        private static int RotateLeft(int value, int count)
        {
            uint v;
            uint val = (uint)value;
            v= (uint)((val << count) | (val >> (32 - count)));
            uint mask;
            mask = 0b11111111111111110000000000000000;
            v = v & ~(mask);
            return (int)v;
        }

        private static int RotateRight(int value, int count)
        {
            uint v;
            uint val = (uint)value;
            v =(uint)((value >> count) | (value << (32 - count)));
            uint mask;
            mask = 0b11111111111111110000000000000000;
            v=v & ~(mask);
            return(int) v;
        }

        private int varbitscale_decode (int scaled)
        {
            int[] VBS_SCALED_BASE = { 3328, 1792, 1280, 512, 0 };
            int[] VBS_SCALED_LVL = { 4, 3, 2, 1, 0 };
            int[] VBS_TARGET_BASE = { 14, 12, 11, 9, 0 };

            for(int i= 0;i<VBS_SCALED_BASE.Length;i++)
            {
                int remain = scaled - VBS_SCALED_BASE[i];
                if (remain>=0)
                {
                    scalelvl = VBS_SCALED_LVL[i];
                    return (RotateLeft(0x01,VBS_TARGET_BASE[i]) + (RotateLeft(remain, VBS_SCALED_LVL[i])));
                }
            }
            scalelvl = 0;
            return 0;
        }

    }
}
