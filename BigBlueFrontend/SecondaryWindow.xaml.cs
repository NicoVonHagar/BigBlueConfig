using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BigBlue
{
    /// <summary>
    /// Interaction logic for Marquee.xaml
    /// </summary>
    public partial class SecondaryWindow : Window
    {
        public Dictionary<int, ImageBrush> staticBrushes = new Dictionary<int, ImageBrush>(2);
        public List<ImageBrush> damndBrushes = new List<ImageBrush>(2);
        List<SolidColorBrush> damndBackgroundBrushes = new List<SolidColorBrush>(2);
        public DispatcherTimer damndAnimationTimer = new DispatcherTimer(DispatcherPriority.Background);
        public bool damndGloating = false;
        public bool staticAnimating = false;
        int damndFrameToDisplay = 0;
        
        const double baseWidth = 60;
        const double baseHeight = 78;

        bool integerMultiplier = false;

        Rect staticViewBox1 = new Rect(0, 0, baseWidth, baseHeight);
        Rect staticViewBox2 = new Rect(64, 0, baseWidth, baseHeight);
        
        private bool IsInteger(double d)
        {
            return unchecked(d == (int)d);
        }

        private void setDamndDimensions(System.Windows.Forms.Screen secondaryScreen)
        {
            int width = secondaryScreen.Bounds.Width;
            int height = secondaryScreen.Bounds.Height;

            DamndBackground.Width = width;
            DamndBackground.Height = height;

            double secondaryDisplayXMultiplier = width / 1920;

            if (width >= 1920 && IsInteger(secondaryDisplayXMultiplier))
            {
                integerMultiplier = true;
            }

            BitmapImage staticImage1 = ImageLoading.loadImage("static1.png", integerMultiplier);
            BitmapImage staticImage2 = ImageLoading.loadImage("static2.png", integerMultiplier);
            
            double secondaryDisplayAdjustedWidth = baseWidth * secondaryDisplayXMultiplier;
            double secondaryDisplayAdjustedHeight = baseHeight * secondaryDisplayXMultiplier;

            Rect secondaryStaticViewPort = new Rect(0, 0, secondaryDisplayAdjustedWidth, secondaryDisplayAdjustedHeight);

            staticBrushes[0] = ImageLoading.loadTiledAnimationFrame(staticImage1, secondaryStaticViewPort, BrushMappingMode.Absolute, TileMode.Tile, integerMultiplier);
            staticBrushes[1] = ImageLoading.loadTiledAnimationFrame(staticImage2, secondaryStaticViewPort, BrushMappingMode.Absolute, TileMode.Tile, integerMultiplier);

            double damndHeightMultiplier = height / 378;
            
            double damndWidth = 615 * damndHeightMultiplier;
            double damndHeight = height;
            double damndHorizontalOffset = (width / 2) - (damndWidth / 2);

            string damndImageName = "damnd";

            if (height > width)
            {
                double damndAspectRatio = 480 / 386;
                damndWidth = width;
                damndHeight = width / damndAspectRatio;
                damndHorizontalOffset = 0;
                damndImageName = damndImageName + "portrait";
            }
            
            int damndWidthDecoded = Convert.ToInt32(damndWidth);
            int damndHeightDecoded = Convert.ToInt32(damndHeight);
            BitmapImage damnd1Bitmap = ImageLoading.loadImageDecoded(damndImageName + "1.png", damndWidthDecoded, damndHeightDecoded, integerMultiplier);
            BitmapImage damnd2Bitmap = ImageLoading.loadImageDecoded(damndImageName + "2.png", damndWidthDecoded, damndHeightDecoded, integerMultiplier);

            ImageBrush d1Brush = new ImageBrush();
            d1Brush.ImageSource = damnd1Bitmap;
            d1Brush.AlignmentX = AlignmentX.Left;
            d1Brush.AlignmentY = AlignmentY.Top;
            d1Brush.Freeze();

            damndBrushes.Add(d1Brush);

            ImageBrush d2Brush = new ImageBrush();
            d2Brush.ImageSource = damnd2Bitmap;
            d2Brush.AlignmentX = AlignmentX.Left;
            d2Brush.AlignmentY = AlignmentY.Top;
            d2Brush.Freeze();

            damndBrushes.Add(d2Brush);

            Damnd.Width = damndWidth;
            Damnd.Height = damndHeight;
            
            Canvas.SetLeft(Damnd, damndHorizontalOffset);
        }

        public SecondaryWindow(System.Windows.Forms.Screen screen)
        {
            InitializeComponent();

            SolidColorBrush damndBgColor1 = new SolidColorBrush(Color.FromRgb(64, 48, 48));
            damndBgColor1.Freeze();
            damndBackgroundBrushes.Add(damndBgColor1);

            SolidColorBrush damndBgColor2 = new SolidColorBrush(Color.FromRgb(80, 64, 64));
            damndBgColor2.Freeze();
            damndBackgroundBrushes.Add(damndBgColor2);

            DamndBackground.Fill = damndBackgroundBrushes[0];

            setDamndDimensions(screen);

            damndAnimationTimer.Interval = new TimeSpan(0, 0, 0, 0, 140);
        }
        
        private void animateDamnd()
        {
            if (damndGloating == true)
            {
                DamndBackground.Fill = damndBackgroundBrushes[damndFrameToDisplay];
                Damnd.Fill = damndBrushes[damndFrameToDisplay];

                if (damndFrameToDisplay == 0)
                {
                    damndFrameToDisplay = 1;
                }
                else
                {
                    damndFrameToDisplay = 0;
                }
            }
        }

        int staticFrameToDisplay = 0;

        private void animateStatic()
        {
            SecondaryWindowStatic.Fill = staticBrushes[staticFrameToDisplay];
            
            if (staticFrameToDisplay == 0)
            {
                staticFrameToDisplay = 1;
            }
            else
            {
                staticFrameToDisplay = 0;
            }
        }

        public void animateSecondaryDisplay(object sender, EventArgs e)
        {
            if (staticAnimating == true)
            {
                animateStatic();
            }
            else
            {
                animateDamnd();
            }
        }
    }
}
