using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CPU_Preference_Changer
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
                    ShowWindow(p.MainWindowHandle, WindowState.SW_SHOWNORMAL);
                    SetForegroundWindow(p.MainWindowHandle);
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Win32 API SetForegroundWindow 선언,, 이걸로해보고 잘안되면 ShowWindow를 써보던가 한다...
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Win32 API ShowWindow, 최소화된 프로그램을 강제로 깨우는 함수
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        /*
         *  SW_HIDE	 0	 보이지 않도록 합니다.
         *  SW_SHOWNORMAL	 1	 Window를 보이도록 하되 최대화 또는 최소화 되어 있으면 원래상태로 되돌립니다.
         *  SW_SHOWMINIMIZED	 2	 Window를 활성화 하고 최소화 합니다.
         *  SW_MAXIMIZE	 3	 최대화 합니다.
         *  SW_SHOWNOACTIVATE	 4	 Window를 보이도록 하지만 활성화 하지는 않습니다.
         *  SW_SHOW	 5	 Window를 보이도록 합니다.
         *  SW_MINIMIZE	 6	 최소화 한 후 이전 Window를 활성화 합니다.
         *  SW_SHOWMINNOACTIVE	 7	 Window를 최소화하지만 활성화 하지는 않습니다.
         *  SW_SHOWNA	 8	 Window를 보이도록 하지만 활성화 하지는 않습니다.
         *  SW_RESTORE	 9	 원상태로 되돌립니다.
         *  SW_SHOWDEFAULT	 10	 -
         *  SW_FORCEMINIMIZE	 11	 최소화 합니다.
         */
        public class WindowState
        {
            public static int SW_HODE = 0;
            public static int SW_SHOWNORMAL = 1;
            public static int SW_SHOWMINIMIZED = 2;
            public static int SW_MAXIMIZE = 3;
            public static int SW_SHOWNOACTIVATE = 4;
            public static int SW_SHOW = 5;
            public static int SW_MINIMIZE = 6;
            public static int SW_SHOWMINNOACTIVE = 7;
            public static int SW_SHOWNA = 8;
            public static int SW_RESTORE = 9;
            public static int SW_SHOWDEFAULT = 10;
            public static int SW_FORCEMINIMIZE = 11;
        };
    }
}
