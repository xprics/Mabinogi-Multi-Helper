using CPU_Preference_Changer.BackgroundTask;
using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using CPU_Preference_Changer.Core.SingleTonTemplate;
using System;

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
        /// 전역적으로 관리. 
        /// 시스템 예약 종료 작업...
        /// </summary>
        public SystemShutdownTask shutdownTask { get; private set; }

        /// <summary>
        /// 시스템 예약종료 작업 핸들
        /// </summary>
        public HBFT sysShutdownTaskHandle { get; set; }




        public MMHGlobal()
        {
            sysShutdownTaskHandle = null;
            shutdownTask = new SystemShutdownTask(DateTime.Now);
            backgroundFreqTaskManager = new BackgroundFreqTaskMgmt();
        }

        public void Release()
        {
            backgroundFreqTaskManager.Release();
        }

    }
}
