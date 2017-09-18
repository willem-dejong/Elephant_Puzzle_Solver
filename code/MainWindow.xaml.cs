using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace elephant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //solutions will be printed under results tab the numbers represent which "elephant" to turn.
        private List<Polygon> turners = new List<Polygon>();//for turning arrows
        private List<bool> unturned = new List<bool>();//for turning arrows and loop solve
        private List<TextBox> label = new List<TextBox>();//for labeling arrows and turning arrows
        private Stack<int> stacky = new Stack<int>();//for loop solve keeps track of where the first instance of an unturned arrow is
        private int stacky2 =0;//represents number of turned "elephants", allows for quick check to see if all are turned
        private string s = "";//string that will contain the solution
        private int num = 0;//number of elephants
        private int last=-1;//keeps track of the last arrow to be turned this is for visual effect of high lighting which arrow has just been turned
        private long moves = 0;//number of moves left until solved
        private bool one = false;
        private bool manual = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        //***************important section for solving recursivly**********************
        static private string expand1(int n)//the "inorder" recusive string expansion that give the solution.
        {
            if (n == 1)//base case 1
            {
                return "1 ";
            }
            else if (n == 2) //base case 2
            {
                return "2 1 ";
            }
            else//other case
            {
                return expand1(n - 2)+Convert.ToString(n) + " "+expand2(n - 2)+expand1(n - 1);//inorder of the expansion of n-1 followed by n followed by reverse order of the expanion of n-2 followed by the inorder expanion of n-1 recursive call s
            }
        }
        static private string expand2(int n)//reverse order recursive call this is only should be called by expand1.
        {
            if (n == 1)//base case 1
            {
                return "1 ";
            }
            else if (n == 2) //base case 2
            {
                return "1 2 ";
            }
            else//other cases
            {
                return expand2(n - 1)+expand1(n - 2)+Convert.ToString(n) + " "+expand2(n - 2);
            }
        }
        public void recexpand(int n)//what other programs and methods should call to do a recursive solve
        {
            s =expand1(n);
            
        }
        //**********************************end of recursive solving section*******************************
        public void loopexpand(int n)//the lop solve
        {
            
            s="";
            if (one)//if an odd number of "elephants" the first move should be the first elephant other wise if even the first move shoud be the second Elephant;
            {
                stacky.Pop();
                stacky2++;
                s = s + Convert.ToString(1) + " ";
                unturned[0] = false;
            }
            while (stacky2< n)//loop that will follow the logic turn the elephant after the first unturned elephant then turn the first elephant then repeat until all are turned.
            {
                int m = stacky.Pop();//one after first unturned elephant
                s = s + Convert.ToString(m+1) + " ";
                if (unturned[m])
                {
                    unturned[m] = false;
                    stacky2++;
                    stacky.Pop();
                }
                else
                {
                    unturned[m] = true;
                    stacky2--;
                    stacky.Push(m + 1);
                }
                s = s + Convert.ToString(1) + " ";//first elephant
                if (m == 1)
                {
                    unturned[0] = false;
                    stacky2++;
                }
                else
                {
                    stacky.Push(m);
                    unturned[0] = true;
                    stacky2--;
                    stacky.Push(1);
                }

            }
            for (int z = 0; z < unturned.Count; z++)//lazy fix to issue of bools no longer at true at the end of method.
                unturned[z] = true;
        }
        void turn(int n)//turns the nth Elephant
        {
            if (unturned[n - 1])//turns
            {
                turners[n - 1].Points = new PointCollection();
                turners[n - 1].Points.Add(new Point(20, 8));
                turners[n - 1].Points.Add(new Point(6.67, 8));
                turners[n - 1].Points.Add(new Point(6.67, 3.33));
                turners[n - 1].Points.Add(new Point(0, 10));
                turners[n - 1].Points.Add(new Point(6.67, 16.67));
                turners[n - 1].Points.Add(new Point(6.67, 12));
                turners[n - 1].Points.Add(new Point(20, 12));
                unturned[n - 1] = false;
                stacky2++;
            }
            else//unturns
            {
                turners[n - 1].Points = new PointCollection();
                turners[n - 1].Points.Add(new Point(8, 0));
                turners[n - 1].Points.Add(new Point(8, 13.33));
                turners[n - 1].Points.Add(new Point(3.33, 13.33));
                turners[n - 1].Points.Add(new Point(10, 20));
                turners[n - 1].Points.Add(new Point(16.67, 13.33));
                turners[n - 1].Points.Add(new Point(12, 13.33));
                turners[n - 1].Points.Add(new Point(12, 0));
                unturned[n - 1] = true;
                stacky2--;
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)//makes a single move on puzzle when "enter" is pressed
        {
            if (e.Key == Key.Enter)
            {
                if (s.Length > 1 && !manual)//reads solution and makes move
                {
                    string temp = s.Substring(0, s.IndexOf(" "));
                    try
                    {
                        s = s.Substring(s.IndexOf(" ") + 1);
                    }
                    catch
                    {
                        s = "";
                    }
                    int n = Convert.ToInt32(temp);
                    change(n);
                    moves--;//updates the number of moves left until puzzle is solved.
                    info.Text = info.Text.Substring(0, info.Text.IndexOf("#")) + "# of moves to solved:" + Convert.ToString(moves);
                }
                else if(stacky2<num)//solves as you go
                {
                    if (!manual)
                    {
                        manual = true;
                        loop.IsEnabled = false;
                    }
                    int m = stacky.Pop();//one after first unturned elephant
                    if (one)
                    {
                        if (!rec.IsEnabled)
                        {
                            s = s + Convert.ToString(1) + " ";//first elephant
                            results.Text = s;
                        }
                        change(1);
                        if (m != 1)
                        {
                            stacky.Push(m);
                            stacky.Push(1);
                        }
                        one = false;
                    }
                    else
                    {
                        if (!rec.IsEnabled)
                        {
                            s = s + Convert.ToString(m + 1) + " ";
                            results.Text = s;
                        }
                        if (unturned[m])
                        {
                            stacky.Pop();
                        }
                        else
                        {
                            stacky.Push(m + 1);
                        }
                        change(m + 1);
                        stacky.Push(m);
                        one = true;
                    }
                    moves--;
                    if (rec.IsEnabled)
                        info.Text = "You are solving manually. Press enter to make the next move." + "# of moves to solved:" + Convert.ToString(moves);
                    else
                        info.Text = info.Text.Substring(0, info.Text.IndexOf("#")) + "# of moves to solved:" + Convert.ToString(moves);
                }
            }
        }
        int calcnumofmoves(int n)//calculate total number of turns it takes to solve a puzzle of n elephants 
        {   //f(n) = 2^(n-1)+2^(n-3)+2^(n-5)+...+(2^1 or 2^0)=> f(1)=2^0=1, f(2)=2^(2-1)=2, f(n)=2^(n-1)+f(n-2)
            if (n == 0)
                return 0;
            if (n == 1)
                return 1;
            return (int)Math.Pow(2.0, (double)(n - 1)) + calcnumofmoves(n - 2);
        }
        private void set_Click(object sender, RoutedEventArgs e)//sets up the GUI with arrows that represent the Elephants
        {
            turners = new List<Polygon>();//resets lists and stacks for new puzzle
            unturned=new List<bool>();
            label = new List<TextBox>();
            stacky = new Stack<int>();
            stacky2=0;
            manual = false;
            results.Text = "";//clears previous solution
            s = "";//clears previous solution
            last = -1;//resets last arrow turned
            veiw.Children.Clear();//removes the previous puzzle's arrows
            try
            {
                num = Convert.ToInt32(numlab.Text);//reads the user inputed number
            }
            catch//incase user enters a non integer
            {
                num = 0;
                numlab.Text = "ERR";
                return;
            }
            if (num % 2 == 1)
                one = true;
            else
                one = false;
            int z = -75;//for wraping arrows down a level when current line is full
            Stack<int> tempst = new Stack<int>();//temp stack inorder to put into stacky inorder
            for (int x = 0; x < num; x++)//loop that creats all Elephants
            {
                if (x%32 == 0)//checks to see if the arrows need to go into a new row
                    z = z+75;//cals y coord location
                TextBox t = new TextBox();//numarical label for arrow
                t.Text = Convert.ToString(x + 1);
                t.Width = 30;
                t.Height = 25;
                t.Focusable = false;
                t.Background = new SolidColorBrush(Colors.Transparent);
                t.HorizontalAlignment = HorizontalAlignment.Left;
                t.VerticalAlignment = VerticalAlignment.Top;
                t.IsReadOnly = true;
                t.BorderBrush = new SolidColorBrush(Colors.Transparent);
                Polygon p = new Polygon();//arrow
                p.Points.Add(new Point(8, 0));
                p.Points.Add(new Point(8, 13.33));
                p.Points.Add(new Point(3.33, 13.33));
                p.Points.Add(new Point(10, 20));
                p.Points.Add(new Point(16.67, 13.33));
                p.Points.Add(new Point(12, 13.33));
                p.Points.Add(new Point(12, 0));
                p.Fill = new SolidColorBrush(Colors.Black);
                p.VerticalAlignment = VerticalAlignment.Top;
                p.HorizontalAlignment = HorizontalAlignment.Left;
                double y = 20.0 + 30 *(x%32);//calculates its x coord location
                t.Margin = new Thickness(y, 29+z, 0, 0);
                p.Margin = new Thickness(y, 55+z, 0, 0);
                turners.Add(p);
                tempst.Push(x + 1);
                unturned.Add(true);
                label.Add(t);
                veiw.Children.Add(t);
                veiw.Children.Add(p);
            }
            for (int v = 0; v < num; v++)//populates stacky in numarical order
                stacky.Push(tempst.Pop());
            //setup = true;
            maintb.Refresh();
            //expand1(num);
            //loopexpand(num);
            //s = results.Text;
            //results.Text = s;
            if (num > 25)
                info.Text = "Solution not given due to its size. manually solve by holding enter.";
            else
            {
                info.Text = "Select recursive solve.";
                if (num > 16)
                {
                    info.Text = info.Text + "loop solve will not be given do to solution size.";
                }
                else
                    loop.IsEnabled = true;//enables buttons
                rec.IsEnabled = true;
            }
            moves=calcnumofmoves(num);//calculates number of moves to solve
            info.Text=info.Text+" # of moves to solve:"+Convert.ToString(moves);
            maintb.Focus();
        }

        void change(int n)//changes to GUI that represents move.
        {

            turn(n);
            if(last>-1)//if there was a previously turned arrow it removes the high light
                label[last - 1].Background = new SolidColorBrush(Colors.Transparent);
            label[n - 1].Background = new SolidColorBrush(Colors.Red);
            label[n - 1].Refresh();
            last = n;
        }

        private void loop_Click(object sender, RoutedEventArgs e)//button to solve using a loop
        {
            loopexpand(num);//constructs solution
            results.Text = s;//updates result
            info.Text = "Solution is under \"Results\" tab. Press enter key to make a move. # of moves to solved:" + Convert.ToString(moves);
            rec.IsEnabled = false;//disable buttons
            loop.IsEnabled = false;
            //update information
        }

        private void rec_Click(object sender, RoutedEventArgs e)
        {
            recexpand(num);//constructs solution using recursion
            if (num < 17)
            {
                results.Text = s;//updates result
                info.Text = "Solution is under \"Results\" tab. Press enter key to make a move. # of moves to solved:" + Convert.ToString(moves);
            }
            else
                info.Text = "Solution not shown due to size it will crash UI.Press enter key to make a move. # of moves to solved:" + Convert.ToString(moves);
            rec.IsEnabled = false;//disables buttons
            loop.IsEnabled = false;
            
        }
    }
    public static class ExtensionMethods//class to force GUI refresh. This was found online.
    {

        private static Action EmptyDelegate = delegate() { };



        public static void Refresh(this UIElement uiElement)
        {

            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);

        }

    }
}
