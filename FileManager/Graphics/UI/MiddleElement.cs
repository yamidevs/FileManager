using FileManager.Graphics.Engine;
using FileManager.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FileManager.Graphics.UI
{
    public class MiddleElement : Element, IElement
    {
        public double width, height;
        public ElementPosition position;
        public override string Id { get; set; }
        public string shortcut_id = "";

        BitmapImage imageInfo;
        public Image imageSource = null;
        public NameElement nameElement;

        public override int Type { get; set; }
        public override bool Enabled { get; set; } = true;

        public MiddleElement(int type, string name, string Id)
        {
            this.Type = type;
            this.Id = Id;
            imageInfo = new BitmapImage(new Uri($"resources/{type}.png", UriKind.Relative));
            width = imageInfo.Width;
            height = imageInfo.Height;
            nameElement = new NameElement(name);
        }
        public override void LoadDefault(Canvas current)
        {
            this.LoadImage(current, $"resources/{this.Type}.png");

        }
        public override void LoadHover(Canvas current)
        {
            if(Enabled == true)
            this.LoadImage(current, $"resources/{this.Type}_hover.png");
        }
        public void LoadImage(Canvas current, string path)
        {
            if (imageSource != null)
                current.Children.Remove(imageSource);
            imageInfo = new BitmapImage(new Uri(path, UriKind.Relative));
            this.Generate();
            this.Display(current);
            this.LoadName(current);
        }
        public void setPosition(int x, int y)
        {
            position.x = x;
            position.y = y;
        }
        public void Display(Canvas current)
        {
            if (imageSource != null)
            {
                Canvas.SetTop(imageSource, position.y);
                Canvas.SetLeft(imageSource, position.x);     
                current.Children.Add(imageSource);
                this.LoadName(current);
            }
        }
        public void setShortCut(string id)
        {
            this.shortcut_id = id;
        }
        public void Generate()
        {
            Enabled = true;
            if (imageSource == null)
            {
                imageSource = new Image
                {
                    Source = this.imageInfo,
                    Width = this.width,
                    Height = this.height
                };
            }
            else
            {
                imageSource.Source = null;
                imageSource.Source = imageInfo;
                imageSource.Width = imageInfo.Width;
                imageSource.Height = imageInfo.Height;
            }

            if (shortcut_id != "")
                imageSource.Opacity = 0.5;

        }

        public void LoadName(Canvas current)
        {
            if (this.nameElement != null)
                current.Children.Remove(this.nameElement.label);
            this.nameElement.Generate();
            if (shortcut_id != "")
                nameElement.label.Opacity = 0.5;
            this.nameElement.Display(current);
        }


        public override void Hide(Canvas current)
        {
            Enabled = false;
            current.Children.Remove(nameElement.label);
            current.Children.Remove(imageSource);
            

        }
    }
}
