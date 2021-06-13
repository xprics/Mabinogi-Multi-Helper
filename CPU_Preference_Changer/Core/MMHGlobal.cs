using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using CPU_Preference_Changer.Core.SingleTonTemplate;

namespace CPU_Preference_Changer.Core {

    /// <summary>
    /// 프로그램 내 전역적 자료 보관용도 클래스
    /// </summary>
    class MMHGlobal : IMMHGlobalInstance {
        public BackgroundFreqTaskMgmt backgroundFreqTaskManager = null;

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
