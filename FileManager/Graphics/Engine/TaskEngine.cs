using FileManager.Graphics.UI;
using FileManager.Graphics.UI.Bar;
using FileManager.Interface.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileManager.Graphics.Engine
{
    public class TaskEngine
    {
        private Color ColorTask = Colors.IndianRed;
        private Canvas canvas;
        private double height, positionTask;
        private List<TaskElement> FileElements;
        private SearchElement search;
        private ExitElement exit;
        private int x = 50, count = 0;

        public TaskEngine(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void Initilization(double height, double positionTask)
        {
            this.height = height;
            FileElements = new List<TaskElement>();
            search = new SearchElement(5);
            exit = new ExitElement(6);
            this.positionTask = positionTask;
            this.Draw();
        }

        public void addTask(Element elem)
        {
            var element = new TaskElement(elem.Type, elem.Id);
            element.setPosition(x, (int)positionTask + 10);
            FileElements.Add(element);
            element.Generate();
            element.Display(this.canvas);
            x += (int)element.width + 15;
            count++;
        }
        public void setKey(string aid, string id)
        {
            FileElements.First(x => x.Id == aid).Id = id;
        }
        public bool ContainsKey(string id)
        {
            return FileElements.Any(x => x.Id == id);
        }
        public Element ContainsSearch(int xmouse , int ymouse)
        {
            if ((xmouse >= search.position.x && (xmouse <= search.position.x + search.width)) && (ymouse >= search.position.y && (ymouse <= search.position.y + search.height)))
                return search;
            else
                return null;
        }

        public Element ContainsExit(int xmouse, int ymouse)
        {
            if ((xmouse >= exit.position.x && (xmouse <= exit.position.x + exit.width)) && (ymouse >= exit.position.y && (ymouse <= exit.position.y + exit.height)))
                return exit;
            else
                return null;
        }
        public Element Contains(int xmouse, int ymouse)
        {
            var element = this.FileElements.Find(x => (xmouse >= x.position.x && (xmouse <= x.position.x + x.width)) && (ymouse >= x.position.y && (ymouse <= x.position.y + x.height)));
            return element;
        }
        public void Hide(string id)
        {
            if (FileElements.Any(x => x.Id == id))
            {
                var element = FileElements.First(x => x.Id == id);
                element.Hide(canvas);
                FileElements.Remove(element);
                foreach (var result in FileElements)
                {
                    result.Hide(canvas);
                }
                x = 50;
                foreach (var result in FileElements)
                {
                    result.setPosition(x, (int)positionTask + 10);
                    result.Generate();
                    result.Display(this.canvas);
                    x += (int)element.width + 15;
                }
                count--;
            }
        }
        private void Draw()
        {
            var rectangle = new Rectangle
            {
                Width = System.Windows.SystemParameters.PrimaryScreenWidth,
                Height = height
            };
            rectangle.Fill = new SolidColorBrush() { Color = ColorTask };
            Canvas.SetTop(rectangle, positionTask);
            this.canvas.Children.Add(rectangle);
            search.setPosition((int)System.Windows.SystemParameters.PrimaryScreenWidth - 45, (int)positionTask + 6);
            search.Generate();
            search.Display(canvas);

            exit.setPosition(10, (int)positionTask + 9);
            exit.Generate();
            exit.Display(canvas);
        }
    }
}
