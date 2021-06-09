using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CPU_Preference_Changer.MabiProcessListView
{
    public delegate void OnProcessNameClicked(LV_MabiProcessRowData rowData);
    public delegate void OnPCoreStateClicked(LV_MabiProcessRowData rowData);


    /// <summary>
    /// LV_MabiProcess.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LV_MabiProcess : UserControl
    {
        /// <summary>
        /// 리스트 뷰 출력중인 아이템 보관
        /// </summary>
        private LvMabiDataCollection lvItms;

        /// <summary>
        /// 새로고침 시 목록이 새로 만들어지기 때문에 이전 본캐 선택 값(PID) 보관을 위해 필요
        /// </summary>
        private int? iMainCharacterPID = null;

        private OnProcessNameClicked onProcessNameClick;
        private OnPCoreStateClicked onCoreStateClick;

        public LV_MabiProcess()
        {
            InitializeComponent();
            this.iMainCharacterPID = null;
            this.onProcessNameClick = null;
            this.onCoreStateClick = null;
        }

        public void setClickEvt(OnProcessNameClicked evtNameClick,
                                OnPCoreStateClicked evtCoreStatusClick)
        {
            this.onProcessNameClick = evtNameClick;
            this.onCoreStateClick = evtCoreStatusClick;
        }

        /// <summary>
        /// idx번째 아이템 얻기
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private LV_MabiProcessRowData GetLvRowItmData(int idx)
        {
            if (idx < 0 || idx >= lvItms.Count) return null;
            return lvItms[idx];
        }

        /// <summary>
        /// 본캐 플래그 초기화
        /// </summary>
        private void ResetMainCharFlag()
        {
            foreach( LV_MabiProcessRowData  x in lvItms) {
                x.bMainCharacter = false;
            }
        }

        /// <summary>
        /// 본캐 선택된 행 idx얻기
        /// </summary>
        /// <returns></returns>
        public int GetMainCharRowIdx()
        {
            for(int i=0; i<lvItms.Count; ++i) {
                if (lvItms[i].bMainCharacter) return i;
            }
            return -1;
        }

        /// <summary>
        /// 리스트뷰에서 관리중인 RowData 얻기
        /// </summary>
        /// <returns></returns>
        public LvMabiDataCollection getLvItems()
        {
            return lvItms;
        }

        /// <summary>
        /// 본캐를 선택했는지?
        /// </summary>
        /// <returns></returns>
        public bool isMainCharSel()
        {
            /*
            foreach (LV_MabiProcessRowData x in lvItms) {
                if (x.bMainCharacter) return true;
            }
            return false;
            */
            return this.iMainCharacterPID == null ? false : true;
        }

        /// <summary>
        /// 프로세스 재갱신하면서 본캐릭 설정 유지
        /// </summary>
        /// <param name="items">재갱신 프로세스 컬렉션</param>
        private void ComposeMainCharacter(ref LvMabiDataCollection items)
        {
            bool checkMain = false;
            if (this.iMainCharacterPID != null)
            {
                foreach (LV_MabiProcessRowData data in items)
                {
                    if ((int)data.userParam == this.iMainCharacterPID)
                    {
                        data.bMainCharacter = true;
                        checkMain = true;
                    }
                }
            }

            if (checkMain == false)
                this.iMainCharacterPID = null;
        }

        /// <summary>
        /// 리스트뷰 연동 데이터 모두 해제...
        /// </summary>
        private void DisposeAllResourece()
        {
            //참조 제거
            MabiProcessListView.ItemsSource = null;
            MabiProcessListView.Items.Clear();
            lvItms = null;
        }

        /// <summary>
        /// 주어진 컬렉션을 리스트뷰와 연동하기..
        /// </summary>
        /// <param name="items"></param>
        public void setDataSoure(LvMabiDataCollection items)
        {
            if (items == null) return;
            ComposeMainCharacter(ref items);
            DisposeAllResourece();
            lvItms = items;
            MabiProcessListView.ItemsSource = items;
        }


        /// <summary>
        /// 컨트롤의 Tag를 찾아서 int로 변환
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int ctlTagStrToInt(object obj)
        {
            int ret; string tagStr;
            if (obj == null) return -1;

            var type = obj.GetType();
            if (type == typeof(TextBlock)) {
                tagStr = (obj as TextBlock).Tag.ToString();
            } else if (type == typeof(RadioButton)) {
                tagStr = (obj as RadioButton).Tag.ToString();
            } else {
                return -1;
            }
            /**/
            if ( int.TryParse(tagStr, out ret) ) {
                return ret;
            }
            return -1;
        }

        /// <summary>
        /// 라디오 그룹 버튼 눌렀을 때..,,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            int selIdx = ctlTagStrToInt(sender);
            var data = GetLvRowItmData(selIdx);
            if (data == null) return;
            ResetMainCharFlag();
            data.bMainCharacter = true;
            this.iMainCharacterPID = (int)data.userParam;
            MabiProcessListView.SelectedItem = data;
            MabiProcessListView.SelectedIndex = selIdx;
        }

        #region 프로세스명 클릭 이벤트 처리
        private bool bProcessNameClick = false;

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bProcessNameClick = true;
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (bProcessNameClick) {
                /*이벤트 처리 콜백 함수 실행*/
                if (onProcessNameClick != null)
                    onProcessNameClick(GetLvRowItmData(ctlTagStrToInt(sender)));
                bProcessNameClick = false;
            }
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            if (bProcessNameClick) bProcessNameClick = false;
        }
        #endregion

        #region 코어 할당 마우스 클릭이벤트 처리
        /// <summary> 
        /// 마우스 클릭이벤트를 위함...
        /// </summary>
        private bool bCoreStateMouseDown;

        private void tbCoreState_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bCoreStateMouseDown = true;
        }

        private void tbCoreState_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (bCoreStateMouseDown) {
                /*이벤트 처리 콜백 함수 실행*/
                if(onCoreStateClick!=null)
                    onCoreStateClick(GetLvRowItmData(ctlTagStrToInt(sender)));
                bCoreStateMouseDown = false;
            }
        }

        private void tbCoreState_MouseLeave(object sender, MouseEventArgs e)
        {
            if (bCoreStateMouseDown)
                bCoreStateMouseDown = false;
        }
        #endregion

    }
}
