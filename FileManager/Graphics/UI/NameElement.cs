using FileManager.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileManager.Graphics.UI
{
    public class NameElement : IElement
    {
        public ElementPosition position;
        public Label label;
        public string Name;
        private double size;
        public NameElement(string name)
        {
            this.Name = name;           
        }

        public void setPosition(int x, int y)
        {
            position.x = x;
            position.y = y;
        }
        public void setSize(double size)
        {
            this.size = size;
        }
        public void Generate()
        {
            label = new Label
            {
                Content = Name,
            };
            label.FontSize = size;
            label.FontWeight = FontWeights.Bold;
        }

        public void Display(Canvas current)
        {
            if (label != null)
            {
                Canvas.SetTop(label, position.y);
                Canvas.SetLeft(label, position.x);
                current.Children.Add(label);
            }
        }

    }
}
