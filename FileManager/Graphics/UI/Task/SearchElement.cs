using FileManager.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FileManager.Graphics.UI.Bar
{
    public class SearchElement : Element, IElement
    {
        public override int Type { get; set; }
        public override string Id { get; set; }
        public override bool Enabled { get; set; } = true;
        public double width, height;
        public ElementPosition position;

        BitmapImage imageInfo;
        public Image imageSource = null;
        public SearchElement(int type)
        {
            this.Type = type;
            imageInfo = new BitmapImage(new Uri($"resources/{type}.png", UriKind.Relative));
            width = imageInfo.Width;
            height = imageInfo.Height;
        }
        public override void LoadDefault(Canvas current)
        {
            this.LoadImage(current, $"resources/{this.Type}.png");

        }
        public override void LoadHover(Canvas current)
        {
            this.LoadImage(current, $"resources/{this.Type}_hover.png");
        }
        public void LoadImage(Canvas current, string path)
        {
            if (imageSource != null)
                current.Children.Remove(imageSource);
            imageInfo = new BitmapImage(new Uri(path, UriKind.Relative));
            this.Generate();
            this.Display(current);
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
            }
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

        }



        public override void Hide(Canvas current)
        {
            Enabled = false;
            current.Children.Remove(imageSource);

        }
    }
}
