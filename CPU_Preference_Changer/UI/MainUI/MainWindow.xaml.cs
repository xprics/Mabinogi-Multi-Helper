using CPU_Preference_Changer.MabiProcessListView;
using System;
using System.ComponentModel;
using CPU_Preference_Changer.Core;
using System.Windows;
using System.Windows.Controls;
using CPU_Preference_Changer.UI.OptionForm;
using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using CPU_Preference_Changer.Core.SingleTonTemplate;

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
        private object lockObj = new object();

        public MainWindow()
        {
            InitializeComponent();

            //윈도우 글자, 이벤트 초기화
            initWindow();

            // init trayicon
            initTrayIcon();

            /*
             * init timer (TimerMainWindow.cs)
            initTimer();
            2021.06.11 by LT인척하는엘프;
                ==> 타이머를 이용하지 않고 백그라운드 Task 매니저를 이용하게 함
            */

            /*
            ProcessKillRunner.Instance.Start(); //start process killer
            2021.06.11 by LT인척하는엘프; 
                ==>백그라운드 Task 매니저를 구현하여 Task매니저가 Task단위로 관리하게 함
            */

            //백그라운드 Task Manager 시작
            BackgroundFreqTaskMgmt backMgmt = MMHGlobalInstance<BackgroundFreqTaskMgmt>.GetInstance();
            backMgmt.startTaskManager();
            backMgmt.addFreqTask(new ProcessListRefreshTask(this));

            // ignore maximize title button
            this.ResizeMode = ResizeMode.CanMinimize;

#if __WIN__64__DBG__
            /*디버그 모드에서만 보이도록 한다!*/
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
#if __OLD__CODE__
    2021.06.11 by LT인척하는엘프; 백그라운드 Task 매니저를 구현하여 Task매니저거 관리하게 함
            // dispose process killer
            ProcessKillRunner.Instance.Stop();
#else
            MMHGlobalInstance<BackgroundFreqTaskMgmt>.GetInstance().Release();
#endif
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
         void ControlTextUpdateInvoke(object c,string str)
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

        /// <summary>
        /// 프로세스 목록 다시 가져오기
        /// </summary>
        public void RefresMabiProcess()
        {
            // lock
            lock (this.lockObj) {
                // work thread invoke UI thread
                UI_DispatchEvt(new Action(delegate {
                    MabiProcessListView.LvMabiDataCollection lvItm = new MabiProcessListView.LvMabiDataCollection();
                    object param = lvItm; /*함수인자에서 바로 object로 캐스팅하면 에러 발생한다.*/
                    MabiProcess.getAllTargets(CB_FindMabiProcess, ref param);
                    lvMabiProcess.setDataSoure(lvItm);
                }));
            }
        }

        /// <summary>
        /// refresh Time 보여주는 Label의 텍스트 변경하는 함수.
        /// </summary>
        /// <param name="str"></param>
        public void updateRefreshTimeLabelText(string str)
        {
            ControlTextUpdateInvoke(refreshTimeLabel, str);
        }

    }
}
