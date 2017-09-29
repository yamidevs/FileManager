using FileManager.Engine;
using FileManager.Graphics.Engine;
using FileManager.Graphics.Enum;
using FileManager.Graphics.UI;
using FileManager.IO;
using FileManager.structs;
using FileManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileManager.Graphics
{

    public class Window
    {
        public State state = State.Desktop;
        private Canvas canvas;
        private TaskEngine task;
        public DesktopEngine desktop;
        private MiddleEngine middle;
        private MainWindow content;
        private Image newImage = null;
        private TextBox newBox = null;
        private Element current , copy , cut;
        private int currentItem = -1;
        private TextBox rename = null;
        private string idRename = null;
       
        public bool isDown { get; set; } = false;
        private Dictionary<int, int> index = new Dictionary<int, int>()
        {
            {0,0 },
            {1,1 },
            {2,4 }
        };
        private Dictionary<int, bool> restriction = new Dictionary<int, bool>()
        {
            {0,true },
            {1,true },
            {4,false }
        };
        private List<Tuple<string,State>> Items = new List<Tuple<string, State>>()
        {
            Tuple.Create("Nouveau Disque",State.Desktop),
            Tuple.Create("Nouveau Dossier",State.All),
            Tuple.Create("Nouveau Fichier",State.All),

        };
        // value == %
        private double taskY = 94 , BarTop  = 4;
        public static double Width = System.Windows.SystemParameters.PrimaryScreenWidth, Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        public Window(Canvas canvas , MainWindow content)
        {
            this.canvas = canvas;
            task = new TaskEngine(canvas);
            desktop = new DesktopEngine(canvas);
            middle = new MiddleEngine(canvas);
            middle.click_prev += OnPrev;
            this.content = content;
        }

        public void Initialization(List<string> disks,List<Information>infos)
        {
            task.Initilization((Height *  (100 - taskY)) /100,(Height * taskY)/100);
            middle.Initilization(BarTop, (Height * (100 - taskY)) / 100);
            desktop.Initilization(disks,infos);       
        }

        public void Click(int xmouse, int ymouse)
        {
            if (state == State.Desktop)
            {
                var element = desktop.Contains(xmouse, ymouse);
                if (element != null && restriction[element.Type])
                {
                    if(GraphReader.Element(Kernel.getElement(element.Id)).info.shortcut != "")
                    {
                        string shortcut = GraphReader.Element(Kernel.getElement(element.Id)).info.shortcut;
                        element = desktop.First(shortcut);
                        //then in middle
                        if (element == null)
                        {
                            bool search = false;
                            foreach(var result in middle.FileElements)
                            {
                                foreach(var req in result.Value.elements)
                                {
                                    if(req.Id == shortcut)
                                    {
                                        element = req;
                                        search = true;
                                    }
                                    break;
                                }
                                if (search == true)
                                    break;
                            }
                        }
                        
                    }
                    if(rename != null)
                    {
                        canvas.Children.Remove(rename);
                    }
                    desktop.Hide();
                    state = State.Middle;
                    if(!task.ContainsKey(element.Id))
                        task.addTask(element);

                    var elementData = Kernel.getElement(element.Id);
                    var son = Kernel.getSon(elementData);
                    this.middle.setCurrent(element.Id);
                    this.middle.GenerateGraph(element.Id, son);
                    this.middle.OnMiddle(4);
                    middle.setTitle(element.Id);
                }

            }else
            {
                int type = -1;
                var element = middle.Contains(xmouse, ymouse,ref type);
                if(element != null && type > -1)
                {

                    if (type == 0) // close
                    {
                     
                    }
                    else if (type == 1)
                    {
                        middle.hideMiddle();
                        middle.setCurrent("");
                        state = State.Desktop;
                        desktop.Generate();
                    }
                    else if (type == 2 && restriction[element.Type])
                    {
                        if (rename != null)
                        {
                            canvas.Children.Remove(rename);
                        }
                        if (GraphReader.Element(Kernel.getElement(element.Id)).info.shortcut != "")
                        {
                            string shortcut = GraphReader.Element(Kernel.getElement(element.Id)).info.shortcut;
                            element = middle.First(shortcut);
                            //then in dekstop
                            if (element == null)
                            {
                                bool search = false;
                                foreach (var result in middle.FileElements)
                                {
                                    foreach (var req in result.Value.elements)
                                    {
                                        if (req.Id == shortcut)
                                        {
                                            element = req;
                                            search = true;
                                        }
                                        break;
                                    }
                                    if (search == true)
                                        break;
                                }
                            }

                        }
                        current = null;
                        middle.current = null;
                        var elementData = Kernel.getElement(element.Id);
                        var son = Kernel.getSon(elementData);
                        middle.hideCurrent();
                        if(task.ContainsKey(middle.getCurrent()))
                        {
                            task.setKey(middle.getCurrent(), element.Id);
                        }
                        this.middle.setCurrent(element.Id);
                        this.middle.GenerateGraph(element.Id, son);
                        middle.setTitle(element.Id);
                    }
                }

            }
        }

        public void ClickOne(int xmouse, int ymouse)
        {
            if(task.Contains(xmouse,ymouse) != null)
            {
                var element = task.Contains(xmouse, ymouse);
                var elementData = Kernel.getElement(element.Id);
                var son = Kernel.getSon(elementData);
                if (state == State.Middle)
                {
                    middle.hideCurrent();
                    this.middle.setCurrent(element.Id);
                    this.middle.GenerateGraph(element.Id, son);
                }else
                {
                    desktop.Hide();
                    state = State.Middle;
                    this.middle.setCurrent(element.Id);
                    this.middle.GenerateGraph(element.Id, son);
                    this.middle.OnMiddle(4);
                }
                middle.setTitle(element.Id);

            }else if(task.ContainsSearch(xmouse,ymouse) != null)//search
            {
                var search = new Search(this);
                search.Show();
            }
            else if (task.ContainsExit(xmouse, ymouse) != null)//exit
            {
                Application.Current.Shutdown();
            }
            else if (state == State.Desktop)
            {
  
                if(desktop.current != null)
                {
                     desktop.current.LoadDefault(canvas);
                    desktop.current = null;
                }
                var element = desktop.Contains(xmouse, ymouse);
                if(element != null)
                {
                    desktop.current = element;
                    desktop.current.LoadHover(canvas);
                }
            }
            else
            {
                if(middle.liste.Contains(xmouse,ymouse) != "")
                {
                    var Id = middle.liste.Contains(xmouse, ymouse);
                    current = null;
                    middle.current = null;
                    var elementData = Kernel.getElement(Id);
                    var son = Kernel.getSon(elementData);
                    middle.hideCurrent();
                    if (task.ContainsKey(middle.getCurrent()))
                    {
                        task.setKey(middle.getCurrent(), Id);
                    }
                    this.middle.setCurrent(Id);
                    this.middle.GenerateGraph(Id, son);
                    middle.setTitle(Id);
                }
                else
                {
                    int type = -1;
                    var element = middle.Contains(xmouse, ymouse, ref type);
                    if (element != null && type > -1)
                    {
                        if (type == 0) // close
                        {
                            if (task.ContainsKey(middle.getCurrent()))
                            {
                                task.Hide(middle.getCurrent());
                            }
                            middle.hideMiddle();
                            middle.delete(middle.getCurrent());
                            middle.setCurrent("");
                            state = State.Desktop;
                            desktop.Generate();
                        }
                        else if (type == 1)
                        {
                            middle.hideMiddle();
                            middle.setCurrent("");
                            state = State.Desktop;
                            desktop.Generate();
                        }
                        else
                        {
                            if (middle.current != null && middle.Contains(middle.current))
                            {
                                middle.current.LoadDefault(canvas);
                                middle.current = null;
                            }

                            middle.current = element;
                            middle.current.LoadHover(canvas);

                        }

                    }
                    else
                    {
                        if (middle.current != null && middle.Contains(middle.current))
                        {
                            middle.current.LoadDefault(canvas);
                            middle.current = null;
                        }
                    }
                }        
            }
        }

        public void open_id(string Id)
        {
            if(state == State.Desktop)
            {
                desktop.Hide();
                state = State.Middle;
                var elementData = Kernel.getElement(Id);
                var son = Kernel.getSon(elementData);
                this.middle.setCurrent(Id);
                this.middle.GenerateGraph(Id, son);
                this.middle.OnMiddle(4);
                middle.setTitle(Id);
            }else
            {
                current = null;
                middle.current = null;
                var elementData = Kernel.getElement(Id);
                var son = Kernel.getSon(elementData);
                middle.hideCurrent();
                if (task.ContainsKey(middle.getCurrent()))
                {
                    task.setKey(middle.getCurrent(), Id);
                }
                this.middle.setCurrent(Id);
                this.middle.GenerateGraph(Id, son);
                middle.setTitle(Id);
            }
        }      
        public void enter_search(string name)
        {
            var infos = new List<Information>();
            GraphReader.getPtrName(Kernel.getData(), ref infos, name); 
            var search = new SearchView(infos,this);
            search.Show();
        }
        public void MouseMove(int xmouse, int ymouse)
        {
            Element element = null;
   
            if (task.Contains(xmouse, ymouse) != null)
            {
                element = task.Contains(xmouse, ymouse);

            }else if(task.ContainsSearch(xmouse , ymouse) != null)
            {
                element = task.ContainsSearch(xmouse, ymouse);
            }
            else if (task.ContainsExit(xmouse, ymouse) != null)
            {
                element = task.ContainsExit(xmouse, ymouse);
            }
            else if (state == State.Desktop)
            {
                if (desktop.Contains(xmouse, ymouse) != null)
                {
                    element = desktop.Contains(xmouse, ymouse);
                }
            }
            else
            {
                int type = -1;
                element = middle.Contains(xmouse, ymouse, ref type);
            }
            if(middle.liste.Contains(xmouse, ymouse) != "")
            {
                middle.liste.hover(xmouse, ymouse);
            }
            if (current != null)
            {
                if (current.Enabled)
                {
                    if(state == State.Desktop)
                    {
                        if (current != desktop.current)
                            current.LoadDefault(this.canvas);
                    }else
                    {
                        if (current != middle.current)
                        {
                            if(middle.Contains(current))
                                current.LoadDefault(this.canvas);
                        }
                        if(middle.liste.current != null)
                        {
                            if (middle.liste.current.id != middle.liste.Contains(xmouse,ymouse))
                            {
                                middle.liste.current.defaultLabel();
                                middle.liste.current = null;
                            }
                        }

                    }
                    

                } 

                Mouse.OverrideCursor = Cursors.Arrow;
            }
            if (element != null)
            {
                if (element != desktop.current && element != middle.current)
                    current = element;
                Mouse.OverrideCursor = Cursors.Hand;
                element.LoadHover(this.canvas);
            }
        }
              
        public void ShowMenu(int xmouse , int ymouse)
        {
            ContextMenu menu = new ContextMenu();
            foreach(var item in Items)
            {
               
                if (item.Item2 == state || item.Item2 == State.All)
                {
                    var menuItem = new MenuItem();
                    menuItem.Header = item.Item1;
                    menuItem.Click += click_Item;
                    menu.Items.Add(menuItem);
                }
            }
            //trie
            var trie = new MenuItem();
            trie.Header = "Trie par nom";
            trie.Click += trie_Item;
            menu.Items.Add(trie);
            //copy
            int type = -1;
            if(current != null && (desktop.Contains(xmouse,ymouse) != null || middle.Contains(xmouse,ymouse,ref type) != null))
            {
                var menuItem = new MenuItem();
                menuItem.Header = "Copier";
                menuItem.Click += copy_Item;
                menu.Items.Add(menuItem);

                var cut = new MenuItem();
                cut.Header = "Couper";
                cut.Click += cut_Item;
                menu.Items.Add(cut);

                if(state == State.Middle)
                {
                    List<Information> list;
                    list = GraphReader.getElementList(GraphReader.Element(Kernel.getElement(middle.getCurrent())).son).Where(x => x.id != current.Id && x.type != -1).ToList();

                    if (current.Type != 4)
                    {
                        list = list.Where(x => x.type != 4).ToList();
                    }
                    if (list.Count > 0)
                    {
                        var move = new MenuItem();
                        move.Header = "Deplacer vers";
                        foreach (var result in list)
                        {
                            var sub_menu = new MenuItem() { Header = result.name };
                            sub_menu.Click += move_Item;
                            move.Items.Add(sub_menu);
                        }
                        move.Click += cut_Item;
                        menu.Items.Add(move);
                    }
                    
                }

                var rename = new MenuItem();
                rename.Header = "Renommer";
                rename.Click += rename_Item;
                menu.Items.Add(rename);

                var supp = new MenuItem();
                supp.Header = "Supprimer";
                supp.Click += delete_Item;
                menu.Items.Add(supp);
            }

            //past
            if(copy != null ||cut != null)
            {
                var menuItem = new MenuItem();
                menuItem.Header = "Coller";
                menuItem.Click += past_Item;
                menu.Items.Add(menuItem);
            }
           
            //past racc
            if(copy != null)
            {
                var menuracc = new MenuItem();
                menuracc.Header = "Coller raccourci";
                menuracc.Click += past_shortcut_Item;
                menu.Items.Add(menuracc);
            }
            var p = new MenuItem();
            p.Header = "Propriétés";
            p.Click += p_Item;
            menu.Items.Add(p);
            this.content.ContextMenu = menu;

        }

        private void p_Item(object sender, RoutedEventArgs e)
        {
            string str = "";
            str += $"Nombres éléments : {GraphReader.Count(Kernel.getData())} \n";
            str += $"Nombres réprértoires : {GraphReader.Count(Kernel.getData(),0)} \n";
            str += $"Nombres dossiers : {GraphReader.Count(Kernel.getData(), 1)} \n";
            str += $"Nombres fichiers : {GraphReader.Count(Kernel.getData(), 4)} \n";
            MessageBox.Show(str);

        }

        private void move_Item(object sender, RoutedEventArgs e)
        {
            if (state == State.Middle)
            {
                var item = (MenuItem)e.OriginalSource;
                var name = (string)item.Header;
                var element = middle.FindName(name);
                if (element != null && element.shortcut_id == "")
                {
                    if (current != null)
                    {
                        var elemcurrent = middle.First(current.Id);
                        if (elemcurrent != null)
                        {
                            Information info;
                            info.name = middle.nameValid(elemcurrent.nameElement.Name);
                            info.id = GetUniqueId();
                            info.type = elemcurrent.Type;
                            info.shortcut = "";

                            Kernel.addGraph(Kernel.getElement(element.Id), info, 1);
                            if (GraphReader.Element(Kernel.getElement(elemcurrent.Id)).son != IntPtr.Zero)
                            {
                                var ptr = Kernel.getElement(info.id);
                                Information infos;
                                infos.name = GraphReader.Element(GraphReader.Element(Kernel.getElement(elemcurrent.Id)).son).info.name;
                                infos.id = GetUniqueId();
                                infos.type = GraphReader.Element(GraphReader.Element(Kernel.getElement(elemcurrent.Id)).son).info.type;
                                infos.shortcut = "";
                                Kernel.addGraph(ptr, infos, 1);
                                copyElement(Kernel.getElement(infos.id), GraphReader.Element(Kernel.getElement(elemcurrent.Id)).son, 1);

                            }

                            Information cutInfo;
                            cutInfo.id = elemcurrent.Id;
                            cutInfo.name = elemcurrent.nameElement.Name;
                            cutInfo.type = -1;
                            cutInfo.shortcut = "";
                            if (task.ContainsKey(elemcurrent.Id))
                            {
                                task.Hide(elemcurrent.Id);
                            }
                            Kernel.Update(cutInfo);
                            //stupid code a lot of repeat
                            current = null;
                            middle.current = null;
                            var elementData = Kernel.getElement(middle.getCurrent());
                            var son = Kernel.getSon(elementData);
                            middle.hideCurrent();
                            if (task.ContainsKey(middle.getCurrent()))
                            {
                                task.setKey(middle.getCurrent(), middle.getCurrent());
                            }
                            this.middle.setCurrent(middle.getCurrent());
                            this.middle.GenerateGraph(middle.getCurrent(), son);
                            middle.setTitle(middle.getCurrent());

                        }
                    }
                }
            }
        }

        private void past_shortcut_Item(object s , RoutedEventArgs e)
        {
            var element = GraphReader.Element(Kernel.getElement(copy.Id));
            Information info;
            info.name = $"{element.info.name}";
            info.type = element.info.type;
            info.id = GetUniqueId();
            info.shortcut = copy.Id;
            if (state == State.Desktop)
            {
                info.name = desktop.nameValid(info.name);
                Kernel.addGraph(info, 0);
                desktop.Add(info.name, info.type, info.id, info.shortcut);
            }else
            {
                info.name = middle.nameValid(info.name);
                Kernel.addGraph(Kernel.getElement(middle.getCurrent()),info,1);
                middle.Add(info.id, info.type, info.name, info.shortcut);

            }
            copy = null;
        }
        private void trie_Item(object sender, RoutedEventArgs e)
        {
            if(state == State.Desktop)
            {
                Api.sorts(Kernel.getData());
                desktop.Clean();
                desktop.GenerateGraph();
            }else
            {
                Api.sorts(GraphReader.Element(Kernel.getElement(middle.getCurrent())).son);
                middle.hideCurrent();
                middle.GenerateGraph(middle.getCurrent(), GraphReader.Element(Kernel.getElement(middle.getCurrent())).son);
            }
            
        }
        private void rename_Item(object s, RoutedEventArgs e)
        {
            if (rename != null)
                canvas.Children.Remove(rename);

            rename = new TextBox();
            if(desktop.Contains(current.Id) != null)
            {
                var element = desktop.Contains(current.Id);
                rename.Text = element.nameElement.Name;
                rename.Width = element.width;
                rename.KeyDown += rename_keyDown;
                Canvas.SetTop(rename, element.nameElement.position.y);
                Canvas.SetLeft(rename, element.nameElement.position.x);
                canvas.Children.Add(rename);
                idRename = element.Id; 
                element.Enabled = false;
            }else
            {
                var element = middle.First(current.Id);
                rename.Text = element.nameElement.Name;
                rename.Width = element.width;
                rename.KeyDown += rename_keyDown;
                Canvas.SetTop(rename, element.nameElement.position.y);
                Canvas.SetLeft(rename, element.nameElement.position.x);
                canvas.Children.Add(rename);
                idRename = element.Id;
                element.Enabled = false;
            }
        }

        private void rename_keyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (rename != null)
                {
                    if (state == State.Desktop)
                    {
                        if (rename.Text != "")
                        {
                            var ptr = Kernel.getElement(idRename);
                            var info = GraphReader.Element(Kernel.getElement(idRename)).info;
                            Information ninfo;
                            ninfo.id = info.id;
                            ninfo.name =desktop.nameValid(rename.Text);
                            ninfo.type = info.type;
                            ninfo.shortcut = info.shortcut;
                            Api.setElement(ptr, ninfo);
                            desktop.Contains(idRename).nameElement.label.Content = ninfo.name;
                            desktop.Contains(idRename).nameElement.Name = ninfo.name;
                            desktop.Contains(idRename).Enabled = true;

                        }
                    }else
                    {
                        var ptr = Kernel.getElement(idRename);
                        var info = GraphReader.Element(Kernel.getElement(idRename)).info;
                        Information ninfo;
                        ninfo.id = info.id;
                        ninfo.name = middle.nameValid(rename.Text);
                        ninfo.type = info.type;
                        ninfo.shortcut = info.shortcut;
                        Api.setElement(ptr, ninfo);
                        middle.First(idRename).nameElement.label.Content = ninfo.name;
                        middle.First(idRename).nameElement.Name = ninfo.name;
                        middle.First(idRename).Enabled = true;
                    }

                    canvas.Children.Remove(rename);
                }

                rename = null;
            }
            
        }

        private void copy_Item(object s , RoutedEventArgs e)
        {
            copy = current;
        }

        private void cut_Item(object s, RoutedEventArgs e)
        {
            cut = current;
        }

        private void delete_Item(object s, RoutedEventArgs e)
        {
            try
            {
                Information info;
              
                info.id = GraphReader.Element(Kernel.getElement(current.Id)).info.id;
                info.name = GraphReader.Element(Kernel.getElement(current.Id)).info.name;
                info.type = -1;
                info.shortcut = "";
                if (task.ContainsKey(info.id))
                {
                    task.Hide(info.id);
                }
                Kernel.Update(info);
                
                if (state == State.Desktop)
                {
                    desktop.Clean();
                    desktop.GenerateGraph();
                }
                else
                {
                    middle.hideCurrent();
                    middle.GenerateGraph(middle.getCurrent(), GraphReader.Element(Kernel.getElement(middle.getCurrent())).son);
                    middle.refresh();

                }
            }
            catch
            {
                MessageBox.Show("Veuillez selectionnez un element ");
            }
            
        }

        private void deleteElement(IntPtr ptr)
        {
            deleteElement(GraphReader.Element(ptr).next);
            deleteElement(GraphReader.Element(ptr).son);
            Marshal.FreeHGlobal(ptr);
        }

        private void past_Item(object s , RoutedEventArgs e)
        {
            if (state == State.Desktop)
            {
                if(copy != null)
                {
                    var elem = GraphReader.Element(Kernel.getElement(copy.Id));
                    Information info;
                    info.name = desktop.nameValid(elem.info.name);
                    info.id = GetUniqueId();
                    info.type = elem.info.type;
                    info.shortcut = "";

                    Kernel.addGraph(info, 0);
                    if (GraphReader.Element(Kernel.getElement(copy.Id)).son != IntPtr.Zero)
                    {
                        var ptr = Kernel.getElement(info.id);
                        Information infos;
                        infos.name = GraphReader.Element(GraphReader.Element(Kernel.getElement(copy.Id)).son).info.name;
                        infos.id = GetUniqueId();
                        infos.type = GraphReader.Element(GraphReader.Element(Kernel.getElement(copy.Id)).son).info.type;
                        infos.shortcut = "";
                        Kernel.addGraph(ptr, infos, 1);
                        copyElement(Kernel.getElement(infos.id), GraphReader.Element(Kernel.getElement(copy.Id)).son, 1);
                        desktop.Clean();
                        desktop.GenerateGraph();
                    }
                    else
                    {
                        desktop.Add(info.name, info.type, info.id);
                    }

                    copy = null;
                }
                
                
            }
            else
            {
               
                Graph elem;
                if(cut == null)
                    elem = GraphReader.Element(Kernel.getElement(copy.Id));
                else
                    elem = GraphReader.Element(Kernel.getElement(cut.Id));

                Information info;
                info.name = middle.nameValid(elem.info.name);
                info.id = GetUniqueId();
                info.type = elem.info.type;
                info.shortcut = ""; 


                if (cut == null)
                {
                    if(desktop.First(copy.Id) == null)
                    {
                        Kernel.addGraph(GraphReader.Element(Kernel.getElement(middle.getCurrent())).son, info, 0);
                        if (copy != null)
                        {
                            if (GraphReader.Element(Kernel.getElement(copy.Id)).son == IntPtr.Zero)
                            {
                                middle.Add(info.id, info.type, info.name);
                            }
                            else
                            {
                                var ptr = Kernel.getElement(info.id);
                                Information infos;
                                infos.name = GraphReader.Element(GraphReader.Element(Kernel.getElement(copy.Id)).son).info.name;
                                infos.id = GetUniqueId();
                                infos.type = GraphReader.Element(GraphReader.Element(Kernel.getElement(copy.Id)).son).info.type;
                                infos.shortcut = "";
                                Kernel.addGraph(ptr, infos, 1);
                                copyElement(Kernel.getElement(infos.id), GraphReader.Element(Kernel.getElement(copy.Id)).son, 1);
                                middle.hideCurrent();
                                middle.GenerateGraph(middle.getCurrent(), (GraphReader.Element(Kernel.getElement(middle.getCurrent())).son));
                                middle.refresh();
                            }
                        }
                    }                  

                }
                else
                {
                    if(desktop.First(cut.Id) == null)
                    {
                        Kernel.addGraph(Kernel.getElement(middle.getCurrent()), info, 1);
                        if (GraphReader.Element(Kernel.getElement(cut.Id)).son == IntPtr.Zero)
                        {
                            middle.Add(info.id, info.type, info.name);

                        }
                        else
                        {
                            var ptr = Kernel.getElement(info.id);
                            Information infos;
                            infos.name = GraphReader.Element(GraphReader.Element(Kernel.getElement(cut.Id)).son).info.name;
                            infos.id = GetUniqueId();
                            infos.type = GraphReader.Element(GraphReader.Element(Kernel.getElement(cut.Id)).son).info.type;
                            infos.shortcut = "";
                            Kernel.addGraph(ptr, infos, 1);
                            copyElement(Kernel.getElement(infos.id), GraphReader.Element(Kernel.getElement(cut.Id)).son, 1);
                            middle.hideCurrent();
                            middle.GenerateGraph(middle.getCurrent(), GraphReader.Element(Kernel.getElement(middle.getCurrent())).son);
                        }
                    }                  

                }
                    

                if (cut != null)
                {
                    Information cutInfo;

                    cutInfo.id = GraphReader.Element(Kernel.getElement(cut.Id)).info.id;
                    cutInfo.name = GraphReader.Element(Kernel.getElement(cut.Id)).info.name;
                    cutInfo.type = -1;
                    cutInfo.shortcut = "";
                    if (task.ContainsKey(cutInfo.id))
                    {
                        task.Hide(cutInfo.id);
                    }
                    Kernel.Update(cutInfo);
                    //stupid code  a lot of repeat
                   current = null;
                    middle.current = null;
                    var elementData = Kernel.getElement(middle.getCurrent());
                    var son = Kernel.getSon(elementData);
                    middle.hideCurrent();
                    if (task.ContainsKey(middle.getCurrent()))
                    {
                        task.setKey(middle.getCurrent(), middle.getCurrent());
                    }
                    this.middle.setCurrent(middle.getCurrent());
                    this.middle.GenerateGraph(middle.getCurrent(), son);
                    middle.setTitle(middle.getCurrent());
                    cut = null;
                }else
                {
                    copy = null;
                }

            }
        }
        private void OnPrev(int i )
        {
            if(state == State.Middle)
            {
                var parent = GraphReader.Element(Kernel.getElement(middle.getCurrent())).parent;
                if(parent != IntPtr.Zero)
                {
                    var element = GraphReader.Element(parent).info;
                    current = null;
                    middle.current = null;
                    var elementData = Kernel.getElement(element.id);
                    var son = Kernel.getSon(elementData);
                    middle.hideCurrent();
                    if (task.ContainsKey(middle.getCurrent()))
                    {
                        task.setKey(middle.getCurrent(), element.id);
                    }
                    this.middle.setCurrent(element.id);
                    this.middle.GenerateGraph(element.id, son);
                    middle.setTitle(element.id);
                }else
                {
                    middle.hideMiddle();
                    middle.setCurrent("");
                    state = State.Desktop;
                    desktop.Generate();
                }             
            }            
        }
        private void copyElement(IntPtr ptr , IntPtr data , int type )
        {
            if (data != IntPtr.Zero && ptr != IntPtr.Zero)
            {
                if(GraphReader.Element(data).next != IntPtr.Zero)
                {
                    Information info;
                    info.name = GraphReader.Element(GraphReader.Element(data).next).info.name;
                    info.type = GraphReader.Element(GraphReader.Element(data).next).info.type;
                    info.id = GetUniqueId();
                    info.shortcut = "";
                    Kernel.addGraph(ptr, info, 0);
                    copyElement(GraphReader.Element(ptr).next, GraphReader.Element(data).next, 0);
                }

                if (GraphReader.Element(data).son != IntPtr.Zero)
                {
                    Information info;
                    info.name = GraphReader.Element(GraphReader.Element(data).son).info.name;
                    info.type = GraphReader.Element(GraphReader.Element(data).son).info.type;
                    info.id = GetUniqueId();
                    info.shortcut = "";
                    Kernel.addGraph(ptr, info, 1);
                    copyElement(GraphReader.Element(ptr).son, GraphReader.Element(data).son, 1);
                }
            }
        }
        private void click_Item(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            if(Items.Any(x => x.Item1 == (string)item.Header))
            {
                var element = Items.First(x => x.Item1 == (string)item.Header);                    
                this.ShowElement(index[Items.IndexOf(element)]);
            }
        }

        public void ShowElement(int type)
        {
            currentItem = type;

            var image = new BitmapImage(new Uri($"resources/{type}.png", UriKind.Relative));
            if (newImage != null)
            {
                canvas.Children.Remove(newImage);
            }
            newImage = new Image
            {
                Source = image,
                Width = image.Width,
                Height = image.Height
            };
            if(state == State.Desktop)
            {
                if (desktop.x + (int)newImage.Width + 15 > FileManager.Graphics.Window.Width)
                {
                    Canvas.SetTop(newImage, (int)(newImage.Height + (newImage.Height / 2)));
                    Canvas.SetLeft(newImage, Configuration.startX);

                }
                else
                {
                    Canvas.SetTop(newImage, desktop.y);
                    Canvas.SetLeft(newImage, desktop.x);
                }
            }else
            {
                if (middle.FileElements[middle.getCurrent()].x + (int)newImage.Width + 15 > FileManager.Graphics.Window.Width)
                {
                    Canvas.SetTop(newImage, (int)(newImage.Height + (newImage.Height / 2)));
                    Canvas.SetLeft(newImage, Configuration.startX);

                }
                else
                {
                    Canvas.SetTop(newImage, middle.FileElements[middle.getCurrent()].y);
                    Canvas.SetLeft(newImage, middle.FileElements[middle.getCurrent()].x);
                }
            }
           
            if (newBox != null)
            {
                canvas.Children.Remove(newBox);
            }
            newBox = new TextBox();
            newBox.Width = newImage.Width;
            newBox.KeyDown += keyDown;
            if(state == State.Desktop)
            {
                if (desktop.x + (int)newImage.Width + 15 > FileManager.Graphics.Window.Width)
                {
                    Canvas.SetTop(newBox, (int)(newImage.Height + (newImage.Height / 2) + (newImage.Width + 5)));
                    Canvas.SetLeft(newBox, Configuration.startX);

                }
                else
                {
                    Canvas.SetTop(newBox, desktop.y + (newImage.Width + 5));
                    Canvas.SetLeft(newBox, desktop.x);
                }
                
            }else
            {
                if (middle.FileElements[middle.getCurrent()].x + (int)newImage.Width + 15 > FileManager.Graphics.Window.Width)
                {
                    Canvas.SetTop(newBox, (int)(newImage.Height + (newImage.Height / 2) + (newImage.Width + 5)));
                    Canvas.SetLeft(newBox, Configuration.startX);

                }
                else
                {
                    Canvas.SetTop(newBox, middle.FileElements[middle.getCurrent()].y + (newImage.Width + 5));
                    Canvas.SetLeft(newBox, middle.FileElements[middle.getCurrent()].x);
                }
            }
           
            canvas.Children.Add(newImage);
            canvas.Children.Add(newBox);
        }

        private void keyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                var value = ((TextBox)sender).Text;
                canvas.Children.Remove(newImage);
                newImage.Source = null;
                canvas.Children.Remove(newBox);
                newBox = null;
                Information info;
                info.type = currentItem;
                info.id = GetUniqueId();
                info.shortcut = "";

                if (state == State.Desktop)
                {
                    if (currentItem != -1)
                    {
                        info.name = desktop.nameValid(value);

                        Kernel.addGraph(info, 0);
                        this.desktop.Add(info.name, currentItem, info.id);
                        currentItem = -1;
                    }
                }else
                {
                    if (currentItem != -1)
                    {
                        info.name = middle.nameValid(value);
                        Kernel.addGraph(Kernel.getElement(this.middle.getCurrent()), info, 1);
                        this.middle.Add(info.id, info.type, info.name);
                        currentItem = -1;
                    }
                }

            }

        }
        public static string GetUniqueId()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            return GuidString;
        }
    }
}
