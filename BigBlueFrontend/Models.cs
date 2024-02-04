using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace BigBlue
{
    class Models
    {
        public enum ListType
        {
            Image,
            Text
        }

        public enum LaunchType
        {
            pre,
            main,
            post,
            shell,
            desktop
        }

        public enum SurroundPosition
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        public enum FrontEndExitMode
        {
            quit,
            restart,
            shutdown
        }
        
        public class ImageAnimation
        {
            public Rectangle Image { get; set; }

            public List<ImageBrush> Frames { get; set; }

            public int CurrentFrame { get; set; }

            public int TotalFrames { get; set; }

            public long Speed { get; set; }

            public System.Diagnostics.Stopwatch Timer { get; set; }
        }

        public class ListItemBlock : INotifyPropertyChanged
        {
            private string name;

            // Declare the event
            public event PropertyChangedEventHandler PropertyChanged;

            public ListItemBlock()
            {
            }

            public ListItemBlock(string value)
            {
                this.name = value;
            }

            public string Name
            {
                get { return name; }

                set
                {
                    name = value;

                    // Call OnPropertyChanged whenever the property is updated
                    OnPropertyChanged("Name");
                }
            }

            // Create the OnPropertyChanged method to raise the event
            protected void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public class FrontendInputState
        {
            public bool isPressed;
            public bool wasPressed;
            public bool isRepeating;

            public FrontendInputState(bool currentPress, bool lastPress, bool repeating)
            {
                isPressed = currentPress;
                wasPressed = lastPress;
                isRepeating = repeating;
            }
        }

        public class FrontendSnapshot
        {
            public Uri Path { get; set; }
            
            public string Type { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public System.Windows.Controls.Image ImageControl { get; set; }
        }

        public class FrontendListItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public Guid ID { get; set; }

            public int Total { get; set; }

            public string ListName { get; set; }

            public int CurrentListIndex { get; set; }

            public int TextBlockItemsToPage { get; set; }

            public int ImageItemsToPage { get; set; }

            public ObservableCollection<FrontendListItem> ListItems { get; set; }

            public Guid? ParentID { get; set; }

            public int IndexOfParent { get; set; }

            public Guid? ChildID { get; set; }

            public string FolderSearchPattern { get; set; }

            public string ParentFolder { get; set; }

            public string ChildFolder { get; set; }

            public string Title { get; set; }

            public string SearchTitle { get; set; }

            public string PreBinary { get; set; }

            public string Binary { get; set; }

            public string PostBinary { get; set; }

            public string PreDirectory { get; set; }

            public string Directory { get; set; }

            public string PostDirectory { get; set; }

            public string PreArguments { get; set; }

            public string Arguments { get; set; }

            public string PostArguments { get; set; }

            public bool KillTask { get; set; }

            public List<FrontendSnapshot> Snapshots { get; set; }

            public Uri Video { get; set; }

            public string SnapTemplate { get; set; }

            public string MarqueeTemplate { get; set; }

            public string InstructionTemplate { get; set; }

            public string FlyerTemplate { get; set; }

            public string VideoTemplate { get; set; }

            private bool _selected;
            public bool Selected
            {
                get { return _selected; }
                set { _selected = value; OnPropertyChanged(); }
            }       
        }
    }
}
