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

        /// <summary>
        /// 숨김 처리
        /// </summary>
        public bool isHide { get; set; }

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
    }
}
