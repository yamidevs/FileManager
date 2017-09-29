using FileManager.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileManager.IO
{
    public static class GraphReader
    {
        public static int Count(IntPtr ptr)
        {
            if(ptr != IntPtr.Zero)
            {
                if(Element(ptr).info.type == -1)
                {
                    return Count(Element(ptr).next) + Count(Element(ptr).son);
                }else
                    return 1 + Count(Element(ptr).next) + Count(Element(ptr).son);
            }
            else
            {
                return 0;
            }
        }

        public static int Count(IntPtr ptr , int type)
        {
            if (ptr != IntPtr.Zero)
            {
                if (Element(ptr).info.type == type)
                {
                    return  1+Count(Element(ptr).next, type) + Count(Element(ptr).son,type);
                }
                else
                    return  Count(Element(ptr).next, type) + Count(Element(ptr).son,type);
            }
            else
            {
                return 0;
            }
        }

        public static int CountNext(IntPtr ptr)
        {
            int count = 0;
            while (ptr != IntPtr.Zero)
            {
                Graph element = Element(ptr);
                if(element.info.type != -1)
                    count++;
                ptr = element.next;
            }
            return count;
        }
   
        public static void Display(IntPtr ptr)
        {
            if(ptr != IntPtr.Zero)
            {
                MessageBox.Show(Element(ptr).info.id);
                Display(Element(ptr).next);
                Display(Element(ptr).son);

            }
        }

        public static IEnumerable<Information> getElements(IntPtr ptr)
        {
            while (ptr != IntPtr.Zero)
            {
                Graph element = Element(ptr);
                yield return element.info;
                ptr = element.next;
            }
        }

        public static List<Information> getElementList(IntPtr ptr)
        {
            List<Information> infos = new List<Information>();
            while (ptr != IntPtr.Zero)
            {
                Graph element = Element(ptr);
                infos.Add(element.info);
                ptr = element.next;
            }

            return infos;
        }

        public static void getPtr(IntPtr ptr , string Id , ref IntPtr refptr)
        {
            if(ptr != IntPtr.Zero)
            {
                var elem = Element(ptr);
                if(elem.info.id == Id)
                {
                    refptr = ptr;
                    return;
                }          
                getPtr(elem.son, Id,ref refptr);
                getPtr(elem.next,Id, ref refptr);

            }
        }



        public static void getPtr(IntPtr ptr , ref List<Information> infos)
        {
            if (ptr != IntPtr.Zero)
            {
                var elem = Element(ptr);
                infos.Add(elem.info);
                getPtr(elem.next,ref infos);
                getPtr(elem.son,ref infos);
            }
        }

        public static void getPtrName(IntPtr ptr, ref List<Information> infos , string name)
        {
            if (ptr != IntPtr.Zero)
            {
                var elem = Element(ptr);
                if(elem.info.name == name)
                    infos.Add(elem.info);
                getPtrName(elem.next, ref infos,name);
                getPtrName(elem.son, ref infos,name);
            }
        }

        public static void getPtrParent(IntPtr ptr, ref List<Information> infos)
        {
            if (ptr != IntPtr.Zero)
            {
                var elem = Element(ptr);
                infos.Add(elem.info);
                getPtrParent(elem.parent,ref infos);
            }
        }

        public static Graph Element(IntPtr ptr)
        {
            return (Graph)(Marshal.PtrToStructure(ptr,typeof(Graph)));
        }
    }
}
