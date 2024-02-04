using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Globalization;

namespace BigBlue
{
    public static class TextRendering
    {
        public static Size MeasureString(TextBlock textBlock)
        {
            DpiScale dpiInfo = VisualTreeHelper.GetDpi(textBlock);

            //new FormattedText(textBlock.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch), textBlock.FontSize, textBlock.Foreground);

            FormattedText formattedText = new FormattedText(textBlock.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch), textBlock.FontSize, textBlock.Foreground, dpiInfo.PixelsPerDip);

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
