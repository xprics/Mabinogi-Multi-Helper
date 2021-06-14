using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CPU_Preference_Changer.BackgroundTask {
    class MabiClientKillTask : IBackgroundFreqTask{

        /// <summary>
        /// 예약 종료 지정된 프로세스 PID값 보관
        /// </summary>
        private int PID;

        /// <summary>
        /// 예약 종료 시간 보관...
        /// </summary>
        private DateTime killTime;

        private const ulong freq = 1000; /*1초단위로 여유롭게 감시한다.*/

        public MabiClientKillTask(int PID, DateTime killTime)
        {
            this.PID = PID;
            this.killTime = killTime;
        }

        public ulong getFreqTick()
        {
            return freq;
        }

        /// <summary>
        /// by LT인척하는엘프 - killClientProcess에서 catch로 빠져서
        /// 종료시키지 못 했을 때 또다른 방법으로 강종시켜봄
        /// </summary>
        private void killCLientProcess2()
        { 

        }


        /// <summary>
        /// by LT골든힐트
        /// 주어진 PID값을 가진 프로세스 종료
        /// </summary>
        private void killClientProcess()
        {
            using (Process p = Process.GetProcessById(PID)) {
                try {
                    // kill process
                    if (p != null)
                        p.Kill();
                } catch {
                    // kill exception or access exception
                    p.Dispose();
                    killCLientProcess2();
                }
            }
        }

        public bool runFreqWork(HBFT taskHandle, object param)
        {
            DateTime curTime = DateTime.Now;
            curTime.CompareTo(killTime);

            if (curTime.CompareTo(killTime) > 0) {
                /*예약 시간을 넘었다! 해당 PID값 확인해보고 
                 * 여전히 존재한다면 종료!*/
                killClientProcess();
                return false;
            }
            return true;
        }
    }
}
