using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CPU_Preference_Changer.MabiProcessListView
{
    /// <summary>
    /// Listview Data Binding 클래스.. (리스트 뷰 행 데이터)
    /// </summary>
    public class LV_MabiProcessRowData : INotifyPropertyChanged
    {
        /// <summary>
        /// 프로세스 명과 binding 되는 변수
        /// </summary>
        public string processName { get; private set; }

        /// <summary>
        /// PID와 binding 되는 변수
        /// </summary>
        public string processID { get; private set; }

        /// <summary>
        /// 시작 시간과 바인딩 되는 변수
        /// </summary>
        public string startTime { get; private set; }

        /// <summary>
        /// OneWay모드 바인딩을 위해
        /// </summary>
        private string _coreState;
        /// <summary>
        /// 리스트뷰 코어 상태와 바인딩 되는 변수
        /// </summary>
        public string coreState {
            get {
                return _coreState;
            }
            set {
                _coreState = value;
                NotifyPropertyChanged("coreState");
            }
        }

        /// <summary>
        /// OneWay모드 바인딩 위해
        /// </summary>
        private string _reservedKillTime;
        /// <summary>
        /// 프로세스 예약 종료 시간 저장을 위하여...
        /// </summary>
        public string reservedKillTime {
            get {
                return _reservedKillTime;
            }
            set {
                _reservedKillTime = value;
                NotifyPropertyChanged("reservedKillTime");
            }
        }

        /// <summary>
        /// TMI정보와 바인딩되는 변수
        /// (프로세스 실행 파일 경로를 화면에 출력)
        /// </summary>
        public string processFilePath { get; private set; }

        /// <summary>
        /// 아이템 인덱스 보관용도
        /// </summary>
        public string tbCoreStateIdxTag { get; set; }

        /// <summary>
        /// 본캐 여부 저장용 
        /// </summary>
        public bool bMainCharacter { get; set; }

        /// <summary>
        /// 다양한 용도로 사용 될 유저 Param..
        /// (fixed) process PID
        /// </summary>
        public object userParam { get; set; }

        private bool _isHide;
        /// <summary>
        /// 숨김 처리되어있는가?
        /// </summary>
        public bool isHide {
            get {
                return _isHide;
            }
            set {
                _isHide = value;
                NotifyPropertyChanged("isHide");
            }
        }

        /// <summary>
        /// 예약종료 되어있는가?
        /// </summary>
        private bool _isKillReserved;

        /// <summary>
        /// 양방향 바인딩을 위해
        /// </summary>
        public bool isKillReserved {
            get {
                return _isKillReserved;
            }
            set {
                _isKillReserved = value;
                NotifyPropertyChanged("isKillReserved");
            }
        }

        /// <summary>
        /// 리스트 뷰 아이템 구조 생성자
        /// </summary>
        /// <param name="prName">프로세스 명</param>
        /// <param name="PID">PID값</param>
        /// <param name="sTime">시작 시간</param>
        /// <param name="coreState">코어 할당 상태값</param>
        /// <param name="processFilePath">실행파일경로</param>
        public LV_MabiProcessRowData(string prName, string PID, string sTime,
                                  string coreState, string processFilePath)
        {
            this.processName = prName;
            this.processID = PID;
            this.processFilePath = processFilePath;
            this.startTime = sTime;
            this.coreState = coreState;
            this.bMainCharacter = false;
            this.isHide = false;
            this.reservedKillTime = "None"; /*예약 종료 시간 기본 표시 글자.. 뭐로할까..?*/
        }

        public LV_MabiProcessRowData() { }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// coreState의 경우 oneway로 바인딩하여 자동 갱신되게 하였음
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    /// <summary>
    /// 위 요소에 대한 data collection 정의 
    /// </summary>
    public class LvMabiDataCollection : ObservableCollection<LV_MabiProcessRowData>
    {
        int itmIdx = 0;

        /// <summary>
        /// add함수 재정의,,, 자동으로 idx를 생성한 뒤 넣게한다!
        /// </summary>
        /// <param name="newData"></param>
        public new void Add(LV_MabiProcessRowData newData)
        {
            // 인덱스 정보 자동 추가하여 보관한다!
            newData.tbCoreStateIdxTag = itmIdx + "";
            itmIdx++;
            base.Add(newData);
        }

        /// <summary>
        /// 삭제함수 재정의...
        /// </summary>
        /// <param name="i"></param>
        public new void RemoveAt(int i)
        {
            base.RemoveAt(i);
        }

        /// <summary>
        /// 기존 데이터에, 새 정보 중 일부를 가져와서 업데이트한다.
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="newData"></param>
        public void updateData(int pid,LV_MabiProcessRowData newData)
        {
            /*숨김상태 이외 업데이트 필요한 정보 있는지 모르겠음.*/
            foreach (LV_MabiProcessRowData x in this.Items) {
                if (x.processID.Equals(pid + "")) {
                    x.isHide = newData.isHide;
                }
            }
        }

        /// <summary>
        /// 해당 PID값을 가지는 데이터가 있는지?
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private bool havePidData(int pid)
        {
            bool ret = false;
            foreach (LV_MabiProcessRowData x in this.Items) {
                if (  x.processID.Equals(pid + "") ) {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public delegate void beforeItmRemoved(LV_MabiProcessRowData removeData);

        /// <summary>
        /// 새 데이터 콜렉션을 참고하여 현재 데이터 콜렉션을 갱신시킨다!
        /// </summary>
        /// <param name="newDataCollection"></param>
        public void updateDataCollection(LvMabiDataCollection newDataCollection, beforeItmRemoved beforeItmRemoved)
        {
            /*CASE 1. 프로세스가 종료되어 제거되어야 하는 데이터가 있는지 조사*/
            /* 주의 !!! 배열을 뒤쪽부터 돌아야 모든 아이템을 순회할 수 있음 
               (아이템이 5개인데, 0번부터 돌다가 1번째를 삭제하고, idx2로 넘어가면??
                최초 5개일 때 기준 4번째 데이터를 보게 될 것,...)*/
            int pid;

            for (int i = this.Items.Count-1; i >= 0; --i) {
                if (int.TryParse(this.Items[i].processID, out pid)) {
                    if (newDataCollection.havePidData(pid) == false) {
                        beforeItmRemoved(this.Items[i]);
                        this.RemoveAt(i);
                    }
                }
            }
            /*CASE 2. 프로세스가 새로 실행되어 새롭게 추가된 정보찾아서 Add.*/
            foreach(LV_MabiProcessRowData x in newDataCollection) {
                if ( int.TryParse(x.processID, out pid)) {
                    if (this.havePidData(pid) == false) {
                        this.Add(x);
                    } else {
                        /*프로세스 상태가 변경되어 Update되어야 할 경우도 있음.
                         (예 : 프로세스 목록 얻은 순간에 이제 막 마비노기를 실행하면,
                        당연하게도 창이 숨겨져서 bHide == true인데.
                        조금만 지나서 화면이 뜨면 bHide==false로 변해야한다.*/
                        this.updateData(pid, x);
                    }
                }
            }
        }

        /// <summary>
        /// tagIndex를 통해 특정 아이템 찾아서 반환
        /// </summary>
        /// <param name="tagIdx"></param>
        /// <returns></returns>
        public LV_MabiProcessRowData findItmFromTagIndex(int tagIdx)
        {
            LV_MabiProcessRowData findData=null;

            foreach (LV_MabiProcessRowData x in this.Items) {
                if ( x.tbCoreStateIdxTag.Equals(tagIdx + "")) {
                    findData = x;
                    break;
                }
            }
            return findData;
        }

        /// <summary>
        /// tagIdx를 가진 아이템의 배열 index값 얻어오기...
        /// </summary>
        /// <param name="tagIdx"></param>
        /// <returns></returns>
        public int findItmIdxFromTagIndex(int tagIdx)
        {
            int retVal = -1;

            for ( int i = 0; i < this.Items.Count; ++i) {
                if (this.Items[i].tbCoreStateIdxTag.Equals(tagIdx + "")) {
                    retVal= i;
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// 모든 데이터를 순회한다
        /// </summary>
        /// <param name="enumCallBack"></param>
        public void enumAllData(Action<LV_MabiProcessRowData> enumCallBack)
        {
            foreach(LV_MabiProcessRowData x in this.Items) {
                enumCallBack(x);
            }
        }
    }
}
