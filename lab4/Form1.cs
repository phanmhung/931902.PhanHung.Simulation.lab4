using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab4
{
    public partial class Form1 : Form
    {
        private bool isRunning = false;
        private bool isRandom = false;
        private bool[,] map;
        private int cols;
        private int rows;
        private Graphics graphic;
        private int resolution;

        public Form1()
        {
            InitializeComponent();

        }

        private void calNextGen()
        {
            graphic.Clear(Color.DodgerBlue);

            var newMap = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int neighbours = countNeighbours(x, y);
                    bool livable = map[x, y];

                    //Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                    if (livable && neighbours < 2)
                    {
                        newMap[x, y] = false;
                    }
                    
                    //Any live cell with two or three live neighbours lives on to the next generation.                    
                    if (livable && (neighbours == 2 || neighbours == 3))
                    {
                        newMap[x, y] = true;
                    }

                    //Any live cell with more than three live neighbours dies, as if by overpopulation.                    
                    if (livable && neighbours > 3)
                    {
                        newMap[x, y] = false;
                    }

                    //Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                    if (!livable && neighbours == 3)
                    {
                        newMap[x, y] = true;
                    }


                    if (livable)
                    {
                        graphic.FillRectangle(Brushes.LightYellow, x * resolution, y * resolution, resolution - 1, resolution - 1);
                    }
                    
                }
            }

            map = newMap;
            pictureBox1.Refresh();
        }

        private int countNeighbours(int x, int y)
        {
            int result = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int col = (x + i + cols) % cols;
                    int row = (y + j + rows) % rows;
                    bool isSelfChecking = col == x && row == y;

                    if (!isSelfChecking && map[col, row])
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        private void startSimulation()
        {
            if (isRunning) return;

            resolution = 8;
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;
            map = new bool[cols, rows];

            Random random = new Random();

            if (isRandom)
            {
                for (int x = 0; x < cols; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        map[x, y] = random.Next(2) == 0;
                    }
                }
            }
            else
            {
                for (int x = 0; x < cols; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        map[x, y] = false;
                    }
                }
            }
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height); //init pictureBox
            graphic = Graphics.FromImage(pictureBox1.Image); // init graphic
            timer1.Start();
            isRunning = true;

        }

        private void stopSimulation()
        {
            if (!isRunning) return;
            timer1.Stop();
            isRunning = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            calNextGen();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            startSimulation();
            cbRandom.Enabled = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopSimulation();
            cbRandom.Enabled = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isRunning) return;
            
            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                
                if (validateMousePosition(x, y)) map[x, y] = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                
                if (validateMousePosition(x, y)) map[x, y] = false;
            }
        }

        private bool validateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }

        private void cbRandom_CheckedChanged(object sender, EventArgs e)
        {
            isRandom = !isRandom;
        }
    }
}