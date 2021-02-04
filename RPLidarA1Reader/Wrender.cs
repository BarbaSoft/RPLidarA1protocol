using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace RPLidarA1Reader
{
    class Wrender
    {
        public GameWindow rwin;
        public Point[] Vertici = new Point[10000];
        float scala,magnify;
        float transx, transy;
        public Point[] polygon_points = new Point[400];
        public int polygon_pos=0;
        Point PMouse = new Point();
        int[] quadro=new int[100];
        public int[,] Rif_Quadro = new int[100, 4]; //0 tipo  1 soglia   2 valore attuale   3 config

        public Wrender(GameWindow WindowInput)
        {
            rwin = WindowInput;
            rwin.Load += Rwin_Load;
            rwin.RenderFrame += Rwin_RenderFrame;
            rwin.UpdateFrame += Rwin_UpdateFrame;
            rwin.Closing += Rwin_Closing;
            rwin.MouseWheel += Rwin_MouseWheel;
            rwin.MouseMove += Rwin_MouseMove;
        }


        private void Rwin_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            if (Mouse.GetState().IsButtonDown(MouseButton.Left))
            {
                if (e.XDelta > 0) transx = transx + 0.05f;
                if (e.XDelta < 0) transx = transx - 0.05f;
                if (e.YDelta > 0) transy = transy - 0.05f;
                if (e.YDelta < 0) transy = transy + 0.05f;
            }
        }

        private void Rwin_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta>0)
            {
                scala = scala + 0.02f;
                if (scala >= 1.0f)
                {
                    scala = 1.0f;
                    magnify = magnify + 0.1f;
                }
            }
            else
            {
                if (magnify > 1.0f)
                {
                    magnify = magnify - 0.1f;
                }
                else scala = scala - 0.02f;

                if (scala < 0)
                {
                    scala = 0;
                }
            }
        }

        private void Rwin_Load(object sender, EventArgs e)
        {
            scala = 1.0f;
            magnify = 1.0f;
            transx = 0;
            transy = 0;
            PMouse.X = 0;
            PMouse.Y = 0;
            rwin.Title = "RPLidar OpenGL";
            for (int g = 0; (g < 100)&&(polygon_pos==0); g++)
            {
                Rif_Quadro[g, 0] = 0;
                Rif_Quadro[g, 1] = 20;
                Rif_Quadro[g, 2] = 0;
                Rif_Quadro[g, 3] = 0;
            }
        }

        private void Rwin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        KeyboardState lastkeystate;
        private void Rwin_UpdateFrame(object sender, FrameEventArgs e)
        {
            KeyboardState keystate = Keyboard.GetState();
            if ((keystate.IsKeyUp(Key.Left) && (lastkeystate.IsKeyDown(Key.Left)))||((keystate.IsKeyDown(Key.Q))
                &&(keystate.IsKeyDown(Key.Left))))
            {
                if (keystate.IsKeyDown(Key.Q)) PMouse.X = PMouse.X - 100;
                else PMouse.X--;
            }
            if ((keystate.IsKeyUp(Key.Right) && (lastkeystate.IsKeyDown(Key.Right))) || ((keystate.IsKeyDown(Key.Q))
                && (keystate.IsKeyDown(Key.Right))))
            {
                if (keystate.IsKeyDown(Key.Q)) PMouse.X = PMouse.X + 100;
                else PMouse.X++;
            }
            if ((keystate.IsKeyUp(Key.Up) && (lastkeystate.IsKeyDown(Key.Up))) || ((keystate.IsKeyDown(Key.Q))
                   && (keystate.IsKeyDown(Key.Up))))
            {
                if (keystate.IsKeyDown(Key.Q)) PMouse.Y = PMouse.Y + 100;
                else PMouse.Y++;
            }
            if ((keystate.IsKeyUp(Key.Down) && (lastkeystate.IsKeyDown(Key.Down))) || ((keystate.IsKeyDown(Key.Q))
                  && (keystate.IsKeyDown(Key.Down))))
            {
                if (keystate.IsKeyDown(Key.Q)) PMouse.Y = PMouse.Y - 100;
                else PMouse.Y--;
            }

            if ((keystate.IsKeyUp(Key.Space) && (lastkeystate.IsKeyDown(Key.Space))))
            {
                int resto;
                resto = polygon_pos % 4;
                switch (resto)
                {
                    case 0:
                        polygon_points[polygon_pos] = PMouse;
                        break;

                    case 1:
                        int a, b;
                        a = Math.Abs((PMouse.X + 50000) - (polygon_points[polygon_pos - 1].X + 50000));
                        b = Math.Abs((PMouse.Y + 50000) - (polygon_points[polygon_pos - 1].Y + 50000));
                        if (a> b)
                        {
                            polygon_points[polygon_pos].X = PMouse.X;
                            polygon_points[polygon_pos].Y = polygon_points[polygon_pos - 1].Y;
                        }
                        else
                        {
                            polygon_points[polygon_pos].Y = PMouse.Y;
                            polygon_points[polygon_pos].X = polygon_points[polygon_pos - 1].X;
                        }
                        break;

                    case 2:
                        if ((polygon_points[polygon_pos - 1].X) == (polygon_points[polygon_pos - 2].X))
                        {
                            polygon_points[polygon_pos].X = PMouse.X;
                            polygon_points[polygon_pos].Y = polygon_points[polygon_pos - 1].Y;
                        }
                        else
                        {
                            polygon_points[polygon_pos].Y = PMouse.Y;
                            polygon_points[polygon_pos].X = polygon_points[polygon_pos - 1].X;
                        }
                        break;

                    case 3:
                        if ((polygon_points[polygon_pos - 1].X) == (polygon_points[polygon_pos - 2].X))
                        {
                            polygon_points[polygon_pos].X = polygon_points[polygon_pos-3].X;
                            polygon_points[polygon_pos].Y = polygon_points[polygon_pos - 1].Y;
                        }
                        else
                        {
                            polygon_points[polygon_pos].Y = polygon_points[polygon_pos - 3].Y;
                            polygon_points[polygon_pos].X = polygon_points[polygon_pos - 1].X;
                        }
                        break;

                    default:
                        break;
                }
                
                

                polygon_pos++;
            }

            if ((keystate.IsKeyUp(Key.C))&& (lastkeystate.IsKeyDown(Key.C)))
            {
                int resto;
                resto = polygon_pos % 4;
                if (resto != 0)
                {
                    for (int i = polygon_pos; i > (polygon_pos - resto); i--)
                    {
                        polygon_points[i].X = 0;
                        polygon_points[i].Y = 0;
                    }
                    polygon_pos = polygon_pos - resto; ;
                }
                else
                {
                    for (int i = polygon_pos; i > (polygon_pos-4); i--)
                    {
                        polygon_points[i].X = 0;
                        polygon_points[i].Y = 0;
                    }
                    if (polygon_pos!=0)
                    {
                        Rif_Quadro[(polygon_pos / 4), 0] = 0;
                        Rif_Quadro[(polygon_pos / 4), 1] = 20;
                        Rif_Quadro[(polygon_pos / 4), 2] = 0;
                        Rif_Quadro[(polygon_pos / 4), 3] = 0;
                    }
                    polygon_pos = polygon_pos - 4;
                }
            }

            if (keystate.IsKeyUp(Key.S) && (lastkeystate.IsKeyDown(Key.S)))
            {
                scala = scala + 0.02f;
                if (scala>=1.0f)
                {
                    scala = 1.0f;
                    magnify = magnify + 0.1f;
                }
            }
            if (keystate.IsKeyUp(Key.A) && (lastkeystate.IsKeyDown(Key.A)))
            {
                if (magnify > 1.0f)
                {
                    magnify = magnify - 0.1f;
                }
                else scala = scala - 0.02f;

                if (scala < 0)
                {
                    scala = 0;
                }
            }
            lastkeystate = keystate;
        }

        private void Rwin_RenderFrame(object sender, FrameEventArgs e)
        {
            int qq;
            GL.ClearColor(Color.Black);
            GL.ClearDepth(1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 projMatrix = Matrix4.CreateOrthographicOffCenter(-3000,3000,-3000, 3000, 0, 1)
                * Matrix4.CreateTranslation(transx, transy, 0)* Matrix4.CreateScale(scala);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.PointSize(3);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(Color.Green);
            GL.Vertex2(PMouse.X * magnify, PMouse.Y * magnify);
            GL.End();

            GL.PointSize(1);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(Color.Red);
            for (int g=0;g<100;g++)
            {
                quadro[g] = 0;
            }
            foreach (Point p in Vertici)
            {
                GL.Vertex2(p.X * magnify, p.Y * magnify);//GL.Vertex3((p.X*magnify),(p.Y*magnify),0);

                qq = 0;
                for (int i = 0; i < polygon_pos; i = i + 4) //POINT IN THE BOX ?
                {
                    int maxx,minx,maxy,miny, px,py;
                    if ((i + 3) > (polygon_pos - 1)) break;
                    px = p.X + 20000;
                    py = p.Y + 20000;
                    minx = 50000;
                    miny = 50000;
                    maxx = 0;
                    maxy = 0;
                    for (int f=0;f<4;f++)
                    {
                        if (polygon_points[i + f].X +20000< minx) minx = polygon_points[i + f].X + 20000;
                        if (polygon_points[i + f].Y +20000< miny) miny = polygon_points[i + f].Y + 20000;
                        if (polygon_points[i + f].X +20000> maxx) maxx = polygon_points[i + f].X + 20000;
                        if (polygon_points[i + f].Y +20000> maxy) maxy = polygon_points[i + f].Y + 20000;
                    }

                    if ((px<maxx)&&(px>minx)&&(py<maxy)&&(py>miny))
                    {
                        quadro[qq] = quadro[qq] + 1;
                    }
                    qq++;
                }
            }
            GL.End();

            int ss;
            Color c;
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            int resto;
            resto = polygon_pos % 4;
            qq = 0;
            for (int i = 0; i < (polygon_pos-resto); i = i + 4)
            {
                Rif_Quadro[qq, 2] = quadro[qq];
                if (Rif_Quadro[qq,2] >= Rif_Quadro[qq,1])
                {
                    ss = 4;
                    c = Color.Magenta;
                }
                else
                {
                    ss = 1;
                    c = Color.Yellow;
                }

                if (Rif_Quadro[qq,3]==1)   // Config
                {
                    ss = 4;
                    c = Color.Green;
                }

                GL.LineWidth(ss);
                GL.Color3(c);
                GL.Begin(PrimitiveType.Quads);
                for (int ii = i; ii <(i+4); ii++)
                {
                    GL.Vertex2(polygon_points[ii].X * magnify, polygon_points[ii].Y * magnify);
                }

                GL.End();
                qq++;
            }

            if (resto!=0)
            {
                GL.LineWidth(1);
                GL.Color3(Color.Yellow);
                int i;
                i = polygon_pos - resto;
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(polygon_points[i].X * magnify, polygon_points[i].Y * magnify);
                if ((i+1)<polygon_pos)
                {
                    GL.Vertex2(polygon_points[i+1].X * magnify, polygon_points[i+1].Y * magnify);
                }
                GL.End();
                if ((i + 2) < polygon_pos)
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(polygon_points[i + 1].X * magnify, polygon_points[i + 1].Y * magnify);
                    GL.Vertex2(polygon_points[i + 2].X * magnify, polygon_points[i + 2].Y * magnify);
                    GL.End();
                }
            }

            GL.Flush();
            rwin.SwapBuffers();

        }

    }
}
