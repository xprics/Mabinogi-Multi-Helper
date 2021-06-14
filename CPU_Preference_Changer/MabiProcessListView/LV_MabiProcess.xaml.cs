using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CPU_Preference_Changer.MabiProcessListView
{
    /// <summary>
    /// LV_MabiProcess.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LV_MabiProcess : UserControl
    {
        /// <summary>
        /// 리스트 뷰 출력중인 아이템 보관
        /// </summary>
        private LvMabiDataCollection lvItms;

        /*귀찮아서 콜백받아서 쓰던것들 이벤트로 만들어 주기 위해 클래스 안으로 옮겼다...*/
        public delegate void OnProcessNameClicked(LV_MabiProcessRowData rowData);
        public delegate void OnPCoreStateClicked(LV_MabiProcessRowData rowData);
        public delegate void OnProcessNameRightCLick(LV_MabiProcessRowData rowData);
        public delegate void OnCbHideClicked(LV_MabiProcessRowData rowData);
        public delegate void OnCbRkClicked(LV_MabiProcessRowData rowData);

        public event OnProcessNameClicked onProcessNameClick;
        public event OnPCoreStateClicked onCoreStateClick;
        public event OnCbRkClicked onCbRkClicked;
        public event OnCbHideClicked onCbHideClicked;

        // process name right click
        public event OnProcessNameRightCLick onProcessNameRightClick;

        public LV_MabiProcess()
        {
            InitializeComponent();
            this.onProcessNameClick = null;
            this.onCoreStateClick = null;
        }

        /// <summary>
        /// idx번째 아이템 얻기
        /// </summary>
        /// <param name="tagIdx"></param>
        /// <returns></returns>
        private LV_MabiProcessRowData GetLvRowItmData(int tagIdx)
        {
            return lvItms.findItmFromTagIndex(tagIdx);
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
            foreach (LV_MabiProcessRowData x in lvItms) {
                if (x.bMainCharacter) return true;
            }
            return false;
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
            } else if (type == typeof(CheckBox)) {
                tagStr = (obj as CheckBox).Tag.ToString();
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
            int tagIdx = ctlTagStrToInt(sender);
            var data = GetLvRowItmData(tagIdx);
            if (data == null) return;
            ResetMainCharFlag();
            data.bMainCharacter = true;
            
            int rowIdx = lvItms.findItmIdxFromTagIndex(tagIdx);
            MabiProcessListView.SelectedItem = data;
            MabiProcessListView.SelectedIndex = rowIdx;
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
            if (bProcessNameRightClick) bProcessNameRightClick = false;
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

        #region 프로세스명 오른쪽 클릭 이벤트 처리
        private bool bProcessNameRightClick = false;

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            bProcessNameRightClick = true;
        }

        private void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (bProcessNameRightClick)
            {
                /*이벤트 처리 콜백 함수 실행*/
                if (this.onProcessNameRightClick != null)
                    this.onProcessNameRightClick(GetLvRowItmData(ctlTagStrToInt(sender)));
                bProcessNameRightClick = false;
            }
        }
        #endregion

        #region 숨김 클릭 이벤트 처리
        private bool bCbHideClick = false;

        private void cbHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bCbHideClick = true;
        }

        private void cbHide_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (bCbHideClick) {
                /*이벤트 처리 콜백 함수 실행*/
                if (onCbHideClicked != null)
                    onCbHideClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
                bCbHideClick = false;
            }
        }

        private void cbHide_MouseLeave(object sender, MouseEventArgs e)
        {
            if (bCbHideClick) bCbHideClick = false;
        }
        #endregion

        #region 예약종료 체크박스 클릭 이벤트 처리
        private bool bCbRkClick = false;

        private void cbRK_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bCbRkClick = true;
        }

        private void cbRK_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (bCbRkClick) {
                /*이벤트 처리 콜백 함수 실행*/
                if (onCbRkClicked != null)
                    onCbRkClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
                bCbRkClick = false;
            }
        }

        private void cbRK_MouseLeave(object sender, MouseEventArgs e)
        {
            if (bCbRkClick) bCbRkClick = false;
        }
        #endregion

        private void cbRK_Checked(object sender, RoutedEventArgs e)
        {
            if (onCbRkClicked != null)
                onCbRkClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
        }

        private void cbRK_Unchecked(object sender, RoutedEventArgs e)
        {
            if (onCbRkClicked != null)
                onCbRkClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
        }

        private void cbHide_Checked(object sender, RoutedEventArgs e)
        {
            if (onCbHideClicked != null)
                onCbHideClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
        }

        private void cbHide_Unchecked(object sender, RoutedEventArgs e)
        {
            if (onCbHideClicked != null)
                onCbHideClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
        }
    }
}
