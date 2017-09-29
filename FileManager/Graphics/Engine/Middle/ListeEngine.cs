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
using System.Windows.Media;
using System.Windows.Shapes;

namespace FileManager.Graphics.Engine
{
    public class Liste
    {
        public Label label;
        public int x, y;
        public string id;
        public double width, height;
        public Liste(Label label , double width, double height)
        {
            this.label = label;
            this.width = width;
            this.height = height;
        }
        public void setPosition(int x , int y)
        {
            this.x = x;
            this.y = y;
        }

        public void defaultLabel()
        {
            label.FontSize = 14;
            Canvas.SetLeft(label, x);
            label.FontWeight = FontWeights.Normal;
        }

        public void hoverLabel()
        {
            label.FontSize = 15;
            Canvas.SetLeft(label, x + 3);
            label.FontWeight = FontWeights.Medium;
        }
    }
    public class ListeEngine
    {
        private MiddleEngine middle;
        private Rectangle Liste;
        private Canvas canvas;
        private double heightBar, taskHeight;
        private List<Liste> labels = new List<Liste>();
        public Liste current { get; set; } = null;
        private double x = 10, y = 50;
        public ListeEngine(MiddleEngine middle, Canvas canvas)
        {
            this.middle = middle;
            this.canvas = canvas;
        }
        public void Initialization(double heightBar , double taskHeight)
        {
            this.heightBar = heightBar;
            this.taskHeight = taskHeight;
        }

        public void drawList()
        {
            if (Liste == null)
            {
                Liste = new Rectangle()
                {
                    Width = 200,
                    Height = FileManager.Graphics.Window.Height - (heightBar + taskHeight)
                };
                Liste.Fill = new SolidColorBrush() { Color = Colors.White };
                Liste.StrokeThickness = 1;
                Liste.Stroke = new SolidColorBrush() { Color = Colors.Gray };
                Canvas.SetTop(Liste, heightBar);
                this.canvas.Children.Add(Liste);
            }
        }

        public string Contains(int xmouse , int ymouse)
        {
            var element = this.labels.Find(x => (xmouse >= x.x && (xmouse <= x.x + x.width)) && (ymouse >= x.y && (ymouse <= x.y + x.height)));
            if(element != null)
            {
                if (element.id != "")
                    return element.id;
                else
                    return "";
            }else
            {
                return "";
            }
        }

        public void hover(int xmouse , int ymouse)
        {
            if (current != null)
                current.defaultLabel();
            var lab = this.labels.First(x => (xmouse >= x.x && (xmouse <= x.x + x.width)) && (ymouse >= x.y && (ymouse <= x.y + x.height)));
            if(lab != null)
            {
                lab.hoverLabel();
                current = lab;
            }

        }
        public void refreshListe()
        {
            this.Clear();
            var allelems = new Label();
            allelems.Content = $"Nombres éléments(R)({GraphReader.Count(GraphReader.Element(Kernel.getElement(middle.getCurrent())).son)}) : ";
            allelems.FontSize = 14;
            allelems.FontWeight = FontWeights.Bold;
            labels.Add(new Engine.Liste(allelems,0,0));
            Canvas.SetTop(allelems, y);
            Canvas.SetLeft(allelems, x);
            canvas.Children.Add(allelems);
            
            var all = new List<Information>();
            GraphReader.getPtr(GraphReader.Element(Kernel.getElement(middle.getCurrent())).son, ref all);
            y += 5;
            foreach (var result in all.Where(x => x.type != -1).ToList())
            {
                y += 20;
                var lab = new Label();
                lab.Content = $"{ Configuration.types[result.type]} : { result.name}";
                lab.FontSize = 14;
                lab.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                Rect measureRect = new Rect(lab.DesiredSize);
                lab.Arrange(measureRect);
                var liste = new Liste(lab , measureRect.Width , measureRect.Height);
                liste.setPosition((int)x, (int)y);
                liste.id = result.id;

                labels.Add(liste);
                Canvas.SetTop(liste.label, y);
                Canvas.SetLeft(liste.label, x);
                canvas.Children.Add(liste.label);
            }
            y += 20;

            var nelems = new Label();
            nelems.Content = $"Nombres éléments({GraphReader.CountNext(GraphReader.Element(Kernel.getElement(middle.getCurrent())).son)}) : ";
            nelems.FontSize = 14;
            nelems.FontWeight = FontWeights.Bold;
            labels.Add(new Engine.Liste(nelems,0,0));
            Canvas.SetTop(nelems, y);
            Canvas.SetLeft(nelems, x);
            canvas.Children.Add(nelems);
            all = GraphReader.getElementList(GraphReader.Element(Kernel.getElement(middle.getCurrent())).son);
            foreach (var result in all.Where(x => x.type != -1).ToList())
            {
                y += 20;

                var lab = new Label();
                lab.Content = $"{ Configuration.types[result.type]} : { result.name}";
                lab.FontSize = 14;
                lab.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                Rect measureRect = new Rect(lab.DesiredSize);
                lab.Arrange(measureRect);
                var liste = new Liste(lab, measureRect.Width, measureRect.Height);
                liste.setPosition((int)x, (int)y);
                liste.id = result.id;

                labels.Add(liste);
                Canvas.SetTop(liste.label, y);
                Canvas.SetLeft(liste.label, x);
                canvas.Children.Add(liste.label);
            }
        }

        public void Clear()
        {
            x = 10;
            y = 50;
            foreach (var result in labels)
            {
                canvas.Children.Remove(result.label);
            }
            labels.Clear();
        }
        public void hideList()
        {
            if(Liste != null)
            {
                canvas.Children.Remove(Liste);
                Liste = null;
                
            }
            this.Clear();
        }
    }
}
