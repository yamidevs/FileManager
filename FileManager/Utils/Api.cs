using FileManager.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Utils
{
    public static class Api
    {

        [DllImport(Configuration.name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateGraph([In, Out]  ref IntPtr graph ,Information info);

        [DllImport(Configuration.name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AddGraph([In, Out]  IntPtr graph, Information info , int type);

        [DllImport(Configuration.name, CallingConvention = CallingConvention.Cdecl)]
        public static extern int freeGraph( [In, Out]  IntPtr graph , string id);

        [DllImport(Configuration.name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setElement([In, Out]  IntPtr graph, Information info);

        [DllImport(Configuration.name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sorts([In, Out]  IntPtr graph);
    }
}
