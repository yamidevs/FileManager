using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FileManager.Graphics.UI
{
    public abstract class Element
    {
        public abstract int Type { get; set; }
        public abstract string Id { get; set; }
        public abstract bool Enabled { get; set; }
        public abstract void LoadHover(Canvas current);
        public abstract void LoadDefault(Canvas current);
        public abstract void Hide(Canvas current);


    }
}
