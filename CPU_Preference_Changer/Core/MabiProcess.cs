using CPU_Preference_Changer.WinAPI_Wrapper;
using System;
using System.Diagnostics;

namespace CPU_Preference_Changer.Core
{
    class MabiProcess
    {
        public delegate void FindMabiProcess(string pName, int PID, string startTime, IntPtr coreState, string runPath, ref object usrParam);

        /// <summary>
        /// 마비노기로 추정되는 프로세스 목록 얻기
        /// </summary>
        /// <returns></returns>
        public static void getAllTargets(FindMabiProcess fnFindMabiProcess, ref object usrParam)
        {
            // 실행 프로세스 중 Client(마비노기 클라이언트 프로세스 이름) 가져오기
            Process[] lst = Process.GetProcessesByName("Client");
            //Process[] lst = Process.GetProcesses();
            foreach (Process p in lst)
            {
                using (p)
                {
                    try
                    {
                        /*창 이름이 "마비노기"인 것 찾는다*/
                        if (string.Compare(p.MainWindowTitle, "마비노기") == 0)
                        {
                            /*콜백함수 실행*/
                            fnFindMabiProcess(p.ProcessName,
                                      p.Id,
                                      p.StartTime.ToString(),
                                      p.ProcessorAffinity,
                                      p.MainModule.FileName,
                                      ref usrParam);
                        }
                    }
                    catch
                    {
                        // GetProcesses() 에서 System Process를 건들 경우 Exception 발생
                    }
                }
            }
        }

        /// <summary>
        /// 코어 할당량을 주어진 값에 맞게 한다!
        /// </summary>
        /// <param name="pid">적용할 프로세스 PID (마비노기가 아니여도 작동하긴 함)</param>
        /// <param name="numOfCore">할당할 코어 수 (CPU수보다 많이한들 의미없음..</param>
        public static void setTargetCoreState(int pid, IntPtr Affinity)
        {
            
            using(Process p = Process.GetProcessById(pid))
            {
                if (p != null) p.ProcessorAffinity = Affinity;
            }
        }

        /// <summary>
        /// 현재 시스템이 가질 수 있는 최대 값을 계산하여 반환한다!
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetMaxAffinityVal()
        {
            int cnt = SystemInfo.GetCpuCoreCnt();
            /*-----------------------------------------------------*/
            if (cnt == 0) return IntPtr.Zero;
            /*-----------------------------------------------------*/
            ulong ret = 0; ulong v = 0x0000000000000001;
            /*-----------------------------------------------------*/
            for (int i = 0; i < cnt; ++i) {
                ret |= v; v <<= 1;
            }
            /*-----------------------------------------------------*/
            return ConvToSystemBit(ret);
        }

        /// <summary>
        /// OS 비트수에 맞게 적절히 마스킹 함.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static IntPtr ConvToSystemBit(ulong val)
        {
            if (Environment.Is64BitOperatingSystem) {
                return (IntPtr)val;
            } else {
                return (IntPtr)((uint)(val & 0xffffffff));
            }
        }

        /// <summary>
        /// 최소 1개만 쓸 경우 필요한 설정 값 계산하여 반환
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetMinAffinityVal()
        {
            /* 0x01 값이 0번 코어임!   
             * 하위비트부터 0,1,2,3,4,... 순으로 할당된다!
             * 32비트 시스템에서는 코어숫자 32개까지가 최대고
             * 64비트 시스템에서는 64개까지가 최대 임 ( MSDN  ProcessorAffinity 참고 )
               0번코어 써도되겠지만 클라이언트는 맨 마지막 코어번호를 얻도록한다...  */
            int cnt = SystemInfo.GetCpuCoreCnt();
            if (cnt == 0) return IntPtr.Zero;
            ulong v = 0x0000000000000001;
            /* v가 0x01들어 있기때문에 i=1부터임에 주의*/
            for (int i = 1; i < cnt; ++i) {
                v <<= 1;
            }
            return ConvToSystemBit(v);
        }

        public static bool SetActivityWindow(int pid)
        {
            bool result = true;
            using (Process p = Process.GetProcessById(pid))
            {
                try
                {
                    WinAPI.ShowWindow(p.MainWindowHandle, SwindOp.SW_SHOWNORMAL);
                    WinAPI.SetForegroundWindow(p.MainWindowHandle);
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }



    }
}
