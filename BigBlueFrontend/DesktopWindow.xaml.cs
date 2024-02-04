using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace BigBlue
{
    // you need to add global inputs onto this if you want to be able to kill tasks

    /// <summary>
    /// Interaction logic for DesktopWindow.xaml
    /// </summary>
    public partial class DesktopWindow : BigBlueWindow
    {
        public static bool Enabled = true;
        
        public DesktopWindow(string baseDirectory, XmlNode config)
        {
            InitializeComponent();
            
            this.DataContext = this;

            this.Path = baseDirectory;
            this.ConfigNode = config;
            this.snapshotImageControl = SnapShotRectangle;
            this.videoMe = VideoElement;
            this.videoCanvas = Video;

            BigBlue.NativeMethods.ProvisionRawInputs(this, false);

            //  base.readConfigFile();

            snapshotImageControl = SnapShotRectangle;

            LoadFrontendList();

            //provisionUIAnimations(LayoutRoot, FrontEndContainer);
            ProvisionVideoAnimations();
            
            ItemList.ItemsSource = frontendLists[selectedListGuid].ListItems;

            ItemList.SelectedIndex = 0;

            CompositionTarget.Rendering += OnFrame;

            videoFadeInStopwatch.Start();
        }

        private void SetBackGroundImage()
        {
            if (ConfigNode["desktopbg"] != null)
            {
                string bgPath = ConfigNode["desktopbg"].InnerText;

                if (!string.IsNullOrWhiteSpace(bgPath))
                {
                    if (System.IO.File.Exists(bgPath))
                    {
                        Uri bgUri;
                        if (Uri.TryCreate(bgPath, UriKind.RelativeOrAbsolute, out bgUri))
                        {
                            ImageBrush ib = BigBlue.ImageLoading.loadImageBrushFromUri(bgUri, null, null);
                            LayoutRoot.Background = ib;
                        }
                    }
                }
            }
        }

        internal override void ReadConfigFile()
        {
            if (this.ConfigNode != null)
            {
                if (ConfigNode["title"] != null)
                {
                    string windowTitle = ConfigNode["title"].InnerText;

                    if (!string.IsNullOrWhiteSpace(windowTitle))
                    {
                        this.Title = ConfigNode["title"].InnerText;
                    }
                    else
                    {
                        this.Title = DEFAULT_FRONTEND_NAME;
                    }
                }
                else
                {
                    this.Title = DEFAULT_FRONTEND_NAME;
                }

                if (ConfigNode["desktopwidth"] != null)
                {
                    double windowWidth;
                    if (double.TryParse(ConfigNode["desktopwidth"].InnerText, out windowWidth))
                    {
                        this.Width = windowWidth;
                    }
                }

                if (ConfigNode["desktopheight"] != null)
                {
                    double windowHeight;
                    if (double.TryParse(ConfigNode["desktopheight"].InnerText, out windowHeight))
                    {
                        this.Height = windowHeight;
                    }
                }

                if (ConfigNode["desktopitemfont"] != null)
                {
                    string itemFontFamily = ConfigNode["desktopitemfont"].InnerText;

                    if (!string.IsNullOrWhiteSpace(itemFontFamily))
                    {
                        ItemList.FontFamily = new FontFamily(itemFontFamily);
                    }
                }

                if (ConfigNode["desktoplistfont"] != null)
                {
                    string listFontFamily = ConfigNode["desktoplistfont"].InnerText;

                    if (!string.IsNullOrWhiteSpace(listFontFamily))
                    {
                        ItemTree.FontFamily = new FontFamily(listFontFamily);
                    }
                }

                if (ConfigNode["desktoplistfontsize"] != null)
                {
                    double listFontSize;
                    if (double.TryParse(ConfigNode["desktoplistfontsize"].InnerText, out listFontSize))
                    {
                        ItemTree.FontSize = listFontSize;
                    }
                }

                if (ConfigNode["desktopitemfontsize"] != null)
                {
                    double itemFontSize;
                    if (double.TryParse(ConfigNode["desktopitemfontsize"].InnerText, out itemFontSize))
                    {

                        ItemList.FontSize = itemFontSize;
                    }
                }

                if (ConfigNode["desktopsearchfont"] != null)
                {
                    string searchFontFamily = ConfigNode["desktopsearchfont"].InnerText;

                    if (!string.IsNullOrWhiteSpace(searchFontFamily))
                    {
                        SearchTextBox.FontFamily = new FontFamily(searchFontFamily);
                    }
                }

                if (ConfigNode["desktopsearchfontsize"] != null)
                {
                    double searchFontSize;
                    if (double.TryParse(ConfigNode["desktopsearchfontsize"].InnerText, out searchFontSize))
                    {
                        SearchTextBox.FontSize = searchFontSize;
                    }
                }


                SetBackGroundImage();
                
                bool.TryParse(ConfigNode["globalinputs"]?.InnerText, out globalInputs);
                
                // set the MediaElement's volume for videos

                bool.TryParse(ConfigNode["loopvideo"]?.InnerText, out loopVideo);
                double.TryParse(ConfigNode["videovolume"]?.InnerText, out wmpVolume);

                // don't let ridiculous values in there for the volume
                // if it's greater than 1, we'll force it to 1/100%
                if (wmpVolume > 1)
                {
                    wmpVolume = 1;
                }

                // if it's less than 0, we'll force it to 0
                if (wmpVolume < 0)
                {
                    wmpVolume = 0;
                }

                originalWmpVolume = wmpVolume;

                marqueeDisplayName = ConfigNode["marqueedisplay"]?.InnerText;

                flyerDisplayName = ConfigNode["flyerdisplay"]?.InnerText;

                instructionDisplayName = ConfigNode["instructiondisplay"]?.InnerText;
                
                int.TryParse(ConfigNode["inputdelayonlaunch"]?.InnerText, out launchInputDelay);
            }
        }
        

        internal override void ProcessFrontendAction(string action, bool? inputDown)
        {
            if (SearchTextBox.IsKeyboardFocused)
            {
                return;
            }

            //SearchTextBox.Focus() || 
            // if we've launched a game, and the action isn't the exit key or one of the volume controls, we really don't even want to try processing anything
            if (itsGoTime == true && action != "BIG_BLUE_EXIT" && action != "RAMPAGE_EXIT" && action != "RTYPE_EXIT" && action != "BIG_BLUE_MUTE" && action != "BIG_BLUE_VOLUME_UP" && action != "BIG_BLUE_VOLUME_DOWN")
            {
                ReleaseInput(action);

                return;
            }

            switch (action)
            {
                case "BIG_BLUE_EXIT":
                case "RAMPAGE_EXIT":
                case "RTYPE_EXIT":
                    if (globalInputs == true && frontendLists[selectedListGuid].ListItems[selectedListItemIndex].KillTask == true && itsGoTime == true)
                    {
                        BigBlue.ProcessHandling.CloseAllProcesses(processes);

                        //processName = string.Empty;    
                        //processId = -1;

                        ReturnFromGame(FrontEndContainer);
                    }
                    break;
                case "BIG_BLUE_MUTE":
                    if (inputDown == true)
                    {
                        BigBlue.NativeMethods.SendMessageW(windowHandle, BigBlue.NativeMethods.WM_APPCOMMAND, windowHandle, (IntPtr)BigBlue.NativeMethods.APPCOMMAND_VOLUME_MUTE);
                    }
                    break;
                case "BIG_BLUE_VOLUME_UP":
                    BigBlue.NativeMethods.SendMessageW(windowHandle, BigBlue.NativeMethods.WM_APPCOMMAND, windowHandle, (IntPtr)BigBlue.NativeMethods.APPCOMMAND_VOLUME_UP);
                    break;
                case "BIG_BLUE_VOLUME_DOWN":
                    BigBlue.NativeMethods.SendMessageW(windowHandle, BigBlue.NativeMethods.WM_APPCOMMAND, windowHandle, (IntPtr)BigBlue.NativeMethods.APPCOMMAND_VOLUME_DOWN);
                    break;
            }
        }

        private void ColorizeTreeViewItems(Models.FrontendListItem fli)
        {
            //MessageBox.Show(fli.Title + ": " + fli.ID.ToString());

            if (fli.ChildID == selectedListGuid)
            {
                fli.Selected = true;
                
            }
            else
            {
                fli.Selected = false;
            }
            

            if (fli.ListItems != null)
            {
                foreach (Models.FrontendListItem fli2 in fli.ListItems)
                {
                    ColorizeTreeViewItems(fli2);
                }
            }
        }

        private void TextSearch()
        {
            MatchingSearchWords.Clear();

            if (selectedListGuid != searchListGuid)
            {
                frontendLists[searchListGuid].ParentID = selectedListGuid;
                frontendLists[searchListGuid].IndexOfParent = ItemList.SelectedIndex;
            }

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string[] words = SearchTextBox.Text.Split(' ');

                foreach (string word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        MatchingSearchWords.Add(word);
                    }
                }
            }
            else
            {
                if (selectedListGuid == searchListGuid)
                {
                    
                    selectedListGuid = (Guid)frontendLists[searchListGuid].ParentID;

                    ColorizeTreeViewItems(treeViewListItems[0]);

                }
                

                ItemList.ItemsSource = frontendLists[selectedListGuid].ListItems;
                ItemList.SelectedIndex = frontendLists[selectedListGuid].CurrentListIndex;
            }

            //if (!string.IsNullOrWhiteSpace(searchCommand))
            if (MatchingSearchWords.Count() > 0)
            {
                //searchCommand = searchCommand.ToLowerInvariant();

                bool searchMatch = false;

                foreach (BigBlue.Models.FrontendListItem l in frontendLists.Values)
                {
                    // you have to exclude the actual search list otherwise you're just going to keep getting more and more duplicates
                    if (l.ListItems != null && l.ID != searchListGuid)
                    {
                        foreach (BigBlue.Models.FrontendListItem li in l.ListItems)
                        {

                            foreach (string searchCommand in MatchingSearchWords)
                            {
                                // instead of this duplicate crap, it needs to actually do a comparison on multiple values
                                // it needs to check if the title and exe are the same
                                // if they are, screw that crap

                                //frontendLists[searchListGuid].ListItems = frontendLists[searchListGuid].ListItems.OrderByDescending(n => n.ChildID.HasValue).ThenBy(o => o.Title).ToList();


                                //&& string.IsNullOrWhiteSpace(li.ChildFolder) 
                                if (!string.IsNullOrWhiteSpace(li.SearchTitle))
                                {
                                    if (li.SearchTitle.IndexOf(searchCommand, StringComparison.InvariantCultureIgnoreCase) >= 0 && string.IsNullOrWhiteSpace(li.ParentFolder))
                                    {

                                        if (!searchMatch)
                                        {
                                            frontendLists[searchListGuid].ListItems.Clear();
                                            searchMatch = true;
                                        }

                                        // observable collection experiment
                                        //var duplicates = bbWindow.frontendLists[bbWindow.searchListGuid].ListItems.Find(n => n.SearchTitle == li.SearchTitle && n.Binary == li.Binary);

                                        IEnumerable<BigBlue.Models.FrontendListItem> duplicates = frontendLists[searchListGuid].ListItems.Where(n => n.SearchTitle == li.SearchTitle && n.Binary == li.Binary);

                                        if (duplicates.Count() == 0)
                                        {
                                            frontendLists[searchListGuid].ListItems.Add(li);

                                            //searchListItems.Add(li);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                int itemCount = frontendLists[searchListGuid].ListItems.Count();

                if (itemCount > 0)
                {
                    frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;
                                        
                    //observable collection experiment
                    //bbWindow.frontendLists[bbWindow.searchListGuid].ListItems = bbWindow.frontendLists[bbWindow.searchListGuid].ListItems.OrderByDescending(n => n.ChildID.HasValue).ThenBy(o => o.Title).ToList();
                    frontendLists[searchListGuid].ListItems = new ObservableCollection<Models.FrontendListItem>(frontendLists[searchListGuid].ListItems.Where(x => x.Title != "").OrderByDescending(n => n.ChildID.HasValue).ThenBy(o => o.Title));


                    selectedListItemIndex = 0;
                    
                    
                    selectedListGuid = searchListGuid;
                    ColorizeTreeViewItems(treeViewListItems[0]);


                    frontendLists[searchListGuid].CurrentListIndex = 0;

                    if (textBlockListItemsToPage > itemCount)
                    {
                        frontendLists[searchListGuid].TextBlockItemsToPage = 1;
                    }
                    else
                    {
                        frontendLists[searchListGuid].TextBlockItemsToPage = textBlockListItemsToPage;
                    }

                    if (imageBlockListItemsToPage > itemCount)
                    {
                        frontendLists[searchListGuid].ImageItemsToPage = 1;
                    }
                    else
                    {
                        frontendLists[searchListGuid].ImageItemsToPage = imageBlockListItemsToPage;
                    }

                    frontendLists[searchListGuid].Total = itemCount - 1;

                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        SelectList(searchListGuid, false);

                        Finished();

                    }, System.Windows.Threading.DispatcherPriority.Render, null);

                    ItemList.ItemsSource = frontendLists[searchListGuid].ListItems;
                    ItemList.SelectedIndex = 0;

                    return;
                }
            }

            
            //Dispatcher.BeginInvoke((Action)async delegate
            //{
                //selectList(searchListGuid, false);
                SetGameSnapshots(false);
                ResetVideo();
                //awaitingAsync = false;
            //}, System.Windows.Threading.DispatcherPriority.Render, null);
            
        }

        override public void OnFrame(object sender, EventArgs e)
        {            
            ManageVideo();
        }

        private void BuildTreeViewList(Guid listGuid, ObservableCollection<BigBlue.Models.FrontendListItem> treeViewListItems)
        {
            int listIndex = 0;

            foreach (BigBlue.Models.FrontendListItem item in frontendLists[listGuid].ListItems)
            {
                if (item.ChildID != null)
                {
                    BigBlue.Models.FrontendListItem itemToAdd = new Models.FrontendListItem();
                    
                    itemToAdd.ID = listGuid;

                    //itemToAdd.SelectedID = (Guid)item.ChildID;
                    //itemToAdd.ID = (Guid)item.ChildID;
                    itemToAdd.ChildID = item.ChildID;
                    
                                        
                    if (item.ParentID == null)
                    {
                        itemToAdd.ParentID = rootListGuid;
                    }
                    else
                    {
                        itemToAdd.ParentID = item.ParentID;
                    }
                    
                    itemToAdd.IndexOfParent = listIndex;
                    
                    itemToAdd.FolderSearchPattern = item.FolderSearchPattern;
                    
                    itemToAdd.Title = item.Title;
                    itemToAdd.ChildFolder = item.ChildFolder;
                    itemToAdd.ParentFolder = item.ParentFolder;
                    
                                        
                    // MessageBox.Show(item.Title);

                    if (frontendLists[(Guid)item.ChildID] != null)
                    {
                        if (frontendLists[(Guid)item.ChildID].ListItems != null)
                        {
                            
                            itemToAdd.ListItems = new ObservableCollection<Models.FrontendListItem>();
                            BuildTreeViewList((Guid)item.ChildID, itemToAdd.ListItems);
                        }
                    }
                    

                    treeViewListItems.Add(itemToAdd);
                }

                listIndex = listIndex + 1;
            }
        }
                
        private async void BackSelect()
        {
            frontendLists[selectedListGuid].CurrentListIndex = ItemList.SelectedIndex;

            if (screenSaver == false && menuOpen == false && awaitingAsync == false && itsGoTime == false && (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentID != null || !string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder) || selectedListGuid == searchListGuid))
            {
                bool subFolderList = false;

                string originalParentFolder = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder;

                if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentID != null)
                {
                    subFolderTrail.Clear();

                    Guid parentGuid = (Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentID;

                    // when we're going back, let's set the current index of the list to be whatever it was at that time
                    frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;


                    selectedListItemIndex = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].IndexOfParent;

                    SelectList(parentGuid, true);
                }
                else
                {
                    //!string.IsNullOrWhiteSpace(originalParentFolder) && 
                    if (subFolderTrail.Count > 0)
                    {
                        subFolderTrail.RemoveAt(subFolderTrail.Count - 1);
                    }

                    bool returnToOriginatingList = false;

                    if (selectedListGuid == searchListGuid)
                    {
                        returnToOriginatingList = true;
                    }

                    //if (!string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder))
                    if (subFolderTrail.Count > 0 && !returnToOriginatingList)
                    {
                        subFolderList = true;
                        //frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder

                        Loading();

                        //GameList.Visibility = Visibility.Hidden;
                        SetGameSnapshots(true);

                        bool validList = await Task.Run(() => { return OpenSubFolderListItem(subFolderTrail[subFolderTrail.Count - 1], folderListGuid); });

                        // subFolderTrail.RemoveAt(subFolderTrail.Count - 1);

                        if (validList)
                        {
                            RenderSubFolderList(videoMe, videoCanvas);
                        }
                        else
                        {
                            returnToOriginatingList = true;
                            //subFolderTrail.RemoveAt(subFolderTrail.Count - 1);
                            // this bullcrap needs to check to make sure that the originating folder list even still exists, and if it doesn't, grab its parent

                        }

                        Finished();
                        //GameList.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        returnToOriginatingList = true;
                        //selectList(originatingFolderListGuid);
                    }

                    if (returnToOriginatingList)
                    {
                        subFolderTrail.Clear();

                        if (originatingFolderListGuid == Guid.Empty || selectedListGuid == searchListGuid)
                        {
                            selectedListItemIndex = frontendLists[rootListGuid].CurrentListIndex;
                            SelectList(rootListGuid, true);
                        }
                        else
                        {
                            Guid? fallbackGuid = frontendLists[originatingFolderListGuid].ListItems[0].ParentID;

                            if (fallbackGuid != null)
                            {
                                // when we're going back, let's set the current index of the list to be whatever it was at that time
                                frontendLists[(Guid)fallbackGuid].CurrentListIndex = selectedListItemIndex;

                                //selectedListItemIndex = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].IndexOfParent;

                                //selectedListItemIndex = frontendLists[(Guid)fallbackGuid].CurrentListIndex;

                                SelectList((Guid)fallbackGuid, true);
                            }
                        }
                    }
                }

                ItemList.ItemsSource = frontendLists[selectedListGuid].ListItems;
                ItemList.SelectedIndex = selectedListItemIndex;

                if (!subFolderList)
                {
                    ColorizeTreeViewItems(treeViewListItems[0]);
                }
            }
        }

        private void Loading()
        {
            awaitingAsync = true;
            Mouse.OverrideCursor = Cursors.Wait;
            SearchTextBox.IsEnabled = false;
            ItemTree.IsEnabled = false;
            ItemList.IsEnabled = false;
            BackButton.IsEnabled = false;
            PlayButton.IsEnabled = false;
            BackButton.Opacity = 0.5;
            PlayButton.Opacity = 0.5;
        }

        private void Finished()
        {
            awaitingAsync = false;
            Mouse.OverrideCursor = null;
            SearchTextBox.IsEnabled = true;
            ItemTree.IsEnabled = true;
            ItemList.IsEnabled = true;
            BackButton.IsEnabled = true;
            PlayButton.IsEnabled = true;
            BackButton.Opacity = 1;
            PlayButton.Opacity = 1;
        }

        private async void TreeViewSelect()
        {
            if (awaitingAsync == false && itsGoTime == false)
            {
                if (!string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder))
                {
                    //xaudioPlayer.PlaySound(SelectListSoundKey, string.Empty);

                    bool returnToOriginatingList = false;

                    string childFolderName = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder;


                    if (!string.IsNullOrWhiteSpace(childFolderName))
                    {
                        subFolderTrail.Add(childFolderName);
                    }

                    if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID == null)
                    {
                        Loading();

                        SetGameSnapshots(true);

                        if (textBlockListCanvas != null)
                        {
                            textBlockListCanvas.Visibility = Visibility.Hidden;
                        }

                        bool validList = await Task.Run(() => { return OpenSubFolderListItem(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder, folderListGuid); });

                        if (validList)
                        {
                            RenderSubFolderList(videoMe, videoCanvas);
                        }
                        else
                        {
                            returnToOriginatingList = true;
                        }

                        Finished();
                        
                        if (textBlockListCanvas != null)
                        {
                            textBlockListCanvas.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;

                        Guid lg = (Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID;

                        originatingFolderParentListItem = frontendLists[selectedListGuid].ListItems[selectedListItemIndex];
                        originatingFolderListGuid = lg;

                        Loading();

                        SetGameSnapshots(true);

                        if (textBlockListCanvas != null)
                        {
                            textBlockListCanvas.Visibility = Visibility.Hidden;
                        }

                        bool validList = await Task.Run(() => { return OpenFolderListItem(lg); });

                        if (validList)
                        {
                            RenderFolderList(lg);
                        }
                        else
                        {
                            returnToOriginatingList = true;
                        }

                        Finished();

                        if (textBlockListCanvas != null)
                        {
                            textBlockListCanvas.Visibility = Visibility.Visible;
                        }
                    }

                    if (returnToOriginatingList)
                    {
                        subFolderTrail.Clear();

                        if (frontendLists[originatingFolderListGuid].ListItems != null)
                        {
                            // can't do anything if there are 0 items
                            if (frontendLists[originatingFolderListGuid].ListItems.Count > 0)
                            {
                                Guid? fallbackGuid = frontendLists[originatingFolderListGuid].ListItems[0].ParentID;

                                if (fallbackGuid != null)
                                {
                                    SelectList((Guid)fallbackGuid, false);
                                }
                            }
                            else
                            {
                                // this is a last resort in case something really horrible goes wrong, but the count should always be greater than 0
                                SelectList(selectedListGuid, false);
                            }
                        }
                    }
                }
                else if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID != null)
                {
                    //xaudioPlayer.PlaySound(SelectListSoundKey, string.Empty);

                    frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;

                    SelectList((Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID, false);

                }

                SearchTextBox.Text = string.Empty;

                ItemList.ItemsSource = frontendLists[selectedListGuid].ListItems;
                ItemList.SelectedIndex = frontendLists[selectedListGuid].CurrentListIndex;
            }
        }

        private async void ListBoxSelect()
        {
            if (awaitingAsync == false && itsGoTime == false)
            {
                bool refreshList = false;
                bool subFolderList = false;

                if (!string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder))
                {
                    //xaudioPlayer.PlaySound(SelectListSoundKey, string.Empty);

                    bool returnToOriginatingList = false;

                    string childFolderName = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder;


                    if (!string.IsNullOrWhiteSpace(childFolderName))
                    {
                        subFolderTrail.Add(childFolderName);
                    }

                    if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID == null)
                    {
                        subFolderList = true;

                        Loading();

                        SetGameSnapshots(true);
                        
                        bool validList = await Task.Run(() => { return OpenSubFolderListItem(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder, folderListGuid); });

                        if (validList)
                        {
                            RenderSubFolderList(videoMe, videoCanvas);
                        }
                        else
                        {
                            returnToOriginatingList = true;
                        }

                        Finished();
                    }
                    else
                    {
                        frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;

                        Guid lg = (Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID;

                        originatingFolderParentListItem = frontendLists[selectedListGuid].ListItems[selectedListItemIndex];
                        originatingFolderListGuid = lg;

                        Loading();

                        SetGameSnapshots(true);
                        
                        bool validList = await Task.Run(() => { return OpenFolderListItem(lg); });

                        if (validList)
                        {
                            RenderFolderList(lg);
                        }
                        else
                        {
                            returnToOriginatingList = true;
                        }

                        Finished();
                    }

                    if (returnToOriginatingList)
                    {
                        subFolderTrail.Clear();

                        if (frontendLists[originatingFolderListGuid].ListItems != null)
                        {
                            // can't do anything if there are 0 items
                            if (frontendLists[originatingFolderListGuid].ListItems.Count > 0)
                            {
                                Guid? fallbackGuid = frontendLists[originatingFolderListGuid].ListItems[0].ParentID;

                                if (fallbackGuid != null)
                                {
                                    SelectList((Guid)fallbackGuid, false);
                                }
                            }
                            else
                            {
                                // this is a last resort in case something really horrible goes wrong, but the count should always be greater than 0
                                SelectList(selectedListGuid, false);
                            }
                        }
                    }

                    refreshList = true;
                }
                else if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID != null)
                {
                    //xaudioPlayer.PlaySound(SelectListSoundKey, string.Empty);

                    frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;

                    SelectList((Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID, false);

                    refreshList = true;
                }
                else
                {
                    if (ValidateProgramLaunch())
                    {
                        itsGoTime = true;
                        ReleaseAllInputs();

                        // disable inputs if you've got 'em
                        if (launchInputDelay > 0)
                        {
                            DisableKeys(this);
                        }
                        
                        // play the select sound
                        videoMe.Volume = 0;
                                                
                        BeginGameLaunch();
                    }

                    refreshList = false;
                }
                
                if (refreshList)
                {
                    SearchTextBox.Text = string.Empty;

                    ItemList.ItemsSource = frontendLists[selectedListGuid].ListItems;
                    ItemList.SelectedIndex = frontendLists[selectedListGuid].CurrentListIndex;
                    //ItemList.SelectedIndex = 0;

                    if (!subFolderList)
                    {
                        ColorizeTreeViewItems(treeViewListItems[0]);
                    }
                }
            }
        }

        internal override void BackToGameList()
        {
            base.BackToGameList();
            
            CompositionTarget.Rendering += OnFrame;
            
            videoMe.Volume = minimumVolume;

            this.Activate();
            //FrontEndWindow.Topmost = true;

            itsGoTime = false;

            Finished();

            StopVideo();
            ResetVideo();
            
            //ShowFrontEndStoryboard.Begin();
            isVideoPlaying = false;
        }

        private void ResumeVideo()
        {
            // if (categorySelection == false && shutdownSequenceActivated == false && videoUrls != null)
            if (shutdownSequenceActivated == false)
            {
                if (videoFadeInStopwatch.ElapsedMilliseconds > 0)
                {
                    videoFadeInStopwatch.Start();
                }

                if (videoMe.Source != null)
                {
                    if (videoFadeOutStopwatch.ElapsedMilliseconds > 0)
                    {
                        videoFadeOutStopwatch.Start();
                    }

                    ClockState fadeInState = videoFadeInStoryboard.GetCurrentState();

                    switch (fadeInState)
                    {
                        case ClockState.Active:
                            break;
                        case ClockState.Filling:
                            break;
                        case ClockState.Stopped:
                            if (isVideoPlaying == true)
                            {
                                videoMe.Play();
                                PlayMediaElement();
                            }
                            break;
                    }

                    if (DependencyPropertyHelper.GetValueSource(videoCanvas, Canvas.OpacityProperty).IsAnimated)
                    {
                        if (videoMaterializing == true)
                        {
                            if (videoFadeInStoryboard.GetIsPaused() == true)
                            {
                                videoFadeInStoryboard.Resume();
                            }
                        }

                        if (videoFading == true)
                        {
                            if (videoFadeOutStoryboard.GetIsPaused() == true)
                            {
                                videoFadeOutStoryboard.Resume();
                            }

                        }
                    }

                    if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Video != null)
                    {
                        videoMe.Play();
                    }
                }
            }
        }

        internal override void BeginGameLaunch()
        {
            try
            {
                //releaseAllInputs();

                StopVideo();
                Loading();

                CompositionTarget.Rendering -= OnFrame;
                                
                LaunchProgram(FrontEndContainer, directoryToLaunch, fileNameToLaunch, argumentsToLaunch, lt);
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    ResetVideo();
                });
            }
        }

        private void TreeViewList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                SelectTreeViewListItem(sender, e);
            }

            e.Handled = true;
        }
        


        private bool SelectTreeViewListItem(object sender, System.Windows.Input.InputEventArgs e)
        {
            TreeViewItem tviSender = sender as TreeViewItem;
            
            if (tviSender.IsSelected)
            {
                BigBlue.Models.FrontendListItem i = tviSender.Header as BigBlue.Models.FrontendListItem;

                //frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;

                if (selectedListGuid != i.ChildID)
                {
                    selectedListGuid = (Guid)i.ChildID;

                    ColorizeTreeViewItems(treeViewListItems[0]);

                    selectedListGuid = i.ID;

                    if (i.ParentID == null && i.ID == rootListGuid)
                    {
                        ItemList.ItemsSource = frontendLists[rootListGuid].ListItems;
                        ItemList.SelectedIndex = 0;
                    }
                    else
                    {
                        selectedListItemIndex = i.IndexOfParent;
                        TreeViewSelect();
                    }
                }

                



                //ItemList.ItemsSource = frontendLists[selectedListGuid].ListItems;
                //ItemList.SelectedIndex = 0;

                e.Handled = true;
            }

            return false;
        }

        private void TreeViewList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectTreeViewListItem(sender, e);
        }

        ObservableCollection<BigBlue.Models.FrontendListItem> treeViewListItems = null;

        internal override void InitializeFrontEnd()
        {
            base.InitializeFrontEnd();

            ObservableCollection<BigBlue.Models.FrontendListItem> rootItems = frontendLists[rootListGuid].ListItems;

            treeViewListItems = new ObservableCollection<Models.FrontendListItem>();

            BigBlue.Models.FrontendListItem rootItem = new Models.FrontendListItem();
            rootItem.ID = rootListGuid;
            rootItem.ChildID = rootListGuid;
            rootItem.ListName = "Home";
            rootItem.Title = "Home";
            rootItem.ListItems = new ObservableCollection<Models.FrontendListItem>();
            rootItem.Selected = true;

            BuildTreeViewList(rootListGuid, rootItem.ListItems);

            treeViewListItems.Add(rootItem);

            ItemTree.ItemsSource = treeViewListItems;
        }

        private void CreateVideoControl()
        {
            MediaElement me = new MediaElement();
            me.Opacity = 0;
            me.Width = snapShotWidth;
            me.Height = snapShotHeight;
            me.IsHitTestVisible = false;
            me.Stretch = Stretch.Fill;
            me.MediaFailed += VideoElement_MediaFailed;
            me.LoadedBehavior = MediaState.Manual;
            //me.MediaEnded += gameVideo_MediaEnded;

            VideoElement = me;
        }

        private void ResetVideoControl()
        {
            StopVideo();
            videoMe = null;
            //Video.Children.Remove(VideoElement);

            CreateVideoControl();
            videoFadeInStopwatch.Start();
        }

        private void VideoElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //MessageBox.Show(e.ErrorException.Message);
            ResetVideoControl();
        }

        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BigBlue.Models.FrontendListItem item = ItemList.SelectedItem as BigBlue.Models.FrontendListItem;

            if (ItemList.SelectedIndex != -1)
            {
                selectedListItemIndex = ItemList.SelectedIndex;

                frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;

                SetGameSnapshots(false);

                StopVideo();
                ResetVideo();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextSearch();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemList.SelectedIndex != -1)
            {
                BackSelect();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemList.SelectedIndex != -1)
            {
                ListBoxSelect();
            }
        }

        private void ItemList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (e.ClickCount % 2 == 0)
            {
                ListBoxSelect();
            }
        }

        private void ItemList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine(e.ClickCount.ToString());

            if (e.ChangedButton == MouseButton.XButton1 && e.ButtonState == MouseButtonState.Pressed)
            {
                BackSelect();
            }

            if (e.ChangedButton == MouseButton.XButton2 && e.ButtonState == MouseButtonState.Pressed)
            {
                ListBoxSelect();
            }

            //e.Handled = true;
        }
    }
}
