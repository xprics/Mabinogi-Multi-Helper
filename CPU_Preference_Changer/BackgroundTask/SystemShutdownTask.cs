using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using CPU_Preference_Changer.UI.OptionForm;
using System;

namespace CPU_Preference_Changer.BackgroundTask {

    /// <summary>
    /// by LT인척하는엘프
    /// 시스템 종료 작업 정보 클래스
    /// </summary>
    class SystemShutdownTask : IBackgroundFreqTask {
        const ulong freq = 1000 ; /*1초 단위로 반복할 작업임*/
        /// <summary>
        /// 예약 종료시간
        /// </summary>
        public DateTime targetTime { get; private set; }

        /// <summary>
        /// 생성자..
        /// </summary>
        /// <param name="targetTime"></param>
        public SystemShutdownTask(DateTime targetTime)
        {
            this.targetTime = targetTime;
        }

        /// <summary>
        /// 시스템 종료 시간 업데이트
        /// </summary>
        public void modiSysShutdownTime(DateTime targetTime)
        {
            this.targetTime = targetTime;
        }

        public ulong getFreqTick()
        {
            return freq;
        }

        public bool runFreqWork(HBFT hTask, object param)
        {
            DateTime curTime = DateTime.Now;

            if (curTime.CompareTo(targetTime) > 0) {
                /* 종료 시간이 되었다.
                 * 유저의 변심을 고려하여 최후의 1분 선택지를 유저에게 보여준다.*/
                SysShutdownAskForm askForm = new SysShutdownAskForm();
                askForm.ShowDialog();
                return false;
            }
            return true;
        }
    }
}
