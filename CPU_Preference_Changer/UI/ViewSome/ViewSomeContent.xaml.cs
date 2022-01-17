using CPU_Preference_Changer.Core.WInStyleHelp.WIndowSytle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CPU_Preference_Changer.UI.ViewSome
{
    /// <summary>
    /// ViewSomeContent.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewSomeContent : Window
    {
        public ViewSomeContent()
        {
            InitializeComponent();
        }


        private Process _process;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        private static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;
        const string patran = "patran";

        private void LaunchChildProcess()
        {
            _process = Process.Start("notepad.exe");
            _process.WaitForInputIdle();

            var helper = new WindowInteropHelper(this);

            SetParent(_process.MainWindowHandle, helper.Handle);

            // remove control box
            int style = GetWindowLong(_process.MainWindowHandle, GWL_STYLE);
            style = style & ~WS_CAPTION & ~WS_THICKFRAME;
            SetWindowLong(_process.MainWindowHandle, GWL_STYLE, style);
            // resize embedded application & refresh
            ResizeEmbeddedApp();
        }

        private void ResizeEmbeddedApp()
        {
            if (_process == null)
                return;
            SetWindowPos(_process.MainWindowHandle, IntPtr.Zero, 50, 50, (int)ActualWidth-30, (int)ActualHeight-30,  SWP_NOACTIVATE);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);
            ResizeEmbeddedApp();
            return size;
        }











        /// <summary>
        /// 타이틀바에 적절히 컨트롤 등록하기위해 로딩되고나서 실행되게 함..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //LaunchChildProcess();

            
            DockPanel titleArea = CustModernWin.getTitleDockPanel(this);
            if (titleArea == null) return;

            DockPanel wp = new DockPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Slider opacitySlider = new Slider(){
                Margin = new Thickness(0, 0, 10, 0),
                ToolTip = "투명도 조절",
                Maximum = 255,
                Minimum = 76, /* 윈도우 까지 투명해지니까 너무 투명하면 안됨! 약 30%투명도,,*/
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Width=100,
                Value=255
            };
            opacitySlider.ValueChanged += slider_valueChanged;

            CheckBox cbAlwayTop = new CheckBox()
            {
                Content = "항상 위",
                ToolTip = "이 옵션을 활성화 하면 이 창은 다른창보다 항상 위에 존재합니다."
            };
            cbAlwayTop.Checked += CbAlwayTop_Checked;
            cbAlwayTop.Unchecked += CbAlwayTop_Unchecked;

            Button btnProcessSel = new Button() { Content="프로세스 선택" };
            btnProcessSel.Click += BtnProcessSel_Click;

            wp.Children.Add(cbAlwayTop);
            wp.Children.Add(btnProcessSel);
            wp.Children.Add(opacitySlider);
            titleArea.Children.Add(wp);

            CustModernWin.setContentPresenterZero(this);
            LaunchChildProcess();

        }

        /// <summary>
        /// 프로세스 선택 버튼,,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProcessSel_Click(object sender, RoutedEventArgs e)
        {
            Process p = Process.Start("notepad.exe");
            
        }

        /// <summary>
        /// 항상 위 해제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbAlwayTop_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
        }

        /// <summary>
        /// 항상 위 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbAlwayTop_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
        }

        /// <summary>
        /// 투명도 슬라이더 움직일 때,,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slider_valueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte b = (byte)e.NewValue;

            /*브러쉬 알파채널 값를 통한 투명도 조절기법을 사용한다!*/
            var brush = new SolidColorBrush(Color.FromArgb(b, 255, 255, 255));
            //grid_win.Background = brush;
            this.Background = brush;

            /*
             * grid의 모든 자손을 순회하면서 해도되긴한데 그냥 컨트롤이 몇 없기도하고,,
             * 투명하면 잘안보이기도 하니까 그냥 냅둠
             */
           // formHost.Background = brush;
           // formHost.Opacity = b;
        }
    }
}
