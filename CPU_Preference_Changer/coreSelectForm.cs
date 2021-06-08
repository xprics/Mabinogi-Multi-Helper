using System;
using System.Windows.Forms;

namespace CPU_Preference_Changer {
    public partial class coreSelectForm : Form {
        private IntPtr selCoreState = IntPtr.Zero;

        public coreSelectForm(int maxCoreCnt, ulong curState)
        {
            InitializeComponent();
            initChkListBox(maxCoreCnt, curState);
            cbCheckLB.CheckOnClick = true;
            bInitState = false;
        }

        /// <summary>
        /// 체크박스 초기화...
        /// </summary>
        /// <param name="maxCoreCnt"></param>
        /// <param name="curState">현재 상태 값..</param>
        private void initChkListBox(int maxCoreCnt, ulong curState)
        {
            if( curState == (ulong)MabiProcess.GetMaxAffinityVal()) {
                cbCheckLB.Items.Add("전체 사용", true);
                for( int i=0; i < maxCoreCnt; ++i) {
                    cbCheckLB.Items.Add(string.Format("Core [{0}]",i), true);
                }
            } else {
                cbCheckLB.Items.Add("전체 사용", false);
                /*msdn Process.Affinity속성 설명에 따르면 최하위 비트가 0번 코어다..*/
                for (int i = 0; i < maxCoreCnt; ++i) {
                    bool bSel = (curState & 0x01)==0x01 ? true : false;
                    cbCheckLB.Items.Add(string.Format("Core [{0}]", i), bSel);
                    curState >>= 1;
                }
            }
        }

        /// <summary>
        /// 결과 반환
        /// </summary>
        /// <returns></returns>
        public IntPtr GetDlgResultValue()
        {
            return selCoreState;
        }

        /// <summary>
        /// OK버튼 눌렀을 때..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            /*선택 된 아이템에 맞게 비트 배치하여 적절히 보관..*/
            if (cbCheckLB.GetItemChecked(0)) {
                /*전체 선택인 경우 그냥 바로 MaxValue세팅*/
                selCoreState = MabiProcess.GetMaxAffinityVal();
            } else {
                ulong v = 0x01, ret=0;
                for (int i=1; i<cbCheckLB.Items.Count; ++i) {
                    if(cbCheckLB.GetItemChecked(i)) {
                        /*체크 된 경우이므로 CPU사용 Flag를 저장해둔다..*/
                        ret |= v;
                    }
                    v <<= 1;
                }
                //시스템 비트 수에 맞게 적절히 변환하여 보관한다.
                selCoreState = MabiProcess.ConvToSystemBit(ret);
            }
        }

        /// <summary>
        /// 이 변수 없다면 이벤트가 계속 호출되어 콜 스택 오버플로 발생
        /// </summary>
        private bool bProgramCheck = false;

        /// <summary>
        /// 다이얼로그 초기화 과정중인가?
        /// 초기화 중 ItemCheck이벤트 들어가서 이상하게 선택 되는 것 방지..
        /// </summary>
        private bool bInitState = true;

        /// <summary>
        /// 체크박스 체크 이벤트 발생 시 적절한 작업 수행한다...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCheckLB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (bProgramCheck) return;

            if (e.NewValue == CheckState.Unchecked) {
                if( cbCheckLB.CheckedItems.Count == 1)
                    btOK.Enabled = false;
            } else {
                btOK.Enabled = true;
            }

            /*전체 선택체크박스 연동...*/
            if (e.Index == 0) {
                bProgramCheck = true;
                bool state = e.NewValue == CheckState.Checked ? true : false;
                for (int i = 1; i < cbCheckLB.Items.Count; ++i) {
                    if(!bInitState) cbCheckLB.SetItemChecked(i, state);
                }
                bProgramCheck = false;
                if (state == false) btOK.Enabled = false;
            } else {
                if (e.NewValue == CheckState.Unchecked && cbCheckLB.GetItemChecked(0)) {
                    /*전체선택 -> 아무거나 하나 해제 하여 전체가 아니게 된 경우임..*/
                    bProgramCheck = true;
                    if (!bInitState) cbCheckLB.SetItemChecked(0, false);
                    bProgramCheck = false;
                } else if(e.NewValue==CheckState.Checked) {
                    /* 0~N번 코어가 이제 체크 된 경우!! */
                    bool allCheck = true;
                    for (int i = 1; i < cbCheckLB.Items.Count; ++i) {
                        if (i == e.Index) continue; // 나 자신은 이제 체크 중이므로 무시
                        if (cbCheckLB.GetItemChecked(i) == false) {
                            allCheck = false; 
                            break;
                        }
                    }
                    if( allCheck ) {
                        /*모두 체크되었다면 "전체선택"도 자동으로 체크하여 자연스럽게한다..*/
                        bProgramCheck = true;
                        if (!bInitState) cbCheckLB.SetItemChecked(0, true);
                        bProgramCheck = false;
                    }
                }
            }
        }

        /// <summary>
        /// 취소 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
