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
    public class AbsorbenceEngine
    {
        private MiddleEngine middle;
        private Rectangle Bar;
        private Canvas canvas;
        private double heightBar, widthListe;
        private List<Label> labels = new List<Label>();
        private double x = 10, y = 50;
        public int count { get; set; } = 0;
        public AbsorbenceEngine(MiddleEngine middle, Canvas canvas)
        {
            this.middle = middle;
            this.canvas = canvas;
        }
        public void Initialization(double heightBar, double widthListe)
        {
            this.heightBar = heightBar;
            this.widthListe = widthListe;
        }

        public void draw()
        {
            if (Bar == null)
            {

                Bar = new Rectangle()
                {
                    Width = FileManager.Graphics.Window.Width - 200,
                    Height = 25
                };
                Bar.Fill = new SolidColorBrush() { Color = Colors.Beige };
                Bar.StrokeThickness = 1;
                Bar.Stroke = new SolidColorBrush() { Color = Colors.Gray };
                Canvas.SetTop(Bar, heightBar);
                Canvas.SetLeft(Bar, 200);
                x = 210;
                y = heightBar;
                this.canvas.Children.Add(Bar);
            }
        }

        public void refresh()
        {
            this.Clear();
            var all = new List<Information>();
            GraphReader.getPtrParent(Kernel.getElement(middle.getCurrent()), ref all);
            all.Reverse();
            foreach (var result in all)
            {
                count++;
                var lab = new Label();
                if(count == all.Count)
                {
                    if (count == 1)
                    {
                        lab.Content = $"{result.name.ToUpper()} :\\";
                    }
                    else
                    {
                        lab.Content = $"{result.name}";
                    }

                }
                else
                {
                    if(count == 1)
                    {
                        lab.Content = $"{result.name.ToUpper()} :\\";
                    }
                    else
                    {
                        lab.Content = $"{result.name} > ";
                    }
                }

                lab.FontSize = 14;
                lab.FontWeight = FontWeights.Medium;
                labels.Add(lab);
                lab.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                Rect measureRect = new Rect(lab.DesiredSize);
                lab.Arrange(measureRect);
                Canvas.SetTop(lab, y);
                Canvas.SetLeft(lab, x);
                canvas.Children.Add(lab);
                
                x += measureRect.Width  - 5;
            }
        }
        public void Clear()
        {
            x = 210;
            y = heightBar;
            count = 0;
            foreach (var result in labels)
            {
                canvas.Children.Remove(result);
            }
            labels.Clear();
        }
        public void hide()
        {
            if (Bar != null)
            {
                canvas.Children.Remove(Bar);
                Bar = null;

            }
            this.Clear();
        }
    }
}
