using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;

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
        public delegate void OnLvClicked(object sender, RoutedEventArgs e);

        public event OnProcessNameClicked onProcessNameClick;
        public event OnPCoreStateClicked onCoreStateClick;
        public event OnCbRkClicked onCbRkClicked;
        public event OnCbHideClicked onCbHideClicked;
        public event OnLvClicked onLvClicked;

        /// <summary>
        /// 리스트 뷰 출력중인 데이터의 스레드 동기화를 위해
        /// </summary>
        /// 
        private Mutex lvDataMutex;
        public LV_MabiProcess()
        {
            InitializeComponent();
            this.onProcessNameClick = null;
            this.onCoreStateClick = null;
            this.MabiProcessListView.Width = this.Width;
            MabiProcessListView.ItemsSource = new LvMabiDataCollection();
            lvDataMutex = new Mutex();
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
            if (lvItms == null)
                return false;
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
            /*MabiProcessListView.ItemsSource = null;*/
            LvMabiDataCollection items = (LvMabiDataCollection)MabiProcessListView.ItemsSource;
            items.Clear();
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

            System.Type type = obj.GetType();
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

        #region 본캐(라디오 버튼) 이벤트 처리
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
        #endregion

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
                if (onProcessNameClick != null) {
                    LvMabi_WaitSingleObject();
                    onProcessNameClick(GetLvRowItmData(ctlTagStrToInt(sender)));
                    LvMabi_ReleaseMutex();
                }
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
                if (onCoreStateClick != null) {
                    LvMabi_WaitSingleObject();
                    onCoreStateClick(GetLvRowItmData(ctlTagStrToInt(sender)));
                    LvMabi_ReleaseMutex();
                }
                bCoreStateMouseDown = false;
            }
        }

        private void tbCoreState_MouseLeave(object sender, MouseEventArgs e)
        {
            if (bCoreStateMouseDown)
                bCoreStateMouseDown = false;
        }
        #endregion

        #region 체크박스 예약종료 이벤트 처리
        private void cbRK_Checked(object sender, RoutedEventArgs e)
        {
            LvMabi_WaitSingleObject();
            if (onCbRkClicked != null)
                onCbRkClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
            LvMabi_ReleaseMutex();
        }

        private void cbRK_Unchecked(object sender, RoutedEventArgs e)
        {
            LvMabi_WaitSingleObject();
            if (onCbRkClicked != null)
                onCbRkClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
            LvMabi_ReleaseMutex();
        }
        #endregion

        #region 체크박스 숨김 이벤트 처리
        private void cbHide_Checked(object sender, RoutedEventArgs e)
        {
            LvMabi_WaitSingleObject();
            if (onCbHideClicked != null)
                onCbHideClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
            LvMabi_ReleaseMutex();
        }

        private void cbHide_Unchecked(object sender, RoutedEventArgs e)
        {
            LvMabi_WaitSingleObject();
            if (onCbHideClicked != null)
                onCbHideClicked(GetLvRowItmData(ctlTagStrToInt(sender)));
            LvMabi_ReleaseMutex();
        }
        #endregion

        #region 프로세스 목록 정렬 이벤트 처리
        private void MabiProcessListView_Click(object sender, RoutedEventArgs e)
        {
            LvMabi_WaitSingleObject();
            if (onLvClicked != null)
                onLvClicked(sender, e);
            LvMabi_ReleaseMutex();
        }

        /// <summary>
        /// 출력중인 내용을 프로세스 시작시간을 기준으로 정렬하기
        /// </summary>
        /// <param name="sortDirection"></param>
        public void sortListData(ListSortDirection sortDirection)
        {
            if (lvItms == null) return;

            List<LV_MabiProcessRowData> sortedList;

            LvMabi_WaitSingleObject();
            /* 메인캐릭터 찍어둔 정보가 맨위로 올라오게 1차 정렬한 후 
               프로세스 시작 시간으로 2차 정렬 함.*/
            switch (sortDirection)
            {
                case ListSortDirection.Descending:
                    sortedList = lvItms.OrderByDescending(x => x.bMainCharacter).ThenByDescending(x => x.startTime).ToList();
                    break;
                case ListSortDirection.Ascending:
                    sortedList = lvItms.OrderByDescending(x => x.bMainCharacter).ThenBy(x => x.startTime).ToList();
                    break;
                default:
                    return;
            }

            /* LIST로 나오기때문에 ObservableCollection으로 바로 덮어쓸 수 없다.
              lvItms = sortedList 처럼..*/
            foreach (LV_MabiProcessRowData x in sortedList)
            {
                lvItms.Move(lvItms.IndexOf(x), sortedList.IndexOf(x));
            }
            LvMabi_ReleaseMutex();
        }
        #endregion

        #region 쓰레드 동기화 Mutex
        /// <summary>
        /// 리스트뷰에 관리중인 데이터의 스레드 동기화를 위해 사용. (락 걸어두기)
        /// </summary>
        public void LvMabi_WaitSingleObject()
        {
            lvDataMutex.WaitOne();
        }

        /// <summary>
        /// 리스트뷰에 관리중인 데이터의 스레드 동기화를 위해 사용. (락 해제)
        /// </summary>
        public void LvMabi_ReleaseMutex()
        {
            lvDataMutex.ReleaseMutex();
        }
        #endregion
    }
}
