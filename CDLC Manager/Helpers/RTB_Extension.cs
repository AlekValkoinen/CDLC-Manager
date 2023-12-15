using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CDLC_Manager.Helpers
{
    public static class RTB_Extension
    {

        //This extention is ripped from Nathan Baulch on Stack Overflow, Big shoutout for the RTB color extension.
        public static void AppendText(this RichTextBox richTextBox, string text, Brush color)
        {
            var paragraph = new Paragraph(new Run(text) { Foreground = color });
            richTextBox.Document.Blocks.Add(paragraph);
        }
    }
}
