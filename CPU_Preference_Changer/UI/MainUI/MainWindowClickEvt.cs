using CPU_Preference_Changer.BackgroundTask;
using CPU_Preference_Changer.Core;
using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using CPU_Preference_Changer.Core.SingleTonTemplate;
using CPU_Preference_Changer.MabiProcessListView;
using CPU_Preference_Changer.UI.InfoForm;
using CPU_Preference_Changer.UI.OptionForm;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// by LT인척하는엘프 2021.06.11 ; 
/// 메인 화면의 클릭 이벤트만 모아서 분리하여 작성 함..
/// </summary>
namespace CPU_Preference_Changer.UI.MainUI {
    public partial class MainWindow : Window {
        /// <summary>
        /// 자동 할당 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAutoSet_Click(object sender, RoutedEventArgs e)
        {
            dbgLogWriteStr("btAutoSet_Click", "btAutoSet Clicked.");
            if (lvMabiProcess.isMainCharSel() == false) {
                showMessage("어떤 클라이언트가 본캐인지 선택하세요!\n"
                           +"프로세스 명을 클릭하면 마비노기 화면으로 전환됩니다!");
                return;
            }

            lvMabiProcess.LvMabi_WaitSingleObject();
            /*------------------------------------------------------------------*/
            /*본캐 체크된 것만... 최대한 코어 사용하게 하고 나머지는 하나로 몰아 넣는다!*/
            var lst = lvMabiProcess.getLvItems();
            /*------------------------------------------------------------------*/
            if (lst == null) return;
            /*------------------------------------------------------------------*/
            dbgLogWriteStr("btAutoSet_Click", string.Format("{0} ; Target Count->{1}",
                                                            "Enum Start... ",
                                                            lst.Count));
            IntPtr val = MabiProcess.GetMaxAffinityVal();
            lst.enumAllData(curData =>
            {
                if (curData.bMainCharacter) {
                    ulong max, min;
                    max = (ulong)MabiProcess.GetMaxAffinityVal();
                    min = (ulong)MabiProcess.GetMinAffinityVal();
                    /* 부캐가 점유하고있는 CPU를 제외하고 활성화 함*/
                    val = MabiProcess.ConvToSystemBit(max & (~min));
                } else {
                    val = MabiProcess.GetMinAffinityVal();
                }
                MabiProcess.setTargetCoreState(((LvRowParam)curData.userParam).PID, val);
                curData.coreState = val + "";
            });
            lvMabiProcess.LvMabi_ReleaseMutex();
            /*------------------------------------------------------------------*/
            showMessage("설정 완료");
        }

        /// <summary>
        /// 새로고침 버튼을 클릭했을 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            dbgLogWriteStr("btReset_Click", "btReset Clicked.");
            /*------------------------------------------------------------------*/
            lvMabiProcess.LvMabi_WaitSingleObject();
            /*------------------------------------------------------------------*/
            /*모든 프로세스에 대하여 초기화 수행!*/
            var lst = lvMabiProcess.getLvItems();
            /*------------------------------------------------------------------*/
            if (lst == null) return;
            /*------------------------------------------------------------------*/
            dbgLogWriteStr("btReset_Click", string.Format("{0} ; Target Count->{1}"
                                                          ,"Enum Start... "
                                                          ,lst.Count));
            IntPtr resetVal = MabiProcess.GetMaxAffinityVal();
            lst.enumAllData(curData =>
            {
                MabiProcess.setTargetCoreState(((LvRowParam)curData.userParam).PID, resetVal);
                curData.coreState = resetVal + "";
            });
            lvMabiProcess.LvMabi_ReleaseMutex();
            /*------------------------------------------------------------------*/
            showMessage("설정 완료");
        }

