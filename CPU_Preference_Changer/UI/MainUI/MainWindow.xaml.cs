using CPU_Preference_Changer.MabiProcessListView;
using System;
using System.ComponentModel;
using CPU_Preference_Changer.Core;
using System.Windows;
using System.Windows.Controls;
using CPU_Preference_Changer.UI.OptionForm;
using static CPU_Preference_Changer.UI.OptionForm.CloseAskForm;
using CPU_Preference_Changer.UI.InfoForm;

namespace CPU_Preference_Changer.UI.MainUI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 트레이 아이콘 변수
        /// </summary>
        public System.Windows.Forms.NotifyIcon trayIcon = null;

        public MainWindow()
        {
            InitializeComponent();
            initWindow();

            // init trayicon
            initTrayIcon();

            // init timer (TimerMainWindow.cs)
            initTimer();

            // start process killer
            ProcessKillRunner.Instance.Start();

            // ignore maximize title button
            this.ResizeMode = ResizeMode.CanMinimize;

#if __WIN__64__DBG__
            dbgPanel.Visibility = Visibility.Visible;
#endif
        }

        /// <summary>
        /// 콜백함수, 마비노기로 추정되는 프로세스를 찾았을 때 실행
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="PID"></param>
        /// <param name="startTime"></param>
        /// <param name="coreState"></param>
        /// <param name="runPath"></param>
        /// <param name="usrParam"></param>
        private void CB_FindMabiProcess(string pName, int PID, string startTime, IntPtr coreState, string runPath, bool isHide, ref object usrParam)
        {
            LvMabiDataCollection lvItm = (LvMabiDataCollection)usrParam;
            var newData = new LV_MabiProcessRowData(pName,
                                                PID + "",
                                                startTime,
                                                coreState + "",
                                                runPath);
            newData.userParam = PID; //찾았던 프로세스 정보 보관해서 나중에 써먹기위함
            newData.isHide = isHide;
            lvItm.Add(newData);
        }

        /// <summary>
        /// 윈도우 초기화
        /// </summary>
        private void initWindow()
        {
            //lv 클릭 이벤트 등록
            lvMabiProcess.setClickEvt(MabiLv_OnProcessNameClicked,
                                      MabiLv_OnCoreClicked,
                                      MabiLv_OnProcessNameRightClicked);

            this.tb_CpuName.Text = SystemInfo.GetCpuName();
            this.tb_CpuCoreCnt.Text = SystemInfo.GetCpuCoreCntStr();

            /*
             * 별도 타이머에서 작업하게되어 필요없어짐.
            LvMabiDataCollection lvItm = new LvMabiDataCollection();
            object param = lvItm; 함수인자에서 바로 object로 캐스팅하면 에러 발생한다.
            MabiProcess.getAllTargets(CB_FindMabiProcess, ref param);
            lvMabiProcess.setDataSoure(lvItm);
            */
        }

        /// <summary>
        /// 메세지 박스 Show.
        /// </summary>
        /// <param name="msg"></param>
        private void showMessage(string msg)
        {
            MessageBox.Show(this,
                            msg,
                            "안내",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        /// <summary>
        /// 자동 할당 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAutoSet_Click(object sender, RoutedEventArgs e)
        {
            if (lvMabiProcess.isMainCharSel() == false) {
                showMessage("어떤 클라이언트가 본캐인지 선택하세요!\n프로세스 명을 클릭하면 마비노기 화면으로 전환됩니다!");
                return;
            }

            /*본캐 체크된 것만... 최대한 코어 사용하게 하고 나머지는 하나로 몰아 넣는다!*/
            var lst = lvMabiProcess.getLvItems();
            IntPtr val = MabiProcess.GetMaxAffinityVal();
            foreach (var x in lst)
            {
                if( x.bMainCharacter) {
                    ulong max,min;
                    max = (ulong)MabiProcess.GetMaxAffinityVal();
                    min = (ulong)MabiProcess.GetMinAffinityVal();
                    /* 부캐가 점유하고있는 CPU를 제외하고 활성화 함*/
                    val = MabiProcess.ConvToSystemBit(max & (~min));
                } else {
                    val = MabiProcess.GetMinAffinityVal();
                }
                MabiProcess.setTargetCoreState((int)x.userParam, val);
                x.coreState = val + "";
            }
            lvMabiProcess.setDataSoure(lst);
            showMessage("설정 완료");
        }

        /// <summary>
        /// 새로고침 버튼을 클릭했을 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            /*모든 프로세스에 대하여 초기화 수행!*/
            var lst = lvMabiProcess.getLvItems();
            IntPtr resetVal = MabiProcess.GetMaxAffinityVal();
            foreach(var x in lst) {
                MabiProcess.setTargetCoreState((int)x.userParam, resetVal);
                x.coreState = resetVal + "";
            }
            showMessage("설정 완료");
        }

        /// <summary>
        /// 리스트 뷰에서 프로세스 명을 클릭 했을 때...
        /// </summary>
        /// <param name="rowData">클릭된 리스트뷰 Row 정보</param>
        private void MabiLv_OnProcessNameClicked(LV_MabiProcessRowData rowData)
        {
            /*이 프로세스를 가장 앞으로 옮긴다!*/
            if (MabiProcess.SetActivityWindow((int)rowData.userParam))
                rowData.isHide = false;

            //MabiProcess.ShowWindow(p.MainWindowHandle, MabiProcess.WindowState.SW_SHOWNORMAL);
            //MabiProcess.SetForegroundWindow(p.MainWindowHandle);
            
            /*딜레이 없이하면 자기자신(이 프로그램)만 활성화 됨
             * 문제는 PC마다 딜레이 시간이 다를 수 있고,, 알트탭으로 하면 더빨리
             * 작업가능해서 필요없을수있다. 그냥 막음.
            Thread.Sleep(1000);
            MabiProcess.SetForegroundWindow(new WindowInteropHelper(this).Handle);
            */
        }

        /// <summary>
        /// 리스트 뷰에서 코어 할당 정보 클릭했을 때...
        /// </summary>
        /// <param name="rowData">클릭된 리스트뷰 Row 정보</param>
        private void MabiLv_OnCoreClicked(LV_MabiProcessRowData rowData)
        {
            ulong val;

            if (ulong.TryParse(rowData.coreState, out val) == false) return;

            //코어 선택 화면 띄우고...
            coreSelectForm selForm = new coreSelectForm(SystemInfo.GetCpuCoreCnt(), val);

            //확인을 누른 경우 해당 Process에 대하여 유저가 설정한 값으로 설정한다!
            if(System.Windows.Forms.DialogResult.OK == selForm.ShowDialog()) {
                IntPtr newVal = selForm.GetDlgResultValue();
                MabiProcess.setTargetCoreState((int)rowData.userParam, newVal);
                rowData.coreState = newVal + "";
                showMessage("설정 완료");
            }
        }

        /// <summary>
        /// 리스트 뷰에서 프로세스 이름을 오른쪽 클릭 했을 때
        /// </summary>
        /// <param name="rowData"></param>
        private void MabiLv_OnProcessNameRightClicked(LV_MabiProcessRowData rowData)
        {
            // minimize window
            //MabiProcess.SetMinimizeWindow((int)rowData.userParam);

            // hide window
            rowData.isHide = true;
            MabiProcess.SetHideWindow((int)rowData.userParam);
        }

        /// <summary>
        /// 새로고침 눌렀음..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRefresh_Click(object sender, RoutedEventArgs e)
        {
            initWindow();
            showMessage("새로고침 완료");
        }

        // added function

        /// <summary>
        /// 트레이 아이콘
        /// </summary>
        private void initTrayIcon()
        {
            try
            {
                // allocation trayicon
                this.trayIcon = new System.Windows.Forms.NotifyIcon();
                this.trayIcon.Icon = Properties.Resources.TrayIcon;
                this.trayIcon.Visible = false;
                this.trayIcon.Text = "마비노기 CPU 할당";

                // allocation trayicon menu
                System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();
                menu.MenuItems.Clear();

                // set exit button item
                System.Windows.Forms.MenuItem exitItem = new System.Windows.Forms.MenuItem
                {
                    Index = 0,
                    Text = "종료"
                };
                exitItem.Click += (object trayIconExitClickSender, EventArgs clickEvent) =>
                {
                    this.CloseApplication();
                };

                // add item
                menu.MenuItems.Add(exitItem);

                // add context menu
                this.trayIcon.ContextMenu = menu;

                // tray icon double click action
                this.trayIcon.DoubleClick += (object doubleClickSender, EventArgs doubleClickEvent) => {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    this.Visibility = Visibility.Visible;
                    this.trayIcon.Visible = false;
                };
            }
            catch (Exception err)
            {
                showMessage("트레이 아이콘 생성 실패 : " + err.Message);
            }
        }

        /// <summary>
        /// X버튼 클릭 시 호출되는 함수 override
        /// </summary>
        /// <param name="e">OnClosing 이벤트 파라미터</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                if (this.trayIcon != null)
                {
                    /*바로 종료할건지 트레이로 보낼건지 물어보게 유저에게 물어본다.*/
                    CloseAskForm askForm = new CloseAskForm();
                    askForm.ShowDialog();

                    switch (askForm.getUsrSelect()) {
                        /*그냥 종료하기 누루면 바로 종료..*/
                        case CloseAskFormResult.eClose:
                            CloseApplication();
                            return;
                            break;/* return때문에 필요없지만 프로그래머를 위해 명시적으로 넣어 둠*/

                        /*트레이로 보내기 누루면 보냄*/
                        case CloseAskFormResult.eGoTray:
                            // cancel event
                            e.Cancel = true;
                            // hide thie window
                            this.Hide();
                            // activity tray icon 
                            this.trayIcon.Visible = true;
                            // up BalloonTip
                            this.trayIcon.ShowBalloonTip(500, "마비 CPU", "트레이아이콘 활성화", System.Windows.Forms.ToolTipIcon.None);
                            break;
                        /*취소 or 예외 발생 시 아무것도 안함*/
                        case CloseAskFormResult.eCancel: default:
                            e.Cancel = true;
                            break;
                    }
                }
            }
            catch
            {
                e.Cancel = false;
            }
            finally
            {
                // call base event
                base.OnClosing(e);
            }
        }

        /// <summary>
        /// 프로그램 종료 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btExit_Click(object sender, RoutedEventArgs e)
        {
            this.CloseApplication();
        }

        /// <summary>
        /// 찐탱 프로그램 종료하는 코드
        /// </summary>
        private void CloseApplication()
        {
            // dispose tray icon
            if (this.trayIcon != null)
            {
                this.trayIcon.Visible = false;
                this.trayIcon.Dispose();
            }
            this.trayIcon = null;

            // dispose refresh process list
            this.CloseRefreshTimer();

            // dispose process killer
            ProcessKillRunner.Instance.Stop();

            // real shutdown this process
            Application.Current.Shutdown();
        }

        private void UI_DispatchEvt(Delegate method, params object[] obj)
        {
            Dispatcher.Invoke(method, obj);
        }

        /// <summary>
        /// CROSS THREAD 에러를 방지하면서 LABEL 내용 업데이트.
        /// </summary>
        /// <param name="c"></param>
        private void ControlTextUpdateInvoke(object c,string str)
        {
            var type = c.GetType();
            
            UI_DispatchEvt(new Action(delegate {
                if (type == typeof(TextBlock)) {
                    (c as TextBlock).Text = str;
                } else if ( type == typeof(Label)) {
                    (c as Label).Content = str;
                } else {
                    /*일단 아무것도 안함*/
                }
            }));
        }

        private void menu_Close_Click(object sender, RoutedEventArgs e)
        {
            CloseApplication();
        }

        private void menu_Info_Click(object sender, RoutedEventArgs e)
        {
            ProgramInfo pi = new ProgramInfo(this);
            pi.ShowDialog();
        }
    }
}
