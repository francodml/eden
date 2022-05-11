using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SkiaSharp;
using ExCSS;

namespace Eden
{
    public class Style
    {
        public Stylesheet? Stylesheet { get; private set; }
        public string? FilePath { get; set; }

        protected static List<Style> ExistingStyles = new();
        public Style(Stylesheet? stylesheet)
        {
            Stylesheet = stylesheet;
        }

        public static Style FromFile(string path)
        {
            if (!File.Exists(path))
            {
                return new Style(null);
            }

            if (ExistingStyles.Any(x => x.FilePath == path))
            {
                return ExistingStyles.First(x => x.FilePath == path);
            }

            var css = File.OpenText(path).ReadToEnd();
            var style = Style.FromString(css);
            style.FilePath = path;

            return style;
        }

        public static Style FromString(string css)
        {
            StylesheetParser parser = new();
            var stylesheet = parser.Parse(css);

            Style style = new Style(stylesheet);
            return style;
        }
    }
}
