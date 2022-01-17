using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CPU_Preference_Changer.Core.WInStyleHelp.WIndowSytle
{
    partial class CustModernWin : ResourceDictionary
    {
        private void winCLoseBtn_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null) {
                var v = Window.GetWindow(b);
                v?.Close();
            }
        }

        private void winMinMaxBtn_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null) {
                var v = Window.GetWindow(b);
                if (v == null) return;
                if(v.WindowState== WindowState.Maximized) {
                    v.WindowState = WindowState.Normal;
                } else {
                    v.WindowState = WindowState.Maximized;
                }
            }
        }

        private void winMinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null) {
                var v = Window.GetWindow(b);
                if (v == null) return;
                v.WindowState = WindowState.Minimized;
            }
        }

        private void grid_mouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) {
                TextBlock ctl = sender as TextBlock;
                if (ctl != null) {
                    var v = Window.GetWindow(ctl);
                    v?.DragMove();
                }
            }
        }

        /// <summary>
        /// 타이틀 패널 찾아 반환..
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static DockPanel getTitleDockPanel(Window w)
        {
            if (w == null) return null;
            return w.Template.FindName("winTitleDockArea",w) as DockPanel;
        }

        public static void setContentPresenterZero(Window w)
        {
            if (w == null) return ;
            var p = w.Template.FindName("wContentPresenter", w) as ContentPresenter;
            p.Width = 0;
            p.Height = 0;
        }
    }
}
