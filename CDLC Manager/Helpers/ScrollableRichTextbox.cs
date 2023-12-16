using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace CDLC_Manager.Helpers
{
    public class ScrollableRichTextBox : RichTextBox
    {
        //This is no longer needed as the box settings use readonly instead of disabling.
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (!IsEnabled)
            {
                var scrollViewer = GetScrollViewer();
                if (scrollViewer != null)
                {
                    if (e.Delta > 0)
                    {
                        scrollViewer.LineUp();
                    }
                    else
                    {
                        scrollViewer.LineDown();
                    }

                    e.Handled = true;
                }
            }
        }

        private ScrollViewer GetScrollViewer()
        {
            DependencyObject obj = this;
            while (obj != null && obj is not ScrollViewer)
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            return obj as ScrollViewer;
        }
    }
}
