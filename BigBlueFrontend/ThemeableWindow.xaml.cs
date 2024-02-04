using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace BigBlue
{
    /// <summary>
    /// Interaction logic for ThemeableWindow.xaml
    /// </summary>
    public partial class ThemeableWindow : BigBlueWindow
    {
        Canvas gameListCanvasOutlines = null;
        
        double? themeListWidth;
        double? themeListHeight;

        SolidColorBrush gameListSelectedBackgroundColor;
        SolidColorBrush gameListSelectedForegroundColor;
        SolidColorBrush gameListUnselectedTopColor;
        SolidColorBrush gameListUnselectedBottomColor;
        SolidColorBrush gameListOutlineColor;

        SolidColorBrush mainMenuSelectedColor;
        SolidColorBrush mainMenuUnselectedColor;

        XmlNode themeConfigNode;

        Dictionary<Guid, Models.ImageAnimation> imageAnimations = new Dictionary<Guid, Models.ImageAnimation>();

        Dictionary<Guid, FrameworkElement> templateObjects = new Dictionary<Guid, FrameworkElement>();

        Dictionary<Guid, Storyboard> animations = new Dictionary<Guid, Storyboard>();

        private string themeDir;

        public ThemeableWindow(string appDirectory, string themeDirectory, XmlNode config, XmlNode themeConfig)
        {
            try
            {
                hideWindowControls = true;

                InitializeComponent();

                Path = appDirectory;
                themeDir = themeDirectory;
                ConfigNode = config;
                themeConfigNode = themeConfig;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Big Blue");
            }
        }

        void AnimateImage(Models.ImageAnimation ia)
        {
            // if the animation's timer is less than the time it takes to draw a new frame, leave
            if (ia.Timer.ElapsedMilliseconds < ia.Speed)
            {
                return;
            }
            // we're past due to update the frame, so let's do it
            else
            {
                // set the imge to the object
                ia.Image.Fill = ia.Frames[ia.CurrentFrame];

                // choose the next frame; if we're at the total, then we're going to restart
                if (ia.CurrentFrame == ia.TotalFrames)
                {
                    ia.CurrentFrame = 0;
                }
                else
                {
                    // otherwise, increment by one to show the next frame
                    ia.CurrentFrame = ia.CurrentFrame + 1;
                }

                // restart the timer
                ia.Timer.Restart();
            }
        }


        private void CreateAnimation(XmlNode layerObject, FrameworkElement obj)
        {
            try
            {
                XmlNodeList animationNodes = layerObject.SelectNodes("animation");

                TransformGroup tg = new TransformGroup();

                Storyboard animationStoryBoard = new Storyboard();

                TimeSpan zeroTs = TimeSpan.FromSeconds(0);

                animationStoryBoard.BeginTime = zeroTs;

                Storyboard transFormStoryBoard = new Storyboard();
                Guid animationGuid = Guid.NewGuid();
                Guid transformGuid = Guid.NewGuid();

                for (int i = 0; i < animationNodes.Count; i++)
                {                    
                    XmlNode animationNode = animationNodes[i];

                    RepeatBehavior rb;

                    double loopCount;
                    if (double.TryParse(animationNode.Attributes["loop"]?.Value, out loopCount))
                    {
                        rb = new RepeatBehavior(loopCount);
                    }
                    else
                    {
                        rb = RepeatBehavior.Forever;
                    }

                    double speed;

                    if (!double.TryParse(animationNode.Attributes["speed"]?.Value, out speed))
                    {
                        speed = 1;
                    }

                    double startDelayInSeconds = 0;
                    double.TryParse(animationNode.Attributes["delay"]?.Value, out startDelayInSeconds);

                    TimeSpan currentAnimationDuration = TimeSpan.FromSeconds(startDelayInSeconds);

                    double duration;
                    double.TryParse(animationNode.Attributes["duration"]?.Value, out duration);

                    Duration animationDuration = new Duration(TimeSpan.FromSeconds(duration));

                    string animationType = animationNode.Attributes["type"]?.Value;

                    switch (animationType)
                    {
                        case "scaleX":
                            double sxFactor;
                            double.TryParse(animationNode.Attributes["to"]?.Value, out sxFactor);

                            ScaleTransform scaleXTransform = new ScaleTransform();

                            scaleXTransform.CenterX = obj.Width / 2;
                            scaleXTransform.CenterY = obj.Height / 2;

                            tg.Children.Add(scaleXTransform);

                            DoubleAnimation scaleXAnimation = new DoubleAnimation();
                            scaleXAnimation.Duration = animationDuration;
                            scaleXAnimation.AutoReverse = true;
                            scaleXAnimation.From = 1;
                            scaleXAnimation.To = sxFactor;
                            scaleXAnimation.AutoReverse = true;
                            scaleXAnimation.SpeedRatio = speed;
                            scaleXAnimation.RepeatBehavior = rb;

                            Storyboard.SetTarget(scaleXAnimation, obj);

                            int scaleXPropertyIndex = tg.Children.Count - 1;

                            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.Children[" + scaleXPropertyIndex + "].ScaleX"));

                            scaleXAnimation.Freeze();

                            transFormStoryBoard.Children.Add(scaleXAnimation);
                            break;
                        case "scaleY":
                            double syFactor;
                            double.TryParse(animationNode.Attributes["to"]?.Value, out syFactor);

                            ScaleTransform scaleYTransform = new ScaleTransform();

                            scaleYTransform.CenterX = obj.Width / 2;
                            scaleYTransform.CenterY = obj.Height / 2;

                            tg.Children.Add(scaleYTransform);

                            DoubleAnimation scaleYAnimation = new DoubleAnimation();
                            scaleYAnimation.Duration = animationDuration;
                            scaleYAnimation.AutoReverse = true;
                            scaleYAnimation.From = 1;
                            scaleYAnimation.To = syFactor;
                            scaleYAnimation.RepeatBehavior = rb;


                            Storyboard.SetTarget(scaleYAnimation, obj);

                            int scaleYPropertyIndex = tg.Children.Count - 1;

                            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.Children[" + scaleYPropertyIndex + "].ScaleY"));

                            scaleYAnimation.Freeze();

                            animationStoryBoard.AutoReverse = true;

                            animationStoryBoard.Children.Add(scaleYAnimation);
                            break;
                        case "rotation":
                            DoubleAnimation rAnimation = new DoubleAnimation();

                            double rFromPosition;
                            double.TryParse(animationNode.Attributes["from"]?.Value, out rFromPosition);

                            double rToPosition;
                            double.TryParse(animationNode.Attributes["to"]?.Value, out rToPosition);

                            rAnimation.From = rFromPosition;
                            rAnimation.To = rToPosition;
                            rAnimation.Duration = animationDuration;
                            rAnimation.RepeatBehavior = rb;
                            rAnimation.AutoReverse = true;

                            RotateTransform rt = new RotateTransform(rFromPosition, obj.Width / 2, obj.Height / 2);
                            rt.Freeze();

                            tg.Children.Add(rt);

                            Storyboard.SetTarget(rAnimation, obj);

                            int rotatationPropertyIndex = tg.Children.Count - 1;

                            Storyboard.SetTargetProperty(rAnimation, new PropertyPath("(UIElement.RenderTransform).Children[" + rotatationPropertyIndex + "].(RotateTransform.Angle)"));

                            rAnimation.Freeze();

                            animationStoryBoard.Children.Add(rAnimation);

                            break;
                        case "bgcolor":
                        case "color":
                            ColorAnimation cAnimation = new ColorAnimation();

                            string argbFromHex = animationNode.Attributes["from"]?.Value;

                            Color fromColor = (Color)ColorConverter.ConvertFromString(argbFromHex);

                            string argbToHex = animationNode.Attributes["to"]?.Value;

                            Color toColor = (Color)ColorConverter.ConvertFromString(argbToHex);
                            
                            Shape shape = obj as Shape;

                            if (animationType == "bgcolor")
                            {
                                SolidColorBrush rFill = shape.Fill as SolidColorBrush;

                                Storyboard.SetTarget(cAnimation, rFill);
                            }
                            else
                            {
                                SolidColorBrush rFill = shape.Stroke as SolidColorBrush;

                                Storyboard.SetTarget(cAnimation, rFill);
                            }
                            

                            Storyboard.SetTargetProperty(cAnimation, new PropertyPath(SolidColorBrush.ColorProperty));

                            cAnimation.From = fromColor;
                            cAnimation.To = toColor;
                            cAnimation.Duration = animationDuration;
                            cAnimation.AutoReverse = true;
                            cAnimation.RepeatBehavior = rb;

                            cAnimation.Freeze();

                            animationStoryBoard.Children.Add(cAnimation);

                            animationStoryBoard.AutoReverse = true;

                            break;
                        case "opacity":
                            DoubleAnimation oAnimation = new DoubleAnimation();

                            double oFromPosition;
                            double.TryParse(animationNode.Attributes["from"]?.Value, out oFromPosition);

                            obj.Opacity = oFromPosition;

                            double oToPosition;
                            double.TryParse(animationNode.Attributes["to"]?.Value, out oToPosition);

                            oAnimation.From = oFromPosition;
                            oAnimation.To = oToPosition;
                            oAnimation.Duration = animationDuration;
                            oAnimation.AutoReverse = true;
                            oAnimation.RepeatBehavior = rb;

                            animationStoryBoard.Children.Add(oAnimation);

                            animationStoryBoard.AutoReverse = true;

                            Storyboard.SetTarget(oAnimation, obj);

                            Storyboard.SetTargetProperty(oAnimation, new PropertyPath("(Opacity)"));

                            break;
                        case "x":
                            DoubleAnimation xAnimation = new DoubleAnimation();

                            double xFromPosition;
                            double.TryParse(animationNode.Attributes["from"]?.Value, out xFromPosition);

                            double xToPosition;
                            double.TryParse(animationNode.Attributes["to"]?.Value, out xToPosition);

                            // only set the initial position if it's the first animation
                            if (i == 0)
                            {
                                Canvas.SetLeft(obj, xFromPosition);
                            }

                            xAnimation.From = xFromPosition;
                            xAnimation.To = xToPosition;
                            xAnimation.Duration = animationDuration;
                            xAnimation.RepeatBehavior = rb;
                            xAnimation.SpeedRatio = speed;
                            xAnimation.BeginTime = currentAnimationDuration;
                            
                            Storyboard.SetTarget(xAnimation, obj);
                            Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));

                            xAnimation.Freeze();

                            animationStoryBoard.Children.Add(xAnimation);

                            break;
                        case "y":
                            DoubleAnimation yAnimation = new DoubleAnimation();

                            double yFromPosition;
                            double.TryParse(animationNode.Attributes["from"]?.Value, out yFromPosition);

                            double yToPosition;
                            double.TryParse(animationNode.Attributes["to"]?.Value, out yToPosition);

                            if (i == 0)
                            {
                                Canvas.SetTop(obj, yFromPosition);
                            }

                            yAnimation.From = yFromPosition;
                            yAnimation.To = yToPosition;
                            yAnimation.Duration = animationDuration;
                            yAnimation.RepeatBehavior = rb;
                            yAnimation.SpeedRatio = speed;
                            yAnimation.BeginTime = currentAnimationDuration;

                            animationStoryBoard.Children.Add(yAnimation);

                            Storyboard.SetTarget(yAnimation, obj);
                            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(Canvas.Top)"));
                            break;
                    }

                    animationStoryBoard.RepeatBehavior = rb;
                    transFormStoryBoard.RepeatBehavior = rb;
                }

                // Make the Storyboard a resource.
                Resources.Add(animationGuid, animationStoryBoard);

                animations[animationGuid] = animationStoryBoard;

                if (tg.Children != null)
                {
                    if (tg.Children.Count > 0)
                    {
                        Resources.Add(transformGuid, transFormStoryBoard);
                        animations[transformGuid] = transFormStoryBoard;

                        tg.Freeze();
                        obj.RenderTransform = tg;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        
        string mainMenuBackgroundSource = string.Empty;

        Brush imageListBorderBrush = null;

        internal override void ReadConfigFile()
        {
            base.ReadConfigFile();

            if (themeConfigNode.Attributes["icon"] != null)
            {
                if (!string.IsNullOrWhiteSpace(themeConfigNode.Attributes["icon"].InnerText))
                {
                    string customIconPath = themeDir + @"\" + themeConfigNode.Attributes["icon"].InnerText;

                    if (System.IO.File.Exists(customIconPath))
                    {
                        Uri customIconUri;
                        if (Uri.TryCreate(customIconPath, UriKind.RelativeOrAbsolute, out customIconUri))
                        {
                            this.Icon = ImageLoading.loadImageFromUri(customIconUri, null, null);
                        }
                    }
                }
            }

            if (themeConfigNode.Attributes["title"] != null)
            {
                Title = themeConfigNode.Attributes["title"].InnerText;
            }
            else
            {
                Title = DEFAULT_FRONTEND_NAME;
            }

            mainMenuBackgroundSource = themeDir + @"\" + themeConfigNode.Attributes["menuBackground"]?.Value;

            string mmSelectedArgbHex = themeConfigNode.Attributes["menuSelectedArgb"]?.Value;
            Color mmSelectedColor = (Color)ColorConverter.ConvertFromString(mmSelectedArgbHex);
            mainMenuSelectedColor = new SolidColorBrush(mmSelectedColor);

            string mmUnselectedArgbHex = themeConfigNode.Attributes["menuUnselectedArgb"]?.Value;
            Color mmUnselectedColor = (Color)ColorConverter.ConvertFromString(mmUnselectedArgbHex);
            mainMenuUnselectedColor = new SolidColorBrush(mmUnselectedColor);
            
            XmlNodeList layerObjects = themeConfigNode.SelectNodes("objects/object");
                            
            foreach (XmlNode layerObject in layerObjects)
            {
                int objectWidth;
                int objectHeight;
                   
                int.TryParse(layerObject.Attributes["width"]?.Value, out objectWidth);
                int.TryParse(layerObject.Attributes["height"]?.Value, out objectHeight);

                double objectPositionX;
                double objectPositionY;

                double.TryParse(layerObject.Attributes["x"]?.Value, out objectPositionX);
                double.TryParse(layerObject.Attributes["y"]?.Value, out objectPositionY);

                string layerObjectType = layerObject.Attributes["type"].Value;

                int resolutionX;
                int.TryParse(layerObject.Attributes["resolutionX"]?.Value, out resolutionX);
                int resolutionY;
                int.TryParse(layerObject.Attributes["resolutionY"]?.Value, out resolutionY);

                double objectOpacity = 1;
                double.TryParse(layerObject.Attributes["opacity"]?.Value, out objectOpacity);

                switch (layerObjectType)
                {
                    case "image":
                        Uri imageUri;

                        string imagePath = themeDir + @"\" + layerObject.Attributes["src"]?.Value;

                        if (Uri.TryCreate(imagePath, UriKind.Absolute, out imageUri))
                        { 
                            BitmapImage bmi = ImageLoading.loadImageFromUri(imageUri, resolutionX, objectHeight);
                            ImageBrush ib = new ImageBrush();

                            ib.ImageSource = bmi;
                            ib.Freeze();

                            Rectangle imageRectangle = new Rectangle();
                            imageRectangle.Fill = ib;
                            imageRectangle.Width = objectWidth;
                            imageRectangle.Height = objectHeight;

                            FrontEndContainer.Children.Add(imageRectangle);

                            Canvas.SetLeft(imageRectangle, objectPositionX);
                            Canvas.SetTop(imageRectangle, objectPositionY);

                            imageRectangle.Opacity = objectOpacity;

                            CreateAnimation(layerObject, imageRectangle);
                        }
                        break;
                    case "imageframes":
                        XmlNodeList frames = layerObject.SelectNodes("object[@type='imageframe']");

                        if (frames != null)
                        {
                            int frameCount = frames.Count - 1;

                            if (frameCount > 0)
                            {
                                BigBlue.Models.ImageAnimation imgAni = new BigBlue.Models.ImageAnimation();
                                imgAni.Frames = new List<ImageBrush>();
                                imgAni.Image = new Rectangle();
                                imgAni.Image.Width = objectWidth;
                                imgAni.Image.Height = objectHeight;

                                long animationSpeedInMilliseconds;

                                long.TryParse(layerObject.Attributes["speed"]?.Value, out animationSpeedInMilliseconds);

                                imgAni.Speed = animationSpeedInMilliseconds;

                                imgAni.TotalFrames = frameCount;
                                imgAni.CurrentFrame = 1;

                                imgAni.Timer = new System.Diagnostics.Stopwatch();

                                foreach (XmlNode frame in frames)
                                {
                                    Uri frameUri;

                                    if (Uri.TryCreate(themeDir + @"\" + frame.Attributes["src"]?.Value, UriKind.Absolute, out frameUri))
                                    {
                                        ImageBrush frameBrush = BigBlue.ImageLoading.loadImageBrushFromUri(frameUri, objectWidth, objectHeight);

                                        imgAni.Frames.Add(frameBrush);
                                    }
                                }

                                imgAni.Image.Fill = imgAni.Frames[0];

                                imageAnimations[Guid.NewGuid()] = imgAni;

                                // add it to the canvas
                                FrontEndContainer.Children.Add(imgAni.Image);

                                Canvas.SetLeft(imgAni.Image, objectPositionX);
                                Canvas.SetTop(imgAni.Image, objectPositionY);

                                CreateAnimation(layerObject, imgAni.Image);
                            }
                            else
                            {
                                // this should be treated as "image" at this point because it's just one image
                                Uri singleFrameUri;

                                if (Uri.TryCreate(themeDir + @"\" + frames[0].Attributes["src"]?.Value, UriKind.Absolute, out singleFrameUri))
                                {
                                    ImageBrush singleFrameImageBrush = BigBlue.ImageLoading.loadImageBrushFromUri(singleFrameUri, objectWidth, objectHeight);
                                        
                                    Rectangle singleImageRectangle = new Rectangle();
                                    singleImageRectangle.Fill = singleFrameImageBrush;
                                    singleImageRectangle.Width = objectWidth;
                                    singleImageRectangle.Height = objectHeight;

                                    FrontEndContainer.Children.Add(singleImageRectangle);

                                    Canvas.SetTop(singleImageRectangle, objectPositionY);
                                    Canvas.SetLeft(singleImageRectangle, objectPositionX);

                                    CreateAnimation(layerObject, singleImageRectangle);
                                }
                            }
                        }
                            
                        break;
                    case "polygon":
                        Polygon poly = new Polygon();

                        PointCollection pc = new PointCollection();

                        XmlNodeList pointNodes = layerObject.SelectNodes("point");

                        foreach (XmlNode point in pointNodes)
                        {
                            double pointX;
                            double pointY;

                            if (double.TryParse(point.Attributes["x"]?.Value, out pointX))
                            {
                                if (pointX > objectWidth)
                                {
                                    objectWidth = Convert.ToInt32(pointX);
                                }
                            }

                            if (double.TryParse(point.Attributes["y"]?.Value, out pointY))
                            {
                                if (pointY > objectHeight)
                                {
                                    objectHeight = Convert.ToInt32(pointY);
                                }
                            }
                            
                            pc.Add(new Point(pointX, pointY));
                        }

                        poly.Width = objectWidth;
                        poly.Height = objectHeight;

                        poly.Points = pc;

                        double polygonBorderWidth;
                        if (double.TryParse(layerObject.Attributes["borderWidth"]?.Value, out polygonBorderWidth))
                        {
                            poly.StrokeThickness = polygonBorderWidth;
                        }
                        
                        string polygonBorderValue = layerObject.Attributes["border"]?.Value;

                        try
                        {
                            Color polygonImageBorderColor = (Color)ColorConverter.ConvertFromString(polygonBorderValue);

                            SolidColorBrush ibc = new SolidColorBrush(polygonImageBorderColor);
                            //ibc.Freeze();

                            poly.Stroke = ibc;
                        }
                        catch (Exception)
                        {
                            Uri polygonBorderImageSource;

                            if (Uri.TryCreate(polygonBorderValue, UriKind.RelativeOrAbsolute, out polygonBorderImageSource))
                            {
                                ImageBrush polygonBorderBrush = BigBlue.ImageLoading.loadImageBrushFromUri(polygonBorderImageSource, Convert.ToInt32(objectWidth), Convert.ToInt32(objectHeight));
                                poly.Stroke = polygonBorderBrush;
                            }
                        }
                        finally
                        {

                        }

                        string polygonBackgroundValue = layerObject.Attributes["background"]?.Value;

                        try
                        {
                            Color polygonBackgroundColor = (Color)ColorConverter.ConvertFromString(polygonBackgroundValue);

                            SolidColorBrush ibc = new SolidColorBrush(polygonBackgroundColor);

                            poly.Fill = ibc;
                        }
                        catch (Exception)
                        {
                            Uri polygonBackgroundImageSource;

                            if (Uri.TryCreate(themeDir + @"\" + polygonBackgroundValue, UriKind.RelativeOrAbsolute, out polygonBackgroundImageSource))
                            {
                                ImageBrush polygonBackgroundBrush = BigBlue.ImageLoading.loadImageBrushFromUri(polygonBackgroundImageSource, Convert.ToInt32(objectWidth), Convert.ToInt32(objectHeight));
                                poly.Fill = polygonBackgroundBrush;
                            }
                        }
                        
                        

                        FrontEndContainer.Children.Add(poly);

                        Canvas.SetLeft(poly, objectPositionX);
                        Canvas.SetTop(poly, objectPositionY);
                        
                        CreateAnimation(layerObject, poly);
                        break;
                    case "color":
                        Rectangle colorBlock = new Rectangle();

                        string argbHex = layerObject.Attributes["argb"]?.Value;

                        Color color = (Color)ColorConverter.ConvertFromString(argbHex);
                                                        
                        colorBlock.Fill = new SolidColorBrush(color);
                            
                        colorBlock.Width = objectWidth;
                            
                        colorBlock.Height = objectHeight;

                        FrontEndContainer.Children.Add(colorBlock);

                        Canvas.SetLeft(colorBlock, objectPositionX);
                        Canvas.SetTop(colorBlock, objectPositionY);

                        CreateAnimation(layerObject, colorBlock);

                        break;
                    case "imagelist":
                        bool ilPriority;
                        if (bool.TryParse(layerObject.Attributes["control"]?.Value, out ilPriority))
                        {
                            listTypePriority = Models.ListType.Image;
                        }

                        string imageListOrientation = layerObject.Attributes["orientation"]?.Value;

                        double selectedImageWidth;
                        double.TryParse(layerObject.Attributes["selectedImageWidth"]?.Value, out selectedImageWidth);

                        double selectedImageHeight;
                        double.TryParse(layerObject.Attributes["selectedImageHeight"]?.Value, out selectedImageHeight);

                        double unselectedImageWidth;
                        double.TryParse(layerObject.Attributes["unselectedImageWidth"]?.Value, out unselectedImageWidth);

                        double unselectedImageHeight;
                        double.TryParse(layerObject.Attributes["unselectedImageHeight"]?.Value, out unselectedImageHeight);

                        double baseRotationFactor;
                        double.TryParse(layerObject.Attributes["rotationfactor"]?.Value, out baseRotationFactor);

                        string rotationDirection = layerObject.Attributes["rotationdirection"]?.Value;

                        string imageListAlignment = layerObject.Attributes["alignment"]?.InnerText;

                        Canvas imageListCanvas = new Canvas();
                        imageListCanvas.Width = objectWidth;
                        imageListCanvas.Height = objectHeight;

                        string imageShape = layerObject.Attributes["imageShape"]?.Value;

                        double borderWidth;
                        double.TryParse(layerObject.Attributes["borderWidth"]?.Value, out borderWidth);
                            
                        Uri borderImageSource;

                        string borderValue = layerObject.Attributes["border"]?.Value;

                        try
                        {
                            Color imageBorderColor = (Color)ColorConverter.ConvertFromString(borderValue);

                            SolidColorBrush ibc = new SolidColorBrush(imageBorderColor);
                            ibc.Freeze();

                            imageListBorderBrush = ibc;
                        }
                        catch (Exception)
                        {
                            if (Uri.TryCreate(themeDir + @"\" + borderValue, UriKind.RelativeOrAbsolute, out borderImageSource))
                            {
                                ImageBrush mmBrush = BigBlue.ImageLoading.loadImageBrushFromUri(borderImageSource, Convert.ToInt32(objectWidth), Convert.ToInt32(objectHeight));
                                imageListBorderBrush = mmBrush;
                            }
                        }                            

                        AddListImageBlocks(imageListCanvas, selectedImageWidth, selectedImageHeight, unselectedImageWidth, unselectedImageHeight, baseRotationFactor, imageListOrientation, rotationDirection, imageShape, imageListAlignment, borderWidth, imageListBorderBrush);
               
                            
                        FrontEndContainer.Children.Add(imageListCanvas);

                        Canvas.SetLeft(imageListCanvas, objectPositionX);
                        Canvas.SetTop(imageListCanvas, objectPositionY);

                        imageBlockListCanvas = imageListCanvas;

                        CreateAnimation(layerObject, imageListCanvas);

                        break;
                    case "listlabel":
                        double listLabelTextBlockFontSize;
                        double.TryParse(layerObject.Attributes["fontSize"]?.Value, out listLabelTextBlockFontSize);
                            
                        string listLabelFont = themeConfigNode.Attributes["font"]?.Value;

                        FontFamily listLabelFontFamily = null;

                        if (!string.IsNullOrWhiteSpace(listLabelFont))
                        {
                            listLabelFontFamily = new FontFamily(listLabelFont);
                        }

                            

                        string listLabelOutlineColorHex = layerObject.Attributes["outlineArgb"]?.Value;

                        Color listLabelOutlineColor = (Color)ColorConverter.ConvertFromString(listLabelOutlineColorHex);

                        SolidColorBrush listLabelOutlineBrush = new SolidColorBrush(listLabelOutlineColor);
                        listLabelOutlineBrush.Freeze();

                            

                        string listLabelColorHex = layerObject.Attributes["foregroundArgb"]?.Value;

                        Color listLabelColor = (Color)ColorConverter.ConvertFromString(listLabelColorHex);

                        SolidColorBrush listLabelBrush = new SolidColorBrush(listLabelColor);
                        listLabelBrush.Freeze();
                            
                        TextAlignment alignment = TextAlignment.Center;

                        switch (layerObject.Attributes["alignment"]?.InnerText)
                        {
                            case "Left":
                                alignment = TextAlignment.Left;
                                break;
                            case "Right":
                                alignment = TextAlignment.Right;
                                break;
                                
                        }


                        AddListLabelOutlines(layerObject, objectWidth, objectHeight, objectPositionX - 1, objectPositionY - 1, alignment, listLabelOutlineBrush, listLabelTextBlockFontSize, listLabelFontFamily);

                        AddListLabelOutlines(layerObject, objectWidth, objectHeight, objectPositionX - 1, objectPositionY + 1, alignment, listLabelOutlineBrush, listLabelTextBlockFontSize, listLabelFontFamily);

                        AddListLabelOutlines(layerObject, objectWidth, objectHeight, objectPositionX + 1, objectPositionY - 1, alignment, listLabelOutlineBrush, listLabelTextBlockFontSize, listLabelFontFamily);

                        AddListLabelOutlines(layerObject, objectWidth, objectHeight, objectPositionX + 1, objectPositionY + 1, alignment, listLabelOutlineBrush, listLabelTextBlockFontSize, listLabelFontFamily);

                        AddListLabelOutlines(layerObject, objectWidth, objectHeight, objectPositionX, objectPositionY, alignment, listLabelBrush, listLabelTextBlockFontSize, listLabelFontFamily);
                            
                        break;
                    case "textlist":
                        bool tlPriority;
                        if (bool.TryParse(layerObject.Attributes["control"]?.Value, out tlPriority))
                        {
                            listTypePriority = Models.ListType.Text;
                        }

                        // always set these to 0 since the theme can configure them
                        gameListMarginX = 0;
                        gameListMarginY = 0;
                        gameListOverscanX = 0;
                        gameListOverscanY = 0;
                            
                        double.TryParse(layerObject.Attributes["listitemhorizontalpadding"]?.InnerText, out listItemHorizontalPadding);
                        double.TryParse(layerObject.Attributes["unselecteditemverticalpadding"]?.InnerText, out unselectedItemVerticalPadding);
                        double.TryParse(layerObject.Attributes["selecteditemverticalpadding"]?.InnerText, out selectedItemVerticalPadding);
                        bool.TryParse(layerObject.Attributes["antialiastext"]?.InnerText, out antialiasedText);

                        Guid guid = Guid.NewGuid();

                        string font = themeConfigNode.Attributes["font"]?.Value;

                        if (!string.IsNullOrWhiteSpace(font))
                        {
                            gameListFont = new FontFamily(font);
                        }

                        string outlineColorHex = layerObject.Attributes["outlineArgb"]?.Value;

                        Color outlineColor = (Color)ColorConverter.ConvertFromString(outlineColorHex);

                        gameListOutlineColor = new SolidColorBrush(outlineColor);
                        gameListOutlineColor.Freeze();



                        switch (layerObject.Attributes["alignment"]?.InnerText)
                        {
                            case "Left":
                                gameListAlignment = TextAlignment.Left;
                                break;
                            case "Right":
                                gameListAlignment = TextAlignment.Right;
                                break;
                        }

                        double.TryParse(layerObject.Attributes["selectedFontSize"]?.Value, out selectedTextSize);

                        double.TryParse(layerObject.Attributes["unselectedFontSize"]?.Value, out unselectedTextSize);
                            
                        string glSelectedForegroundHex = layerObject.Attributes["selectedForegroundArgb"]?.Value;
                        Color glSelectedForegroundColor = (Color)ColorConverter.ConvertFromString(glSelectedForegroundHex);
                        gameListSelectedForegroundColor = new SolidColorBrush(glSelectedForegroundColor);

                        string glUnselectedForegroundTopHex = layerObject.Attributes["unselectedForegroundTopArgb"]?.Value;
                        Color glUnselectedForegroundTopColor = (Color)ColorConverter.ConvertFromString(glUnselectedForegroundTopHex);
                        gameListUnselectedTopColor = new SolidColorBrush(glUnselectedForegroundTopColor);

                        string glUnselectedForegroundBottomHex = layerObject.Attributes["unselectedForegroundBottomArgb"]?.Value;
                        Color glUnselectedForegroundBottomColor = (Color)ColorConverter.ConvertFromString(glUnselectedForegroundBottomHex);
                        gameListUnselectedBottomColor = new SolidColorBrush(glUnselectedForegroundBottomColor);

                        string glSelectedBackgroundHex = layerObject.Attributes["selectedBackgroundArgb"]?.Value;
                        Color glSelectedBackgroundColor = (Color)ColorConverter.ConvertFromString(glSelectedBackgroundHex);
                        gameListSelectedBackgroundColor = new SolidColorBrush(glSelectedBackgroundColor);
                            
                        themeListWidth = objectWidth;
                        themeListHeight = objectHeight;

                        Canvas glCanvas = new Canvas();
                        glCanvas.Width = objectWidth;
                        glCanvas.Height = objectHeight;
                        glCanvas.Name = "GameList";
                            

                        Canvas glOutlineCanvas = new Canvas();
                        glOutlineCanvas.Width = objectWidth;
                        glOutlineCanvas.Height = objectHeight;
                        glOutlineCanvas.Name = "GameListOutlines";
                        glCanvas.Children.Add(glOutlineCanvas);

                        this.textBlockListCanvas = glCanvas;
                        this.gameListCanvasOutlines = glOutlineCanvas;


                        FrontEndContainer.Children.Add(glCanvas);

                        templateObjects[guid] = glCanvas;

                        Canvas.SetLeft(glCanvas, objectPositionX);
                        Canvas.SetTop(glCanvas, objectPositionY);

                        double glRotation;
                        double.TryParse(layerObject.Attributes["rotate"]?.Value, out glRotation);

                        if (glRotation != 0)
                        {
                            RotateTransform glRotate = new RotateTransform(glRotation, glCanvas.Width / 2, glCanvas.Height / 2);
                            glCanvas.RenderTransform = glRotate;
                        }

                        CreateAnimation(layerObject, glCanvas);

                        break;
                    case "preview":
                        string noImagePath = themeDir + @"\" + layerObject.Attributes["emptyimg"]?.Value;

                        Uri.TryCreate(noImagePath, UriKind.RelativeOrAbsolute, out notFoundImageUri);

                        versusThumbnailImage = BigBlue.ImageLoading.loadImageFromUri(notFoundImageUri, resolutionX, resolutionY);

                        string speechImagePath = themeDir + @"\" + layerObject.Attributes["speechimg"]?.Value;

                        Uri speechImageUri;
                        Uri.TryCreate(speechImagePath, UriKind.RelativeOrAbsolute, out speechImageUri);

                        speechThumbnailImage = BigBlue.ImageLoading.loadImageFromUri(speechImageUri, resolutionX, resolutionY);

                        Canvas listItemPreviewCanvas = new Canvas();
                        listItemPreviewCanvas.Width = objectWidth;
                        listItemPreviewCanvas.Height = objectHeight;

                        Rectangle snapshotBgRectangle = new Rectangle();
                        snapshotBgRectangle.Width = objectWidth;
                        snapshotBgRectangle.Height = objectHeight;
                        snapshotBgRectangle.VerticalAlignment = VerticalAlignment.Center;
                        snapshotBgRectangle.HorizontalAlignment = HorizontalAlignment.Center;
                        snapshotBgRectangle.IsHitTestVisible = false;
                        snapshotBgRectangle.Fill = blackBrush;

                        listItemPreviewCanvas.Children.Add(snapshotBgRectangle);

                        snapshotImageControl = new Image();
                        snapshotImageControl.Width = objectWidth;
                        snapshotImageControl.Height = objectHeight;
                        snapshotImageControl.VerticalAlignment = VerticalAlignment.Center;
                        snapshotImageControl.Stretch = Stretch.Uniform;
                        snapshotImageControl.HorizontalAlignment = HorizontalAlignment.Center;
                        snapshotImageControl.IsHitTestVisible = false;
                            
                        listItemPreviewCanvas.Children.Add(snapshotImageControl);

                        videoCanvas = new Canvas();
                        videoCanvas.Width = objectWidth;
                        videoCanvas.Height = objectHeight;
                        videoCanvas.IsHitTestVisible = false;
                        videoCanvas.Background = blackBrush;
                        videoCanvas.Opacity = 0;
                            
                        videoMe = new MediaElement();
                        videoMe.Width = objectWidth;
                        videoMe.Height = objectHeight;
                        videoMe.Name = "VideoElement";
                        videoMe.Stretch = Stretch.Uniform;
                        videoMe.HorizontalAlignment = HorizontalAlignment.Center;
                        videoMe.IsHitTestVisible = false;
                        videoMe.MediaFailed += VideoElement_MediaFailed;
                        videoMe.Volume = minimumVolume;

                        videoCanvas.Children.Add(videoMe);
                            
                        listItemPreviewCanvas.Children.Add(videoCanvas);

                        FrontEndContainer.Children.Add(listItemPreviewCanvas);

                        double previewRotation;
                        double.TryParse(layerObject.Attributes["rotate"]?.Value, out previewRotation);

                        if (previewRotation != 0)
                        {
                            RotateTransform glRotate = new RotateTransform(previewRotation);
                            listItemPreviewCanvas.RenderTransform = glRotate;
                        }

                        Canvas.SetLeft(listItemPreviewCanvas, objectPositionX);
                        Canvas.SetTop(listItemPreviewCanvas, objectPositionY);
                            
                        snapShotWidth = resolutionX;
                        snapShotHeight = resolutionY;

                        CreateAnimation(layerObject, listItemPreviewCanvas);

                        //listItemPreviewCanvas.Opacity = 0.4;
                        if (objectOpacity > 0)
                        {
                            listItemPreviewCanvas.Opacity = objectOpacity;
                        }


                        break;
                }
            }
            
            double.TryParse(themeConfigNode.Attributes["width"]?.InnerText, out configResolutionX);
            double.TryParse(themeConfigNode.Attributes["height"]?.InnerText, out configResolutionY);
        }

        private void AddListLabelOutlines(XmlNode layerObject, double objectWidth, double objectHeight, double leftPosition, double topPosition, TextAlignment alignment, SolidColorBrush textColor, double fontSize, FontFamily fontFamily)
        {
            TextBlock listLabelTextBlockOutline = new TextBlock();
            listLabelTextBlockOutline.Width = objectWidth;
            listLabelTextBlockOutline.Height = objectHeight;
            listLabelTextBlockOutline.Focusable = false;
            listLabelTextBlockOutline.TextTrimming = TextTrimming.CharacterEllipsis;
            listLabelTextBlockOutline.IsHitTestVisible = false;
            listLabelTextBlockOutline.TextWrapping = TextWrapping.NoWrap;
            listLabelTextBlockOutline.AllowDrop = false;
            listLabelTextBlockOutline.TextAlignment = alignment;
            listLabelTextBlockOutline.Foreground = textColor;
            listLabelTextBlockOutline.FontSize = fontSize;
            listLabelTextBlockOutline.FontFamily = FontFamily;
            listLabelTextBlockOutline.FontWeight = FontWeights.Bold;

            FrontEndContainer.Children.Add(listLabelTextBlockOutline);

            Canvas.SetLeft(listLabelTextBlockOutline, leftPosition);
            Canvas.SetTop(listLabelTextBlockOutline, topPosition);
            

            Binding listLabelTextBlockOutlineBinding = new Binding("Name");
            listLabelTextBlockOutlineBinding.Source = currentListName;

            listLabelTextBlockOutline.SetBinding(TextBlock.TextProperty, listLabelTextBlockOutlineBinding);

            CreateAnimation(layerObject, listLabelTextBlockOutline);
        }

        private void StyleGameListTextBlocks()
        {
            if (gameListElements != null)
            {
                if (gameListElements.Count > 0)
                {
                    for (int i = 0; i < numberOfTextBlockListItems; i++)
                    {
                        TextBlock gameListEntry = gameListElements[i];

                        if (i < textBlockListHalfWayPoint)
                        {
                            gameListEntry.Foreground = gameListUnselectedTopColor;
                        }
                        else if (i == textBlockListHalfWayPoint)
                        {
                            gameListEntry.Background = gameListSelectedBackgroundColor;
                            gameListEntry.Foreground = gameListSelectedForegroundColor;
                        }
                        else
                        {
                            gameListEntry.Foreground = gameListUnselectedBottomColor;
                        }
                    }

                    gameListCanvasOutlines.Visibility = Visibility.Visible;
                }
            }
        }

        private void ScaleScreenToFit()
        {
            // the resolution doesn't match the template
            // we're going to have to rescale it
            if (configResolutionX != screenWidth || configResolutionY != screenHeight)
            {
                ScaleTransform st = new ScaleTransform();

                bool adjustMarginX = false;
                bool adjustMarginY = false;

                double margin = 0;

                double configRatio = configResolutionX / configResolutionY;
                double screenRatio = screenWidth / screenHeight;

                double scaleXMultiplier = screenWidth / configResolutionX;
                double scaleYMultiplier = screenHeight / configResolutionY;

                double scaleMultiplier = 1;

                if (screenRatio >= configRatio)
                {
                    scaleMultiplier = screenHeight / configResolutionY;
                }
                else
                {
                    scaleMultiplier = screenWidth / configResolutionX;
                }

                // if the screen ratio is greater than or equal to the config's ratio,
                // then we're going to be adjusting the horizonal margin
                if (screenRatio > configRatio && configResolutionX > screenWidth)
                {
                    adjustMarginX = true;
                }

                if (screenRatio < configRatio && configResolutionY > screenHeight)
                {
                    adjustMarginY = true;
                }


                if (screenWidth > configResolutionX)
                {
                    st.CenterX = configResolutionX / 2;
                }
                else
                {
                    st.CenterX = 0;
                }

                if (screenHeight > configResolutionY)
                {
                    st.CenterY = configResolutionY / 2;
                }
                else
                {
                    st.CenterY = 0;
                }

                st.ScaleX = scaleMultiplier;
                st.ScaleY = scaleMultiplier;

                FrontEndContainer.RenderTransform = st;

                if (adjustMarginY)
                {
                    margin = (screenHeight - (scaleMultiplier * configResolutionY)) / 2;
                    Canvas.SetTop(FrontEndContainer, margin);
                }

                if (adjustMarginX)
                {
                    margin = (screenWidth - (scaleMultiplier * configResolutionX)) / 2;
                    Canvas.SetLeft(FrontEndContainer, margin);
                }
            }            
        }

        internal override void InitializeFrontEnd()
        {
            InitializeSound();

            ProvisionMainWindow(LayoutRoot, FrontEndContainer);

            SetImageScalingMode();

            // we set this so that the main menu dimensions can be calculated correctly at different resolutions
            width = configResolutionX;
            height = configResolutionY;

            SetMainMenuDimensions(MainMenu, mainMenuUnselectedColor);

            Uri menuBackgroundImageSource;

            if (Uri.TryCreate(mainMenuBackgroundSource, UriKind.RelativeOrAbsolute, out menuBackgroundImageSource))
            {
                ImageBrush mmBrush = BigBlue.ImageLoading.loadImageBrushFromUri(menuBackgroundImageSource, Convert.ToInt32(MainMenu.Width), Convert.ToInt32(MainMenu.Height));
                MainMenu.Background = mmBrush;
            }
            else
            {
                Color menuBackgroundColor = (Color)ColorConverter.ConvertFromString(mainMenuBackgroundSource);

                SolidColorBrush mbgc = new SolidColorBrush(menuBackgroundColor);
                mbgc.Freeze();

                MainMenu.Background = mbgc;
            }

            if (textBlockListCanvas != null)
            {
                bool validGameListDimensions = false;

                while (validGameListDimensions == false)
                {
                    validGameListDimensions = SetGameListDimensions(this.textBlockListCanvas, gameListCanvasOutlines, 0, 0, themeListWidth, themeListHeight);
                }
            }
            
            // always set the frontend container to be the dimensions of the template
            FrontEndContainer.Width = configResolutionX;
            FrontEndContainer.Height = configResolutionY;
                      
            if (stretchSnapshots)
            {
                snapshotImageControl.Stretch = Stretch.Fill;
                videoMe.Stretch = Stretch.Fill;

                if (marqueeDisplay)
                {
                    marqueeWindow.SecondaryWindowSnapshot.Stretch = Stretch.Fill;
                }

                if (instructionDisplay)
                {
                    instructionWindow.SecondaryWindowSnapshot.Stretch = Stretch.Fill;
                }

                if (flyerDisplay)
                {
                    flyerWindow.SecondaryWindowSnapshot.Stretch = Stretch.Fill;
                }
            }
            
            LoadFrontendList();

            if (textBlockListCanvas != null)
            {
                AddGameListTextBlocks(textBlockListCanvas, gameListCanvasOutlines, gameListOutlineColor);
                StyleGameListTextBlocks();    
            }
            
            ProvisionUIAnimations(LayoutRoot, FrontEndContainer);

            // resize the screen if needed
            ScaleScreenToFit();

            currentListName.Name = frontendLists[selectedListGuid].ListName;
            
            RenderGameList(false);
                        
            // start timers
            if (screenSaverTimeInMinutes >= 1)
            {
                screenSaverTimer.Start();
            }

            // call this on init to kick off the animations
            foreach (BigBlue.Models.ImageAnimation ia in imageAnimations.Values)
            {
                ia.Timer.Start();
            }
            
            CompositionTarget.Rendering += OnFrame;

            // prevent the windows cursor from getting out of the window
            this.Activated += MainWindow_Activated;
            
            ShowFrontEndStoryboard.Begin();

            BigBlue.NativeMethods.SetForegroundWindow(windowHandle);
        }

        internal override void ProvisionFrontendFadeAnimations(System.Windows.FrameworkElement feContainer)
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation();
            fadeInAnimation.From = 0;
            fadeInAnimation.To = 1;
            fadeInAnimation.SpeedRatio = 1;
            fadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            fadeInAnimation.AutoReverse = false;

            Storyboard.SetTarget(fadeInAnimation, feContainer);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));

            fadeInAnimation.Freeze();

            // fade out animation start
            DoubleAnimation fadeOutAnimation = new DoubleAnimation();
            fadeOutAnimation.From = 1;
            fadeOutAnimation.To = 0;
            fadeOutAnimation.SpeedRatio = 1;
            fadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            fadeOutAnimation.AutoReverse = false;

            Storyboard.SetTarget(fadeOutAnimation, feContainer);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));

            fadeOutAnimation.Freeze(); // fade out animation end

            DoubleAnimation fadeOutGameLaunchAnimation = new DoubleAnimation();
            fadeOutGameLaunchAnimation.From = 1;
            fadeOutGameLaunchAnimation.To = -1;
            fadeOutGameLaunchAnimation.SpeedRatio = 1;
            fadeOutGameLaunchAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));
            fadeOutGameLaunchAnimation.AutoReverse = false;
            fadeOutGameLaunchAnimation.Completed += SelectGame;

            Storyboard.SetTarget(fadeOutGameLaunchAnimation, feContainer);
            Storyboard.SetTargetProperty(fadeOutGameLaunchAnimation, new PropertyPath("Opacity"));

            fadeOutGameLaunchAnimation.Freeze(); // fade out animation end

            // wrap up show frontend storyboard
            ShowFrontEndStoryboard.Children.Add(fadeInAnimation);
            ShowFrontEndStoryboard.Completed += ShowFrontEndStoryboard_Completed;
            ShowFrontEndStoryboard.Freeze();

            // wrap up launch game storyboard
            LaunchGameStoryboard.Children.Add(fadeOutGameLaunchAnimation);
            //LaunchGameStoryboard.Completed += selectGame;
            LaunchGameStoryboard.Freeze();
        }

        void ShowFrontEndStoryboard_Completed(object sender, EventArgs e)
        {
            // play music as it fades in
            if (XAudio2Player.LoadedSounds.ContainsKey(MusicSoundKey))
            {
                videoMe.Volume = 0;
                wmpVolume = 0;

                // must be a threading issue
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    BigBlue.XAudio2Player.PlaySound(MusicSoundKey, null);
                });
            }

            // continue counting time towards the screen saver when you return from the game
            if (screenSaverTimeInMinutes >= 1)
            {
                screenSaverTimer.Start();
            }

            if (frontendShown == false)
            {
                frontendShown = true;

                BigBlue.NativeMethods.ProvisionRawInputs(this, false);

                mouseStopWatch.Start();

                // hide mouse cursor code
                if (hideMouseCursor == true)
                {
                    string cursorFileName = "HiddenCursor.cur";

                    IntPtr appstartCursor = BigBlue.NativeMethods.LoadCursorFromFile(cursorFileName);
                    bool setAppstartCursorSuccessfully = BigBlue.NativeMethods.SetSystemCursor(appstartCursor, BigBlue.NativeMethods.OCR_APPSTARTING);

                    IntPtr standardCursor = BigBlue.NativeMethods.LoadCursorFromFile(cursorFileName);
                    bool setStandardCursorSuccessfully = BigBlue.NativeMethods.SetSystemCursor(standardCursor, BigBlue.NativeMethods.OCR_NORMAL);

                    IntPtr hourglassCursor = BigBlue.NativeMethods.LoadCursorFromFile(cursorFileName);
                    bool setHourglassCursorSuccessfully = BigBlue.NativeMethods.SetSystemCursor(hourglassCursor, BigBlue.NativeMethods.OCR_WAIT);
                }

                foreach (Storyboard sb in animations.Values)
                {
                    sb.Begin();
                }
            }

            // start the video fade in stopwatch
            videoFadeInStopwatch.Start();

            
        }

        private void LastItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && awaitingAsync == false && itsGoTime == false && ((action == "RAMPAGE_LAST_ITEM") || (action == "RTYPE_LAST_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    selectedListItemIndex = frontendLists[selectedListGuid].Total;

                    if (RenderGameListCheck() == true)
                    {
                        RenderGameList(true);
                    }

                    frontendInputs[action].wasPressed = true;
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void NextItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && awaitingAsync == false && itsGoTime == false && ((action == "RAMPAGE_NEXT_ITEM") || (action == "RTYPE_NEXT_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    frontendInputs["RAMPAGE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_NEXT_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_PAGE"].wasPressed = false;

                    frontendInputs[action].wasPressed = true;

                    if (menuOpen == true)
                    {
                        CalculateNextMenuItem(MainMenu, mainMenuSelectedColor, mainMenuUnselectedColor);
                    }
                    else
                    {
                        CalculateGame(1);
                    }
                }
            }
            else
            {
                ReleaseInput(action);
                timeToChange = 400;
            }
        }

        private void GameListMouseAction(string action)
        {
            if (mouseStopWatch.ElapsedMilliseconds > mouseMovementSpeed)
            {
                if (screenSaver == false && awaitingAsync == false && itsGoTime == false)
                {
                    if (menuOpen == true)
                    {
                        if ((action == "RAMPAGE_NEXT_ITEM") || (action == "RTYPE_NEXT_ITEM"))
                        {
                            CalculateNextMenuItem(MainMenu, mainMenuSelectedColor, mainMenuUnselectedColor);
                        }

                        if ((action == "RAMPAGE_PREVIOUS_ITEM") || (action == "RTYPE_PREVIOUS_ITEM"))
                        {
                            CalculatePreviousMenuItem(MainMenu, mainMenuSelectedColor, mainMenuUnselectedColor);
                        }
                    }
                    else
                    {
                        if (pausedBySystem == false)
                        {
                            if ((action == "RAMPAGE_NEXT_ITEM") || (action == "RTYPE_NEXT_ITEM"))
                            {
                                CalculateGame(1);
                            }

                            if (action == "RAMPAGE_PREVIOUS_ITEM" || action == "RTYPE_PREVIOUS_ITEM")
                            {
                                CalculateGame(-1);
                            }

                            if (action == "RAMPAGE_PREVIOUS_PAGE" || action == "RTYPE_PREVIOUS_PAGE")
                            {
                                if (listTypePriority == Models.ListType.Image)
                                {
                                    CalculateGame(-frontendLists[selectedListGuid].ImageItemsToPage);
                                }
                                else
                                {
                                    CalculateGame(-frontendLists[selectedListGuid].TextBlockItemsToPage);
                                }
                            }

                            if ((action == "RAMPAGE_NEXT_PAGE") || (action == "RTYPE_NEXT_PAGE"))
                            {
                                if (listTypePriority == Models.ListType.Image)
                                {
                                    CalculateGame(frontendLists[selectedListGuid].ImageItemsToPage);
                                }
                                else
                                {
                                    CalculateGame(frontendLists[selectedListGuid].TextBlockItemsToPage);
                                }
                            }
                        }
                    }
                }
                // this should be moved into the mouse methods and you should return after getting one
                mouseStopWatch.Restart();
            }
        }

        private void PreviousPageKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && pausedBySystem == false && awaitingAsync == false && itsGoTime == false && ((action == "RAMPAGE_PREVIOUS_PAGE") || (action == "RTYPE_PREVIOUS_PAGE")) && menuOpen == false && frontendInputs[action].isRepeating == false)
                {
                    frontendInputs["RAMPAGE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_NEXT_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_PAGE"].wasPressed = false;

                    frontendInputs[action].wasPressed = true;

                    if (listTypePriority == Models.ListType.Image)
                    {
                        CalculateGame(-frontendLists[selectedListGuid].ImageItemsToPage);
                    }
                    else
                    {
                        CalculateGame(-frontendLists[selectedListGuid].TextBlockItemsToPage);
                    }
                }
            }
            else
            {
                ReleaseInput(action);
                timeToChange = 400;
            }
        }

        private void NextPageKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && pausedBySystem == false && awaitingAsync == false && itsGoTime == false && ((action == "RAMPAGE_NEXT_PAGE") || (action == "RTYPE_NEXT_PAGE")) && menuOpen == false && frontendInputs[action].isRepeating == false)
                {
                    frontendInputs["RAMPAGE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_PAGE"].wasPressed = false;

                    frontendInputs[action].wasPressed = true;

                    if (listTypePriority == Models.ListType.Image)
                    {
                        CalculateGame(frontendLists[selectedListGuid].ImageItemsToPage);
                    }
                    else
                    {
                        CalculateGame(frontendLists[selectedListGuid].TextBlockItemsToPage);
                    }
                }
            }
            else
            {
                ReleaseInput(action);
                timeToChange = 400;
            }
        }


        private void FirstItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && awaitingAsync == false && itsGoTime == false && ((action == "RAMPAGE_FIRST_ITEM") || (action == "RTYPE_FIRST_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    
                    selectedListItemIndex = 0;

                    if (RenderGameListCheck() == true)
                    {
                        RenderGameList(true);
                    }

                    frontendInputs[action].wasPressed = true;
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        internal override void ProcessFrontendAction(string action, bool? inputDown)
        {
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
                    MenuAction(action, inputDown);
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
                case "RAMPAGE_FIRST_ITEM":
                case "RTYPE_FIRST_ITEM":
                    FirstItemKeyboardAction(action, inputDown);
                    break;
                case "RAMPAGE_LAST_ITEM":
                case "RTYPE_LAST_ITEM":
                    LastItemKeyboardAction(action, inputDown);
                    break;
                case "RAMPAGE_RANDOM_ITEM":
                case "RTYPE_RANDOM_ITEM":
                    RandomItemKeyboardAction(action, inputDown);
                    break;
                case "RAMPAGE_NEXT_ITEM":
                case "RTYPE_NEXT_ITEM":
                    if (inputDown == null)
                    {
                        // mouse version here
                        GameListMouseAction(action);
                    }
                    else
                    {
                        NextItemKeyboardAction(action, inputDown);
                    }
                    break;
                case "RAMPAGE_PREVIOUS_ITEM":
                case "RTYPE_PREVIOUS_ITEM":
                    if (inputDown == null)
                    {
                        // mouse version here
                        GameListMouseAction(action);
                    }
                    else
                    {
                        PreviousItemKeyboardAction(action, inputDown);
                    }
                    break;
                case "RAMPAGE_NEXT_PAGE":
                case "RTYPE_NEXT_PAGE":
                    if (inputDown == null)
                    {
                        // mouse version here
                        GameListMouseAction(action);
                    }
                    else
                    {
                        NextPageKeyboardAction(action, inputDown);
                    }
                    break;
                case "RAMPAGE_PREVIOUS_PAGE":
                case "RTYPE_PREVIOUS_PAGE":
                    if (inputDown == null)
                    {
                        // mouse version here
                        GameListMouseAction(action);
                    }
                    else
                    {
                        PreviousPageKeyboardAction(action, inputDown);
                    }
                    break;
                case "RAMPAGE_BACK":
                case "RTYPE_BACK":
                    GameListBackAction(action, inputDown);
                    break;
                case "RAMPAGE_START":
                case "RTYPE_START":
                    PlayerStartAction(action, inputDown);
                    break;
                case "BIG_BLUE_SPEECH":
                    InitiateSpeechAction(action, inputDown);
                    break;
                case "RESTART":
                    PlayerMenuShortcutAction(action, BigBlue.Models.FrontEndExitMode.restart, inputDown);
                    break;
                case "SHUTDOWN":
                    PlayerMenuShortcutAction(action, BigBlue.Models.FrontEndExitMode.shutdown, inputDown);
                    break;
                case "QUIT_TO_DESKTOP":
                    PlayerMenuShortcutAction(action, BigBlue.Models.FrontEndExitMode.quit, inputDown);
                    break;
            }
        }

        private void PlayerMenuShortcutAction(string action, BigBlue.Models.FrontEndExitMode em, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (frontendInputs[action].wasPressed == true)
                {
                    frontendInputs[action].isRepeating = true;
                }

                if (!menuOpen && shutdownSequenceActivated == true)
                {
                    exitMode = em;
                    ExitFrontEnd(FrontEndContainer);
                    // unpress all other keys here
                    return;
                }

                if (!menuOpen && !itsGoTime && !screenSaver && !frontendInputs[action].isRepeating)
                {
                    exitMode = em;
                    StartShutdown(false);
                }

                frontendInputs[action].wasPressed = true;
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void PreviousItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && awaitingAsync == false && itsGoTime == false && ((action == "RAMPAGE_PREVIOUS_ITEM") || (action == "RTYPE_PREVIOUS_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    // might not need to manually release the other keys
                    frontendInputs["RAMPAGE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RAMPAGE_NEXT_PAGE"].wasPressed = false;

                    frontendInputs["RTYPE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_PAGE"].wasPressed = false;

                    if (menuOpen == true)
                    {
                        CalculatePreviousMenuItem(MainMenu, mainMenuSelectedColor, mainMenuUnselectedColor);
                    }
                    else
                    {
                        CalculateGame(-1);
                    }

                    frontendInputs[action].wasPressed = true;
                }
            }
            else
            {
                ReleaseInput(action);
                timeToChange = 400;
            }
        }
                
        protected override void FinalListRender(MediaElement videoMe, System.Windows.Controls.Panel videoCanvas)
        {
            if (imageBlockListCanvas != null)
            {
                UpdateImageListImages(imageListSelectedWidth, imageListUnselectedWidth);
            }
            
            if (textBlockListCanvas != null)
            {
                UpdateTextBlockText();
            }
            
            // only do this when the preview's been defined in the theme XML
            SetGameSnapshots(false);

            StopVideo();
            ResetVideo();
        }
        
        internal async void UpdateImageListImages(int selectedImgWidth, int unselectedImgWidth)
        {
            int reverseCount = selectedListItemIndex;
            int forwardCount = selectedListItemIndex;

            // loop through the entries before the middle
            for (int i = imageBlockListHalfWayPoint; i-- > 0;)
            {
                reverseCount = reverseCount - 1;

                if (reverseCount >= 0 && reverseCount <= frontendLists[selectedListGuid].Total)
                {
                    ImageBrush imageBrush = await Task.Run(() => { return GetImageBrushFromPath(frontendLists[selectedListGuid].ListItems[reverseCount].Snapshots[0], unselectedImgWidth); });

                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        listItemImageBlocks[i].Fill = imageBrush;
                        listItemImageBlocks[i].Stroke = imageListBorderBrush;
                    });
                }
                else
                {
                    if (reverseCount < 0 && frontendLists[selectedListGuid].Total >= imageBlockListHalfWayPoint)
                    {
                        reverseCount = frontendLists[selectedListGuid].Total;

                        ImageBrush imageBrush2 = await Task.Run(() => { return GetImageBrushFromPath(frontendLists[selectedListGuid].ListItems[reverseCount].Snapshots[0], unselectedImgWidth); });

                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            listItemImageBlocks[i].Fill = imageBrush2;
                            listItemImageBlocks[i].Stroke = imageListBorderBrush;
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            listItemImageBlocks[i].Fill = null;
                            listItemImageBlocks[i].Stroke = null;
                        });
                    }

                }
            }

            ImageBrush imageBrush3 = await Task.Run(() => { return GetImageBrushFromPath(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Snapshots[0], selectedImgWidth); });

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                listItemImageBlocks[imageBlockListHalfWayPoint].Fill = imageBrush3;
                listItemImageBlocks[imageBlockListHalfWayPoint].Stroke = imageListBorderBrush;
            });

            // loop through the entries after the middle
            for (int i = imageBlockListItemsToPage; i < numberOfImageBlockListItems; i++)
            {
                forwardCount = forwardCount + 1;

                if (forwardCount >= 0 && forwardCount <= frontendLists[selectedListGuid].Total)
                {

                    ImageBrush imageBrush4 = await Task.Run(() => { return GetImageBrushFromPath(frontendLists[selectedListGuid].ListItems[forwardCount].Snapshots[0], unselectedImgWidth); });

                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        listItemImageBlocks[i].Fill = imageBrush4;
                        listItemImageBlocks[i].Stroke = imageListBorderBrush;
                    });
                }
                else
                {
                    if (forwardCount > frontendLists[selectedListGuid].Total && frontendLists[selectedListGuid].Total >= imageBlockListHalfWayPoint)
                    {
                        forwardCount = 0;

                        ImageBrush imageBrush5 = await Task.Run(() => { return GetImageBrushFromPath(frontendLists[selectedListGuid].ListItems[forwardCount].Snapshots[0], unselectedImgWidth); });

                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            listItemImageBlocks[i].Fill = imageBrush5;
                            listItemImageBlocks[i].Stroke = imageListBorderBrush;
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            listItemImageBlocks[i].Fill = null;
                            listItemImageBlocks[i].Stroke = null;
                        });
                    }
                }
            }
        }

        private void RandomItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (menuOpen == false && screenSaver == false && awaitingAsync == false && itsGoTime == false && ((action == "RAMPAGE_RANDOM_ITEM") || (action == "RTYPE_RANDOM_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    // generate a random number between 0 and the totalList amount
                    selectedListItemIndex = r.Next(0, frontendLists[selectedListGuid].Total + 1);

                    if (RenderGameListCheck() == true)
                    {
                        RenderGameList(false);
                    }

                    if (action == "RAMPAGE_RANDOM_ITEM")
                    {
                        PlayerStartAction("RAMPAGE_START", inputDown);
                    }
                    else
                    {
                        PlayerStartAction("RTYPE_START", inputDown);
                    }

                    frontendInputs[action].wasPressed = true;
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private async void GameListBackAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && menuOpen == false && awaitingAsync == false && itsGoTime == false && inputDown == true && (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentID != null || !string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder) || selectedListGuid == searchListGuid) && ((action == "RAMPAGE_BACK") || (action == "RTYPE_BACK")))
                {
                    BigBlue.XAudio2Player.PlaySound(ExitListSoundKey, null);

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
                            //frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder

                            awaitingAsync = true;

                            //TODO
                            // just make this a dark overflay instead of hiding the text block

                            SetGameSnapshots(true);

                            bool validList = await Task.Run(() => { return OpenSubFolderListItem(subFolderTrail[subFolderTrail.Count - 1], folderListGuid); });

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

                            awaitingAsync = false;

                            //TODO
                            // just make this a dark overflay instead of hiding the text block
                            
                        }
                        else
                        {
                            returnToOriginatingList = true;
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
                                    // frontendLists[(Guid)fallbackGuid].CurrentListIndex = selectedListItemIndex;

                                    //selectedListItemIndex = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].IndexOfParent;

                                    selectedListItemIndex = frontendLists[(Guid)fallbackGuid].CurrentListIndex;

                                    SelectList((Guid)fallbackGuid, true);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void PauseFrontend()
        {
            BigBlue.XAudio2Player.PauseAllSounds();

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach (Storyboard sb in animations.Values)
                {
                    sb.Pause();
                }

                PauseVideo(videoMe, videoCanvas);

                pausedBySystem = true;

                //TODO
                // just make this a dark overflay instead of hiding the text block
            });
        }

        private void ToggleMenu()
        {
            selectedMenuIndex = 0;

            if (MainMenu.Visibility == System.Windows.Visibility.Collapsed)
            {
                menuOpen = true;

                RenderMainMenu(MainMenu, false, mainMenuSelectedColor, mainMenuUnselectedColor);

                MainMenu.Visibility = System.Windows.Visibility.Visible;

                PauseFrontend();

                if (showClock)
                {
                    ClockCanvas.Visibility = Visibility.Collapsed;
                }

                BigBlue.XAudio2Player.PlaySound(ExitListSoundKey, null);
            }
            else
            {
                menuOpen = false;

                MainMenu.Visibility = System.Windows.Visibility.Collapsed;
                
                ResumeFrontend();

                BigBlue.XAudio2Player.PlaySound(SelectListSoundKey, null);
            }
        }

        private void ResumeVideo()
        {
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

        private void ResumeFrontend()
        {
            BigBlue.XAudio2Player.ResumeAllSounds();

            foreach (Storyboard sb in animations.Values)
            {
                sb.Resume();
            }

            if (shutdownSequenceActivated == false)
            {
                ResumeVideo();
            }
            
            pausedBySystem = false;

            if (DependencyPropertyHelper.GetValueSource(FrontEndContainer, Canvas.OpacityProperty).IsAnimated)
            {
                ShowFrontEndStoryboard.Resume();
            }
        }

        // async 
        private void MenuAction(string action, bool? inputDown)
        {
            // shouldn't be able to use the menu while a game is starting
            if (inputDown == true)
            {
                if (frontendInputs[action].wasPressed == true)
                {
                    frontendInputs[action].isRepeating = true;
                }

                if (frontendInputs[action].isRepeating == false && (action == "BIG_BLUE_EXIT" || action == "RAMPAGE_EXIT" || action == "RTYPE_EXIT"))
                {
                    if (FrontEndContainer.Opacity == 1 && screenSaver == false && itsGoTime == false && awaitingAsync == false)
                    {
                        if (!disableMenu)
                        {
                            if (shutdownSequenceActivated == true)
                            {
                                ExitFrontEnd(FrontEndContainer);
                                // unpress all other keys here
                                return;
                            }

                            ToggleMenu();
                        }
                    }
                    else
                    {
                        if (globalInputs == true && frontendLists[selectedListGuid].ListItems[selectedListItemIndex].KillTask == true && itsGoTime == true)
                        {
                            BigBlue.ProcessHandling.CloseAllProcesses(processes);
                            
                            //processName = string.Empty;    
                            //processId = -1;

                            ReturnFromGame(FrontEndContainer);

                            
                        }
                    }
                }

                frontendInputs[action].wasPressed = true;
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void StartShutdown(bool w)
        {
        }

        private async void PlayerStartAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                frontendInputs[action].wasPressed = true;

                if (itsGoTime == false && screenSaver == false && frontendInputs[action].isRepeating == false)
                {
                    if ((action == "RAMPAGE_START") || (action == "RTYPE_START"))
                    {
                        if (menuOpen == true)
                        {
                            switch (selectedMenuIndex)
                            {
                                case 0:
                                    ToggleMenu();
                                    break;
                                case 1:
                                    if (displayExitItemInMenu == true)
                                    {
                                        ExitFrontEnd(FrontEndContainer);
                                    }
                                    else
                                    {
                                        exitMode = BigBlue.Models.FrontEndExitMode.shutdown;
                                    }
                                    break;
                                case 2:
                                    if (displayExitItemInMenu == true)
                                    {
                                        exitMode = BigBlue.Models.FrontEndExitMode.shutdown;
                                    }
                                    else
                                    {
                                        exitMode = BigBlue.Models.FrontEndExitMode.restart;
                                    }
                                    StartShutdown(true);
                                    break;
                                case 3:
                                    exitMode = BigBlue.Models.FrontEndExitMode.restart;
                                    StartShutdown(true);
                                    break;
                            }
                        }
                        else
                        {
                            if (awaitingAsync == false && itsGoTime == false)
                            {
                                if (!string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder))
                                {
                                    BigBlue.XAudio2Player.PlaySound(SelectListSoundKey, string.Empty);

                                    bool returnToOriginatingList = false;

                                    string childFolderName = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder;


                                    if (!string.IsNullOrWhiteSpace(childFolderName))
                                    {
                                        subFolderTrail.Add(childFolderName);
                                    }

                                    if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID == null)
                                    {
                                        awaitingAsync = true;
                                        SetGameSnapshots(true);

                                        //TODO
                                        // just make this a dark overflay instead of hiding the text block

                                        bool validList = await Task.Run(() => { return OpenSubFolderListItem(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder, folderListGuid); });

                                        if (validList)
                                        {
                                            RenderSubFolderList(videoMe, videoCanvas);
                                        }
                                        else
                                        {
                                            returnToOriginatingList = true;
                                        }

                                        awaitingAsync = false;

                                        //TODO
                                        // just make this a dark overflay instead of hiding the text block
                                    }
                                    else
                                    {
                                        frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;

                                        Guid lg = (Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID;

                                        originatingFolderParentListItem = frontendLists[selectedListGuid].ListItems[selectedListItemIndex];
                                        originatingFolderListGuid = lg;

                                        awaitingAsync = true;
                                        SetGameSnapshots(true);

                                        //TODO
                                        // just make this a dark overflay instead of hiding the text block

                                        bool validList = await Task.Run(() => { return OpenFolderListItem(lg); });

                                        if (validList)
                                        {
                                            RenderFolderList(lg);
                                        }
                                        else
                                        {
                                            returnToOriginatingList = true;
                                        }

                                        awaitingAsync = false;

                                        //TODO
                                        // just make this a dark overflay instead of hiding the text block
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
                                    BigBlue.XAudio2Player.PlaySound(SelectListSoundKey, string.Empty);

                                    frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;

                                    SelectList((Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID, false);
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

                                        //TODO
                                        // just make this a dark overflay instead of hiding the text block                  

                                        // play the select sound
                                        videoMe.Volume = 0;

                                        awaitingAsync = true;

                                        // you need to wait for at least some of the sound to be played                                         
                                        BigBlue.XAudio2Player.StopAllSounds();

                                        // if we haven't added the sound to launch a game, then we're going to wait a second before launching the program
                                        // otherwise, we'll just launch it when the start sound is done playing
                                        if (XAudio2Player.LoadedSounds.ContainsKey(LaunchListItemSoundKey))
                                        {
                                            BigBlue.XAudio2Player.PlaySound(LaunchListItemSoundKey, null);
                                        }
                                        else
                                        {
                                            await Task.Run(() =>
                                            {
                                                System.Threading.Thread.Sleep(1000);
                                            });
                                            
                                            BeginGameLaunch();
                                        }
                                    }
                                    else
                                    {
                                        awaitingAsync = true;
                                        PlayLoseSound();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        internal override void BeginGameLaunch()
        {
            awaitingAsync = false;

            PauseFrontend();

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                // fade out the screen to black;
                LaunchGameStoryboard.Begin();
            });
        }

        private bool PressInput(string action)
        {
            if (frontendInputs[action].wasPressed == true)
            {
                frontendInputs[action].isRepeating = true;
            }
            
            return false;
        }

        private void InitiateSpeechAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                frontendInputs[action].wasPressed = true;

                if (awaitingAsync == false && itsGoTime == false && screenSaver == false && frontendInputs[action].isRepeating == false && recognizer != null)
                {
                    MatchingSearchWords.Clear();
                    recognizer.RecognizeAsyncCancel();

                    awaitingAsync = true;

                    //TODO
                    // just make this a dark overflay instead of hiding the text block

                    StopVideo();

                    // set all the images to the speech thumbnail
                    snapshotImageControl.Source = speechThumbnailImage;

                    if (marqueeDisplay)
                    {
                        marqueeWindow.SecondaryWindowSnapshot.Source = speechThumbnailImage;
                    }

                    if (flyerDisplay)
                    {
                        flyerWindow.SecondaryWindowSnapshot.Source = speechThumbnailImage;
                    }

                    if (instructionDisplay)
                    {
                        instructionWindow.SecondaryWindowSnapshot.Source = speechThumbnailImage;
                    }

                    // recognize the speech
                    recognizer.RecognizeAsync(RecognizeMode.Single);
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }


        void MainWindow_Activated(object sender, EventArgs e)
        {
            if (mouseCursorTrapped == true)
            {
                TrapMouseCursor(FrontEndContainer);
            }
        }

        private void SelectGame(object sender, EventArgs e)
        {
            try
            {
                ReleaseAllInputs();

                StopVideo();

                CompositionTarget.Rendering -= OnFrame;

                // stop counting time towards the screen saver when you start a game
                screenSaverTimer.Stop();
                
                LaunchProgram(FrontEndContainer, directoryToLaunch, fileNameToLaunch, argumentsToLaunch, lt);
            }
            catch
            {
                BackToGameList();

                awaitingAsync = true;
                PlayLoseSound();
            }
        }

        internal override void BackToGameList()
        {
            base.BackToGameList();

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                BigBlue.XAudio2Player.RestartAudioEngine();

                CompositionTarget.Rendering += OnFrame;

                videoMe.Volume = minimumVolume;

                this.Activate();

                itsGoTime = false;

                ResumeFrontend();

                ShowFrontEndStoryboard.Begin();
                isVideoPlaying = false;                
            });
        }

        private void SetImageScalingMode()
        {
            if (width >= baseHorizontalResolution && IsInteger(resolutionXMultiplier))
            {
                integerMultiplier = true;

                RenderOptions.SetBitmapScalingMode(snapshotImageControl, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(videoCanvas, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(videoMe, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(ClockCanvas, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
                RenderOptions.SetEdgeMode(LayoutRoot, EdgeMode.Aliased);
                RenderOptions.SetEdgeMode(FrontEndContainer, EdgeMode.Aliased);
            }
        }

        internal override void InitializeSound()
        {
            BigBlue.XAudio2Player.ProvisionPlayer(miniGameVolume);

            // no point in loading these if the sound is disabled
            if (!BigBlue.XAudio2Player.Disabled)
            {
                XmlNodeList soundNodes = themeConfigNode.SelectNodes("sounds/sound");

                foreach (XmlNode soundNode in soundNodes)
                {
                    string soundKey = soundNode.Attributes["type"]?.Value;
                    string soundUriString = themeDir + @"\" + soundNode.Attributes["src"]?.Value;

                    if (System.IO.File.Exists(soundUriString))
                    {
                        switch (soundKey)
                        {
                            case LaunchListItemSoundKey:
                                BigBlue.XAudio2Player.AddAudioFile("all", soundKey, soundUriString, false, true, false, BeginGameLaunch);
                                break;
                            case FailureSoundKey:
                                BigBlue.XAudio2Player.AddAudioFile("all", soundKey, soundUriString, false, true, false, HumiliationFinished);
                                break;
                            case MusicSoundKey:
                                BigBlue.XAudio2Player.AddAudioFile("all", soundKey, soundUriString, true, true, false, null);
                                break;
                            default:
                                BigBlue.XAudio2Player.AddAudioFile("all", soundKey, soundUriString, false, true, false, null);
                                break;
                        }
                    }
                }
            }
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

            videoMe = me;
        }

        private void ResetVideoControl()
        {
            StopVideo();
            videoMe = null;
            CreateVideoControl();
            videoFadeInStopwatch.Start();
        }

        private void VideoElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ResetVideoControl();
        }

        override public void OnFrame(object sender, EventArgs e)
        {
            if (menuOpen == false)
            {
                ManageVideo();

                ManageGameList();

                foreach (BigBlue.Models.ImageAnimation ia in imageAnimations.Values)
                {
                    AnimateImage(ia);
                }
            }
        }
    }
}