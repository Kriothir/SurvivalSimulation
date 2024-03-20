using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace KPO_Vaja2
{
    public partial class Form1 : Form
    {

        public List<Food> foodCoordinates = new List<Food>();
        public int generationCount = 0;
        public bool start = true;
        public int passiveCount = 50;
        public int agressiveCount = 50;
        public int foodCount = 30;
        Point startPassive;
        Point startAgressive;
        public List<Point> passiveCoordinatesList = new List<Point>();
        public List<Point> agressiveCoordinatesList = new List<Point>();
        Random random = new Random();
        public int reset = 0;
        public Form1()
        {
            InitializeComponent();

            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
           .SetValue(panel2, true, null);

        }
        private void InitializeCells(int pc, int ac)
        {
            for (int i = 0; i < pc; i++)
            {

                int edge = random.Next(0,4); 

                int x = 0, y = 0;
                Console.WriteLine(edge);
                switch (edge)
                {
                    case 0: // Top 
                        x = random.Next(20, panel1.Width - 20);
                        y = 5;
                        break;
                    case 1: // Right 
                        x = panel1.Width - 20;
                        y = random.Next(20, panel1.Height - 20);
                        break;
                    case 2: // Bottom 
                        x = random.Next(20, panel1.Width);
                        y = panel1.Height - 20;
                        break;
                    case 3: // Left 
                        x = 5;
                        y = random.Next(20, panel1.Height - 20);
                        break;
                }
                Cell cell = new Cell(panel1.Size, x, y, CellType.Passive);
                panel1.Controls.Add(cell);
            }
            for (int i = 0; i < ac; i++)
            {

                int edge = random.Next(0, 4); 

                int x = 0, y = 0;
                Console.WriteLine(edge);
                switch (edge)
                {
                    case 0: // Top 
                        x = random.Next(20,panel1.Width - 20);
                        y = 5;
                        break;
                    case 1: // Right 
                        x = panel1.Width - 20;
                        y = random.Next(20, panel1.Height - 20);
                        break;
                    case 2: // Bottom 
                        x = random.Next(20, panel1.Width);
                        y = panel1.Height - 20;
                        break;
                    case 3: // Left 
                        x = 5;
                        y = random.Next(20, panel1.Height - 20);
                        break;
                }
                Cell cell = new Cell(panel1.Size, x, y, CellType.Agressive);
                panel1.Controls.Add(cell);
            }
        }
        private void InitializeFood()
        {
            for(int i = 0; i < foodCount; i ++)
            {
                Food food = new Food(panel1.Size);
                foodCoordinates.Add(food);
                panel1.Controls.Add(food);
            }
        }
        private void RepositionCells()
        {
            foodCoordinates.Clear();
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Cell c)
                {
                    c.SeekFood(true);
                    int edge = random.Next(0, 4); 

                   
                    int x = 0, y = 0;
                    Console.WriteLine(edge);
                    switch (edge)
                    {
                        case 0: // Top 
                            x = random.Next(20, panel1.Width - 20);
                            y = 5;
                            break;
                        case 1: // Right 
                            x = panel1.Width - 20;
                            y = random.Next(20, panel1.Height - 20);
                            break;
                        case 2: // Bottom 
                            x = random.Next(20, panel1.Width);
                            y = panel1.Height - 20;
                            break;
                        case 3: // Left 
                            x = 5;
                            y = random.Next(20, panel1.Height - 20);
                            break;
                    }
                    c.SetPosition(x,y);
                }
                else if(ctrl is Food f)
                {
                    panel1.Controls.Remove(f);
                }

            }
            for (int i = 0; i < foodCount; i++)
            {
                Food food = new Food(panel1.Size);
                foodCoordinates.Add(food);
                panel1.Controls.Add(food);
            }
        }
        private double getDistance(Point cell, Point food)
        {

            int deltaX = food.X - cell.X;
            int deltaY = food.Y - cell.Y;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = passiveCount.ToString();
            label3.Text = agressiveCount.ToString();
            //Cells seek food
            if (generationCount <= 30000)
            {
                start = true;
                foreach (Control ctrl in panel1.Controls)
                {
                    if (ctrl is Cell c && c.IsSeeking())
                    {
                        for (int i = 0; i < foodCoordinates.Count(); i++)
                        {
                            if ((getDistance(c.GetPosition(), foodCoordinates[i].GetPosition()) < 7) && foodCoordinates[i].GetFreeSlots())
                            {
                                c.SeekFood(false);
                                foodCoordinates[i].AddSlot(c.GetType());
                            }
                        }
                        c.Move(random);
                    }
                    generationCount++;

                }
                label1.Text = generationCount.ToString();   
            }
            else if(start == true && generationCount >= 30000) //End of generation
            {
                //Set status of cells that found food
                foreach (Control ctrl in panel1.Controls)
                {
                    if (ctrl is Cell c && !c.IsSeeking())
                    {
                        for (int i = 0; i < foodCoordinates.Count(); i++)
                        {
                            if ((getDistance(c.GetPosition(), foodCoordinates[i].GetPosition()) < 7))
                            {
                                c.SetSurvivalStatus(foodCoordinates[i].GetResult());
                            }
                        }
                    }
                }
                //Remove, survive and multiply cells
                foreach (Control ctrl in panel1.Controls)
                {
                    if (ctrl is Cell c)
                    {
                        if(c.GetStatus() == 0) // Certain death
                        {
                            if(c.GetType() == CellType.Passive)
                            {
                                passiveCount--;
                            }
                            if(c.GetType() == CellType.Agressive)
                            {
                                agressiveCount--;
                            }
                            panel1.Controls.Remove(c);
                        }
                        else if(c.GetStatus() == 1) // One cell for one pair, cell survives and multiplies
                        {
                            panel1.Controls.Add(new Cell(panel1.Size, c.GetPosition().X + 20, c.GetPosition().Y + 20, c.GetType()));
                            if (c.GetType() == CellType.Passive)
                            {
                                passiveCount++;
                            }
                            if (c.GetType() == CellType.Agressive)
                            {
                                agressiveCount++;
                            }
                        }
                        else if(c.GetStatus() == 2) // Agressive and passive, agressive survives with 50% of multiplying, passive has 50% chance to survive
                        {
                            if(c.GetType() == CellType.Passive)
                            {
                                double rndNum = random.NextDouble();
                                if (rndNum < 0.5)
                                {
                                    panel1.Controls.Remove(c);
                                    passiveCount--;
                                }
                            }
                            if(c.GetType() == CellType.Agressive)
                            {
                                double rndNum = random.NextDouble();
                                if (rndNum < 0.5)
                                {
                                    panel1.Controls.Add(new Cell(panel1.Size, c.GetPosition().X + 20, c.GetPosition().Y + 20, c.GetType()));
                                    agressiveCount++;
                                }
                            }
                        }
                        else if(c.GetStatus() == 3)//Both passive, both survive
                        {
                            continue;
                        }
                        else if(c.GetStatus() == 4)
                        {
                            panel1.Controls.Remove(c);
                            agressiveCount--;
                        } // Both agressive, both die
                    }


                }
                label2.Text = passiveCount.ToString();
                label3.Text = agressiveCount.ToString();

                passiveCoordinatesList.Add(new Point(passiveCoordinatesList.Last().X + 50, startPassive.Y  + passiveCount));
                agressiveCoordinatesList.Add(new Point(agressiveCoordinatesList.Last().X + 50, startAgressive.Y + agressiveCount));
                panel2.Invalidate();

                RepositionCells();  
                Invalidate();
                start = false;
                generationCount = 0;
            }


        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black, 3);
            Pen bluePen = new Pen(Color.Blue, 3);
            Pen redPen = new Pen(Color.Red, 3);

            e.Graphics.DrawLine(blackPen, 20, panel2.Height - 20, panel2.Width - 20, panel2.Height - 20);
            e.Graphics.DrawLine(blackPen, 20, panel2.Height - 20, 20, 10);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            for (int i = 0; i < passiveCoordinatesList.Count - 1; i++)
            {

                e.Graphics.DrawLine(bluePen, passiveCoordinatesList[i], passiveCoordinatesList[i + 1]);
            }
            for (int i = 0; i < agressiveCoordinatesList.Count - 1; i++)
            {

                e.Graphics.DrawLine(redPen, agressiveCoordinatesList[i], agressiveCoordinatesList[i + 1]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (reset == 0)
            {

                passiveCount = Int32.Parse(textBox1.Text);
                agressiveCount = Int32.Parse(textBox2.Text);
                foodCount = Int32.Parse(textBox3.Text);

                InitializeCells(passiveCount, agressiveCount);
                InitializeFood();
                startPassive = new Point(20, panel2.Height - 10 - passiveCount);
                startAgressive = new Point(20, panel2.Height - 10 - agressiveCount);
                passiveCoordinatesList.Add(startPassive);
                agressiveCoordinatesList.Add(startAgressive);
                reset++;

            }            
            else if(reset % 2 != 0)
            {
                passiveCoordinatesList.Clear();
                agressiveCoordinatesList.Clear();
                foodCoordinates.Clear();
                panel1.Controls.Clear();
                panel2.Controls.Clear();
                panel2.Invalidate();
                passiveCount = 0;
                agressiveCount = 0;

                foodCount= 0;
                generationCount = 0;

                reset++;
            }
            else if(reset % 2 == 0)
            {

                passiveCount = Int32.Parse(textBox1.Text);
                agressiveCount = Int32.Parse(textBox2.Text);
                foodCount = Int32.Parse(textBox3.Text);

                InitializeCells(passiveCount, agressiveCount);
                InitializeFood();
                startPassive = new Point(20, panel2.Height - 10 - passiveCount);
                startAgressive = new Point(20, panel2.Height - 10 - agressiveCount);
                passiveCoordinatesList.Add(startPassive);
                agressiveCoordinatesList.Add(startAgressive);
                reset++;
            }
        }
    }

    public enum CellType { Agressive, Passive};
    public class Cell : Control
    {
        private const int size = 8;
        private CellType type;
        private Point position;
        private Size panelSize;
        private int survivalStatus = 0;
        private bool seekFood = true;
        Random random = new Random();

        public Cell(Size panelSize, int x, int y, CellType type)
        {
            this.panelSize = panelSize;
            SetPosition(x, y);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            Size = new Size(size, size);
            this.type = type;
        }

        public Point GetPosition()
        {
            return position;
        }
        public void SetSurvivalStatus(string status)
        {
            if(status == "M")
            {
                survivalStatus = 1;
            }
            else if(status == "AP")
            {
                survivalStatus = 2;
            }
            else if(status == "PS")
            {
                survivalStatus = 3;
            }
            else if(status == "AD")
            {
                survivalStatus = 4;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (type == CellType.Agressive)
            {
                e.Graphics.FillEllipse(Brushes.Red, new Rectangle(Point.Empty, Size));

            }
            else
            {
                e.Graphics.FillEllipse(Brushes.Blue, new Rectangle(Point.Empty, Size));

            }
        }
        public CellType GetType()
        {
            return type;
        }
        public void SetPosition(int x, int y)
        {
            position = new Point(x, y);
            Location = position;
        }

        public bool CellMultiply()
        {
            double rndNum = random.NextDouble();
            return rndNum < 0.5;
        }
        public int GetStatus()
        {
            return survivalStatus;
        }
        public void SeekFood(bool seek)
        {
            seekFood = seek;
        }
        public bool IsSeeking() 
        {
            return seekFood;
        }
        public void Move(Random random)
        {
            if(seekFood == true)
            {
                int newX = Math.Max(Math.Min(position.X + random.Next(-10, 12), panelSize.Width - size), 0);
                int newY = Math.Max(Math.Min(position.Y + random.Next(-10, 12), panelSize.Height - size), 0);
                position = new Point(newX, newY);
                Location = position;
                Invalidate();
            }
 
        }
    }
    public class Food : Control
    {
        private const int size = 6;
        private Point position;
        private Size panelSize;
        private List<int> slot = new List<int>() { 0, 0};
        private int slotIndex = 0;
        private int slotCount = 0;
        Random random = new Random();

        // 1= aggressive, 2=passive
        public bool GetFreeSlots()
        {
            if(slotCount == 2)
            {
                return false;
            }
            else
            {
                slotCount++;
                return true;
            }
        }
        public void ResetSlots()
        {
            slot = new List<int> { 0, 0 };
        }
        public void AddSlot(CellType cellType)
        {
            int cType = 0;

            if (cellType == CellType.Agressive)
            {
                cType = 1;
            }
            else if(cellType == CellType.Passive)
            {
                cType = 2;

            }
            slot[slotIndex] = cType;
            slotIndex++;
        }
        // 1 passive, 0 aggressive = 100% survive and multiply -> return "M"
        // 0 passive, 1 agressive = 100% survive and multiplay ->"AM"
        // 2 passive = 100% survive, no multiply ->"PS"
        // 2 aggressive = 100% death -> "AD"
        // 1 passive, 1 agressive = Agressive survives (50% chance multiply), passive survives(no multiply) - > "AP"
        public string GetResult()
        {
            if ((slot[0] == 1 && slot[1] == 0) || (slot[0] == 2 && slot[1] == 0)){
                return "M";
            }
            else if (slot[0] == 2 && slot[1] == 2)
            {
                return "PS";
            }
            else if(slot[0] == 1 && slot[1] == 1)
            {
                return "AD";
            }
            else if ((slot[0] == 1 && slot[1] == 2) || (slot[0] == 2 && slot[1] == 1)){
                return "AP";
            }
            else
            {
                return "Null";
            }
        }
        public Point GetPosition()
        {

            return position;
        }
        public Food(Size panelSize)
        {
            this.panelSize = panelSize;
            SetPosition();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            Size = new Size(size, size);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillEllipse(Brushes.Green, new Rectangle(Point.Empty, Size));
        }

        private void SetPosition()
        {
            position = new Point(RandomPoint(30, panelSize.Width - 30), RandomPoint(30, panelSize.Height - 30));
            Location = position;
        }

        private int RandomPoint(int max,int min)
        {
            return random.Next(max, min);
        }

    }
}
