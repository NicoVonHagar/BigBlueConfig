using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace BigBlue
{
    public class BigBlueWindow : System.Windows.Window
    {
        internal bool openExplorerOnQuit = true;

        internal const string DEFAULT_FRONTEND_NAME = "Big Blue Frontend";

        internal bool hideWindowsErrorReportUI = false;

        internal bool hideWindowControls = false;

        internal BigBlue.Models.ListItemBlock currentListName = new Models.ListItemBlock();

        internal Uri notFoundImageUri;

        internal int imageListSelectedWidth;
        internal int imageListUnselectedWidth;

        internal const string MusicSoundKey = "bgmusic";
        internal const string ListNavigationSoundKey = "listmove";
        internal readonly string SelectListSoundKey = "listselect";
        internal const string LaunchListItemSoundKey = "launchprogram";
        internal const string ExitListSoundKey = "listback";
        internal const string FailureSoundKey = "failure";

        internal Random r = new Random();

        internal Storyboard LaunchGameStoryboard = new Storyboard();
        internal Storyboard ShowFrontEndStoryboard = new Storyboard();
        internal Storyboard ScreensaverStartStoryboard = new Storyboard();
        internal Storyboard ScreensaverEndStoryboard = new Storyboard();
        
        internal bool showClock = false;

        internal Image snapshotImageControl = null;

        internal MediaElement videoMe = null;

        internal System.Windows.Controls.Panel videoCanvas = null;

        internal Canvas textBlockListCanvas = null;
        internal Canvas imageBlockListCanvas = null;

        internal bool shutdownSequenceActivated = false;

        internal List<string> MatchingSearchWords = new List<string>();

        internal HwndSource win32Window;

        internal bool frontendShown = false;

        internal IntPtr windowHandle;

        internal bool gameLaunchSoundFinished = false;

        internal BitmapImage speechThumbnailImage;

        internal BitmapImage versusThumbnailImage;

        internal string marqueeDisplayName = string.Empty;
        internal string flyerDisplayName = string.Empty;
        internal string instructionDisplayName = string.Empty;

        internal Storyboard videoFadeOutStoryboard;
        internal Storyboard videoFadeInStoryboard;

        internal Stopwatch screenSaverTimer = new Stopwatch();

        internal double configResolutionX = 0;
        internal double configResolutionY = 0;

        internal TimeSpan oneSecondTimeSpan = TimeSpan.FromSeconds(0);

        internal Stopwatch videoFadeInStopwatch = new Stopwatch();
        internal Stopwatch videoFadeOutStopwatch = new Stopwatch();

        internal BigBlue.SecondaryWindow marqueeWindow;
        internal BigBlue.SecondaryWindow flyerWindow;
        internal BigBlue.SecondaryWindow instructionWindow;

        internal double videoLength = 0;

        internal Dictionary<string, BigBlue.Models.FrontendInputState> frontendInputs = new Dictionary<string, BigBlue.Models.FrontendInputState>();

        internal List<TextBlock> gameListElements = new List<TextBlock>();
        internal List<BigBlue.Models.ListItemBlock> listItemBlocks = new List<BigBlue.Models.ListItemBlock>();

        internal List<System.Windows.Shapes.Shape> listItemImageBlocks = new List<System.Windows.Shapes.Shape>();

        internal List<string> voiceChoices = new List<string>();

        internal Guid folderListGuid = Guid.NewGuid();
        internal Guid searchListGuid = Guid.NewGuid();

        internal Dictionary<Guid, BigBlue.Models.FrontendListItem> frontendLists = new Dictionary<Guid, BigBlue.Models.FrontendListItem>();

        internal Guid rootListGuid;

        internal SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();

        internal Dictionary<SharpDX.DirectInput.Joystick, Dictionary<string, string>> directInputDevices = new Dictionary<SharpDX.DirectInput.Joystick, Dictionary<string, string>>();
        internal Dictionary<IntPtr, Dictionary<string, string>> mouseDevices = new Dictionary<IntPtr, Dictionary<string, string>>();
        internal Dictionary<XInputDotNetPure.PlayerIndex, Dictionary<string, string>> xInputDevices = new Dictionary<XInputDotNetPure.PlayerIndex, Dictionary<string, string>>();
        internal Dictionary<IntPtr, Dictionary<ushort, string>> keyboardDictionary = new Dictionary<IntPtr, Dictionary<ushort, string>>();

        internal bool marqueeDisplay = false;
        internal bool flyerDisplay = false;
        internal bool instructionDisplay = false;

        internal BigBlue.Models.ListType listTypePriority = Models.ListType.Text;

        internal Stopwatch gameListTimer = new Stopwatch();

        internal string lastFolder = string.Empty;

        internal List<string> subFolderTrail = new List<string>();

        internal const string allSearchPattern = "*.*";

        internal Guid originatingFolderListGuid = Guid.Empty;

        // save the current and last list ID
        internal Guid selectedListGuid;

        internal double screenSaverTimeInMinutes = 0;

        internal int screenRotation = 0;
        internal int launchInputDelay = 0;

        internal bool itsGoTime = false;
        internal bool menuOpen = false;

        internal string directoryToLaunch = string.Empty;
        internal string fileNameToLaunch = string.Empty;
        internal string argumentsToLaunch = string.Empty;
        internal BigBlue.Models.LaunchType lt = BigBlue.Models.LaunchType.main;

        internal string Path { get; set; }

        internal BigBlue.Models.FrontendListItem originatingFolderParentListItem = null;

        internal List<string> processes = new List<string>();

        internal Stopwatch mouseStopWatch = new Stopwatch();

        internal long mouseMovementSpeed = 180;

        internal bool displayWindowsBorder = false;

        internal bool awaitingAsync = false;

        internal double snapShotWidth;
        internal double snapShotHeight;

        internal int aspectRatioIndex = 0;
        internal int portraitModeIndex = 0;

        internal bool stretchSnapshots = false;

        internal BigBlue.Models.FrontEndExitMode exitMode;

        internal static double width = 0;
        internal static double height = 0;

        internal static int selectedMenuIndex = 0;
        internal int selectedListItemIndex = 0;

        internal double selectedTextSize = 81;
        internal double unselectedTextSize = 27;

        internal int numberOfTextBlockListItems;
        internal int numberOfImageBlockListItems;

        internal const double aspectRatio43 = 1.333333333333333;
        internal const double aspectRatio34 = 0.75;
        internal const double aspectRatio169 = 1.777777777777778;

        internal double individualExtraMargin = 0;
        internal double selectedGameHeight = 0;
        internal double unselectedGameHeight = 0;

        internal double resolutionXMultiplier = 1;
        internal bool integerMultiplier = false;

        internal double listItemHorizontalPadding = 20;
        internal double selectedItemVerticalPadding = 20;
        internal double unselectedItemVerticalPadding = 20;

        internal double gameListMarginX = 20;
        internal double gameListMarginY = 20;

        internal double gameListOverscanX = 0;
        internal double gameListOverscanY = 0;

        internal int textBlockListHalfWayPoint = 0;
        internal int imageBlockListHalfWayPoint = 0;
        internal int textBlockListItemsToPage = 0;
        internal int imageBlockListItemsToPage = 0;

        internal bool stretch = false;
        internal bool cleanStretch = false;
        internal bool music = false;
        internal bool keepAspectRatio = false;


        internal int timeToChange = 400;
        internal int timeToChangeScale = 100;

        internal bool videoFading = false;
        internal bool videoMaterializing = false;
        
        internal bool isVideoPlaying = false;

        internal static double surroundGameListWidthOffset = 0;

        internal static SolidColorBrush whiteBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        internal static Brush blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));

        internal System.Windows.TextAlignment gameListAlignment = TextAlignment.Center;
        // default to arial in case there's NO config
        internal static FontFamily gameListFont = new FontFamily("Arial");

        internal double wmpVolume = 1;
        internal double originalWmpVolume = 1;
        internal float miniGameVolume = 1.0f;

        internal const double videoFadeInDuration = 1000;
        internal const double videoFadeOutDuration = 3000;
        internal const double minimumVideoLength = 1000;

        internal string surroundMonitor2DisplayName = string.Empty;
        internal static BigBlue.Models.SurroundPosition surroundPosition = BigBlue.Models.SurroundPosition.None;

        internal static double mainMenuFontSize = 48;
        internal static double mainMenuPaddingX = 16;
        internal static double mainMenuPaddingY = 16;
        internal static string mainMenuReturnLabel = "Return";
        internal static string mainMenuExitLabel = "Exit";
        internal static string mainMenuShutdownLabel = "Shutdown";
        internal static string mainMenuRestartLabel = "Restart";

        internal bool disableMenu = false;
        internal bool loopVideo = false;
        internal bool antialiasedText = true;

        internal const double minimumVolume = 0.01;

        internal static bool displayExitItemInMenu = true;
        internal static int totalMenuItems = 3;

        internal System.Xml.XmlNode ConfigNode { get; set; }
        
        internal System.Speech.Recognition.SpeechRecognitionEngine recognizer;

        internal BigBlue.NativeMethods.RAWINPUTDEVICE[] registeredInputDevices = null;
        
        internal bool globalInputs = false;

        internal Thickness selectedGamePadding;
        internal Thickness gamePadding;

        internal bool screenSaver = false;
        internal bool hideMouseCursor = false;
        internal bool mouseCursorTrapped = false;
        internal bool pausedBySystem = false;

        internal void StartFrontend()
        {            
            try
            {
                ProvisionSecondaryDisplays();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Secondary Display Error: " + ex.Message, "Big Blue");
            }

            InitializeFrontEnd();
        }

        internal virtual void InitializeFrontEnd()
        {
        }

        internal virtual void InitializeSound()
        {

        }

        internal void ManageVideo()
        {
            if (awaitingAsync == false)
            {
                if (videoLength > 0 && videoFadeOutStopwatch.ElapsedMilliseconds >= (videoLength - videoFadeOutDuration))
                {
                    videoFadeOutStoryboard.Begin();
                    videoFadeOutStopwatch.Stop();
                    videoFadeOutStopwatch.Reset();
                    videoFading = true;
                }

                if (itsGoTime == false)
                {
                    AdjustVideoVolume(videoMe, videoCanvas);
                }

                if (videoFadeInStopwatch.ElapsedMilliseconds >= videoFadeInDuration)
                {
                    Models.FrontendListItem listItem = frontendLists[selectedListGuid];

                    if (listItem.ListItems != null)
                    {
                        if (listItem.ListItems[selectedListItemIndex]?.Video != null && shutdownSequenceActivated == false && isVideoPlaying == false)
                        {
                            if (videoLength == 0)
                            {
                                if (listItem.ListItems[selectedListItemIndex].Video != null)
                                {
                                    // 2017/06/01 experiment; this will clear out the video source and clock
                                    ClearVideo();

                                    // the source is being set here, but media opened isn't called until after you execute Play()
                                    videoMe.Source = listItem.ListItems[selectedListItemIndex].Video;
                                }
                            }

                            // this is being called every frame, but if we ever hit this point, it will execute only once, because the stopwatch is being reset here	
                            videoFadeInStopwatch.Stop();
                            videoFadeInStopwatch.Reset();

                            // if the video length isn't 0, it means that the MediaOpened event already triggered
                            if (videoLength != 0)
                            {
                                // i'm not sure this code is ever even called
                                // this is probably a leftover from when i tried to not reset the source for each loop/replay
                                videoMe.Position = oneSecondTimeSpan;
                                videoMe.Play();

                                // because MediaOpened would have already had to have triggered, you have to call this again manually
                                PlayMediaElement();
                            }
                            else
                            {
                                isVideoPlaying = true;
                                videoMe.Play();
                                // MediaOpened should trigger after this, which will actually start it off
                            }
                        }
                    }
                }
            }
        }
        internal void HumiliationFinished()
        {
            awaitingAsync = false;
        }

        internal virtual void BeginGameLaunch()
        {
            gameLaunchSoundFinished = true;
        }

        internal bool IsInteger(double d)
        {
            return unchecked(d == (int)d);
        }

        internal double screenWidth = 0;
        internal double screenHeight = 0;

        internal void ProvisionMainWindow(Canvas layoutRoot, Canvas feContainer)
        {
            System.Windows.Forms.Screen thisScreen = System.Windows.Forms.Screen.FromHandle(windowHandle);

            screenWidth = thisScreen.Bounds.Width;
            screenHeight = thisScreen.Bounds.Height;
            //System.Windows.SystemParameters.PrimaryScreenWidth;

            bool portraitMode = false;

            if (screenHeight > screenWidth)
            {
                portraitMode = true;
            }

            // set the resolution of the window
            try
            {
                System.Windows.Forms.Screen[] monitors = System.Windows.Forms.Screen.AllScreens;

                // you're always going to get the primary screen for screen #1
                System.Windows.Forms.Screen s1 = System.Windows.Forms.Screen.PrimaryScreen;
                System.Windows.Forms.Screen s2 = FindScreen(surroundMonitor2DisplayName);

                // if the user selected the same screen for both, abandon ship
                if (s2 == null || s1 == s2)
                {
                    surroundPosition = BigBlue.Models.SurroundPosition.None;
                }

                if (monitors.Count() >= 2 && displayWindowsBorder == false && surroundPosition != BigBlue.Models.SurroundPosition.None)
                {
                    width = 0;
                    //screenWidth = 0;
                    height = 0;

                    double monitor1BoundsX = Convert.ToDouble(s1.Bounds.Width);
                    double monitor1BoundsY = Convert.ToDouble(s1.Bounds.Height);

                    double monitor2BoundsX = Convert.ToDouble(s2.Bounds.Width);
                    double monitor2BoundsY = Convert.ToDouble(s2.Bounds.Height);

                    double screen1Width = CorrectAspectRatio(monitor1BoundsX, monitor1BoundsY, keepAspectRatio, aspectRatioIndex);
                    double screen2Width = CorrectAspectRatio(monitor2BoundsX, monitor2BoundsY, keepAspectRatio, aspectRatioIndex);

                    double minimumWidth = Math.Min(screen1Width, screen2Width);

                    // if the monitors are different sizes, we're going to have to clip to bounds so you can't see garbage off the screen
                    /*
                    if (monitor1BoundsY != monitor2BoundsY || monitor1BoundsX != monitor2BoundsX)
                    {
                        FrontEndContainer.ClipToBounds = true;
                    }
                    */

                    switch (surroundPosition)
                    {
                        case BigBlue.Models.SurroundPosition.Left:
                            this.Top = 0;
                            screenWidth = monitor1BoundsX + monitor2BoundsX;
                            screenHeight = Math.Max(monitor1BoundsY, monitor2BoundsY);
                            width = screen1Width + screen2Width;
                            height = Math.Min(monitor1BoundsY, monitor2BoundsY);
                            this.Left = -monitor2BoundsX;

                            surroundGameListWidthOffset = screen2Width;
                            double leftOffset = monitor2BoundsX - screen2Width;
                            Canvas.SetLeft(feContainer, leftOffset);
                            //Canvas.SetLeft(MainMenuContainer, leftOffset);

                            break;
                        case BigBlue.Models.SurroundPosition.Right:
                            this.Top = 0;
                            screenWidth = monitor1BoundsX + monitor2BoundsX;
                            screenHeight = Math.Max(monitor1BoundsY, monitor2BoundsY);
                            width = screen1Width + screen2Width;
                            height = Math.Min(monitor1BoundsY, monitor2BoundsY);
                            this.Left = 0;

                            surroundGameListWidthOffset = screen1Width;
                            double rightOffset = monitor1BoundsX - screen1Width;
                            Canvas.SetLeft(feContainer, rightOffset);

                            break;
                        case BigBlue.Models.SurroundPosition.Up:
                            screenWidth = Math.Max(monitor1BoundsX, monitor2BoundsX);
                            screenHeight = monitor1BoundsY + monitor2BoundsY;
                            width = Math.Min(screen1Width, screen2Width);
                            height = screenHeight;
                            this.Top = -monitor2BoundsY;
                            this.Left = 0;

                            surroundGameListWidthOffset = monitor2BoundsY;

                            double aboveOffset = (screenWidth - width) / 2;
                            Canvas.SetLeft(feContainer, aboveOffset);

                            break;
                        case BigBlue.Models.SurroundPosition.Down:
                            screenWidth = Math.Max(monitor1BoundsX, monitor2BoundsX);
                            screenHeight = monitor1BoundsY + monitor2BoundsY;
                            width = Math.Min(screen1Width, screen2Width);
                            height = screenHeight;
                            this.Top = 0;
                            this.Left = 0;
                            surroundGameListWidthOffset = monitor1BoundsY;

                            double belowOffset = (screenWidth - width) / 2;
                            Canvas.SetLeft(feContainer, belowOffset);

                            break;
                        default:
                            break;
                    }

                    // always set the multiplier using the smallest values as a basis
                    resolutionXMultiplier = SetResolutionMultiplier(minimumWidth, height, stretch, cleanStretch);

                    this.WindowState = System.Windows.WindowState.Normal;
                    this.WindowStyle = System.Windows.WindowStyle.None;

                    this.Width = screenWidth;
                    this.Height = screenHeight;

                    layoutRoot.Width = screenWidth;
                    layoutRoot.Height = screenHeight;
                }
                else
                {
                    surroundPosition = BigBlue.Models.SurroundPosition.None;

                    if (configResolutionX > 0 && configResolutionX <= screenWidth)
                    {
                        width = configResolutionX;
                    }
                    else
                    {
                        width = screenWidth;
                    }

                    if (configResolutionY > 0 && configResolutionY <= screenHeight)
                    {
                        height = configResolutionY;
                    }
                    else
                    {
                        height = screenHeight;
                    }

                    // no point in showing it in a border when it's the same size of the window or greater
                    if (displayWindowsBorder == true && width < screenWidth && height < screenHeight)
                    {
                        this.WindowState = System.Windows.WindowState.Normal;
                        this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                        this.SizeToContent = SizeToContent.WidthAndHeight;
                        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        // if we're displaying the windows border, we're basically treating the window's resolution as the whole screen's resolution
                        screenWidth = width;
                        screenHeight = height;

                        // we need to set both the main window and the layout root to be whichever resolution we decided on
                        this.Width = width;
                        this.Height = height;

                        layoutRoot.Width = width;
                        layoutRoot.Height = height;
                    }
                    else
                    {
                        this.SizeToContent = SizeToContent.Manual;
                        this.Top = 0;
                        this.Left = 0;
                        this.WindowStyle = WindowStyle.None;
                        //this.WindowState = WindowState.Maximized;
                        this.WindowState = WindowState.Normal;
                        // we need to set both the main window and the layout root to be whichever resolution we decided on
                        this.Width = screenWidth;
                        this.Height = screenHeight;
                        this.Width = screenWidth;
                        this.Height = screenHeight;

                        layoutRoot.Width = screenWidth;
                        layoutRoot.Height = screenHeight;
                    }

                    // correct aspect ratio here
                    // store the original values so we can use them for calculations later
                    double originalWidth = width;
                    double originalHeight = height;

                    if (screenRotation != 0)
                    {
                        RotateTransform rt = new RotateTransform(screenRotation);
                        feContainer.LayoutTransform = rt;
                    }

                    switch (screenRotation)
                    {
                        case 90:
                        case 270:
                            width = height;
                            height = originalWidth;
                            break;
                        default:
                            break;
                    }

                    if ((screenRotation == 90 || screenRotation == 270) && portraitMode == false)
                    {
                        if (keepAspectRatio == true)
                        {
                            // if we're in portrait mode with letterboxing, we need to display the screen in 3:4 aspect ratio
                            if (portraitModeIndex == 1)
                            {
                                height = height / aspectRatio43;
                            }
                            else
                            {
                                // if we're not in portrait mode  and the screen is rotated with letterboxing, then we need to show it in 4:3 aspect ratio
                                height = width / aspectRatio43;
                            }
                        }

                        if (screenHeight > width)
                        {
                            double topOffset = (screenHeight - width) / 2;
                            Canvas.SetTop(feContainer, topOffset);
                        }

                        if (screenWidth > height)
                        {
                            double leftOffset = (screenWidth - height) / 2;
                            Canvas.SetLeft(feContainer, leftOffset);
                        }
                    }
                    else if ((screenRotation == 90 || screenRotation == 270) && portraitMode == true)
                    {
                        if (keepAspectRatio == true)
                        {
                            width = height * aspectRatio43;
                        }

                        if (screenWidth > width)
                        {
                            double weow = (screenWidth - height) / 2;
                            Canvas.SetLeft(feContainer, weow);
                        }

                        if (screenHeight > height)
                        {
                            double weow = (screenHeight - width) / 2;
                            Canvas.SetTop(feContainer, weow);
                        }
                    }
                    else if (screenRotation != 90 && screenRotation != 270 && portraitMode == true)
                    {
                        if (keepAspectRatio == true)
                        {
                            if (portraitModeIndex == 0)
                            {
                                height = width / aspectRatio43;
                            }
                            else
                            {
                                height = height / aspectRatio43;
                            }
                        }

                        if (screenWidth > width)
                        {
                            double weow = (screenWidth - width) / 2;
                            Canvas.SetLeft(feContainer, weow);
                        }

                        if (screenHeight > height)
                        {
                            double weow = (screenHeight - height) / 2;
                            Canvas.SetTop(feContainer, weow);
                        }
                    }
                    else
                    {
                        if (keepAspectRatio == true)
                        {
                            if (aspectRatioIndex == 1)
                            {
                                width = height * aspectRatio34;
                            }
                            else if (aspectRatioIndex == 2)
                            {
                                height = width / aspectRatio169;
                            }
                            else
                            {
                                width = height * aspectRatio43;
                            }
                        }

                        if (screenWidth > width)
                        {
                            double weow = (screenWidth - width) / 2;
                            Canvas.SetLeft(feContainer, weow);
                        }

                        if (screenHeight > height)
                        {
                            double weow = (screenHeight - height) / 2;
                            Canvas.SetTop(feContainer, weow);
                        }
                    }

                    resolutionXMultiplier = SetResolutionMultiplier(width, height, stretch, cleanStretch);

                    // if the window size is greater than the frontend's set size, then we need to clip the bounds so you can't see garbage off screen
                    /*
                    if (FrontEndWindow.Width > width || FrontEndWindow.height > height)
                    {
                        FrontEndContainer.ClipToBounds = true;
                    }
                    */
                }
            }
            catch (Exception)
            {
                width = screenWidth;
                height = screenHeight;

                // we need to set both the main window and the layout root to be whichever resolution we decided on
                this.Width = width;
                this.Height = height;

                layoutRoot.Width = width;
                layoutRoot.Height = height;
            }
        }

        internal void AdjustVideoVolume(MediaElement videoMediaElement, System.Windows.Controls.Panel videoCanvas)
        {
            if (videoFading == true || videoMaterializing == true)
            {
                //videoMediaElement.Volume = videoCanvas.Opacity * (wmpVolume / 1);
                // divide by 1? not doing anything
                videoMediaElement.Volume = videoCanvas.Opacity * wmpVolume;
            }
        }

        internal int lastGameIndex = 0;

        internal bool RenderGameListCheck()
        {
            // if the position hasn't changed, we're just going to return false
            if (lastGameIndex == selectedListItemIndex)
            {
                return false;
            }

            // the problem is that this is happening before the image actually changes
            //stopVideo();
            //resetVideo();

            return true;
        }

        private static Size AddMainMenuItem(List<TextBlock> menuItems, string label, Thickness menuItemThickness, double mainMenuFontSize, FontFamily font, TextAlignment alignment, SolidColorBrush defaultColor)
        {
            TextBlock tb = new TextBlock();
            
            tb.Padding = menuItemThickness;

            tb.FontSize = mainMenuFontSize;
            tb.Text = label;
            tb.FontFamily = font;
            tb.TextAlignment = alignment;
            tb.Foreground = defaultColor;
            tb.FontWeight = FontWeights.Bold;
            tb.IsHitTestVisible = false;

            menuItems.Add(tb);

            Size tbSize = BigBlue.TextRendering.MeasureString(tb);

            tb.Height = tbSize.Height + menuItemThickness.Top + menuItemThickness.Bottom;

            tbSize.Height = tb.Height;

            return tbSize;
        }

        private static double CalculateGameListHeight(double height, double bushHeight, double adjustedGameListMarginY, double listItemHorizontalPadding, BigBlue.Models.SurroundPosition surroundPosition, double surroundGameListWidthOffset)
        {
            // this is the available space left for games after we've set the size of the selected game list item
            double gameListHeight = height - (bushHeight + adjustedGameListMarginY + listItemHorizontalPadding);

            if (surroundPosition == BigBlue.Models.SurroundPosition.Up || surroundPosition == BigBlue.Models.SurroundPosition.Down)
            {
                gameListHeight = surroundGameListWidthOffset - (adjustedGameListMarginY + listItemHorizontalPadding);
            }

            return gameListHeight;
        }

        internal bool SetGameListDimensions(Canvas listCanvas, Canvas listOutlineCanvas, double deadHorizontalSpace, double deadVerticalSpace, double? themeListWidth, double? themeListHeight)
        {
            double adjustedGameListMarginX = gameListMarginX + gameListOverscanX;

            double totalGameListMargin = adjustedGameListMarginX + gameListMarginX;

            double adjustedGameListMarginY = gameListMarginY + gameListOverscanY;

            selectedGameHeight = (selectedTextSize * gameListFont.LineSpacing) + (selectedItemVerticalPadding * 2);
            unselectedGameHeight = (unselectedTextSize * gameListFont.LineSpacing) + (unselectedItemVerticalPadding * 2);

            double listHeight = CalculateGameListHeight(height, deadVerticalSpace, adjustedGameListMarginY, listItemHorizontalPadding, surroundPosition, surroundGameListWidthOffset);

            if (themeListHeight != null)
            {
                if (themeListHeight > 0)
                {
                    listHeight = (double)themeListHeight;
                }
            }

            if (listHeight < selectedGameHeight)
            {
                selectedTextSize = selectedTextSize - 1;
                unselectedTextSize = unselectedTextSize - 1;
                return false;
            }

            double verticalSpaceAvailableForUnselectedItems = listHeight - selectedGameHeight;

            double numberOfUnselectedItems = verticalSpaceAvailableForUnselectedItems / unselectedGameHeight;

            numberOfTextBlockListItems = Convert.ToInt32(Math.Floor(numberOfUnselectedItems)) + 1;

            // we want the list to be an odd number so that we can truly place one of the games in the center
            if (numberOfTextBlockListItems % 2 == 0)
            {
                numberOfTextBlockListItems = numberOfTextBlockListItems - 1;
            }

            if (numberOfTextBlockListItems < 1)
            {
                selectedTextSize = selectedTextSize - 1;
                unselectedTextSize = unselectedTextSize - 1;
                return false;
            }

            double leftOver = listHeight - ((unselectedGameHeight * (numberOfTextBlockListItems - 1)) + selectedGameHeight);

            if (leftOver > 0)
            {
                individualExtraMargin = leftOver / (numberOfTextBlockListItems - 1);
            }

            textBlockListHalfWayPoint = numberOfTextBlockListItems / 2;
            textBlockListItemsToPage = textBlockListHalfWayPoint + 1;

            double listWidth = (width - (deadHorizontalSpace * resolutionXMultiplier)) - (totalGameListMargin);

            if (themeListWidth != null)
            {
                if (themeListWidth > 0)
                {
                    listWidth = (double)themeListWidth;
                }
            }

            if (surroundPosition == BigBlue.Models.SurroundPosition.Left || surroundPosition == BigBlue.Models.SurroundPosition.Right)
            {
                listWidth = surroundGameListWidthOffset - totalGameListMargin;
            }


            if (listCanvas != null)
            {
                listCanvas.Margin = new Thickness(adjustedGameListMarginX, adjustedGameListMarginY, 0, 0);

                listOutlineCanvas.Width = listWidth;
                listOutlineCanvas.Height = listHeight;

                listCanvas.Width = listWidth;
                listCanvas.MaxWidth = listWidth;
                listCanvas.MinWidth = listWidth;

                listCanvas.MaxHeight = listHeight;
                listCanvas.Height = listHeight;
                listCanvas.MinHeight = listHeight;
            }

            

            selectedGamePadding = new Thickness(listItemHorizontalPadding, selectedItemVerticalPadding, listItemHorizontalPadding, selectedItemVerticalPadding);

            gamePadding = new Thickness(listItemHorizontalPadding, unselectedItemVerticalPadding, listItemHorizontalPadding, unselectedItemVerticalPadding);

            return true;
        }

        internal static void SetMainMenuDimensions(Canvas mainMenuCanvas, SolidColorBrush defaultColor)
        {
            List<TextBlock> menuItems = new List<TextBlock>();

            Thickness firstMenuitemThickness = new Thickness(mainMenuPaddingX, mainMenuPaddingY * 2, mainMenuPaddingX, mainMenuPaddingY);

            Thickness menuItemThickness = new Thickness(mainMenuPaddingX, mainMenuPaddingY, mainMenuPaddingX, mainMenuPaddingY);

            Thickness lastMenuItemThickness = new Thickness(mainMenuPaddingX, mainMenuPaddingY, mainMenuPaddingX, mainMenuPaddingY * 2);
            TextAlignment alignment = TextAlignment.Center;

            double widestMenuItem = 0;
            double mainMenuAdjustedHeight = 0;

            Size returnItemSize = AddMainMenuItem(menuItems, mainMenuReturnLabel, firstMenuitemThickness, mainMenuFontSize, gameListFont, alignment, defaultColor);

            widestMenuItem = returnItemSize.Width;
            mainMenuAdjustedHeight = mainMenuAdjustedHeight + returnItemSize.Height;

            if (displayExitItemInMenu == true)
            {
                Size exitItemSize = AddMainMenuItem(menuItems, mainMenuExitLabel, menuItemThickness, mainMenuFontSize, gameListFont, alignment, defaultColor);

                if (exitItemSize.Width > widestMenuItem)
                {
                    widestMenuItem = exitItemSize.Width;
                }

                mainMenuAdjustedHeight = mainMenuAdjustedHeight + exitItemSize.Height;
            }
            else
            {
                totalMenuItems = 2;
            }

            Size shutdownItemSize = AddMainMenuItem(menuItems, mainMenuShutdownLabel, menuItemThickness, mainMenuFontSize, gameListFont, alignment, defaultColor);

            if (shutdownItemSize.Width > widestMenuItem)
            {
                widestMenuItem = shutdownItemSize.Width;
            }

            mainMenuAdjustedHeight = mainMenuAdjustedHeight + shutdownItemSize.Height;

            Size restartItemSize = AddMainMenuItem(menuItems, mainMenuRestartLabel, lastMenuItemThickness, mainMenuFontSize, gameListFont, alignment, defaultColor);

            if (restartItemSize.Width > widestMenuItem)
            {
                widestMenuItem = restartItemSize.Width;
            }

            mainMenuAdjustedHeight = mainMenuAdjustedHeight + restartItemSize.Height;

            double mainMenuAdjustedWidth = widestMenuItem + (mainMenuPaddingX * 2);

            for (int i = 0; i < menuItems.Count; i++)
            {
                TextBlock menuItem = menuItems[i];

                menuItem.Width = mainMenuAdjustedWidth;

                switch (i)
                {
                    case 0:
                        Canvas.SetTop(menuItem, 0);
                        break;
                    case 1:
                        Canvas.SetTop(menuItem, menuItems[0].Height);
                        break;
                    case 2:
                        Canvas.SetTop(menuItem, menuItems[0].Height + menuItems[1].Height);
                        break;
                    case 3:
                        Canvas.SetTop(menuItem, menuItems[0].Height + menuItems[1].Height + menuItems[2].Height);
                        break;
                }

                //mainMenuAdjustedHeight = mainMenuAdjustedHeight + menuItem.Height;

                mainMenuCanvas.Children.Add(menuItem);
            }

            mainMenuCanvas.Width = mainMenuAdjustedWidth;
            mainMenuCanvas.Height = mainMenuAdjustedHeight;

            double topMargin = (height - mainMenuAdjustedHeight) / 2;

            if (surroundPosition == BigBlue.Models.SurroundPosition.Up || surroundPosition == BigBlue.Models.SurroundPosition.Down)
            {
                double screen1Height = height - surroundGameListWidthOffset;

                topMargin = surroundGameListWidthOffset + (screen1Height / 2) - (mainMenuAdjustedHeight / 2);

                // screen2 height + (screen1 height - bush) / 2
            }

            double sideMargin = (width - mainMenuAdjustedWidth) / 2;

            if (surroundPosition == BigBlue.Models.SurroundPosition.Left || surroundPosition == BigBlue.Models.SurroundPosition.Right)
            {
                sideMargin = (surroundGameListWidthOffset - mainMenuAdjustedWidth) / 2;
            }

            mainMenuCanvas.Background = blackBrush;

            Canvas.SetTop(mainMenuCanvas, topMargin);
            Canvas.SetLeft(mainMenuCanvas, sideMargin);
        }

        internal void RenderMainMenu(Canvas mainMenuCanvas, bool playSound, Brush selectedBrush, Brush unselectedBrush)
        {
            UIElementCollection menuItems = mainMenuCanvas.Children;

            for (int i = 0; i < menuItems.Count; i++)
            {
                TextBlock menuItem = (TextBlock)menuItems[i];

                if (i == selectedMenuIndex)
                {
                    menuItem.Foreground = selectedBrush;
                }
                else
                {
                    if (menuItem.Foreground == selectedBrush)
                    {
                        menuItem.Foreground = unselectedBrush;
                    }
                }
            }

            if (playSound)
            {
                PlayMenuSound();
            }
        }

        internal void PlayMenuSound()
        {
            BigBlue.XAudio2Player.PlaySound(ListNavigationSoundKey, null);
        }

        internal void CalculateNextMenuItem(Canvas mainMenuCanvas, Brush selectedBrush, Brush unselectedBrush)
        {
            int nextMenuItemIndex = selectedMenuIndex + 1;

            if (nextMenuItemIndex > totalMenuItems)
            {
                selectedMenuIndex = 0;
            }
            else
            {
                selectedMenuIndex = selectedMenuIndex + 1;
            }

            RenderMainMenu(mainMenuCanvas, true, selectedBrush, unselectedBrush);
        }

        internal void CalculatePreviousMenuItem(Canvas mainMenuCanvas, Brush selectedBrush, Brush unselectedBrush)
        {
            int previousMenuItemIndex = selectedMenuIndex - 1;
            if (previousMenuItemIndex < 0)
            {
                selectedMenuIndex = totalMenuItems;
            }
            else
            {
                selectedMenuIndex = previousMenuItemIndex;
            }

            RenderMainMenu(mainMenuCanvas, true, selectedBrush, unselectedBrush);
        }

        internal ImageBrush GetImageBrushFromPath(BigBlue.Models.FrontendSnapshot snap, int decodedPixelWidth)
        {
            BitmapImage bi = new BitmapImage();
            
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            //bi.DecodePixelWidth = snap.Width;
            bi.DecodePixelWidth = decodedPixelWidth;

            if (snap.Path != null)
            {
                bi.UriSource = snap.Path;
            }
            else
            {
                bi.UriSource = notFoundImageUri;
            }

            //bi.StreamSource = mem;
            bi.EndInit();
            bi.Freeze();

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = bi;
            ib.Freeze();

            return ib;


            //}       
        }

        internal System.Windows.Media.Imaging.BitmapImage GetBitMapImageFromPath(BigBlue.Models.FrontendSnapshot snap, int decodedPixelWidth)
        {
            //byte[] buffer = File.ReadAllBytes(p);
            //using (MemoryStream mem = new MemoryStream(buffer))
            //{
            BitmapImage bi = new BitmapImage();

            
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            //bi.DecodePixelWidth = snap.Width;
            bi.DecodePixelWidth = decodedPixelWidth;

            if (snap.Path != null)
            {
                bi.UriSource = snap.Path;
            }
            else
            {
                bi.UriSource = notFoundImageUri;
            }

            //bi.StreamSource = mem;
            bi.EndInit();
            bi.Freeze();
            
            return bi;
            //}       
        }

        private static bool IsSnapShotExtensionValid(string fileName)
        {
            string targetExtension = System.IO.Path.GetExtension(fileName);

            if (String.IsNullOrWhiteSpace(targetExtension))
            {
                return false;
            }
            else
            {
                targetExtension = "*" + targetExtension;
            }

            List<string> recognisedImageExtensions = new List<string>();

            foreach (System.Drawing.Imaging.ImageCodecInfo imageCodec in System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders())
            {
                recognisedImageExtensions.AddRange(imageCodec.FilenameExtension.Split(";".ToCharArray()));
            }

            foreach (string extension in recognisedImageExtensions)
            {
                if (extension.Equals(targetExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        internal void AddSnapshotsToList(BigBlue.Models.FrontendListItem listItem, string imageValue, string imageType)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                BigBlue.Models.FrontendSnapshot snap = new BigBlue.Models.FrontendSnapshot();

                switch (imageType)
                {
                    case "snap":
                        snap.ImageControl = snapshotImageControl;
                        if (snapshotImageControl.Width > 0)
                        {
                            snap.Width = Convert.ToInt32(snapshotImageControl.Width);
                        }
                        if (snapshotImageControl.Height > 0)
                        {
                            snap.Height = Convert.ToInt32(snapshotImageControl.Height);
                        }
                        
                        break;
                    case "marquee":
                        if (marqueeDisplay)
                        {
                            snap.ImageControl = marqueeWindow.SecondaryWindowSnapshot;
                            snap.Width = Convert.ToInt32(marqueeWindow.Width);
                            snap.Height = Convert.ToInt32(marqueeWindow.Height);
                        }
                        break;
                    case "flyer":
                        if (flyerDisplay)
                        {
                            snap.ImageControl = flyerWindow.SecondaryWindowSnapshot;
                            snap.Width = Convert.ToInt32(flyerWindow.Width);
                            snap.Height = Convert.ToInt32(flyerWindow.Height);
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case "instruct":
                        if (instructionDisplay)
                        {
                            snap.ImageControl = instructionWindow.SecondaryWindowSnapshot;
                            snap.Width = Convert.ToInt32(instructionWindow.Width);
                            snap.Height = Convert.ToInt32(instructionWindow.Height);
                        }
                        else
                        {
                            return;
                        }
                        break;
                }

                if (!string.IsNullOrWhiteSpace(imageValue))
                {
                    // sanitize those paths
                    string imagePath = imageValue.Replace(@"/", @"\").Trim();

                    // we know it's not a relative URL in this case and have to prepend the current directory
                    if (!imagePath.Contains(":"))
                    {
                        imagePath = Path + @"\" + imagePath;
                    }

                    if (IsSnapShotExtensionValid(imagePath))
                    {
                        // make sure that the file exists
                        if (System.IO.File.Exists(imagePath))
                        {
                            try
                            {
                                snap.Path = new Uri(imagePath);
                            }
                            catch (Exception ex)
                            {
                                snap.Path = null;
                                Debug.WriteLine(ex.Message);
                            }
                        }
                    }
                }

                if (snap.ImageControl != null)
                {
                    if (listItem.Snapshots == null)
                    {
                        listItem.Snapshots = new List<BigBlue.Models.FrontendSnapshot>();
                    }

                    listItem.Snapshots.Add(snap);
                }
            });
        }

        private string GetXElementValue(XElement listItemElement, string imageType)
        {
            XElement imageNode = listItemElement.Element(imageType);
            if (imageNode != null)
            {
                return imageNode.Value;
            }

            return string.Empty;
        }

        // every time you make a selection, you set the current list as the parent and create a new list and then try populating it
        // when trying to populate it, if it fails, you need to recursively see whether that selection folder actually exists
        // if it doesn't exist, recursively move up the tree until you get back to familiar territory
        internal bool WalkDirectoryTree(System.IO.DirectoryInfo root, string path, Guid listGuid, string searchPattern)
        {
            //List<ListItem> items = null;

            bool isValid = false;

            System.IO.DirectoryInfo[] subDirs = null;
            IEnumerable<string> files = null;

            if (root == null)
            {
                if (System.IO.Directory.Exists(path))
                {
                    root = new System.IO.DirectoryInfo(path);
                }
            }

            if (root != null)
            {
                try
                {
                    subDirs = root.GetDirectories(allSearchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                }
                catch (System.ArgumentException)
                {
                }

                try
                {
                    files = BigBlue.FileHandling.GetFilesByPath(searchPattern, path);
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                }
                catch (System.ArgumentException)
                {
                }
            }

            if (subDirs != null)
            {
                if (subDirs.Count() > 0)
                {
                    isValid = true;
                }
            }

            if (files != null)
            {
                if (files.Count() > 0)
                {
                    isValid = true;
                }
            }

            // if there are no subdirs and no files, we're basically treating this folder list item as invalid
            if (isValid)
            {
                //frontendLists[listGuid].ListName = path;
                currentListName.Name = path;

                // observable collection experiment
                //frontendLists[listGuid].ListItems = new List<BigBlue.Models.FrontendListItem>();
                frontendLists[listGuid].ListItems = new System.Collections.ObjectModel.ObservableCollection<BigBlue.Models.FrontendListItem>();

                int itemCount = 0;

                foreach (System.IO.DirectoryInfo di in subDirs)
                {
                    itemCount = itemCount + 1;

                    BigBlue.Models.FrontendListItem feli = new BigBlue.Models.FrontendListItem();

                    string snapshotImagePath = string.Format(originatingFolderParentListItem.SnapTemplate, di.Name);
                    AddSnapshotsToList(feli, snapshotImagePath, "snap");

                    string videoPath = string.Format(originatingFolderParentListItem.VideoTemplate, di.Name);
                    feli.Video = BigBlue.FileHandling.ValidateUri(videoPath, path);

                    string marqueeImagePath = string.Format(originatingFolderParentListItem.MarqueeTemplate, di.Name);
                    AddSnapshotsToList(feli, marqueeImagePath, "marquee");

                    string instructionImagePath = string.Format(originatingFolderParentListItem.InstructionTemplate, di.Name);
                    AddSnapshotsToList(feli, instructionImagePath, "instruct");

                    string flyerImagePath = string.Format(originatingFolderParentListItem.FlyerTemplate, di.Name);
                    AddSnapshotsToList(feli, flyerImagePath, "flyer");

                    if (listGuid != folderListGuid)
                    {
                        feli.ParentID = selectedListGuid;
                    }
                    else
                    {
                        feli.ParentFolder = lastFolder;

                    }

                    feli.IndexOfParent = selectedListItemIndex;
                    feli.Title = "📂 " + di.Name;
                    feli.ChildFolder = di.FullName;

                    frontendLists[listGuid].ListItems.Add(feli);
                }

                foreach (string fileName in files)
                {
                    itemCount = itemCount + 1;

                    BigBlue.Models.FrontendListItem feli = new BigBlue.Models.FrontendListItem();

                    if (listGuid != folderListGuid)
                    {
                        feli.ParentID = selectedListGuid;
                    }
                    else
                    {
                        feli.ParentFolder = lastFolder;
                    }

                    feli.Title = System.IO.Path.GetFileNameWithoutExtension(fileName);

                    string snapshotImagePath = string.Format(originatingFolderParentListItem.SnapTemplate, feli.Title);
                    AddSnapshotsToList(feli, snapshotImagePath, "snap");

                    string marqueeImagePath = string.Format(originatingFolderParentListItem.MarqueeTemplate, feli.Title);
                    AddSnapshotsToList(feli, marqueeImagePath, "marquee");

                    string instructionImagePath = string.Format(originatingFolderParentListItem.InstructionTemplate, feli.Title);
                    AddSnapshotsToList(feli, instructionImagePath, "instruct");

                    string flyerImagePath = string.Format(originatingFolderParentListItem.FlyerTemplate, feli.Title);
                    AddSnapshotsToList(feli, flyerImagePath, "flyer");

                    string videoPath = string.Format(originatingFolderParentListItem.VideoTemplate, feli.Title);
                    feli.Video = BigBlue.FileHandling.ValidateUri(videoPath, path);

                    feli.Binary = originatingFolderParentListItem.Binary;
                    feli.Directory = originatingFolderParentListItem.Directory;
                    feli.Arguments = String.Format(originatingFolderParentListItem.Arguments, fileName);

                    feli.PreArguments = string.Format(originatingFolderParentListItem.PreArguments, fileName);
                    feli.PostArguments = string.Format(originatingFolderParentListItem.PostArguments, fileName);

                    feli.PreBinary = originatingFolderParentListItem.PreBinary;
                    feli.PostBinary = originatingFolderParentListItem.PostBinary;

                    feli.PreDirectory = originatingFolderParentListItem.PreDirectory;
                    feli.PostDirectory = originatingFolderParentListItem.PostDirectory;

                    // we weren't saving the kill task value for subfolders
                    feli.KillTask = originatingFolderParentListItem.KillTask;

                    feli.IndexOfParent = selectedListItemIndex;

                    frontendLists[listGuid].ListItems.Add(feli);
                }

                if (textBlockListItemsToPage > itemCount)
                {
                    frontendLists[listGuid].TextBlockItemsToPage = 1;
                }
                else
                {
                    frontendLists[listGuid].TextBlockItemsToPage = textBlockListItemsToPage;
                }

                if (imageBlockListItemsToPage > itemCount)
                {
                    frontendLists[listGuid].ImageItemsToPage = 1;
                }
                else
                {
                    frontendLists[listGuid].ImageItemsToPage = imageBlockListItemsToPage;
                }

                frontendLists[listGuid].Total = itemCount - 1;
            }

            return isValid;
        }

        internal Guid LoadFrontendSubList(Guid? containerId, string subListName, IEnumerable<XElement> rootListItems, int containerIndex)
        {
            BigBlue.Models.FrontendListItem list = new BigBlue.Models.FrontendListItem();
            list.ID = Guid.NewGuid();

            if (containerId == null)
            {
                rootListGuid = list.ID;
            }

            if (!string.IsNullOrWhiteSpace(subListName))
            {
                list.ListName = subListName;
            }
            else
            {
                list.ListName = "Home";
            }

            // start off with a 0 index
            //list.LastListIndex = 0;

            // observable collection experiment
            //list.ListItems = new List<BigBlue.Models.FrontendListItem>();
            list.ListItems = new System.Collections.ObjectModel.ObservableCollection<BigBlue.Models.FrontendListItem>();

            int liIndex = 0;

            foreach (XElement listItemElement in rootListItems)
            {
                BigBlue.Models.FrontendListItem listItem = new BigBlue.Models.FrontendListItem();

                listItem.ID = Guid.NewGuid();

                if (containerId != null)
                {
                    listItem.ParentID = containerId;

                    listItem.IndexOfParent = containerIndex;
                }
                else
                {
                    listItem.IndexOfParent = liIndex;
                }

                XElement titleElement = listItemElement.Element("name");

                if (titleElement != null)
                {
                    listItem.Title = titleElement.Value;

                    string prospectiveTitle = listItem.Title;

                    string[] wordsInTitle = listItem.Title.Split(' ');

                    foreach (string w in wordsInTitle)
                    {
                        switch (w)
                        {
                            case "II":
                                prospectiveTitle = prospectiveTitle.Replace("II", "2");
                                break;
                            case "III":
                                prospectiveTitle = prospectiveTitle.Replace("III", "3");
                                break;
                            case "IV":
                                prospectiveTitle = prospectiveTitle.Replace("IV", "4");
                                break;
                            case "V":
                                prospectiveTitle = prospectiveTitle.Replace("V", "5");
                                break;
                            case "VI":
                                prospectiveTitle = prospectiveTitle.Replace("VI", "6");
                                break;
                            case "VII":
                                prospectiveTitle = prospectiveTitle.Replace("VII", "7");
                                break;
                            case "VIII":
                                prospectiveTitle = prospectiveTitle.Replace("VIII", "8");
                                break;
                            case "IX":
                                prospectiveTitle = prospectiveTitle.Replace("IX", "9");
                                break;
                            case "X":
                                prospectiveTitle = prospectiveTitle.Replace("X", "10");
                                break;
                            case "XI":
                                prospectiveTitle = prospectiveTitle.Replace("XI", "11");
                                break;
                            case "XII":
                                prospectiveTitle = prospectiveTitle.Replace("XII", "12");
                                break;
                            case "XIII":
                                prospectiveTitle = prospectiveTitle.Replace("XIII", "13");
                                break;
                            case "XIV":
                                prospectiveTitle = prospectiveTitle.Replace("XIV", "14");
                                break;
                            case "XV":
                                prospectiveTitle = prospectiveTitle.Replace("XV", "15");
                                break;
                            case "XVI":
                                prospectiveTitle = prospectiveTitle.Replace("XVI", "16");
                                break;
                            case "XVII":
                                prospectiveTitle = prospectiveTitle.Replace("XVII", "17");
                                break;
                            case "XVIII":
                                prospectiveTitle = prospectiveTitle.Replace("XVIII", "18");
                                break;
                            case "XIX":
                                prospectiveTitle = prospectiveTitle.Replace("XIX", "19");
                                break;
                        }

                        if (!BigBlue.Speech.IsNoiseWord(w))
                        {
                            if (!voiceChoices.Contains(w, StringComparer.InvariantCultureIgnoreCase))
                            {
                                voiceChoices.Add(w);
                            }
                        }
                    }

                    listItem.SearchTitle = prospectiveTitle;

                    if (!voiceChoices.Contains(prospectiveTitle, StringComparer.InvariantCultureIgnoreCase))
                    {
                        voiceChoices.Add(prospectiveTitle);
                    }
                }
                else
                {
                    listItem.Title = "N/A";
                }

                listItem.Video = BigBlue.FileHandling.ValidateUri(GetXElementValue(listItemElement, "video"), Path);

                // try to add the snapshots if they're available and configured
                AddSnapshotsToList(listItem, GetXElementValue(listItemElement, "snap"), "snap");
                AddSnapshotsToList(listItem, GetXElementValue(listItemElement, "marquee"), "marquee");
                AddSnapshotsToList(listItem, GetXElementValue(listItemElement, "flyer"), "flyer");
                AddSnapshotsToList(listItem, GetXElementValue(listItemElement, "instruct"), "instruct");

                // assign the main program data
                listItem.Binary = BigBlue.FileHandling.CleanUpPath(GetXElementValue(listItemElement, "exe"));
                listItem.Directory = BigBlue.FileHandling.CleanUpPath(GetXElementValue(listItemElement, "dir"));
                listItem.Arguments = GetXElementValue(listItemElement, "args");
                listItem.ChildFolder = GetXElementValue(listItemElement, "folder");

                XAttribute killAttribute = listItemElement.Attribute("kill");

                bool liKillTask = false;

                if (killAttribute != null)
                {
                    bool.TryParse(killAttribute.Value, out liKillTask);
                }

                listItem.KillTask = liKillTask;

                // assign the pre-launch program data
                listItem.PreBinary = BigBlue.FileHandling.CleanUpPath(GetXElementValue(listItemElement, "preexe"));
                listItem.PreDirectory = BigBlue.FileHandling.CleanUpPath(GetXElementValue(listItemElement, "predir"));
                listItem.PreArguments = GetXElementValue(listItemElement, "preargs");

                // assign the post-launch program data
                listItem.PostBinary = BigBlue.FileHandling.CleanUpPath(GetXElementValue(listItemElement, "postexe"));
                listItem.PostDirectory = BigBlue.FileHandling.CleanUpPath(GetXElementValue(listItemElement, "postdir"));
                listItem.PostArguments = GetXElementValue(listItemElement, "postargs");

                XElement childList = listItemElement.Element("list");

                if (string.IsNullOrWhiteSpace(listItem.ChildFolder))
                {
                    if (childList != null)
                    {
                        XAttribute folderAttribute = childList.Attribute("folder");

                        if (folderAttribute != null)
                        {
                            if (!string.IsNullOrWhiteSpace(folderAttribute.Value))
                            {
                                // add in an empty list to use for this dynamic folder list
                                BigBlue.Models.FrontendListItem folderList = new BigBlue.Models.FrontendListItem();
                                folderList.ID = Guid.NewGuid();
                                folderList.ParentID = listItem.ID;
                                //folderList.LastListIndex = 0;

                                // set the folder

                                listItem.ChildFolder = folderAttribute.Value;
                                listItem.ChildID = folderList.ID;

                                listItem.SnapTemplate = GetXElementValue(listItemElement, "snaptemplate");
                                listItem.MarqueeTemplate = GetXElementValue(listItemElement, "marqueetemplate");
                                listItem.FlyerTemplate = GetXElementValue(listItemElement, "flyertemplate");
                                listItem.InstructionTemplate = GetXElementValue(listItemElement, "instructiontemplate");
                                listItem.VideoTemplate = GetXElementValue(listItemElement, "videotemplate");

                                XAttribute searchPatternAttribute = childList.Attribute("searchpattern");

                                if (searchPatternAttribute != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(searchPatternAttribute.Value))
                                    {
                                        listItem.FolderSearchPattern = searchPatternAttribute.Value;
                                    }
                                    else
                                    {
                                        // if there's nothing in there, then just select everything
                                        listItem.FolderSearchPattern = allSearchPattern;
                                    }
                                }

                                XAttribute defaultAttribute = childList.Attribute("default");

                                frontendLists.Add(folderList.ID, folderList);

                                if (defaultAttribute != null)
                                {
                                    bool isDefault;

                                    bool.TryParse(defaultAttribute.Value, out isDefault);

                                    if (isDefault == true)
                                    {
                                        originatingFolderParentListItem = listItem;
                                        originatingFolderListGuid = list.ID;

                                        selectedListGuid = list.ID;
                                        //selectedListGuid = list.ID;

                                        subFolderTrail.Add(listItem.ChildFolder);

                                        //folderList.ListItems = new List<FrontendListItem>();

                                        bool folderResult = WalkDirectoryTree(null, listItem.ChildFolder, folderList.ID, listItem.FolderSearchPattern);

                                        if (!folderResult)
                                        {
                                            selectedListGuid = Guid.Empty;
                                        }
                                        else
                                        {
                                            selectedListGuid = folderList.ID;
                                        }
                                        //OpenSubFolderListItem(frontendLists[selectedListGuid].ListItems[0].ChildFolder, selectedListGuid);
                                        //OpenFolderListItem(folderList.ID);
                                    }
                                }
                            }
                        }

                        if (childList.Elements() != null)
                        {
                            IEnumerable<XElement> childListItems = childList.Elements("item");

                            if (childListItems != null)
                            {
                                if (childListItems.Count() > 0)
                                {
                                    // the parent list item should be set to its childrens' ID
                                    listItem.ChildID = LoadFrontendSubList(list.ID, listItem.Title, childListItems, liIndex);

                                    XAttribute defaultAttribute = childList.Attribute("default");

                                    if (defaultAttribute != null)
                                    {
                                        bool isDefault;

                                        bool.TryParse(defaultAttribute.Value, out isDefault);

                                        if (isDefault == true)
                                        {
                                            selectedListGuid = (Guid)listItem.ChildID;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                liIndex = liIndex + 1;

                list.ListItems.Add(listItem);
            }

            int total = list.ListItems.Count();

            if (textBlockListItemsToPage > total)
            {
                list.TextBlockItemsToPage = 1;
            }
            else
            {
                list.TextBlockItemsToPage = textBlockListItemsToPage;
            }

            if (imageBlockListItemsToPage > total)
            {
                list.ImageItemsToPage = 1;
            }
            else
            {
                list.ImageItemsToPage = imageBlockListItemsToPage;
            }

            list.Total = total - 1;

            // all of them need to be added as unique GUIDs
            frontendLists.Add(list.ID, list);

            return list.ID;
        }

        private static Point GetPointOnCircleFromAngle(Point startingPoint, double radiusX, double radiusY, double angle)
        {
            double x = startingPoint.X + radiusX * Math.Cos(angle * (Math.PI / 180.0));

            double y = startingPoint.Y + radiusY * Math.Sin(angle * (Math.PI / 180.0));

            return new Point(x, y);
        }

        private static Point GetPointOnEllipseFromAngle(Point startingPoint, double radiusX, double radiusY, double angle)
        {
            angle = angle % 360;

            double tanAngle = Math.Abs(Math.Tan(angle * (Math.PI / 180.0)));
            double x = Math.Sqrt((Math.Pow(radiusX, 2) * Math.Pow(radiusY, 2)) / (Math.Pow(radiusY, 2) + Math.Pow(radiusX, 2) * Math.Pow(tanAngle, 2)));
            double y = x * tanAngle;

            if ((angle >= 0) && (angle < 90))
            {
                return new Point(startingPoint.X + (int)x, startingPoint.Y + (int)y);
            }
            else if ((angle >= 90) && (angle < 180))
            {
                return new Point(startingPoint.X - (int)x, startingPoint.Y + (int)y);
            }
            else if ((angle >= 180) && (angle < 270))
            {
                return new Point(startingPoint.X - (int)x, startingPoint.Y - (int)y);
            }
            else
            {
                return new Point(startingPoint.X + (int)x, startingPoint.Y - (int)y);
            }
        }

        private void SetRotationTransForm(RotateTransform rt, double radius, double angle, double centerX, double centerY, double listCenter, bool top)
        {
           // double derkax1 = centerX + radius * Math.Cos(2 * Math.PI / 360 * angle);
          //  double derkay1 = centerY + radius * Math.Sin(2 * Math.PI / 360 * angle);


            double derkax1 = (centerX) + radius * Math.Cos(angle * (Math.PI / 180));
            double derkay1 = centerY + radius * Math.Sin(angle * (Math.PI / 180));

            if (!top)
            {

            }

            /*
             can you share you math pdf files?
<lasdjkashdas> say we have 9 tiles. 5 is the center one. when i add the rectangels to the see, if the index of the rectangle is less than the center, my angle = (index - 5) * baseRotation
* blackwind_123 (~IceChat9@117.192.138.238) has joined ##math
<lasdjkashdas> if the index of the rectangle is greater than the center, my angle = (5 - index) * baseRotation
            */

            

            //rt.CenterX = -derkax1;
            //rt.CenterY = derkay1;

            rt.CenterX = (angle / Math.PI / Math.PI);


        }

        private System.Windows.Shapes.Polygon GetImageBlockTriangle(double width, double height, string triangleType)
        {
            System.Windows.Shapes.Polygon triangle = new System.Windows.Shapes.Polygon();

            PointCollection pc = new PointCollection();

            switch (triangleType)
            {
                case "triangleUp":
                    pc.Add(new Point(width / 2, 0));
                    pc.Add(new Point(width, height));
                    pc.Add(new Point(0, height));
                    pc.Add(new Point(width / 2, 0));
                    break;
                case "triangleDown":
                    pc.Add(new Point(0, 0));
                    pc.Add(new Point(0, width));
                    pc.Add(new Point(width / 2, height));
                    pc.Add(new Point(0, 0));
                    break;
                case "triangleLeft":
                    pc.Add(new Point(width, 0));
                    pc.Add(new Point(0, height / 2));
                    pc.Add(new Point(width, height));
                    pc.Add(new Point(width, 0));
                    break;
                case "triangleRight":
                    pc.Add(new Point(0, 0));
                    pc.Add(new Point(width, height / 2));
                    pc.Add(new Point(0, height));
                    pc.Add(new Point(0, 0));
                    break;
            }
            triangle.Points = pc;

            return triangle;
        }

        private System.Windows.Shapes.Ellipse GetImageBlockCircle(double width, double height)
        {
            System.Windows.Shapes.Ellipse ellipse = new System.Windows.Shapes.Ellipse();
            

            return ellipse;
        }

        private System.Windows.Shapes.Polygon GetImageBlockStar(double width, double height)
        {
            System.Windows.Shapes.Polygon star = new System.Windows.Shapes.Polygon();
            
            //star.Width = width;
            //star.Height = height;
            PointCollection pc = new PointCollection();

            pc.Add(new Point(width / 2, 0));

            //
            pc.Add(new Point((width / 2) + ((width / 2) / 4), (height / 2) - ((height / 2) / 3)));
            pc.Add(new Point(width, (height / 2) - ((height / 2) / 3)));
            //

            //pc.Add(new Point((width / 2) + ((width / 2) / 3) + (((width / 2) / 3) / 10), height / 2));
            pc.Add(new Point((width / 2) + ((width / 2) / 3) + (((width / 2) / 3) / 10), (height / 2) + ((height / 2) / 8)));



            pc.Add(new Point(width - ((width / 2) / 3), height));

            pc.Add(new Point(width / 2, (height / 2) + ((height / 2) / 3.2)));


            pc.Add(new Point((width / 2) / 3, height));
            pc.Add(new Point(((width / 2)) - ((width / 2) / 3) - (((width / 2) / 3) / 10), (height / 2) + ((height / 2) / 8)));

            //
            pc.Add(new Point(0, (height / 2) - ((height / 2) / 3)));
            pc.Add(new Point((width / 2) - ((width / 2) / 4), (height / 2) - ((height / 2) / 3)));
            //

            pc.Add(new Point(width / 2, 0));

            star.Points = pc;

            return star;
        }

        internal void AddHorizontalImagelistBlocks(Canvas gl, double selectedImageWidth, double selectedImageHeight, double unselectedImageWidth, double unselectedImageHeight, string imageShape, string alignment, double borderWidth, Brush borderBrush)
        {
            double baseListWidth = gl.Width;
            double baseListHeight = gl.Height;

            double spaceAvailableForUnselectedItems = 0;
            double numberOfUnselectedItems = 0;
            double leftOverSpace = 0;
            double imageListIndividualExtraMargin = 0;
            
            spaceAvailableForUnselectedItems = baseListWidth - selectedImageWidth;

            numberOfUnselectedItems = spaceAvailableForUnselectedItems / unselectedImageWidth;

            numberOfImageBlockListItems = Convert.ToInt32(Math.Floor(numberOfUnselectedItems)) + 1;

            // we want the list to be an odd number so that we can truly place one of the games in the center
            if (numberOfImageBlockListItems % 2 == 0)
            {
                numberOfImageBlockListItems = numberOfImageBlockListItems - 1;
            }

            leftOverSpace = baseListWidth - ((unselectedImageWidth * (numberOfImageBlockListItems - 1)) + selectedImageWidth);

            if (leftOverSpace > 0)
            {
                imageListIndividualExtraMargin = leftOverSpace / (numberOfImageBlockListItems - 1);
            }

            imageBlockListHalfWayPoint = numberOfImageBlockListItems / 2;
            imageBlockListItemsToPage = imageBlockListHalfWayPoint + 1;

            double widthDiff = selectedImageWidth - unselectedImageWidth;

            double heightDifference = selectedImageHeight - unselectedImageHeight;

            double verticalCenterOfImageList = (selectedImageHeight / 2) - (unselectedImageHeight / 2);

            double listCenter = selectedImageWidth / 2;

            int topHorizontalOffset = (imageBlockListHalfWayPoint - 1);
            int bottomHorizontalOffset = 0;

            for (int i = 0; i < numberOfImageBlockListItems; i++)
            {
                double leftPosition = 0;
                double topPosition = 0;

                System.Windows.Shapes.Shape listImage;

                switch (imageShape)
                {
                    case "circle":
                        listImage = new System.Windows.Shapes.Ellipse();
                        break;
                    case "triangleUp":
                    case "triangleDown":
                    case "triangleLeft":
                    case "triangleRight":
                        if (i == imageBlockListHalfWayPoint)
                        {
                            listImage = GetImageBlockTriangle(selectedImageWidth, selectedImageHeight, imageShape);
                        }
                        else
                        {
                            listImage = GetImageBlockTriangle(unselectedImageWidth, unselectedImageHeight, imageShape);
                        }
                        break;
                    case "star":
                        if (i == imageBlockListHalfWayPoint)
                        {
                            listImage = GetImageBlockStar(selectedImageWidth, selectedImageHeight);
                        }
                        else
                        {
                            listImage = GetImageBlockStar(unselectedImageWidth, unselectedImageHeight);
                        }
                        break;
                    default:
                        listImage = new System.Windows.Shapes.Rectangle();
                        break;
                }


                listImage.Stretch = Stretch.Fill;

                if (i == imageBlockListHalfWayPoint)
                {
                    listImage.Width = selectedImageWidth;
                    listImage.Height = selectedImageHeight;

                    if (numberOfImageBlockListItems == 1)
                    {
                        leftPosition = (baseListWidth - selectedImageHeight) / 2;
                    }
                    else
                    {
                        leftPosition = i * (unselectedImageWidth + imageListIndividualExtraMargin);
                    }
                }
                else if (i > imageBlockListHalfWayPoint)
                {
                    switch (alignment)
                    {
                        case "top":
                            topPosition = 0;
                            break;
                        case "bottom":
                            topPosition = heightDifference;
                            break;
                        default:
                            topPosition = verticalCenterOfImageList;
                            break;
                    }
                    
                    listImage.Width = unselectedImageWidth;
                    listImage.Height = unselectedImageHeight;

                    leftPosition = ((i * (unselectedImageWidth + imageListIndividualExtraMargin)) + widthDiff);
                    
                    bottomHorizontalOffset = bottomHorizontalOffset + 1;
                }
                else
                {
                    listImage.Width = unselectedImageWidth;
                    listImage.Height = unselectedImageHeight;

                    leftPosition = i * (unselectedImageWidth + imageListIndividualExtraMargin);

                    switch (alignment)
                    {
                        case "top":
                            topPosition = 0;
                            break;
                        case "bottom":
                            topPosition = heightDifference;
                            break;
                        default:
                            topPosition = verticalCenterOfImageList;
                            break;
                    }

                    topHorizontalOffset = topHorizontalOffset - 1;

                    gl.Children.Add(listImage);
                }

                if (borderWidth > 0 && borderBrush != null)
                {
                    listImage.StrokeThickness = borderWidth;
                }

                listItemImageBlocks.Add(listImage);
                
                Canvas.SetLeft(listImage, leftPosition);
                Canvas.SetTop(listImage, topPosition);
            }

            // add them in reverse to the bottom
            for (int i = numberOfImageBlockListItems; i-- > imageBlockListHalfWayPoint;)
            {
                //do something
                gl.Children.Add(listItemImageBlocks[i]);
            }
        }

        internal void AddVerticalImageListBlocks(Canvas gl, double selectedImageWidth, double selectedImageHeight, double unselectedImageWidth, double unselectedImageHeight, string imageShape, string alignment, double borderWidth, Brush borderBrush)
        {
            double baseListWidth = gl.Width;
            double baseListHeight = gl.Height;

            double spaceAvailableForUnselectedItems = 0;
            double numberOfUnselectedItems = 0;
            double leftOverSpace = 0;
            double imageListIndividualExtraMargin = 0;

            spaceAvailableForUnselectedItems = baseListHeight - selectedImageHeight;

            numberOfUnselectedItems = spaceAvailableForUnselectedItems / unselectedImageHeight;

            numberOfImageBlockListItems = Convert.ToInt32(Math.Floor(numberOfUnselectedItems)) + 1;

            // we want the list to be an odd number so that we can truly place one of the games in the center
            if (numberOfImageBlockListItems % 2 == 0)
            {
                numberOfImageBlockListItems = numberOfImageBlockListItems - 1;
            }
            
            if (numberOfImageBlockListItems > 3)
            {
                leftOverSpace = baseListHeight - (selectedImageHeight + (unselectedImageHeight * (numberOfImageBlockListItems - 1)) + ((numberOfImageBlockListItems - 2)));
            }
            else
            {
                leftOverSpace = baseListHeight - (selectedImageHeight + (unselectedImageHeight * (numberOfImageBlockListItems - 1)));
            }

            leftOverSpace = baseListHeight - ((unselectedImageHeight * (numberOfImageBlockListItems - 1)) + selectedImageHeight);

            if (leftOverSpace > 0)
            {
                imageListIndividualExtraMargin = leftOverSpace / (numberOfImageBlockListItems - 1);
            }

            imageBlockListHalfWayPoint = numberOfImageBlockListItems / 2;
            imageBlockListItemsToPage = imageBlockListHalfWayPoint + 1;

            double widthDifference = selectedImageWidth - unselectedImageWidth;

            double heightDiff = selectedImageHeight - unselectedImageHeight;

            double centerOfImageList = (selectedImageWidth / 2) - (unselectedImageWidth / 2);

            double listCenter = selectedImageWidth / 2;

            int topHorizontalOffset = (imageBlockListHalfWayPoint - 1);
            int bottomHorizontalOffset = 0;

            for (int i = 0; i < numberOfImageBlockListItems; i++)
            {
                double leftPosition = 0;
                double topPosition = 0;

                System.Windows.Shapes.Shape listImage;

                switch (imageShape)
                {
                    case "circle":
                        listImage = new System.Windows.Shapes.Ellipse();
                        break;
                    case "triangleUp":
                    case "triangleDown":
                    case "triangleLeft":
                    case "triangleRight":
                        if (i == imageBlockListHalfWayPoint)
                        {
                            listImage = GetImageBlockTriangle(selectedImageWidth, selectedImageHeight, imageShape);
                        }
                        else
                        {
                            listImage = GetImageBlockTriangle(unselectedImageWidth, unselectedImageHeight, imageShape);
                        }
                        break;
                    case "star":
                        if (i == imageBlockListHalfWayPoint)
                        {
                            listImage = GetImageBlockStar(selectedImageWidth, selectedImageHeight);
                        }
                        else
                        {
                            listImage = GetImageBlockStar(unselectedImageWidth, unselectedImageHeight);
                        }
                        break;
                    default:
                        listImage = new System.Windows.Shapes.Rectangle();
                        break;
                }


                listImage.Stretch = Stretch.Fill;

                if (i == imageBlockListHalfWayPoint)
                {
                    listImage.Width = selectedImageWidth;
                    listImage.Height = selectedImageHeight;

                    if (numberOfImageBlockListItems == 1)
                    {
                        topPosition = (baseListHeight - selectedImageHeight) / 2;
                    }
                    else
                    {
                        topPosition = i * (unselectedImageHeight + imageListIndividualExtraMargin);
                    }
                }
                else if (i > imageBlockListHalfWayPoint)
                {
                    listImage.Width = unselectedImageWidth;
                    listImage.Height = unselectedImageHeight;

                    topPosition = ((i * (unselectedImageHeight + imageListIndividualExtraMargin)) + heightDiff);

                    switch (alignment)
                    {
                        case "left":
                            leftPosition = 0;
                            break;
                        case "right":
                            leftPosition = widthDifference;
                            break;
                        default:
                            leftPosition = centerOfImageList;
                            break;
                    }
                        
                    bottomHorizontalOffset = bottomHorizontalOffset + 1;
                }
                else
                {
                    listImage.Width = unselectedImageWidth;
                    listImage.Height = unselectedImageHeight;
                    
                    topPosition = i * (unselectedImageHeight + imageListIndividualExtraMargin);

                    switch (alignment)
                    {
                        case "left":
                            leftPosition = 0;
                            break;
                        case "right":
                            leftPosition = widthDifference;
                            break;
                        default:
                            leftPosition = centerOfImageList;
                            break;
                    }

                    topHorizontalOffset = topHorizontalOffset - 1;

                    gl.Children.Add(listImage);
                }

                if (borderWidth > 0 && borderBrush != null)
                {
                    listImage.StrokeThickness = borderWidth;
                }
                
                listItemImageBlocks.Add(listImage);
                
                Canvas.SetLeft(listImage, leftPosition);
                Canvas.SetTop(listImage, topPosition);
            }

            // add them in reverse to the bottom
            for (int i = numberOfImageBlockListItems; i-- > imageBlockListHalfWayPoint;)
            {
                //do something
                gl.Children.Add(listItemImageBlocks[i]);
            }
        }

        internal void AddListImageBlocks(Canvas gl, double selectedImageWidth, double selectedImageHeight, double unselectedImageWidth, double unselectedImageHeight, double baseAngle, string imageOrientationString, string rotationDirection, string imageShape, string alignment, double borderWidth, Brush borderBrush)
        {
            //System.Windows.Shapes.Shape.ci
            
            if (gl != null)
            {
                imageListSelectedWidth = Convert.ToInt32(selectedImageWidth);
                imageListUnselectedWidth = Convert.ToInt32(unselectedImageWidth);

                // if it's horizontal
                if (string.Equals(imageOrientationString, "horizontal", StringComparison.InvariantCultureIgnoreCase))
                {
                    AddHorizontalImagelistBlocks(gl, selectedImageWidth, selectedImageHeight, unselectedImageWidth, unselectedImageHeight, imageShape, alignment, borderWidth, borderBrush);
                }
                else
                {
                    AddVerticalImageListBlocks(gl, selectedImageWidth, selectedImageHeight, unselectedImageWidth, unselectedImageHeight, imageShape, alignment, borderWidth, borderBrush);
                }  
            }
        }

        internal void AddGameListTextBlocks(Canvas gl, Canvas glOutlines, Brush outlineColor)
        {
            try
            {
                if (gl != null)
                {
                    double heightDiff = selectedGameHeight - unselectedGameHeight;

                    for (int i = 0; i < numberOfTextBlockListItems; i++)
                    {
                        BigBlue.Models.ListItemBlock lib = new BigBlue.Models.ListItemBlock();
                        listItemBlocks.Add(lib);

                        // top left
                        AddTextBlocksForListItem(gl, glOutlines, i, heightDiff, -1, -1, lib, outlineColor);

                        // top right
                        AddTextBlocksForListItem(gl, glOutlines, i, heightDiff, 1, -1, lib, outlineColor);

                        // bottom left
                        AddTextBlocksForListItem(gl, glOutlines, i, heightDiff, -1, 1, lib, outlineColor);

                        // bottom right
                        AddTextBlocksForListItem(gl, glOutlines, i, heightDiff, 1, 1, lib, outlineColor);

                        gameListElements.Add(AddTextBlocksForListItem(gl, glOutlines, i, heightDiff, 0, 0, lib, outlineColor));
                        //Canvas.SetTop(gameListEntry, (i * (97 * resolutionXMultiplier)) + (i * individualExtraMargin));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding list text blocks: " + ex.Message, "Big Blue");
            }

            //gameListElements = GameList.Children;
        }

        internal TextBlock AddTextBlocksForListItem(Canvas gl, Canvas glOutlines, int i, double heightDiff, double xOffset, double yOffset, BigBlue.Models.ListItemBlock lib, Brush outlineColor)
        {
            TextBlock gameListEntry = new TextBlock();

            // at low resolutions, the text shouldn't be anti-aliased; there aren't enough pixels for it
            if (!antialiasedText)
            {
                TextOptions.SetTextRenderingMode(gameListEntry, TextRenderingMode.Aliased);
                TextOptions.SetTextFormattingMode(gameListEntry, TextFormattingMode.Display);
            }

            gameListEntry.Focusable = false;
            gameListEntry.TextTrimming = TextTrimming.CharacterEllipsis;
            gameListEntry.IsHitTestVisible = false;
            gameListEntry.TextWrapping = TextWrapping.NoWrap;
            gameListEntry.AllowDrop = false;
            gameListEntry.IsEnabled = false;
            gameListEntry.IsManipulationEnabled = false;

            double leftPosition = 0;
            double topPosition = 0;

            Canvas.SetLeft(gameListEntry, leftPosition);

            gameListEntry.Width = gl.Width;
            gameListEntry.MaxWidth = gl.Width;
            gameListEntry.MinWidth = gl.Width;


            // going to try to scale the text regardless of stretch setting

            //gameListEntry.Height = 97 * resolutionXMultiplier;
            gameListEntry.Text = string.Empty;
            gameListEntry.FontFamily = gameListFont;
            gameListEntry.FontWeight = FontWeights.Bold;
            gameListEntry.TextAlignment = gameListAlignment;
            //gameListEntry.ClipToBounds = true;

            if (i == textBlockListHalfWayPoint)
            {
                gameListEntry.Height = selectedGameHeight;
                gameListEntry.MaxHeight = selectedGameHeight;
                gameListEntry.MinHeight = selectedGameHeight;

                //gameListEntry.Foreground = yellowBrush;
                gameListEntry.Padding = selectedGamePadding;
                //gameListEntry.Height = selectedGameHeight;
                gameListEntry.FontSize = selectedTextSize;

                if (numberOfTextBlockListItems == 1)
                {
                    topPosition = (gl.Height - selectedGameHeight) / 2;
                }
                else
                {
                    topPosition = i * (unselectedGameHeight + individualExtraMargin);
                }
            }
            else if (i > textBlockListHalfWayPoint)
            {
                gameListEntry.Height = unselectedGameHeight;
                gameListEntry.MaxHeight = unselectedGameHeight;
                gameListEntry.MinHeight = unselectedGameHeight;

                //gameListEntry.Height = unselectedGameHeight;
                gameListEntry.Padding = gamePadding;
                gameListEntry.FontSize = unselectedTextSize;

                topPosition = ((i * (unselectedGameHeight + individualExtraMargin)) + heightDiff);
            }
            else
            {
                gameListEntry.Height = unselectedGameHeight;
                gameListEntry.MaxHeight = unselectedGameHeight;
                gameListEntry.MinHeight = unselectedGameHeight;

                //gameListEntry.Height = unselectedGameHeight;
                gameListEntry.Padding = gamePadding;
                gameListEntry.FontSize = unselectedTextSize;

                topPosition = i * (unselectedGameHeight + individualExtraMargin);
            }

            Canvas.SetTop(gameListEntry, topPosition + yOffset);
            Canvas.SetLeft(gameListEntry, leftPosition + xOffset);

            //gameListEntry.Background = new SolidColorBrush(Colors.Green);                                

            // if there's any offset, we know that it's for the outline and so will always be blue
            if (xOffset != 0 || yOffset != 0)
            {
                gameListEntry.Foreground = outlineColor;
                glOutlines.Children.Add(gameListEntry);
            }
            else
            {
                gl.Children.Add(gameListEntry);
            }

            Binding b = new Binding("Name");
            b.Source = lib;
            b.Mode = BindingMode.OneWay;
            
            gameListEntry.SetBinding(TextBlock.TextProperty, b);

            return gameListEntry;
        }

        // this is experimental to see how it works to repeat the video without reloading the source
        internal void ReplayVideo()
        {
            videoMe.Volume = minimumVolume;
            videoMaterializing = false;
            videoFading = false;
            //videoLength = 0;

            videoFadeOutStopwatch.Stop();
            videoFadeOutStopwatch.Reset();

            videoFadeInStopwatch.Stop();
            videoFadeInStopwatch.Reset();

            //videoMe.Stop();
            //videoMe.Close();
            //videoMe.Clock = null;

            //videoMe.Source = null;

            isVideoPlaying = false;

            // stop the storyboards, which will put the opacity on the video canvas back at 0
            if (DependencyPropertyHelper.GetValueSource(videoCanvas, Canvas.OpacityProperty).IsAnimated)
            {
                videoFadeOutStoryboard.Stop();
                videoFadeInStoryboard.Stop();
            }
        }

        private void ClearVideo()
        {
            videoMe.Stop();
            videoMe.Close();
            videoMe.Clock = null;

            videoMe.Source = null;
        }

        internal void StopVideo()
        {
            //Application.Current.Dispatcher.Invoke((Action)delegate
            //{
                videoMe.Volume = minimumVolume;
                videoMaterializing = false;
                videoFading = false;
                videoLength = 0;

                videoFadeOutStopwatch.Stop();
                videoFadeOutStopwatch.Reset();

                videoFadeInStopwatch.Stop();
                videoFadeInStopwatch.Reset();

                ClearVideo();
                            
                isVideoPlaying = false;
                
                // stop the storyboards, which will put the opacity on the video canvas back at 0
                if (DependencyPropertyHelper.GetValueSource(videoCanvas, Canvas.OpacityProperty).IsAnimated)
                {
                    videoFadeOutStoryboard.Stop();
                    videoFadeInStoryboard.Stop();
                }
            //});
        }

        internal void PlayMediaElement()
        {
            videoMaterializing = true;
            isVideoPlaying = true;

            // start the video fade in storyboard
            videoFadeInStoryboard.Begin();

            // restart the video fade out stop watch
            videoFadeOutStopwatch.Restart();

            if (pausedBySystem == true)
            {
                videoFadeInStoryboard.Pause();
                videoFadeInStopwatch.Stop();
            }
        }

        internal void ConfigureSecondaryDisplay(System.Windows.Forms.Screen screen, System.Drawing.Rectangle workingArea, BigBlue.SecondaryWindow secondaryWindow)
        {
            if (secondaryWindow.IsLoaded == false)
            {
                secondaryWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            }

            int secondaryScreenWidth = screen.Bounds.Width;
            int secondaryScreenHeight = screen.Bounds.Height;

            secondaryWindow.Top = workingArea.Top;
            secondaryWindow.Left = workingArea.Left;
            secondaryWindow.Width = secondaryScreenWidth;
            secondaryWindow.Height = secondaryScreenHeight;

            secondaryWindow.SecondaryWindowStatic.Width = secondaryScreenWidth;
            secondaryWindow.SecondaryWindowStatic.Height = secondaryScreenHeight;

            secondaryWindow.SecondaryWindowSnapshot.Width = secondaryScreenWidth;
            secondaryWindow.SecondaryWindowSnapshot.Height = secondaryScreenHeight;

            secondaryWindow.SecondaryWindowContainer.Width = secondaryScreenWidth;
            secondaryWindow.SecondaryWindowContainer.Height = secondaryScreenHeight;

            if (secondaryWindow.IsLoaded == true)
            {
                secondaryWindow.WindowState = WindowState.Maximized;
            }

            secondaryWindow.Topmost = true;
            secondaryWindow.Show();
        }

        internal System.Windows.Forms.Screen FindScreen(string screenName)
        {
            return System.Windows.Forms.Screen.AllScreens.FirstOrDefault(s => s.DeviceName == screenName);
            /*
            System.Windows.Forms.Screen res =
            if (res == null)
            {
                //res = System.Windows.Forms.Screen.AllScreens[0];
            }
                
            return res;
            */
        }

        internal void ProvisionSecondaryDisplay(string displayName, string displayType)
        {
            if (displayName != "None" && !string.IsNullOrWhiteSpace(displayName) && System.Windows.Forms.Screen.AllScreens.Length > 1)
            {
                System.Windows.Forms.Screen s2 = FindScreen(displayName);
                System.Drawing.Rectangle r2 = s2.WorkingArea;

                if (s2 != null)
                {
                    switch (displayType)
                    {
                        case "marquee":
                            marqueeDisplay = true;
                            marqueeWindow = new BigBlue.SecondaryWindow(s2);

                            ConfigureSecondaryDisplay(s2, r2, marqueeWindow);
                            break;
                        case "flyer":
                            flyerDisplay = true;
                            flyerWindow = new BigBlue.SecondaryWindow(s2);

                            ConfigureSecondaryDisplay(s2, r2, flyerWindow);
                            break;
                        case "instruction":
                            instructionDisplay = true;
                            instructionWindow = new BigBlue.SecondaryWindow(s2);

                            ConfigureSecondaryDisplay(s2, r2, instructionWindow);
                            break;
                    }
                }
            }
        }

        internal void ProvisionSecondaryDisplays()
        {
            // provision the marquee window
            ProvisionSecondaryDisplay(marqueeDisplayName, "marquee");

            // provision the flyer window
            ProvisionSecondaryDisplay(flyerDisplayName, "flyer");

            // provision instructions window
            ProvisionSecondaryDisplay(instructionDisplayName, "instruction");
        }

        internal const int baseHorizontalResolution = 1920;

        internal static double SetResolutionMultiplier(double inputResolutionX, double inputResolutionY, bool stretch, bool cleanStretch)
        {
            double multiplier = 1;

            if (stretch == true || inputResolutionX < baseHorizontalResolution)
            {
                multiplier = (inputResolutionX / baseHorizontalResolution);
            }

            // if clean stretch is enabled, we're going to multiply by the biggest integer
            if (stretch == true && cleanStretch == true && multiplier > 1)
            {
                multiplier = (Math.Floor(multiplier));
            }

            return multiplier;
        }

        internal void UpdateTextBlockText()
        {
            int reverseCount = selectedListItemIndex;
            int forwardCount = selectedListItemIndex;

            // loop through the entries before the middle
            for (int i = textBlockListHalfWayPoint; i-- > 0;)
            {
                reverseCount = reverseCount - 1;

                if (reverseCount >= 0 && reverseCount <= frontendLists[selectedListGuid].Total)
                {
                    listItemBlocks[i].Name = frontendLists[selectedListGuid].ListItems[reverseCount].Title;
                    //gameListElements[i].Text = frontendLists[selectedListGuid].ListItems[reverseCount].Title;
                }
                else
                {
                    if (reverseCount < 0 && frontendLists[selectedListGuid].Total >= textBlockListHalfWayPoint)
                    {
                        reverseCount = frontendLists[selectedListGuid].Total;

                        listItemBlocks[i].Name = frontendLists[selectedListGuid].ListItems[reverseCount].Title;
                        //gameListElements[i].Text = frontendLists[selectedListGuid].ListItems[reverseCount].Title;
                    }
                    else
                    {
                        listItemBlocks[i].Name = string.Empty;
                        //gameListElements[i].Text = string.Empty;
                    }

                }
            }

            /*
            if you wanted to indicate that it has children
            if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID != null)
            {
                listItemName3 = listItemName3 + "⏵";
            }
            */

            listItemBlocks[textBlockListHalfWayPoint].Name = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Title;

            // loop through the entries after the middle
            for (int i = textBlockListItemsToPage; i < numberOfTextBlockListItems; i++)
            {
                forwardCount = forwardCount + 1;

                if (forwardCount >= 0 && forwardCount <= frontendLists[selectedListGuid].Total)
                {
                    listItemBlocks[i].Name = frontendLists[selectedListGuid].ListItems[forwardCount].Title;
                    //gameListElements[i].Text = frontendLists[selectedListGuid].ListItems[forwardCount].Title;
                }
                else
                {
                    if (forwardCount > frontendLists[selectedListGuid].Total && frontendLists[selectedListGuid].Total >= textBlockListHalfWayPoint)
                    {
                        forwardCount = 0;

                        listItemBlocks[i].Name = frontendLists[selectedListGuid].ListItems[forwardCount].Title;
                        //gameListElements[i].Text = frontendLists[selectedListGuid].ListItems[forwardCount].Title;
                    }
                    else
                    {
                        listItemBlocks[i].Name = string.Empty;
                        //gameListElements[i].Text = string.Empty;
                    }
                }
            }
        }

        internal void SetGameSnapshots(bool versus)
        {
            List<BigBlue.Models.FrontendSnapshot> snapshots = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Snapshots;

            foreach (BigBlue.Models.FrontendSnapshot snap in snapshots)
            {
                if (snap.Path == null || versus)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        snap.ImageControl.Source = versusThumbnailImage;
                    });
                }
                else
                {
                    //BitmapImage imageSource = await Task.Run(() => { return GetBitMapImageFromPath(snap, snap.Width); });
                    BitmapImage imageSource = GetBitMapImageFromPath(snap, snap.Width);

                    //BitmapImage imageSource = GetBitMapImageFromPath(snap.Path);

                    //Application.Current.Dispatcher.Invoke((Action)delegate
                    //{
                    snap.ImageControl.Source = imageSource;
                    //});
                }
            }

            if (versus)
            {
                StopVideo();
            }
        }

        protected virtual void FinalListRender(MediaElement videoMe, System.Windows.Controls.Panel videoCanvas)
        {
        }

        public virtual void OnFrame(object sender, EventArgs e)
        {
        }

        internal void ResetVideo()
        {
            if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Video != null)
            {
                if (itsGoTime == false)
                {
                    videoFadeInStopwatch.Restart();
                }
            }
        }

        internal void RenderGameList(bool playSound)
        {
            gameListTimer.Reset();
            gameListTimer.Start();

            if (frontendLists[selectedListGuid].Total > -1)
            {
                if (playSound == true)
                {
                    PlayMenuSound();
                }

                FinalListRender(videoMe, videoCanvas);
            }
            else
            {
                SetGameSnapshots(true);
                StopVideo();
                ResetVideo();
            }

            lastGameIndex = selectedListItemIndex;
        }

        internal void SelectList(Guid listIdToLoad, bool back)
        {
            // 8/2/2016 commented this out; might need to re-enable it
            //stopVideo();

            if (!back)
            {
                selectedListItemIndex = frontendLists[listIdToLoad].CurrentListIndex;
            }

            // set the name of the list
            currentListName.Name = frontendLists[listIdToLoad].ListName;
            selectedListGuid = listIdToLoad;

            // render the game list
            RenderGameList(false);

            ResetVideo();
        }

        internal void PauseVideo(MediaElement videoMe, System.Windows.Controls.Panel videoCanvas)
        {
            if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Video != null)
            {
                if (videoFadeInStopwatch.IsRunning == true)
                {
                    videoFadeInStopwatch.Stop();
                }

                if (videoFadeOutStopwatch.IsRunning == true)
                {
                    videoFadeOutStopwatch.Stop();
                }

                if (DependencyPropertyHelper.GetValueSource(videoCanvas, Canvas.OpacityProperty).IsAnimated)
                {
                    if (videoFading == true)
                    {
                        videoFadeOutStoryboard.Pause();
                    }

                    if (videoMaterializing == true)
                    {
                        videoFadeInStoryboard.Pause();
                    }
                }

                videoMe.Pause();
            }
        }

        internal void CalculateGame(int change)
        {
            int prospectiveIndex = selectedListItemIndex + change;

            if (prospectiveIndex > frontendLists[selectedListGuid].Total)
            {
                prospectiveIndex = (prospectiveIndex - frontendLists[selectedListGuid].Total) - 1;
            }
            else if (prospectiveIndex < 0)
            {
                prospectiveIndex = (frontendLists[selectedListGuid].Total + prospectiveIndex) + 1;
            }

            selectedListItemIndex = prospectiveIndex;

            if (RenderGameListCheck() == true)
            {
                RenderGameList(true);
            }
        }

        internal void ChangeListItem(int change)
        {
            if (gameListTimer.ElapsedMilliseconds > timeToChange)
            {
                if (timeToChange > timeToChangeScale)
                {
                    timeToChange = timeToChange - timeToChangeScale;
                }

                gameListTimer.Restart();

                CalculateGame(change);
            }
        }

        internal bool OpenSubFolderListItem(string path, Guid listGuid)
        {
            bool liValid = false;

            try
            {
                //liValid = await 
                //string searchPattern = frontendLists[listGuid].ListItems[selectedListItemIndex].FolderSearchPattern;

                liValid = WalkDirectoryTree(null, path, listGuid, originatingFolderParentListItem.FolderSearchPattern);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            lastFolder = path;

            return liValid;
        }

        internal bool OpenFolderListItem(Guid listGuid)
        {
            bool isValid = false;

            try
            {
                string prospectivePath = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder;
                string searchPattern = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].FolderSearchPattern;

                //liValid = await 
                isValid = WalkDirectoryTree(null, prospectivePath, listGuid, searchPattern);

                lastFolder = prospectivePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return isValid;
        }

        internal void LoadFrontendList()
        {
            bool isListValid = false;

            string listPath = Path + @"\lists.xml";

            if (System.IO.File.Exists(listPath))
            {
                XDocument doc = XDocument.Load(Path + @"\lists.xml");

                if (doc.Root != null)
                {
                    if (doc.Root.Element("item") != null)
                    {
                        IEnumerable<XElement> categoryArray = doc.Root.Elements("item").ToArray();

                        LoadFrontendSubList(null, string.Empty, categoryArray, -1);
                        isListValid = true;
                    }
                }
            }

            if (!isListValid)
            {
                BigBlue.Models.FrontendListItem list = new BigBlue.Models.FrontendListItem();
                list.ID = Guid.NewGuid();

                // start off with a 0 index
                //list.LastListIndex = 0;

                // observable collection experiment
                //list.ListItems = new List<BigBlue.Models.FrontendListItem>();
                list.ListItems = new System.Collections.ObjectModel.ObservableCollection<BigBlue.Models.FrontendListItem>();

                BigBlue.Models.FrontendListItem noListFoundItem = new BigBlue.Models.FrontendListItem();
                noListFoundItem.Title = "No lists found";

                noListFoundItem.Snapshots = new List<BigBlue.Models.FrontendSnapshot>();

                // try to add the snapshots if they're available and configured

                BigBlue.Models.FrontendSnapshot snap = new BigBlue.Models.FrontendSnapshot();
                snap.ImageControl = snapshotImageControl;
                noListFoundItem.Snapshots.Add(snap);

                if (marqueeDisplay)
                {
                    BigBlue.Models.FrontendSnapshot snap2 = new BigBlue.Models.FrontendSnapshot();
                    snap2.ImageControl = this.marqueeWindow.SecondaryWindowSnapshot;
                    noListFoundItem.Snapshots.Add(snap2);
                }

                if (flyerDisplay)
                {
                    BigBlue.Models.FrontendSnapshot snap3 = new BigBlue.Models.FrontendSnapshot();
                    snap3.ImageControl = this.flyerWindow.SecondaryWindowSnapshot;
                    noListFoundItem.Snapshots.Add(snap3);
                }

                if (instructionDisplay)
                {
                    BigBlue.Models.FrontendSnapshot snap4 = new BigBlue.Models.FrontendSnapshot();
                    snap4.ImageControl = this.instructionWindow.SecondaryWindowSnapshot;
                    noListFoundItem.Snapshots.Add(snap4);
                }


                list.ListItems.Add(noListFoundItem);

                frontendLists.Add(list.ID, list);
            }

            if (selectedListGuid == Guid.Empty)
            {
                selectedListGuid = frontendLists.LastOrDefault().Key;
            }
            else
            {
                //  SetDefaultListIndexes(selectedListGuid);
            }

            BigBlue.Models.FrontendListItem windowsFolderList = new BigBlue.Models.FrontendListItem();
            windowsFolderList.ID = folderListGuid;
            frontendLists.Add(folderListGuid, windowsFolderList);

            BigBlue.Models.FrontendListItem speechSearchList = new BigBlue.Models.FrontendListItem();
            speechSearchList.ListName = "Search Results";
            speechSearchList.ID = searchListGuid;
            // observable collection experiment
            //speechSearchList.ListItems = new List<BigBlue.Models.FrontendListItem>();
            speechSearchList.ListItems = new System.Collections.ObjectModel.ObservableCollection<BigBlue.Models.FrontendListItem>();

            frontendLists.Add(searchListGuid, speechSearchList);

            BigBlue.Speech.InitializeVoiceRecognition(this);
        }

        internal void ResetVideoElementVolume()
        {
            wmpVolume = originalWmpVolume;
            //VideoElement.Volume = Video.Opacity * wmpVolume;
        }

        internal async void PlayLoseSound()
        {
            if (BigBlue.XAudio2Player.Disabled)
            {
                await Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(1500);
                });

                awaitingAsync = false;
            }
            else
            {
                BigBlue.XAudio2Player.PlaySound(FailureSoundKey, null);
            }
        }

        internal void RenderFolderList(Guid listGuid)
        {
            //frontendLists[selectedListGuid].LastListIndex = selectedListItemIndex;

            selectedListItemIndex = 0;

            // set this so that the user can't do anything else during the async load
            //awaitingAsync = true;

            // populate the game list array
            //string result = await asyncPopulateGamesList(listToLoad);
            selectedListGuid = listGuid;

            // render the game list
            RenderGameList(false);

            //GameList.Visibility = System.Windows.Visibility.Visible;
            //SnapShotRectangle.Opacity = 1;

            ResetVideo();
        }

        internal void RenderSubFolderList(MediaElement videoMe, System.Windows.Controls.Panel videoCanvas)
        {
            //frontendLists[folderListGuid].LastListIndex = 0;

            selectedListItemIndex = 0;

            // set this so that the user can't do anything else during the async load
            //awaitingAsync = true;

            // we don't want to change the selected list guid?
            selectedListGuid = folderListGuid;

            // render the game list
            RenderGameList(false);

            //GameList.Visibility = System.Windows.Visibility.Visible;
            //SnapShotRectangle.Opacity = 1;

            ResetVideo();
        }

        private void VideoElement_MediaOpened(object sender, RoutedEventArgs e, MediaElement videoMe)
        {
            // you only want to set the video length after you first load the video; no point resetting it every time
            if (videoMe.NaturalDuration.HasTimeSpan == true)
            {
                double videoLengthMilliseconds = videoMe.NaturalDuration.TimeSpan.TotalMilliseconds;

                // check video length
                // shouldn't the video have to be at least as long as the fade in and fade out duration + whatever the minimum length of the video we decide on?
                if (videoLengthMilliseconds >= (videoFadeInDuration + videoFadeOutDuration + minimumVideoLength))
                {
                    videoLength = videoLengthMilliseconds;
                    PlayMediaElement();
                }
            }
        }

        private void VideoFadeOutStoryboard_Completed(object sender, EventArgs e)
        {
            // this is experimental
            // re-enable this to get the old behavior back
            /*
            stopVideo();

            if (loopVideo)
            {
                videoFadeInStopwatch.Start();
            }
            */
        }

        private void VideoFadeInStoryboard_Completed(object sender, EventArgs e, MediaElement videoMe)
        {
            videoMaterializing = false;
            // adjust it one last time given the latest state
            if (!itsGoTime)
            {
                videoMe.Volume = wmpVolume;
            }
        }

        internal void ProvisionUIAnimations(System.Windows.FrameworkElement layoutRoot, System.Windows.FrameworkElement feContainer)
        {
            ProvisionVideoAnimations();
            ProvisionScreenSaverAnimations(layoutRoot);
            ProvisionFrontendFadeAnimations(feContainer);
        }

        internal virtual void ProvisionFrontendFadeAnimations(System.Windows.FrameworkElement feContainer)
        {
        }

        internal void ProvisionScreenSaverAnimations(System.Windows.FrameworkElement layoutRoot)
        {
            DoubleAnimation screenSaverStartAnimation = new DoubleAnimation();
            screenSaverStartAnimation.From = 1;
            screenSaverStartAnimation.To = 0;
            screenSaverStartAnimation.SpeedRatio = 1;
            screenSaverStartAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            screenSaverStartAnimation.AutoReverse = false;

            Storyboard.SetTarget(screenSaverStartAnimation, layoutRoot);
            Storyboard.SetTargetProperty(screenSaverStartAnimation, new PropertyPath("Opacity"));

            screenSaverStartAnimation.Freeze();
            ScreensaverStartStoryboard.Children.Add(screenSaverStartAnimation);
            ScreensaverStartStoryboard.Freeze();

            DoubleAnimation screenSaverEndAnimation = new DoubleAnimation();
            screenSaverEndAnimation.From = 0;
            screenSaverEndAnimation.To = 1;
            screenSaverEndAnimation.SpeedRatio = 1;
            screenSaverEndAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            screenSaverEndAnimation.AutoReverse = false;

            Storyboard.SetTarget(screenSaverEndAnimation, layoutRoot);
            Storyboard.SetTargetProperty(screenSaverEndAnimation, new PropertyPath("Opacity"));

            screenSaverEndAnimation.Freeze();
            ScreensaverEndStoryboard.Children.Add(screenSaverEndAnimation);
            ScreensaverEndStoryboard.Freeze();
        }

        internal void ProvisionVideoAnimations()
        {
            videoMe.LoadedBehavior = MediaState.Manual;
            videoMe.MediaOpened += new RoutedEventHandler((sender, e) => VideoElement_MediaOpened(sender, e, videoMe));
            videoMe.MediaEnded += new RoutedEventHandler((sender, e) => VideoElement_MediaEnded(sender, e, videoMe));

            DoubleAnimation videoFadeOutAnimation = new DoubleAnimation();
            videoFadeOutAnimation.From = 1;
            videoFadeOutAnimation.To = 0;
            videoFadeOutAnimation.SpeedRatio = 1;
            videoFadeOutAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(videoFadeOutDuration));
            videoFadeOutAnimation.AutoReverse = false;

            Storyboard.SetTarget(videoFadeOutAnimation, videoCanvas);
            Storyboard.SetTargetProperty(videoFadeOutAnimation, new PropertyPath("Opacity"));

            videoFadeOutAnimation.Freeze();

            videoFadeOutStoryboard = new Storyboard();
            videoFadeOutStoryboard.Children.Add(videoFadeOutAnimation);
            videoFadeOutStoryboard.Completed += new EventHandler(VideoFadeOutStoryboard_Completed);
            videoFadeOutStoryboard.Freeze();

            videoFadeOutStoryboard.Begin();
            videoFadeOutStoryboard.Stop();

            DoubleAnimation videoFadeInAnimation = new DoubleAnimation();
            videoFadeInAnimation.From = 0;
            videoFadeInAnimation.To = 1;
            videoFadeInAnimation.SpeedRatio = 1;
            videoFadeInAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(videoFadeInDuration));
            videoFadeInAnimation.AutoReverse = false;

            Storyboard.SetTarget(videoFadeInAnimation, videoCanvas);
            Storyboard.SetTargetProperty(videoFadeInAnimation, new PropertyPath("Opacity"));

            videoFadeInAnimation.Freeze();

            videoFadeInStoryboard = new Storyboard();
            videoFadeInStoryboard.Children.Add(videoFadeInAnimation);
            videoFadeInStoryboard.Completed += new EventHandler((sender, e) => VideoFadeInStoryboard_Completed(sender, e, videoMe));
            videoFadeInStoryboard.Freeze();

            videoFadeInStoryboard.Begin();
            videoFadeInStoryboard.Stop();
        }

        private void VideoElement_MediaEnded(object sender, RoutedEventArgs e, MediaElement videoMe)
        {
            ReplayVideo();

            if (loopVideo)
            {
                videoFadeInStopwatch.Start();
            }
        }

        internal double CorrectAspectRatio(double inputResolutionX, double inputResolutionY, bool keepAspectRatio, int aspectRatioIndex)
        {
            // correct the aspect ratio
            if (keepAspectRatio == true)
            {
                if (aspectRatioIndex == 1)
                {
                    return inputResolutionY * aspectRatio34;
                }
                else if (aspectRatioIndex == 2)
                {
                    return inputResolutionX / aspectRatio169;
                }
                else
                {
                    return inputResolutionY * aspectRatio43;
                }
            }

            return inputResolutionX;
        }

        internal void FocusBigBlue()
        {
            BigBlue.NativeMethods.SetForegroundWindow(windowHandle);
        }

        internal virtual void ProcessFrontendAction(string action, bool? inputDown)
        {
        }

        internal virtual void BackToGameList()
        {
            
        }

        internal void ReturnFromPreLaunch(System.Windows.Controls.Panel feContainer)
        {
            try
            {
                string directoryToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Directory;
                string fileNameToLaunch = directoryToLaunch + @"\" + frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Binary;
                string argumentsToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Arguments;

                LaunchProgram(feContainer, directoryToLaunch, fileNameToLaunch, argumentsToLaunch, BigBlue.Models.LaunchType.main);
            }
            catch (Exception)
            {
            }
        }

        internal bool ValidateProgramLaunch()
        {
            bool validLaunch = false;

            if (!string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PreDirectory) && !string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PreBinary))
            {
                validLaunch = true;
                lt = BigBlue.Models.LaunchType.pre;
                directoryToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PreDirectory;
                fileNameToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PreBinary;
                argumentsToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PreArguments;
            }
            else
            {
                lt = BigBlue.Models.LaunchType.main;
                directoryToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Directory;
                fileNameToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Binary;
                argumentsToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Arguments;

                if (!string.IsNullOrWhiteSpace(directoryToLaunch) && !string.IsNullOrWhiteSpace(fileNameToLaunch))
                {
                    validLaunch = true;
                }
            }

            if (validLaunch)
            {
                fileNameToLaunch = directoryToLaunch + @"\" + fileNameToLaunch;

                if (!System.IO.File.Exists(fileNameToLaunch))
                {
                    validLaunch = false;
                }
            }

            return validLaunch;
        }

        internal void ExitFrontEnd(Canvas feCanvas)
        {
            switch (exitMode)
            {
                case BigBlue.Models.FrontEndExitMode.quit:
                    openExplorerOnQuit = true;
                    Close();
                    break;
                case BigBlue.Models.FrontEndExitMode.restart:
                    openExplorerOnQuit = false;
                    ShutDownSystem(false);
                    break;
                case BigBlue.Models.FrontEndExitMode.shutdown:
                    openExplorerOnQuit = false;
                    ShutDownSystem(true);
                    break;
            }
        }

        private void ShutDownSystem(bool shutDownSystem)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "shutdown";

            if (shutDownSystem == false)
            {
                startInfo.Arguments = "/r /t 0";
            }
            else
            {
                startInfo.Arguments = "/s /t 0";
            }

            Process.Start(startInfo);
        }

        // only release them if they're actually pressed
        internal void ReleaseInput(string action)
        {
            frontendInputs[action].isPressed = false;
            frontendInputs[action].isRepeating = false;
            frontendInputs[action].wasPressed = false;
        }

        internal void ReleaseAllInputs()
        {
            foreach (KeyValuePair<string, BigBlue.Models.FrontendInputState> inputState in frontendInputs)
            {
                frontendInputs[inputState.Key].isPressed = false;
                frontendInputs[inputState.Key].isRepeating = false;
                frontendInputs[inputState.Key].wasPressed = false;
            }
        }

        internal void RegisterHotKey(int hotKeyId, uint hotKey)
        {
            //var helper = new WindowInteropHelper(this);

            if (!BigBlue.NativeMethods.RegisterHotKey(windowHandle, hotKeyId, BigBlue.NativeMethods.MOD_CTRL, hotKey))
            {
                // handle error
            }
        }

        internal void UnregisterHotKey(int hotkeyId)
        {
            //var helper = new WindowInteropHelper(this);
            BigBlue.NativeMethods.UnregisterHotKey(windowHandle, hotkeyId);
        }

        internal void TrapMouseCursor(System.Windows.Controls.Panel frontendContainer)
        {
            Point frontEndCoordinates = frontendContainer.PointToScreen(new Point(0, 0));

            int frontEndLeftOffset = (int)frontEndCoordinates.X;
            int frontEndTopOffset = (int)frontEndCoordinates.Y;

            BigBlue.NativeMethods.SetCursorPos(frontEndLeftOffset, frontEndTopOffset);

            //MessageBox.Show(fu.X.ToString());
            System.Drawing.Rectangle r = new System.Drawing.Rectangle(frontEndLeftOffset, frontEndTopOffset, (int)width + frontEndLeftOffset, (int)height + frontEndTopOffset);
            BigBlue.NativeMethods.ClipCursor(ref r);
        }

        protected override void OnClosed(EventArgs e)
        {
            // this unregisters the global hotkey
            //win32Window.RemoveHook(HwndHook);
            //win32Window = null;
            //UnregisterHotKey(ESC_OVERRIDE_HOTKEY_ID);

            // unregister the volume controls
            //UnregisterHotKey(VOLUME_DOWN_HOTKEY_ID);
            //UnregisterHotKey(VOLUME_UP_HOTKEY_ID);
            //UnregisterHotKey(VOLUME_MUTE_HOTKEY_ID);

            // restore mouse cursor
            if (hideMouseCursor == true)
            {
                BigBlue.NativeMethods.SystemParametersInfo(BigBlue.NativeMethods.SPI_SETCURSORS, 0, IntPtr.Zero, 0);
            }

            base.OnClosed(e);
        }

        internal void ReturnFromGame(System.Windows.Controls.Panel feCanvas)
        {
            if (string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PostDirectory) || string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PostBinary))
            {
                BackToGameList();

                if (mouseCursorTrapped == true)
                {
                    TrapMouseCursor(feCanvas);
                }   
            }
            else
            {
                string directoryToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PostDirectory;
                string fileNameToLaunch = directoryToLaunch + @"\" + frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PostBinary;
                string argumentsToLaunch = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].PostArguments;

                LaunchProgram(feCanvas, directoryToLaunch, fileNameToLaunch, argumentsToLaunch, BigBlue.Models.LaunchType.post);
            }
        }

        internal void LaunchProgram(System.Windows.Controls.Panel feContainer, string dir, string fileName, string args, BigBlue.Models.LaunchType type)
        {
            try
            {
                if (BigBlue.XAudio2Player.xaudio2 != null)
                {
                    BigBlue.XAudio2Player.StopAudioEngine();
                }
                                
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = dir;
                startInfo.UseShellExecute = false;
                startInfo.ErrorDialog = false;
                
                //startInfo.UseShellExecute = true;

                // if it's anything other than the shell, we don't want to create a window
                if (type != BigBlue.Models.LaunchType.shell)
                {
                    startInfo.CreateNoWindow = true;
                }

                startInfo.FileName = fileName;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = args;

                Process process = new Process();
                process.StartInfo = startInfo;

                switch (type)
                {
                    case BigBlue.Models.LaunchType.pre:
                        process.EnableRaisingEvents = true;

                        process.Exited += (a, b) =>
                        {
                            ReturnFromPreLaunch(feContainer);
                        };

                        process.Start();

                        break;
                    case BigBlue.Models.LaunchType.main:
                        // if you're going to use global inputs to kill this process, then we don't need to attach the exited event handler onto it
                        if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].KillTask == true && globalInputs == true)
                        {
                            // get all the binaries in the path
                            BigBlue.ProcessHandling.AddPathBinariesToProcessList(dir, processes);
                            
                            process.Start();

                            // get any children processes that this process has
                            BigBlue.ProcessHandling.GetChildProcesses(process, processes);
                        }
                        else
                        {
                            process.EnableRaisingEvents = true;

                            process.Exited += (a, b) =>
                            {
                                ReturnFromGame(feContainer);
                            };

                            process.Start();
                        }
                        break;
                    case BigBlue.Models.LaunchType.post:
                        process.EnableRaisingEvents = true;

                        process.Exited += (a, b) =>
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                BackToGameList();
                            });
                        };

                        process.Start();

                        break;
                    case BigBlue.Models.LaunchType.desktop:
                        process.Start();
                        break;
                    default:
                        break;
                }

                
            }
            catch (Exception)
            {
                // restore the error mode to whatever it was before the program launch
                // should this just return to the frontend?
            }
            finally
            {
            }
        }
                
        private static IntPtr SetHook(BigBlue.NativeMethods.LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return BigBlue.NativeMethods.SetWindowsHookEx(BigBlue.NativeMethods.WH_KEYBOARD_LL, proc, BigBlue.NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        public async void DisableKeys(BigBlueWindow bbWindow)
        {
            // unregister raw input
            bbWindow.win32Window.RemoveHook(HwndHook);
            BigBlue.NativeMethods.UnRegisterInputDevices(bbWindow);

            // set the low level keyboard hook to trap all keys
            BigBlue.NativeMethods._hookID = SetHook(BigBlue.NativeMethods._proc);

            // wait for the configured time
            await Task.Delay(bbWindow.launchInputDelay);

            // unhook the low level keyboard trap
            BigBlue.NativeMethods.UnhookWindowsHookEx(BigBlue.NativeMethods._hookID);

            // re-register raw input       
            bbWindow.win32Window.AddHook(HwndHook);
            BigBlue.NativeMethods.RegisterInputDevices(bbWindow);
        }

        internal IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // I could have done one of two things here.
            // 1. Use a Message as it was used before.
            // 2. Changes the ProcessMessage method to handle all of these parameters(more work).
            //    I opted for the easy way.

            //Note: Depending on your application you may or may not want to set the handled param.
            NativeMethods.ProcessMessage(msg, lParam, wParam, this);

            return IntPtr.Zero;
        }

        internal virtual void ProcessScreenSaverInput()
        {
        }

        internal void ProvisionWindowsHooks()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);

            windowHandle = helper.Handle;
            
            win32Window = HwndSource.FromHwnd(windowHandle);
            win32Window.AddHook(HwndHook);
        }
        
        internal virtual void ReadConfigFile()
        {
            if (ConfigNode != null)
            {
                double.TryParse(ConfigNode["mainmenutextsize"]?.InnerText, out mainMenuFontSize);

                double.TryParse(ConfigNode["mainmenupaddingx"]?.InnerText, out mainMenuPaddingX);

                double.TryParse(ConfigNode["mainmenupaddingy"]?.InnerText, out mainMenuPaddingY);

                if (!string.IsNullOrEmpty(ConfigNode["returnlabel"]?.InnerText))
                {
                    mainMenuReturnLabel = ConfigNode["returnlabel"].InnerText;
                }

                if (!string.IsNullOrEmpty(ConfigNode["exitlabel"]?.InnerText))
                {
                    mainMenuExitLabel = ConfigNode["exitlabel"].InnerText;
                }

                if (!string.IsNullOrEmpty(ConfigNode["shutdownlabel"]?.InnerText))
                {
                    mainMenuShutdownLabel = ConfigNode["shutdownlabel"].InnerText;
                }

                if (!string.IsNullOrEmpty(ConfigNode["restartlabel"]?.InnerText))
                {
                    mainMenuRestartLabel = ConfigNode["restartlabel"].InnerText;
                }
                
                surroundMonitor2DisplayName = ConfigNode["surroundmonitor"]?.InnerText;

                Enum.TryParse(ConfigNode["surroundposition"]?.InnerText, out surroundPosition);

                gameListFont = new FontFamily(ConfigNode["font"]?.InnerText);

                switch (ConfigNode["listalignment"].InnerText)
                {
                    case "Left":
                        gameListAlignment = TextAlignment.Left;
                        break;
                    case "Right":
                        gameListAlignment = TextAlignment.Right;
                        break;
                }

                bool tempDisplayWindowBorder;

                if (bool.TryParse(ConfigNode["windowborder"]?.InnerText, out tempDisplayWindowBorder))
                {
                    displayWindowsBorder = tempDisplayWindowBorder;
                }

                double tempConfigResolutionX;

                if (double.TryParse(ConfigNode["width"]?.InnerText, out tempConfigResolutionX))
                {
                    configResolutionX = tempConfigResolutionX;
                }

                double tempConfigResolutionY;

                if (double.TryParse(ConfigNode["height"]?.InnerText, out tempConfigResolutionY))
                {
                    configResolutionY = tempConfigResolutionY;
                }

                double.TryParse(ConfigNode["gamelistmarginx"]?.InnerText, out gameListMarginX);
                double.TryParse(ConfigNode["gamelistmarginy"]?.InnerText, out gameListMarginY);
                double.TryParse(ConfigNode["gamelistoverscanx"]?.InnerText, out gameListOverscanX);
                double.TryParse(ConfigNode["gamelistoverscany"]?.InnerText, out gameListOverscanY);

                double.TryParse(ConfigNode["listitemhorizontalpadding"]?.InnerText, out listItemHorizontalPadding);
                double.TryParse(ConfigNode["unselecteditemverticalpadding"]?.InnerText, out unselectedItemVerticalPadding);
                double.TryParse(ConfigNode["selecteditemverticalpadding"]?.InnerText, out selectedItemVerticalPadding);
                bool.TryParse(ConfigNode["antialiastext"]?.InnerText, out antialiasedText);

                int.TryParse(ConfigNode["aspectratio"]?.InnerText, out aspectRatioIndex);

                if (aspectRatioIndex == 1)
                {
                    portraitModeIndex = aspectRatioIndex;
                }

                bool.TryParse(ConfigNode["exitoption"]?.InnerText, out displayExitItemInMenu);

                bool.TryParse(ConfigNode["globalinputs"]?.InnerText, out globalInputs);

                double.TryParse(ConfigNode["selectedtextsize"]?.InnerText, out selectedTextSize);
                double.TryParse(ConfigNode["unselectedtextsize"]?.InnerText, out unselectedTextSize);

                bool.TryParse(ConfigNode["trapcursor"]?.InnerText, out mouseCursorTrapped);

                // this is where you can control the frontend's sound volume separately from the rest of the volume
                float.TryParse(ConfigNode["minigamevolume"]?.InnerText, out miniGameVolume);

                if (miniGameVolume > 1)
                {
                    miniGameVolume = 1;
                }

                if (miniGameVolume < 0)
                {
                    miniGameVolume = 0;
                }

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

                double.TryParse(ConfigNode["screensavertime"]?.InnerText, out screenSaverTimeInMinutes);

                screenSaverTimeInMinutes = screenSaverTimeInMinutes * 60000;

                int.TryParse(ConfigNode["inputdelayonlaunch"]?.InnerText, out launchInputDelay);

                bool.TryParse(ConfigNode["stretch"]?.InnerText, out stretch);

                bool.TryParse(ConfigNode["stretchsnapshot"]?.InnerText, out stretchSnapshots);

                bool.TryParse(ConfigNode["cleanstretch"]?.InnerText, out cleanStretch);

                

                int.TryParse(ConfigNode["rotate"]?.InnerText, out screenRotation);

                bool.TryParse(ConfigNode["music"]?.InnerText, out music);

                bool.TryParse(ConfigNode["keepaspect"]?.InnerText, out keepAspectRatio);

                bool.TryParse(ConfigNode["hidemousecursor"]?.InnerText, out hideMouseCursor);

                bool.TryParse(ConfigNode["disablemenu"]?.InnerText, out disableMenu);                
            }
        }

        internal void ProvisionInputStates()
        {
            Models.FrontendInputState speechState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("BIG_BLUE_SPEECH", speechState);

            Models.FrontendInputState randomItemState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_RANDOM_ITEM", randomItemState);

            Models.FrontendInputState randomItemState2 = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_RANDOM_ITEM", randomItemState2);

            Models.FrontendInputState firstItemState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_FIRST_ITEM", firstItemState);

            Models.FrontendInputState firstItemState2 = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_FIRST_ITEM", firstItemState2);

            Models.FrontendInputState lastItemState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_LAST_ITEM", firstItemState);

            Models.FrontendInputState lastItemState2 = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_LAST_ITEM", firstItemState2);

            Models.FrontendInputState previousItemState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_PREVIOUS_ITEM", previousItemState);

            Models.FrontendInputState previousItem2State = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_PREVIOUS_ITEM", previousItem2State);

            Models.FrontendInputState nextItemState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_NEXT_ITEM", nextItemState);

            Models.FrontendInputState nextItem2State = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_NEXT_ITEM", nextItem2State);

            Models.FrontendInputState previousPageState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_PREVIOUS_PAGE", previousPageState);

            Models.FrontendInputState previousPage2State = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_PREVIOUS_PAGE", previousPage2State);

            Models.FrontendInputState nextPageState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_NEXT_PAGE", nextPageState);

            Models.FrontendInputState nextPage2State = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_NEXT_PAGE", nextPage2State);

            Models.FrontendInputState selectGameState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_START", selectGameState);

            Models.FrontendInputState selectGame2State = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_START", selectGame2State);

            Models.FrontendInputState fighterPunchState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_SPECTATE", fighterPunchState);

            Models.FrontendInputState exitState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("BIG_BLUE_EXIT", exitState);

            Models.FrontendInputState exit1State = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_EXIT", exit1State);

            Models.FrontendInputState exit2State = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_EXIT", exit2State);

            Models.FrontendInputState punchLeftState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_PUNCH_LEFT", punchLeftState);

            Models.FrontendInputState punchRightState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_PUNCH_RIGHT", punchRightState);

            Models.FrontendInputState rtypeShootState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_SHOOT", rtypeShootState);

            Models.FrontendInputState rtypeWarpState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_WARP", rtypeWarpState);

            Models.FrontendInputState back1State = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RAMPAGE_BACK", back1State);

            Models.FrontendInputState back2State = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RTYPE_BACK", back2State);

            Models.FrontendInputState restartState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("RESTART", restartState);

            Models.FrontendInputState shutdownState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("SHUTDOWN", shutdownState);

            Models.FrontendInputState quitState = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("QUIT_TO_DESKTOP", quitState);

            Models.FrontendInputState volUp = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("BIG_BLUE_VOLUME_UP", volUp);

            Models.FrontendInputState volDown = new Models.FrontendInputState(false, false, false);
            frontendInputs.Add("BIG_BLUE_VOLUME_DOWN", volDown);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            CompositionTarget.Rendering -= OnFrame;

            if (recognizer != null)
            {
                recognizer.Dispose();
                recognizer = null;
            }

            if (directInput != null)
            {
                directInput.Dispose();
                directInput = null;
            }

            if (BigBlue.XAudio2Player.xaudio2 != null)
            {
                BigBlue.XAudio2Player.Dispose();
                BigBlue.XAudio2Player.xaudio2 = null;
            }

            // close the secondary window
            if (marqueeDisplay == true)
            {
                marqueeWindow.Close();
                marqueeWindow = null;
            }

            if (flyerDisplay == true)
            {
                flyerWindow.Close();
                flyerWindow = null;
            }

            if (instructionDisplay == true)
            {
                instructionWindow.Close();
                instructionWindow = null;
            }
            
            if (videoMe != null)
            {
                videoMe.Close();
                videoMe.Clock = null;
                videoMe.Source = null;
                videoMe = null;
            }
            
            // remove the hook for raw input
            //win32Window.RemoveHook(HwndHook);

            // release the mouse cursor
            BigBlue.NativeMethods.ClipCursor(IntPtr.Zero);

            // if the windows error reporting UI is disabled, let's restore it
            if (hideWindowsErrorReportUI && NativeMethods.GetWindowsErrorReportingDontShowUIValue() == 1)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\Windows Error Reporting", "DontShowUI", 0);
            }

            // if we're opening explorer on quit, check to see whether it's running and act accordingly
            if (openExplorerOnQuit)
            {
                bool defaultShell = BigBlue.NativeMethods.IsDefaultShellEnabled();

                // this needs to run explorer.exe if it isn't open
                Process[] explorerProcesses = System.Diagnostics.Process.GetProcessesByName("explorer");

                // launch windows explorer if the default shell has been overriden
                // OR we can't find an explorer process currently running
                // at some point, we might want to update the configuration program to let the user choose a shell program
                if (explorerProcesses.Count() == 0 || !defaultShell)
                {
                    string windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                    LaunchProgram(null, windowsDirectory, "explorer.exe", string.Empty, BigBlue.Models.LaunchType.desktop);
                }
            }

            /*
            if (ptrHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(ptrHook);
                ptrHook = IntPtr.Zero;
            }
             */

            base.OnClosing(e);

            Application.Current.Shutdown();
        }

        internal void ManageGameList()
        {
            if (frontendInputs["RAMPAGE_NEXT_PAGE"].wasPressed == true || frontendInputs["RTYPE_NEXT_PAGE"].wasPressed == true)
            {
                // todo: choose which control actually works
                if (listTypePriority == Models.ListType.Image)
                {
                    ChangeListItem(frontendLists[selectedListGuid].ImageItemsToPage);
                }
                else
                {
                    ChangeListItem(frontendLists[selectedListGuid].TextBlockItemsToPage);
                }
            }

            if (frontendInputs["RAMPAGE_PREVIOUS_PAGE"].wasPressed == true || frontendInputs["RTYPE_PREVIOUS_PAGE"].wasPressed == true)
            {
                if (listTypePriority == Models.ListType.Image)
                {
                    ChangeListItem(-frontendLists[selectedListGuid].ImageItemsToPage);
                }
                else
                {
                    ChangeListItem(-frontendLists[selectedListGuid].TextBlockItemsToPage);
                }
            }

            if (frontendInputs["RAMPAGE_NEXT_ITEM"].wasPressed == true || frontendInputs["RTYPE_NEXT_ITEM"].wasPressed == true)
            {
                ChangeListItem(1);
            }

            if (frontendInputs["RAMPAGE_PREVIOUS_ITEM"].wasPressed == true || frontendInputs["RTYPE_PREVIOUS_ITEM"].wasPressed == true)
            {
                ChangeListItem(-1);
            }
        }
    }
}