using CPU_Preference_Changer.Core;
using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using System;
using System.Diagnostics;

namespace CPU_Preference_Changer.BackgroundTask {
    /// <summary>
    /// 마비노기 클리어언트 종료 Task.
    /// 지정된 시간이 되면 특정 PID값을 가진 프로세스를 강제종료 함.
    /// </summary>
    class MabiClientKillTask : IBackgroundFreqTask{

        /// <summary>
        /// 클라이언트 강종 되었을 때 일어날 이벤트 타입정의
        /// </summary>
        /// <param name="sender"></param>
        public delegate void OnClientKilled(object sender);

        /// <summary>
        /// 클라이언트를 강종 시켰을 때 일어날 이벤트
        /// </summary>
        public event OnClientKilled onClientProcessKilled;

        /// <summary>
        /// 예약 종료 지정된 프로세스 PID값 보관
        /// </summary>
        private int PID;

        /// <summary>
        /// 예약 종료 시간 보관...
        /// </summary>
        private DateTime killTime;

        private const ulong freq = 1000; /*1초단위로 여유롭게 감시한다.*/
        
        /// <summary>
        /// 클라이언트 종료 Task생성자
        /// </summary>
        /// <param name="PID">종효 할 PID</param>
        /// <param name="killTime">종료 시간</param>
        public MabiClientKillTask(int PID, DateTime killTime)
        {
            this.PID = PID;
            this.killTime = killTime;
        }

        /// <summary>
        /// 이 Task가 어느정도의 주기로 반복 실행하면 되는지 반환
        /// </summary>
        /// <returns></returns>
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
            /*나중에 잘 안되는 일 생기면 그때 구현.*/

            // 엌ㅋㅋㅋㅋㅋㅋㅋㅋ
            if (!SystemProcess.TaskKill(PID))
                if (!SystemProcess.WMICProcessKill(PID))
                    if (!SystemProcess.WMIProcessTerminate(PID)) { }
        }

        /// <summary>
        /// by LT골든힐트
        /// 주어진 PID값을 가진 프로세스 종료
        /// </summary>
        private void killClientProcess()
        {
            try {
                using (Process p = Process.GetProcessById(PID)) {
                    try {
                        /*혹시라도 그짧은 순간에 마비가 종료되고 다른 프로세스로 켜졌을 수 있으니 확인해보고 종료*/
                        if ( (p!=null) && MabiProcess.isMabiProcess(p)) {
                            // kill process
                            p.Kill();
                        }
                    } catch {
                        // kill exception or access exception
                        p.Dispose();
                        killCLientProcess2();
                    }
                }
            } catch {
                //PID에 해당하는 프로세스 하필 이 순간에 사라져서 없을 경우 예외 발생
            }
        }

        /// <summary>
        /// 클라이언트 종료 시간이 되었는지 주기적으로 확인하는 함수
        /// </summary>
        /// <param name="taskHandle"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool runFreqWork(HBFT taskHandle, object param)
        {
            DateTime curTime = DateTime.Now;
            curTime.CompareTo(killTime);

            if (curTime.CompareTo(killTime) > 0) {
                /*예약 시간을 넘었다! 해당 PID값 확인해보고 
                 * 여전히 존재한다면 종료!*/
                killClientProcess();
                if (onClientProcessKilled != null) {
                    onClientProcessKilled(this);
                }
                return false;
            }
            return true;
        }
    }
}
