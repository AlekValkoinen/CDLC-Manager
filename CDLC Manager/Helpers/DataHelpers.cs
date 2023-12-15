using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CDLC_Manager.Helpers
{
    internal class DataHelpers
    {

        public static List<string> populateDlcFilesList(string dlcfolder, string cdlcFolder)
        {
            List<string> dlcFiles = new List<string>();
            string[] files = Directory.GetFiles(dlcfolder);
            foreach (string f in files)
            {
                dlcFiles.Add(f);
            }
            string[] files2 = Directory.GetFiles(cdlcFolder);
            foreach (string f in files2)
            {
                dlcFiles.Add(f);
            }
            return dlcFiles;
        }

        public static void print(string message, RichTextBox tb)
        {

            tb.AppendText(message + "\n");
            tb.ScrollToEnd();
        }
        public static void print(string message, System.Windows.Media.Brush color, RichTextBox tb)
        {
            tb.AppendText(message + "\n", color);
            tb.ScrollToEnd();
        }

        public static string ConvertDate(DateTime dateTime)
        {
            string dateString;
            string Format = "d"; //this allows the standard US datetime specifier
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US"); //This creates the cultural format to use. This can be expanded to an array later for localization.
            dateString = dateTime.ToString(Format, culture);
            return dateString;
        }
    }
}
