using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using CPU_Preference_Changer.Core.Logger;
using CPU_Preference_Changer.Core.SingleTonTemplate;

namespace CPU_Preference_Changer.Core {
    /// <summary>
    /// 프로그램 내 전역적 자료 보관용도 클래스
    /// </summary>
    class MMHGlobal : IMMHGlobalInstance {

        /// <summary>
        /// 백그라운드 Task매니저..
        /// </summary>
        public BackgroundFreqTaskMgmt backgroundFreqTaskManager = null;

        /// <summary>
        /// 예약 종료 ( 시스템 or 클라이언트 )  걸어둔 갯수..
        /// </summary>
        public int reservedTaskCount;

        /// <summary>
        /// 프로그램이 디버그모드로 실행되는가...
        /// </summary>
        public bool bDebugModeRun { get; set; }

        /// <summary>
        /// 디브그용 로거...
        /// </summary>
        public MMH_Logger dbgLogger = null;

        public MMHGlobal()
        {
            backgroundFreqTaskManager = new BackgroundFreqTaskMgmt();
            bDebugModeRun = false;
        }

        /// <summary>
        /// 글로벌 인스턴스 해제 ...
        /// </summary>
        public void Release()
        {
            if ( dbgLogger!=null) {
                dbgLogger.closeLogFile();
            }
            backgroundFreqTaskManager.Release();
        }

    }
}
