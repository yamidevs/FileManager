using FileManager.Engine;
using FileManager.Graphics.UI;
using FileManager.Graphics.UI.Bar;
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
using System.Windows.Media;
using System.Windows.Shapes;

namespace FileManager.Graphics.Engine
{
    public class Tuple<T1,T2,T3>
    {
        public T1 x { get; set; }
        public T2 y { get; set; }
        public T3 elements { get; set; }
    }
    public class MiddleEngine
    {
        private Canvas canvas;
        private double xButton = FileManager.Graphics.Window.Width - 10, heightBar = 0, taskHeight = 0;
        private Rectangle Bar;
        public ListeEngine liste { get; set; }
        private AbsorbenceEngine absorbance;
        private Button prev = null;
        private CloseElement close;
        private ReduceElement reduce;
        private Label title = null;
        public Dictionary<string, Tuple<int, int, List<MiddleElement>>> FileElements = new Dictionary<string, Tuple<int, int, List<MiddleElement>>>();
        private string currentElement = string.Empty;
        public Element current { get; set; }
        public event Action<int> click_prev;

       
        public MiddleEngine(Canvas canvas)
        {
            this.canvas = canvas;
            close = new CloseElement(2);
            reduce = new ReduceElement(3);
            liste = new ListeEngine(this,canvas);
            absorbance = new AbsorbenceEngine(this, canvas);

        }

        public void Initilization(double barHeight, double taskHeight)
        {
            xButton -= close.width;
            close.setPosition((int)xButton, 5);
            xButton -= (reduce.width + 10);
            reduce.setPosition((int)xButton, 5);
            this.taskHeight = taskHeight;
            liste.Initialization((FileManager.Graphics.Window.Height * barHeight) / 100, taskHeight);
            absorbance.Initialization((FileManager.Graphics.Window.Height * barHeight) / 100, taskHeight);

        }
        public void GenerateGraph(string id, IntPtr son)
        {
            if (!FileElements.ContainsKey(id))
            {
                FileElements.Add(id, null);
            }
            FileElements[id] = new Tuple<int, int, List<MiddleElement>>();
            FileElements[id].x = Configuration.startXMiddle;
            FileElements[id].y = Configuration.startYMiddle;
            FileElements[id].elements = new List<MiddleElement>();

            foreach (var result in GraphReader.getElements(son))
            {
                
                this.Add(result.id, result.type, result.name,result.shortcut);
            }
            liste.refreshListe();
            absorbance.refresh();
        }
        public string nameValid(string name)
        {
            int count = 0;
            foreach(var result in FileElements[currentElement].elements)
            {
                if (result.nameElement.Name == name || result.nameElement.Name.Contains('('))
                {
                    
                    if(result.nameElement.Name.Contains('('))
                    {
                        var r = result.nameElement.Name.Split('(')[0];
                        if(r == name)
                        {
                            count++;
                        }
                    }else
                    {
                        count++;
                    }
                }
            }
            if (count > 0)
                return ($"{name}({count})");
            else
                return name;
        }
        public void refresh()
        {
            liste.refreshListe();
            this.absorbance.refresh();
        }
        public MiddleElement FindName(string name)
        {
            var element = FileElements[getCurrent()].elements.Find(x => x.nameElement.Name == name);
            return element;
        }
        public MiddleElement First(string id)
        {
            var element = FileElements[getCurrent()].elements.Find(x => x.Id == id);
            return element;
        }
        public void Add(string id, int type, string name, string id_shortcut = "")
        {
            if (FileElements.ContainsKey(currentElement))
            {
                if (type != -1)
                {
                    var element = new MiddleElement(type, name, id);
                    if (id_shortcut != String.Empty)
                    {
                        element.setShortCut(id_shortcut);
                    }
                    if (FileElements[currentElement].x + (int)element.width + 15 > FileManager.Graphics.Window.Width)
                    {
                        FileElements[currentElement].x = Configuration.startXMiddle;
                        FileElements[currentElement].y += (int)(element.height + (element.height / 2));
                    }
                    element.nameElement.setPosition((int)(FileElements[currentElement].x), (int)(FileElements[currentElement].y + (element.height - 25)));
                    element.nameElement.setSize(9);

                    element.setPosition(FileElements[currentElement].x, FileElements[currentElement].y);
                    FileElements[currentElement].elements.Add(element);
                    FileElements[currentElement].x += (int)element.width + 15;
                    element.Generate();
                    element.Display(canvas);
                    liste.refreshListe();
                    absorbance.refresh();
                }
            }
        }

