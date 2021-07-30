using CPU_Preference_Changer.MabiProcessListView;
using System;
using System.ComponentModel;
using CPU_Preference_Changer.Core;
using System.Windows;
using System.Windows.Controls;
using CPU_Preference_Changer.UI.OptionForm;
using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using CPU_Preference_Changer.Core.SingleTonTemplate;
using CPU_Preference_Changer.BackgroundTask;
using CPU_Preference_Changer.Core.Logger;
using System.Diagnostics;
using System.Threading.Tasks;

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

        /// <summary>
        /// 추적용 로그를 남길것인가...
        /// </summary>
        private bool bDebugRun = false;
        private MMH_Logger logger;

        public MainWindow()
        {
            InitializeComponent();

            //디버그 모드인지 값 받아온다..
            MMHGlobal gInstance = MMHGlobalInstance<MMHGlobal>.GetInstance();
            bDebugRun = gInstance.bDebugModeRun;

            //윈도우 글자, 이벤트 초기화
            initWindow();

            // init trayicon
            initTrayIcon();

            /*크래시 로그를 남기기 위해...*/
            logger = gInstance.dbgLogger = new MMH_Logger("./MMH_Log.txt");
            if (logger == null)
            {
                showMessage("로거 생성에 실패하였습니다..");
            }
            if (bDebugRun)
            {
                if (logger != null)
                {
                    logger.writeLog("ClientFullPath=["
                                    + MabiProcess.mabiRunFilePath
                                    + "]");
                }
#if __WIN__64__DBG__
                /*개발자 디버그 모드에서만 보이도록 한다!*/
                dbgPanel.Visibility = Visibility.Visible;
#endif
            }

            //백그라운드 Task Manager 시작
            BackgroundFreqTaskMgmt backMgmt = gInstance.backgroundFreqTaskManager;
            if (bDebugRun) {
                backMgmt.onLogInfoWrite += BackMgmt_onLogInfoWrite;
            }
            backMgmt.startTaskManager();
            if (null == backMgmt.addFreqTask(new ProcessListRefreshTask(this)))
            {
                if (logger != null)
                {
                    logger.writeLog("ProcessListRefreshTask Add Fail..");
                }
            }
        }

        /// <summary>
        /// BackgroundWorker로그를 기록해야할 때.,,,
        /// </summary>
        /// <param name="state"></param>
        private void BackMgmt_onLogInfoWrite(string state)
        {
            dbgLogWriteStr("BackMgmt_onLogInfoWrite", state);
        }

        /// <summary>
        /// 윈도우 초기화
        /// </summary>
        private void initWindow()
        {
            /*리스트 뷰 이벤트 클릭 리스너 등록...*/
            lvMabiProcess.onProcessNameClick += MabiLv_OnProcessNameClicked;
            lvMabiProcess.onCoreStateClick += MabiLv_OnCoreClicked;
            lvMabiProcess.onCbHideClicked += LvMabiProcess_onCbHideClicked;
            lvMabiProcess.onCbRkClicked += LvMabiProcess_onCbRkClicked;
            lvMabiProcess.onLvClicked += LvMabiProcess_onLvClicked;

            this.tb_CpuName.Text = SystemInfo.GetCpuName();
            this.tb_CpuCoreCnt.Text = SystemInfo.GetCpuCoreCntStr();
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
            try {
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
            } catch (Exception err) {
                programErrLogWrite(err);
                showMessage("트레이 아이콘 생성 실패 : " + err.Message);
            }
        }

        /// <summary>
        /// X버튼 클릭 시 호출되는 함수 override
        /// </summary>
        /// <param name="e">OnClosing 이벤트 파라미터</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            try {
                if (this.trayIcon != null) {
                    /*바로 종료할건지 트레이로 보낼건지 물어보게 유저에게 물어본다.*/
                    CloseAskForm askForm = new CloseAskForm();
                    askForm.ShowDialog();

                    switch (askForm.getUsrSelect()) {
                        /*그냥 종료하기 누루면 바로 종료..*/
                        case CloseAskFormResult.eClose:
                            if (CloseApplication() == false)
                                e.Cancel = true;
                            return;
                            //break;/* return때문에 필요없지만 프로그래머를 위해 명시적으로 넣어 둠*/

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
            } catch (Exception err) {
                programErrLogWrite(err);
                e.Cancel = false;
            }
            finally {
                // call base event
                base.OnClosing(e);
            }
        }


        /// <summary>
        /// 찐탱 프로그램 종료하는 코드
        /// </summary>
        private bool CloseApplication()
        {
            var gInstance = MMHGlobalInstance<MMHGlobal>.GetInstance();
            if (gInstance.reservedTaskCount > 0) {
                var ret = MessageBox.Show(string.Format("{0}\n{1}",
                                                        "프로그램을 종료하면 예약 종료 작업이 실행되지 않습니다.\n",
                                                        "그래도 종료하시겠습니까?")
                                          ,"안내"
                                          ,MessageBoxButton.YesNo
                                          ,MessageBoxImage.Warning);
                if (ret == MessageBoxResult.No) 
                    return false;
            }
            // dispose tray icon
            if (this.trayIcon != null) {
                this.trayIcon.Visible = false;
                this.trayIcon.Dispose();
            }
            this.trayIcon = null;

            MMHGlobalInstance<MMHGlobal>.GetInstance().Release();

            // real shutdown this process
            Application.Current.Shutdown();
            return true;
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
        /// 리스트뷰에서 데이터가 지워지기 전, 초기화해야 할 내용이 있다면 초기화 하는 함수
        /// </summary>
        /// <param name="rmData"></param>
        public void removeReservedInfo(LV_MabiProcessRowData rmData)
        {
            LvRowParam lp;
            try {
                lp = (LvRowParam)rmData.userParam;

                if (lp.hReservedKillTask != null) {
                    /*예약 종료를 걸어놨는데 사람이 수동으로 예약된 시간보다 먼저 종료한 경우 발생.*/
                    MMHGlobal gInstance = MMHGlobalInstance<MMHGlobal>.GetInstance();
                    BackgroundFreqTaskMgmt backTmgr = gInstance.backgroundFreqTaskManager;
                    backTmgr.removeFreqTask(lp.hReservedKillTask);
                }
            } catch (Exception err) {
                programErrLogWrite(err);
            }
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
            try {
                LvMabiDataCollection lvItm = (LvMabiDataCollection)usrParam;

                var newData = new LV_MabiProcessRowData(pName,
                                                   PID + "",
                                                   startTime,
                                                   coreState + "",
                                                   runPath);
                LvRowParam param = new LvRowParam();
                param.PID = PID; param.hReservedKillTask = null;
                newData.userParam = param; //찾았던 프로세스 정보 보관해서 나중에 써먹기위함
                newData.isHide = isHide;
                lvItm.Add(newData);
            }catch(Exception err) {
                programErrLogWrite(err);
            }
        }

        /// <summary>
        /// 마비노기 프로세스 찾기 작업 하기 전 호출되는 콜백함수
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="usrParam"></param>
        private void CB_PreFindMabiProcess(Process[] lst, ref object usrParam)
        {
            if (bDebugRun) {
                /*디버그 모드라면 현재 발견된 목록 수와 프로세스들의 FullPath를 로그에 찍어준다.*/
                dbgLogWriteStr("CB_PreFindMabiProcess",
                               "Target Count=["
                               + (lst == null ? 0 : lst.Length)
                               + "]");
                int i=0;
                foreach (Process p in lst) {
                    dbgLogWriteStr("CB_PreFindMabiProcess",
                                   string.Format("[{0}] FullPath={1}",
                                                  i+1,MabiProcess.getProcessFullPath(p)));
                }
            }
        }

        /// <summary>
        /// 프로세스 목록 다시 가져오기
        /// </summary>
        public void RefresMabiProcess()
        {
            try {
                dbgLogWriteStr("RefreshMabiProcess", "Wait Lock");
                // lock
                lock (this.lockObj) {
                    dbgLogWriteStr("RefreshMabiProcess", "UI_DispatchEvt Start");
                    // work thread invoke UI thread
                    UI_DispatchEvt(new Action(delegate {
                        /*계속 할당받지말고,... 기존에 쓰던것을 활용하게 하다!*/
                        LvMabiDataCollection newList = new LvMabiDataCollection();

                        dbgLogWriteStr("RefreshMabiProcess", "Find Mabinogi Process");
                        object param = newList; /*함수인자에서 바로 object로 캐스팅하면 에러 발생한다.*/
                        MabiProcess.getAllTargets(CB_FindMabiProcess, ref param, CB_PreFindMabiProcess);

                        /*by LT인척하는엘프 2021.06.13
                         귀찮아서 무조건 Set하던것을.. 기존 아이템과 비교하여
                         목록을 적절히 갱신시키게 함!*/
                        /*독립된 스레드이므로 버튼 클릭과 겹치게 되면 곤란하다
                          예: 버튼 클릭 이벤트에서 리스트를 참조중인데 
                              하필 그 순간 프로세스 종료가 감지되어서 여기서 바로 삭제되면...?
                              버튼 클릭이벤트에서 제대로 작동이 되지 않을 수 있다.
                              확실하게 데이터에 Lock을 걸어두고 참조하게 한다.*/
                        dbgLogWriteStr("RefreshMabiProcess", "Wait for Lv Object");
                        lvMabiProcess.LvMabi_WaitSingleObject();
                        var curList = lvMabiProcess.getLvItems();
                        if (curList == null) {
                            dbgLogWriteStr("RefreshMabiProcess", "Lv Data set.");
                            lvMabiProcess.setDataSoure(newList);
                        } else {
                            dbgLogWriteStr("RefreshMabiProcess", "Lv Data Update.");
                            curList.updateDataCollection(newList, removeReservedInfo);
                        }
                        lvMabiProcess.LvMabi_ReleaseMutex();
                        dbgLogWriteStr("RefreshMabiProcess", "Release Lv Object");
                    }));
                    dbgLogWriteStr("RefreshMabiProcess", "UI_DispatchEvt End");
                }
            } catch (Exception err) {
                programErrLogWrite(err);
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

        /// <summary>
        /// 윈도우 창이 로딩되었을 때...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProgramVersionCheck();
        }

        /// <summary>
        /// 프로그램 버전 확인..
        /// </summary>
        private async void ProgramVersionCheck()
        {
            var task= Task.Run(() => ProgramVersionCheckSubProc());

            int ret = await task;
            if (ret == -1) {
                showMessage("새 버전이 있습니다!!\n[프로그램→정보]메뉴를 이용하여 새 버전을 받을 수 있습니다.");
            } else if (ret == 1) {
                showMessage("버전 확인 중 오류가 발생했습니다...");
            }
        }

        /// <summary>
        /// 프로그램 버전 검사 (비동기 처리를 위해 함수 분리)
        /// </summary>
        /// <returns>-1 새 버전 있음
        ///           0 최신버전임
        ///           1 알아보던 중 오류 발생..</returns>
        private int ProgramVersionCheckSubProc()
        {
            try {
                if (ProgramVersionChecker.isNewVersionExist()) {
                    return -1;
                }
            } catch (Exception err) {
                programErrLogWrite(err);
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 디버그 모드일 경우 Log파일로 메세지 출력
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="str"></param>
        private void dbgLogWriteStr(string funcName, string str)
        {
            if (bDebugRun && (logger!=null)) {
                logger.writeLog(string.Format("[{0}] {1}", funcName,str));
            }
        }

        /// <summary>
        /// 예외 상황 발생 시 로그 남기기..
        /// </summary>
        /// <param name="err"></param>
        private void programErrLogWrite(Exception err)
        {
            if (logger != null) {
                logger.writeLog(err);
                showMessage(string.Format("[프로그램 실행 중 오류 발생]\n{0}\n{1}",err.Message,err.StackTrace));
            }
        }

        /// <summary>
        /// 새로고침 클릭...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefresMabiProcess();
        }
    }
}
