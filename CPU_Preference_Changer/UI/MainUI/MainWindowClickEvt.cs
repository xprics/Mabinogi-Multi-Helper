using CPU_Preference_Changer.Core;
using CPU_Preference_Changer.MabiProcessListView;
using CPU_Preference_Changer.UI.InfoForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

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
            if (lvMabiProcess.isMainCharSel() == false) {
                showMessage("어떤 클라이언트가 본캐인지 선택하세요!\n프로세스 명을 클릭하면 마비노기 화면으로 전환됩니다!");
                return;
            }

            /*본캐 체크된 것만... 최대한 코어 사용하게 하고 나머지는 하나로 몰아 넣는다!*/
            var lst = lvMabiProcess.getLvItems();
            IntPtr val = MabiProcess.GetMaxAffinityVal();
            foreach (var x in lst) {
                if (x.bMainCharacter) {
                    ulong max, min;
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
            foreach (var x in lst) {
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
            if (System.Windows.Forms.DialogResult.OK == selForm.ShowDialog()) {
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
        private void menu_SystemPowerOff_Click(object sender, RoutedEventArgs e)
        {
            /*TODO : 1)시스템 예약 종료시간을 입력받고, 예약종료 걸어두기.
                     2)만약 이미 예약종료를 걸어두었다면?? 취소할 수 있게 취소 걸어두기 */
        }
    }
}
