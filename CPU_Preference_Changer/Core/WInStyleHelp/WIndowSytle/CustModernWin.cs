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
        /// <summary>
        /// 닫기 버튼 눌렀을 때.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void winCLoseBtn_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null) {
                var v = Window.GetWindow(b);
                v?.Close();
            }
        }

        /// <summary>
        /// 최대화 / 이전 복원 버튼 놀렀을 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 최소화 눌렀을 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void winMinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null) {
                var v = Window.GetWindow(b);
                if (v == null) return;
                v.WindowState = WindowState.Minimized;
            }
        }

        /// <summary>
        ///  그리드 영역(타이틀바) 눌러서 이동할 때.. 더블클릭도 구현할까...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// contentPresenter영역 사이즈를 강제로 0으로 만듦.
        /// </summary>
        /// <param name="w"></param>
        public static void setContentPresenterZero(Window w)
        {
            if (w == null || w.Template==null ) return ;
            var p = w.Template.FindName("wContentPresenter", w) as ContentPresenter;
            if (p != null) {
                p.Width = 0;
                p.Height = 0;
            }
        }
    }
}