        public void Generate()
        {

            foreach (var req in FileElements[currentElement].elements)
            {
                req.Generate();
                req.Display(canvas);
            }
        }
        public void setCurrent(string id)
        {
            this.currentElement = id;

        }

        public void setTitle(string name)
        {
            if (title != null)
            {
                canvas.Children.Remove(title);
            }
            title = new Label();
            title.FontSize = 15;
            title.FontWeight = FontWeights.Bold;
            var elem = GraphReader.Element(Kernel.getElement(this.currentElement));
            title.Content = $"{Configuration.types[elem.info.type]} : {elem.info.name}";
            Canvas.SetLeft(title, FileManager.Graphics.Window.Width / 2);
            Canvas.SetTop(title, 0);
            canvas.Children.Add(title);          
            liste.refreshListe();
            absorbance.refresh();
        }

        public string getCurrent()
        {
            return this.currentElement;
        }
        public bool Contains(Element element)
        {
            if (FileElements.ContainsKey(currentElement) && element != null)
            {
                if (FileElements[currentElement].elements.Any(x => x.Id == element.Id))
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public void delete(string id)
        {
            this.FileElements.Remove(id);
        }
        public Element Contains(int xmouse, int ymouse, ref int type)
        {
            if ((xmouse >= close.position.x && (xmouse <= close.position.x + close.width)) && (ymouse >= close.position.y && (ymouse <= close.position.y + close.height)))
            {
                type = 0;
                return close;
            } else if ((xmouse >= reduce.position.x && (xmouse <= reduce.position.x + reduce.width)) && (ymouse >= reduce.position.y && (ymouse <= reduce.position.y + reduce.height)))
            {
                type = 1;
                return reduce;
            }
            else
            {
                if (FileElements.ContainsKey(currentElement))
                {
                    var element = this.FileElements[currentElement].elements.Find(x => (xmouse >= x.position.x && (xmouse <= x.position.x + x.width)) && (ymouse >= x.position.y && (ymouse <= x.position.y + x.height)));
                    type = 2;
                    return element;
                }
                else
                {
                    return null;
                }

            }
        }
        public void OnMiddle(double height)
        {
            this.drawLine(height);
            close.Generate();
            close.Display(canvas);
            reduce.Generate();
            reduce.Display(canvas);
            prev = new Button();
            prev.Content = "Précédent";
            prev.Click += (s, e) =>
            {
                click_prev?.Invoke(0);
            };
            Canvas.SetTop(prev,7);
            Canvas.SetLeft(prev, 10);
            canvas.Children.Add(prev);
        }


        public void hideCurrent()
        {
            foreach (var result in FileElements[currentElement].elements)
            {
                result.Hide(canvas);
            }
        }

        public void hideTitle()
        {
            if (title != null)
            {
                canvas.Children.Remove(title);
            }
        }

        public void hideMiddle()
        {
            if (currentElement != "")
            {
                this.hideCurrent();
                this.hideDraw();
                this.hideTitle();
                close.Hide(canvas);
                reduce.Hide(canvas);

            }
        }
        public int countAbs()
        {
            return this.absorbance.count;
        }
        public void clearCurrent()
        {
            this.FileElements[currentElement].elements.Clear();
            this.FileElements[currentElement].x = Configuration.startXMiddle;
            this.FileElements[currentElement].y = Configuration.startYMiddle;

        }

        public void drawLine(double height)
        {
            if (Bar == null)
            {
                Bar = new Rectangle()
                {
                    Width = FileManager.Graphics.Window.Width,
                    Height = (FileManager.Graphics.Window.Height * height) / 100
                };
                heightBar = Bar.Height;
                Bar.Fill = new SolidColorBrush() { Color = Colors.LightGray };
                Bar.StrokeThickness = 1;
                Bar.Stroke = new SolidColorBrush() { Color = Colors.Gray };
                this.canvas.Children.Add(Bar);
                liste.drawList();
                absorbance.draw();
            }
        }

        public void hideDraw()
        {
            if (Bar != null)
            {
                this.canvas.Children.Remove(Bar);
                Bar = null;          
            }
            liste.hideList();
            absorbance.hide();
            canvas.Children.Remove(prev);
            prev = null;
        }
    }
}
