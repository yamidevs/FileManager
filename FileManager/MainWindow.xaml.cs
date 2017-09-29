using FileManager.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileManager
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Kernel kernel;
        public MainWindow()
        {
            InitializeComponent();
            this.resizeGraphic();
            kernel = new Kernel(graphic,(FileManager.MainWindow)content);
            kernel.Initialization();
            graphic.Background = new SolidColorBrush() { Color = Colors.WhiteSmoke };
   

        }

        public void resizeGraphic()
        {
            graphic.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            graphic.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
        }
        private void App_MouseMove(object sender, MouseEventArgs e)
        {

            int xmouse = (int)Mouse.GetPosition(this.graphic).X;
            int ymouse = (int)Mouse.GetPosition(this.graphic).Y;
            this.kernel.MouseMove(xmouse, ymouse);

        }
        private void App_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int xmouse = (int)Mouse.GetPosition(this.graphic).X;
            int ymouse = (int)Mouse.GetPosition(this.graphic).Y;
            if (e.ClickCount == 2)
            {
                this.kernel.Click(xmouse, ymouse);
            }else if(e.ClickCount == 1)
            {
                this.kernel.ClickOne(xmouse, ymouse);
            }
        }
        private void App_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                int xmouse = (int)Mouse.GetPosition(this.graphic).X;
                int ymouse = (int)Mouse.GetPosition(this.graphic).Y;                
                this.kernel.Menu(xmouse, ymouse);

            }
        }
        
    }
}
