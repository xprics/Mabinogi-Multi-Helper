using CPU_Preference_Changer.MabiProcessListView;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace CPU_Preference_Changer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            initWindow();
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
        private void CB_FindMabiProcess(Process p, string pName, string PID, string startTime, string coreState, string runPath,ref object usrParam)
        {
            LvMabiDataCollection lvItm = (LvMabiDataCollection)usrParam;
            var newData = new LV_MabiProcessRowData(pName,
                                                PID,
                                                startTime,
                                                coreState,
                                                runPath);
            newData.userParam = p; //찾았던 프로세스 정보 보관해서 나중에 써먹기위함
            lvItm.Add(newData);
        }

        /// <summary>
        /// 윈도우 초기화
        /// </summary>
        private void initWindow()
        {
            //lv 클릭 이벤트 등록
            lvMabiProcess.setClickEvt(MabiLv_OnProcessNameClicked,
                                      MabiLv_OnCoreClicked);

            this.tb_CpuName.Text = SystemInfo.GetCpuName();
            this.tb_CpuCoreCnt.Text = SystemInfo.GetCpuCoreCntStr();

            LvMabiDataCollection lvItm = new LvMabiDataCollection();
            object param = lvItm; /*함수인자에서 바로 object로 캐스팅하면 에러 발생한다.*/
            MabiProcess.getAllTargets(CB_FindMabiProcess,ref param);
            lvMabiProcess.setDataSoure(lvItm);
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
                MabiProcess.setTargetCoreState((Process)x.userParam, val);
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
                MabiProcess.setTargetCoreState((Process)x.userParam, resetVal);
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
            Process p;
            p = rowData.userParam as Process;
            /*이 프로세스를 가장 앞으로 옮긴다!*/
            MabiProcess.SetForegroundWindow(p.MainWindowHandle);
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
                MabiProcess.setTargetCoreState((Process)rowData.userParam, newVal);
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
    }
}
