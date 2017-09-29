using FileManager.IO;
using FileManager.structs;
using FileManager.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FileManager.Engine
{
     
    public class Kernel
    {        
        private static IntPtr data = IntPtr.Zero;
        private List<string> disks = new List<string>();
        private FileManager.Graphics.Window window;
        private Canvas canvas;

        public Kernel(Canvas canvas , MainWindow content)
        {
            this.canvas = canvas;
            window = new FileManager.Graphics.Window(canvas, content);

     
        }
        public void Initialization()
        {
            this.loadMemory();
            window.Initialization(disks,GraphReader.getElementList(data));
        }

        public void MouseMove(int xmouse , int ymouse)
        {
            this.window.MouseMove(xmouse, ymouse);
        }
        public static IntPtr getData()
        {
            return data;
        }
        public void Click(int xmouse, int ymouse)
        {
            this.window.Click(xmouse, ymouse);
        }


        public void setUp()
        {
            this.window.isDown = false;
        }
        public static void Update(Information info)
        {
            Api.setElement(data, info);
        }
        public void ClickOne(int xmouse , int ymouse)
        {
            this.window.ClickOne(xmouse, ymouse);
        }
        public void Menu(int xmouse , int ymouse)
        {
            this.window.ShowMenu(xmouse, ymouse);
        }

        public static void addGraph(Information info , int type)
        {
            if(data == IntPtr.Zero)
            {
                Api.CreateGraph(ref data, info);
            }else
            {
                Api.AddGraph(data, info,type);
            }
        }

        public static void freeById(string id)
        {
             Api.freeGraph(data, id);            


        }
        public static List<Information> getDesktop()
        {
            List<Information> infos = new List<Information>();
            var ptr = data;
            while(ptr != IntPtr.Zero && ptr != null)
            {
                var elem = GraphReader.Element(ptr);
                infos.Add(elem.info);
                ptr = elem.next;
            }
            return infos;
        }

        public static void addGraph(IntPtr dad , Information info, int type)
        {       
                Api.AddGraph(dad, info, type);
            
        }
        public static void Display()
        {
            GraphReader.Display(data);
        }
        public static IntPtr getElement(string Id)
        {
            IntPtr ptr = IntPtr.Zero;
            GraphReader.getPtr(data, Id,ref ptr);
            return ptr;
        }

     
        public static IntPtr getSon(IntPtr element)
        {
            return GraphReader.Element(element).son;
        }
 
        private void loadMemory()
        {
            var lines = File.ReadAllLines("Disk.txt");
            foreach(var x in lines)
            {
                disks.Add(x);
            }
        }
    }
}