        /// <summary>
        /// 리스트 뷰에서 프로세스 명을 클릭 했을 때...
        /// </summary>
        /// <param name="rowData">클릭된 리스트뷰 Row 정보</param>
        private void MabiLv_OnProcessNameClicked(LV_MabiProcessRowData rowData)
        {
            /*이 프로세스를 가장 앞으로 옮긴다!*/
            if (MabiProcess.SetActivityWindow(((LvRowParam)rowData.userParam).PID))
                rowData.isHide = false;

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
            if (System.Windows.Forms.DialogResult.OK == selForm.ShowDialog()) {
                IntPtr newVal = selForm.GetDlgResultValue();
                MabiProcess.setTargetCoreState(((LvRowParam)rowData.userParam).PID, newVal);
                rowData.coreState = newVal + "";
                showMessage("설정 완료");
            }
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
        /// <summary>
        /// 프로그램 종료 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btExit_Click(object sender, RoutedEventArgs e)
        {
            CloseApplication();
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

        /// <summary>
        /// 시스템 종료 작업 핸들
        /// </summary>
        private HBFT hShutdownTask = null;

        /// <summary>
        /// 시스템 예약 종료 메뉴 클릭했을 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menu_SystemPowerOff_Click(object sender, RoutedEventArgs e)
        {
            /*TODO : 1)시스템 예약 종료시간을 입력받고, 예약종료 걸어두기.
                     2)만약 이미 예약종료를 걸어두었다면?? 취소할 수 있게 취소 걸어두기 */
            MMHGlobal gInstance = MMHGlobalInstance<MMHGlobal>.GetInstance();
            BackgroundFreqTaskMgmt taskMgr = gInstance.backgroundFreqTaskManager;

            /*이미 예약되어 있다면 취소한다. */
            if (menu_SystemPowerOff.IsChecked) {
                taskMgr.removeFreqTask(hShutdownTask);
                Interlocked.Decrement(ref gInstance.reservedTaskCount);
                hShutdownTask = null;
                menu_SystemPowerOff.IsChecked = false;
                return;
            }

            TimeSelectForm tsf = new TimeSelectForm("시스템 종료 시간을 선택하세요?");
            tsf.ShowDialog();
            if (tsf.DialogResult == System.Windows.Forms.DialogResult.OK) {
                /* shutdown -f -t 1000같은 명령어를 사용하면 편하지만..
                 1) 이 프로그램이 종료된 이후 유저가 예약 종료 사실을 잊어버리거나,
                 2) 프로그램을 종료하고 다시 켠 후 예약 종료를 취소하고 싶을 때
                두가지 경우에서 좀 프로그램에서 구현할게 많아진다.
                
                 따라서 아래와 같은 조건에서 시스템을 종료하게 한다.
                1) 프로그램은 계속 실행되고 있어야 한다
                    (예약종료 걸면 종료 불가능하게 하고 강제로 트레이로 보내던가)
                2) Shutdown의 -t옵션을 쓴 예약이 아닌, 프로그램에서 시간을 확인하다가
                   해당시간 5분전이 되면 5분 후 시스템이 종료된다는 창을 보여줘서
                   해당 창에서 5분동안 응답이 없다면 그때가서 실제로 종료하게 한다!*/
                SystemShutdownTask task = new SystemShutdownTask(tsf.selTime);

                /*최초로 종료 예약한 경우 Task매니저에 등록해둔다...*/
                hShutdownTask = taskMgr.addFreqTask(task);

                /*윈도우 Notify 이용하여 알려줌..
                   => 윈도우7은 안되니까 메세지 박스로 간단하게 한다.*/
                showMessage(string.Format("컴퓨터가 [{0}]에 자동 종료 될 예정입니다.", tsf.selTime.ToString("yyyy년 MM월 dd일 HH시 mm분")));

                /* 메뉴 체크 해둔다. */
                menu_SystemPowerOff.IsChecked = true;
                Interlocked.Increment(ref gInstance.reservedTaskCount);
            }
        }

        /// <summary>
        /// 리스트 뷰의 예약종료 체크박스 클릭했을 때..
        /// </summary>
        /// <param name="rowData"></param>
        private void LvMabiProcess_onCbRkClicked(LV_MabiProcessRowData rowData)
        {
            TimeSelectForm tsf = new TimeSelectForm("종료할 시간을 선택하세요??");
            MMHGlobal gInstance = MMHGlobalInstance<MMHGlobal>.GetInstance();
            BackgroundFreqTaskMgmt backTmgr = gInstance.backgroundFreqTaskManager;
            MabiClientKillTask killTask;
            LvRowParam rowParam = (LvRowParam)(rowData.userParam);

            if (rowData.isKillReserved == false) {
                /*예약 종료 취소해야함*/
                gInstance.backgroundFreqTaskManager.removeFreqTask(((LvRowParam)(rowData.userParam)).hReservedKillTask);
                rowParam.hReservedKillTask = null;
                rowData.reservedKillTime = "None";
                Interlocked.Decrement(ref gInstance.reservedTaskCount);
                return;
            }

            /* 클라이언트를 강제 종료시키는 것이기 때문에 
               혹시라도 해당 클라이언트로 게임을 플레이 하던중이라면 
               게임 내용이 저장되지 않아서 문제가 생길 수 있음을 화면에 출력하여 알려준다.*/

            string warnMsg;
            warnMsg = string.Format("{0}\n{1}",
                                "이 기능은 클라이언트를 강제로 종료시킵니다.",
                                "강제 종료로 인해 혹시라도 데이터가 서버에 저장되지 않을 수 있는 점 주의하시길 바랍니다.");
            showMessage(warnMsg);

            tsf.ShowDialog();
            if (tsf.DialogResult == System.Windows.Forms.DialogResult.OK) {
                
                /* 새로운 Task를 생성하고.. */
                killTask = new MabiClientKillTask(rowParam.PID,tsf.selTime);

                /*Task와 연동 될 이벤트 연결*/
                killTask.onClientProcessKilled += MabiClientKillTask_onClientProcessKilled;

                /*Task매니저에 작업 등록*/
                rowParam.hReservedKillTask = backTmgr.addFreqTask(killTask);

                /*리스트뷰에 예약된 시간을 출력한다.*/
                rowData.reservedKillTime = tsf.selTime.ToString();

                /*예약된 작업 수량 증가.*/
                Interlocked.Increment(ref gInstance.reservedTaskCount);
                return;
            }
            /*유저가 취소했으니 체크박스도 다시 해제한다.*/
            rowData.isKillReserved = false;
        }

        /// <summary>
        /// 클라이언트가 강제종료되었을 때 일어나는 이벤트 처리 함수
        /// </summary>
        /// <param name="sender"></param>
        private void MabiClientKillTask_onClientProcessKilled(object sender)
        {
            RefresMabiProcess();
            MMHGlobal gInstance = MMHGlobalInstance<MMHGlobal>.GetInstance();
            Interlocked.Decrement(ref gInstance.reservedTaskCount);
        }

        /// <summary>
        /// 리스트 뷰의 숨기기 버튼 클릭했을 때..,..
        /// </summary>
        /// <param name="rowData"></param>
        private void LvMabiProcess_onCbHideClicked(LV_MabiProcessRowData rowData)
        {
            if (rowData.isHide) {
                /*보여진 것을 숨겨야하는 케이스*/
                MabiProcess.SetHideWindow(((LvRowParam)rowData.userParam).PID);
            } else {
                /*숨겨진 것을 보이게 만드는 케이스*/
                MabiProcess.UnSetHideWindow(((LvRowParam)rowData.userParam).PID);
            }
        }

        ListSortDirection sortDirection = ListSortDirection.Ascending;
        /// <summary>
        /// 리스트 뷰 클릭되었을 때...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvMabiProcess_onLvClicked(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null) {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding) {
                    /*적절히 정렬수행함...*/
                    lvMabiProcess.sortListData(sortDirection);

                    /*다음 클릭 시 반대방향 정렬을 위해 미리 토글해둠*/
                    if (sortDirection == ListSortDirection.Ascending) {
                        sortDirection = ListSortDirection.Descending;
                    } else {
                        sortDirection = ListSortDirection.Ascending;
                    }
                }
            }
        }
    }
}
