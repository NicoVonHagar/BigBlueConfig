using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BigBlue
{
    public static class ImageLoading
    {
        public static ImageBrush loadImageBrushFromUri(Uri path, int? width, int? height)
        {
            BitmapImage bmi = loadImageFromUri(path, width, height);

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = bmi;
            ib.Freeze();

            return ib;
        }

        public static BitmapImage loadImageFromUri(Uri path, int? width, int? height)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;

            if (width != null)
            {
                img.DecodePixelWidth = (int)width;
            }
            
            img.UriSource = path;
            img.EndInit();
            
            img.Freeze();

            return img;
        }

        public static BitmapImage loadImageDecoded(string fileName, int? width, int? height, bool integerMultiplier)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            if (width != null)
            {
                img.DecodePixelWidth = (int)width;
            }
            
            img.UriSource = new Uri("pack://application:,,,/Assets/" + fileName);
            img.EndInit();

            if (integerMultiplier == true)
            {
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
            }

            img.Freeze();

            return img;
        }

        public static BitmapImage loadImage(string fileName, bool integerMultiplier)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = new Uri("pack://application:,,,/Assets/" + fileName);
            img.EndInit();

            if (integerMultiplier == true)
            {
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
            }

            img.Freeze();

            return img;
        }

        public static ImageBrush loadTiledAnimationFrame(ImageSource imgSource, Rect vPort, BrushMappingMode vpUnits, TileMode tMode, bool integerMultiplier)
        {
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = imgSource;
            ib.Viewport = vPort;
            ib.ViewportUnits = vpUnits;
            ib.TileMode = tMode;
            //ib.AlignmentX = aX;
            //ib.AlignmentY = aY;
            ib.Stretch = Stretch.Uniform;

            RenderOptions.SetCachingHint(ib, CachingHint.Cache);

            if (integerMultiplier == true)
            {
                RenderOptions.SetBitmapScalingMode(ib, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(ib, EdgeMode.Aliased);
            }

            ib.Freeze();

            return ib;
        }

        public static ImageBrush loadAnimationFrame(ImageSource imgSource, Rect vBox, BrushMappingMode vbUnits, TileMode tMode, AlignmentX aX, AlignmentY aY, bool integerMultiplier)
        {
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = imgSource;
            ib.ViewboxUnits = vbUnits;
            ib.Viewbox = vBox;
            ib.TileMode = tMode;
            ib.AlignmentX = aX;
            ib.AlignmentY = aY;

            RenderOptions.SetCachingHint(ib, CachingHint.Cache);

            if (integerMultiplier == true)
            {
                RenderOptions.SetBitmapScalingMode(ib, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(ib, EdgeMode.Aliased);
            }

            ib.Freeze();

            return ib;
        }

        public static ImageBrush loadImageBrush(string fileName, TileMode tMode, AlignmentX aX, AlignmentY aY, Rect vb, BrushMappingMode vbUnits, Rect vp, BrushMappingMode vpUnits, bool ignoreSettings, bool integerMultiplier)
        {
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = loadImage(fileName, integerMultiplier);

            if (ignoreSettings == false)
            {
                ib.TileMode = tMode;
                ib.AlignmentX = aX;
                ib.AlignmentY = aY;

                if (vb.IsEmpty == false)
                {
                    ib.Viewbox = vb;
                    ib.ViewboxUnits = vbUnits;
                }

                if (vp.IsEmpty == false)
                {
                    ib.Viewport = vp;
                    ib.ViewportUnits = vpUnits;
                }
            }

            RenderOptions.SetCachingHint(ib, CachingHint.Cache);

            if (integerMultiplier == true)
            {
                RenderOptions.SetBitmapScalingMode(ib, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(ib, EdgeMode.Aliased);
            }

            ib.Freeze();

            return ib;
        }
    }
}
