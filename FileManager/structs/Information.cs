using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.structs
{
    public struct Information
    {
        public int type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string shortcut;
    }
}
