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

        /// <summary>
        /// 타이틀바에 적절히 컨트롤 등록하기위해 로딩되고나서 실행되게 함..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DockPanel titleArea = CustModernWin.getTitleDockPanel(this);
            if (titleArea == null) return;

            DockPanel wp = new DockPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            byte curTransVal = 100;
            Slider opacitySlider = new Slider(){
                Margin = new Thickness(0, 0, 10, 0),
                ToolTip = "투명도 조절",
                Maximum = 255,
                Minimum = 76, /* 윈도우 까지 투명해지니까 너무 투명하면 안됨! 약 30%투명도,,*/
                TickFrequency = 1,
                IsSnapToTickEnabled = true,
                Width=100,
                Value= curTransVal
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

            /*현재 값에 맞게 세팅*/
            changeBackground(curTransVal);

            /*콘텐츠 출력영역 사이즈 0으로 강제로 내려버리는 코드. 테스트용.
             * CustModernWin.setContentPresenterZero(this);*/
        }

        /// <summary>
        /// 프로세스 선택 버튼,,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProcessSel_Click(object sender, RoutedEventArgs e)
        {

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

        private void changeBackground(byte transparentVal)
        {
            /*브러쉬 알파채널 값를 통한 투명도 조절기법을 사용한다!*/
            var brush = new SolidColorBrush(Color.FromArgb(transparentVal, 255, 255, 255));
            //grid_win.Background = brush;
            this.Background = brush;

            /*
             * grid의 모든 자손을 순회하면서 해도되긴한데 그냥 컨트롤이 몇 없기도하고,,
             * 투명하면 잘안보이기도 하니까 그냥 냅둠
             */
        }

        /// <summary>
        /// 투명도 슬라이더 움직일 때,,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slider_valueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte b = (byte)e.NewValue;
            changeBackground(b);
        }
    }
}
