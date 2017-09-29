using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FileManager.Graphics.UI
{
    public interface IElement
    {

        void Generate();
        void Display(Canvas current);
        void setPosition(int x, int y);
    }
    
}
