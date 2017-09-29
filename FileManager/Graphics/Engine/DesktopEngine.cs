using FileManager.Engine;
using FileManager.Graphics.UI;
using FileManager.structs;
using FileManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileManager.Graphics.Engine
{
    public class DesktopEngine
    {
        private Canvas canvas;
        private List<DesktopElement> FileElement;
        private List<string> disks;
        private int count = 0;
        public Element current { get; set; } = null;
        public string cutId { get; set; } = "";
        public int x { get; private set; } = Configuration.startX;
        public int y { get; private set; } = Configuration.startY;
        public DesktopEngine(Canvas canvas)
        {
            this.canvas = canvas;
            FileElement = new List<DesktopElement>();
        }

        public void Initilization(List<string> disks,List<Information> infos)
        {
            this.disks = disks;
            foreach (var disk in disks)
            {
                Information info;
                info.name = disk;
                info.type = 0;
                info.id = FileManager.Graphics.Window.GetUniqueId();
                info.shortcut = "";
                Kernel.addGraph(info, 0);
                this.Add(info.name,info.type,info.id);

            }
        }
        public void GenerateGraph(List<Information> infos)
        {
            foreach(var result in infos)
            {
                this.Add(result.name, result.type, result.id);
            }
        }

        public void GenerateGraph()
        {
            var infos = Kernel.getDesktop();
            foreach (var result in infos)
            {
                this.Add(result.name, result.type, result.id,result.shortcut);
            }
        }
        public void Add(string name , int type , string id ,string id_shortcut = "")
        {
            if(type != -1)
            {
                var element = new DesktopElement(type, name, id);
                if(id_shortcut != String.Empty)
                {
                    element.setShortCut(id_shortcut);
                }
                if (x + (int)element.width + 15 > FileManager.Graphics.Window.Width)
                {
                    x = Configuration.startX;
                    y += (int)(element.height + (element.height / 2));
                }
                element.nameElement.setPosition((int)(x), (int)(y + (element.height - 25)));
                element.nameElement.setSize(9);

                element.setPosition(x, y);
                FileElement.Add(element);
                x += (int)element.width + 15;
                count++;
                element.Generate();
                element.Display(canvas);
            }                 
        }

        public string nameValid(string name)
        {
            int count = 0;
            foreach(var result in FileElement)
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
        public Element Contains(int xmouse, int ymouse)
        {
            var element = this.FileElement.Find(x => (xmouse >= x.position.x && (xmouse <= x.position.x + x.width)) && (ymouse >= x.position.y && (ymouse <= x.position.y + x.height)));
            return element;
        }
        public DesktopElement Contains(string id)
        {
            var element = FileElement.Find(x => x.Id == id);
            return element;
        }
        public void Generate()
        {
            foreach(var req in FileElement)
            {
                req.Generate();
                req.Display(canvas);
            }
        }
        public void Hide()
        {
            if(current != null)
            {
                current.LoadDefault(canvas);
                current = null;
            }
            foreach (var req in FileElement)
            {
                req.Hide(this.canvas);
            }
        }
        public Element First(string id)
        {
            if(FileElement.Any(x => x.Id == id))
            {
                return FileElement.First(x => x.Id == id);
            }
            return null;
        }
        public void Clean()
        {
            this.Hide();
            this.FileElement.Clear();
            x = Configuration.startX;
            y = Configuration.startY;
            count = 0;
        }
    }
}
