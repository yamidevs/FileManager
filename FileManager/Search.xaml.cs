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
using System.Windows.Shapes;

namespace FileManager
{
    /// <summary>
    /// Logique d'interaction pour Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        private FileManager.Graphics.Window window;
        public Search(FileManager.Graphics.Window window)
        {
            InitializeComponent();
            this.window = window;
        }
        
        private void search_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                window.enter_search(search.Text);
                this.Close();
            }
        }
    }
}
