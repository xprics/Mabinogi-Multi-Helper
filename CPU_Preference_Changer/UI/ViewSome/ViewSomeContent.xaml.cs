using CPU_Preference_Changer.Core.WinFormTransPanel;
using CPU_Preference_Changer.Core.WInStyleHelp.WIndowSytle;
using CPU_Preference_Changer.UI.ViewSome.TabSubUI;
using CPU_Preference_Changer.WinAPI_Wrapper;
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
                Minimum = 15, /* 윈도우 까지 투명해지니까 너무 투명하면 안됨! */
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

            wp.Children.Add(cbAlwayTop);
            wp.Children.Add(opacitySlider);
            titleArea.Children.Add(wp);

            /*현재 값에 맞게 세팅*/
            changeBackground(curTransVal);

            lastTransparentVal = curTransVal;

            /*콘텐츠 출력영역 사이즈 0으로 강제로 내려버리는 코드. 테스트용.
             * CustModernWin.setContentPresenterZero(this);*/
        }

        private void testFunc()
        {
            Process[] lst = Process.GetProcesses();
            foreach(var cur in lst) {
                if( cur.MainWindowHandle!= IntPtr.Zero && cur.ProcessName.Equals("chrome")) {
                    IntPtr hwnd = new WindowInteropHelper(this).Handle;
                    WinAPI.SetParent(cur.MainWindowHandle, hwnd);
                    break;
                }
            }
        }

        /// <summary>
        /// 프로세스 선택 버튼,, (테스트용 코드)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProcessSel_Click(object sender, RoutedEventArgs e)
        {
            ProcessSelectWindow w = new ProcessSelectWindow();
            w.Owner = this;
            w.ShowDialog();
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

        private byte lastTransparentVal;

        private void changeBackground(byte transparentVal)
        {
            /*브러쉬 알파채널 값를 통한 투명도 조절기법을 사용한다!*/
            var brush = new SolidColorBrush(Color.FromArgb(transparentVal, 255, 255, 255));
            //grid_win.Background = brush;
            this.Background = brush;
            tabCtl.Background = new SolidColorBrush(Color.FromArgb(transparentVal, 255, 255, 255));

            if (tabCtl.SelectedIndex < 0 || tabCtl.SelectedIndex >= tabCtl.Items.Count)
                return;
            TabItem curTabUI = tabCtl.Items[tabCtl.SelectedIndex] as TabItem;
            if (curTabUI == null) return;
            /*서브 UI투명도 조절 */
            ITransCanChange iTC = curTabUI.Content as ITransCanChange;
            if(iTC != null) {
                iTC.setBackgroundTrans(transparentVal, 255, 255, 255);
            }
            lastTransparentVal = transparentVal;
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

        internal struct MenuCreateInfo
        {
            public string header;
            public RoutedEventHandler evtHandler;
        }
        private ContextMenu createMakeTabContextMenu()
        {
            ContextMenu m = new ContextMenu();
            MenuCreateInfo[] menuInfoArr = new MenuCreateInfo[]
            {
                new MenuCreateInfo(){ header="메모장", evtHandler= btMemoTabAddClick}
                ,new MenuCreateInfo(){ header="사진 뷰어", evtHandler= btPictureTabAddClick}
                ,new MenuCreateInfo(){ header="크롬 계산기", evtHandler= btCalcTabAddClick}
            };

            /*목록에 선언된거 몽땅 등록*/
            foreach(var cur in menuInfoArr) {
                MenuItem mi = new MenuItem();
                mi.Header = cur.header;
                mi.Click += cur.evtHandler;
                m.Items.Add(mi);
            }
            return m;
        }

        /// <summary>
        /// 메모 탭 추가하기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btMemoTabAddClick(object sender, RoutedEventArgs e)
        {
            pushUiToTab("메모", new Tab_Memo());
        }

        /// <summary>
        /// 그림 탭 추가하기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPictureTabAddClick(object sender, RoutedEventArgs e)
        {
            pushUiToTab("사진", new Tab_PictureView());
        }

        /// <summary>
        /// 계산기 탭 추가하기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCalcTabAddClick(object sender, RoutedEventArgs e)
        {
            pushUiToTab("크롬계산기", new ChromeBathCalculator());
        }

        /// <summary>
        /// 탭에 서브ui 넣기..
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="ui"></param>
        private void pushUiToTab(string headerName, UIElement ui)
        {
            TabItem itm = new TabItem();

            itm.Header = headerName;
            itm.Content = ui;
            (ui as ITransCanChange).setBackgroundTrans(lastTransparentVal, 255, 255, 255);

            tabCtl.Items.Add(itm);
            int nCnt = tabCtl.Items.Count;
            /*텝 인덱스를 태그에 저장*/
            itm.Tag = nCnt - 1;
            tabCtl.SelectedIndex = nCnt - 1;
        }

        /// <summary>
        /// 탭 추가 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btTabAdd_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;
            if(b.ContextMenu == null) {
                b.ContextMenu = createMakeTabContextMenu();
            }
            b.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// 윈도우 닫길 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (tabCtl.Items.Count == 0)
                return;
            foreach(var x in tabCtl.Items) {
                TabItem ti = x as TabItem;
                if (ti == null) continue;
                /**/
                IDestroyControl ctlDestroy = ti.Content as IDestroyControl;
                if( ctlDestroy !=null) {
                    /*컨트롤이 완전히 파괴됨을 알림*/
                    ctlDestroy.onDestroy();
                }
            }
        }

        /// <summary>
        /// 서브UI Destroy시키기위해 IDX번째 아이템의 Destroy 인터페이스 얻기 (없다면 널)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private IDestroyControl getDestroyNotiCtlFromTabCtl(int idx)
        {
            if (idx < 0 || idx >= tabCtl.Items.Count)
                return null;
            TabItem ti = tabCtl.Items[idx] as TabItem;
            if(ti != null) {
                return ti.Content as IDestroyControl;
            }
            return null;
        }

        /// <summary>
        /// 탭 닫기 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseTab_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;

            int idx ;
            if (int.TryParse(b.Tag.ToString(), out idx) == false)
                return;
            if (idx < 0 || idx >= tabCtl.Items.Count) return;
            /*i번째 아이템이 파괴된다!*/
            var iDestroyNoti = getDestroyNotiCtlFromTabCtl(idx);
            iDestroyNoti?.onDestroy();
            tabCtl.Items.RemoveAt(idx);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}

