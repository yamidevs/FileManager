using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct Graph
    {
        public IntPtr next;
        public IntPtr son;
        public IntPtr parent;
        public Information info;
    };
}
