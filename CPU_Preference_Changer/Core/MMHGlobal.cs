using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
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

        public MMHGlobal()
        {
            backgroundFreqTaskManager = new BackgroundFreqTaskMgmt();
        }

        public void Release()
        {
            backgroundFreqTaskManager.Release();
        }

    }
}
