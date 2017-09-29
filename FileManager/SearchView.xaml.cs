using FileManager.Engine;
using FileManager.IO;
using FileManager.structs;
using FileManager.Utils;
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
    /// Logique d'interaction pour SearchView.xaml
    /// </summary>
    public class Pair
    {
        public string name { get; set; }
        public string type { get; set; }
        public int typeid { get; set; }
        public string id { get; set; }

        public string path { get; set; }
    }
    public partial class SearchView : Window
    {
        private FileManager.Graphics.Window window;
        public SearchView(List<Information> info , FileManager.Graphics.Window window)
        {
            InitializeComponent();
            this.window = window;

            foreach (var result in info.Where(x => x.type != -1).ToList())
            {
                var elem = new Pair();
                elem.name = result.name;
                elem.type = Configuration.types[result.type];
                elem.id = result.id;
                elem.typeid = result.type;
                List<Information> path = new List<Information>();
                GraphReader.getPtrParent(Kernel.getElement(elem.id),ref path);
                path.Reverse();
                string pathstr = "";
                int count = 0;
                foreach(var p in path)
                {
                    count++;
                    if (count == path.Count)
                    {
                        if (count == 1)
                        {
                            pathstr +=  $"{p.name.ToUpper()} :\\";
                        }
                        else
                        {
                            pathstr +=  $"{p.name}";
                        }

                    }
                    else
                    {
                        if (count == 1)
                        {
                            pathstr += $"{p.name.ToUpper()} :\\";
                        }
                        else
                        {
                            pathstr +=  $"{p.name} > ";
                        }
                    }
                }
                elem.path = pathstr;
                liste.Items.Add(elem);
            }

            liste.MouseDoubleClick += (e, v) => 
            {
                var element = (Pair)liste.SelectedItem;
                if(element.typeid != 4)
                    window.open_id(element.id);
            };

        }

        private void liste_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
