using CPU_Preference_Changer.WinAPI_Wrapper;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Text;

namespace CPU_Preference_Changer.Core
{
    class MabiProcess
    {
        public delegate void FindMabiProcess(string pName, int PID, string startTime, IntPtr coreState, string runPath, bool isHide, ref object usrParam);

        /// <summary>
        /// 마비노기로 추정되는 프로세스 목록 얻기
        /// </summary>
        /// <returns></returns>
        public static void getAllTargets(FindMabiProcess fnFindMabiProcess, ref object usrParam)
        {
            // 실행 프로세스 중 Client(마비노기 클라이언트 프로세스 이름) 가져오기
            const string mabiClientName = "Client";
            Process[] lst = Process.GetProcessesByName(mabiClientName);
            string runFilePath = string.Format(@"{0}\{1}.exe",getMabinogiInstallPathFromReg(), mabiClientName);
            foreach (Process p in lst)
            {
                using (p)
                {
                    try
                    {
                        /*마비노기 폴더에서 실행 된 client.exe라면 마비노기다.
                          toupper를 이용 대문자로 바꿔서 비교한다...*/
                        if( string.Compare(p.MainModule.FileName.ToUpper(), runFilePath.ToUpper()) == 0) {
                            /*콜백함수 실행*/
                            fnFindMabiProcess(p.ProcessName,
                                      p.Id,
                                      p.StartTime.ToString(),
                                      p.ProcessorAffinity,
                                      p.MainModule.FileName,
                                      p.MainWindowHandle == IntPtr.Zero ? true : false,
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

        /// <summary>
        /// pid를 이용해 해당 프로세스를 활성화
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static bool SetActivityWindow(int pid)
        {
            bool result = true;
            using (Process p = Process.GetProcessById(pid))
            {
                try
                {
                    IntPtr windowHandle = IntPtr.Zero;

                    // MainWindowHandle == InPtr.Zero : this process is hide
                    if (p.MainWindowHandle != IntPtr.Zero)
                        windowHandle = p.MainWindowHandle;
                    else
                    {
                        // get root window
                        windowHandle = WinAPI.FindWindow(null, null);
                        while (windowHandle != IntPtr.Zero)
                        {
                            if (WinAPI.GetParent(windowHandle) == IntPtr.Zero)
                            {
                                WinAPI.GetWindowThreadProcessId(windowHandle, out uint getPid);
                                if (getPid == pid)
                                    break;
                            }
                            windowHandle = WinAPI.GetWindow(windowHandle, GetWindowCmd.GW_HWNDNEXT);
                        }
                    }

                    // default show window as show and no activate window
                    result = WinAPI.ShowWindow(windowHandle, SwindOp.SW_SHOW);
                    if (!result)
                        result = WinAPI.ShowWindow(windowHandle, SwindOp.SW_SHOW);
                    if (result)
                        WinAPI.SetForegroundWindow(windowHandle);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// pid를 이용해 해당 프로세스를 최소화
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static bool SetMinimizeWindow(int pid)
        {
            bool result = true;
            using (Process p = Process.GetProcessById(pid))
            {
                try
                {
                    WinAPI.ShowWindow(p.MainWindowHandle, SwindOp.SW_MINIMIZE);
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
        /// pid를 이용해 해당 프로세스를 숨기기 (작업 표시줄에서 사라짐)
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static bool SetHideWindow(int pid)
        {
            bool result = true;
            using (Process p = Process.GetProcessById(pid))
            {
                try
                {
                    result = WinAPI.ShowWindow(p.MainWindowHandle, SwindOp.SW_HIDE);
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 마비노기가 설치되어있다면 컴퓨터마다 REG값이 등록되어있을것이다. 그 값을 읽어온다.
        /// </summary>
        /// <returns></returns>
        public static string getMabinogiInstallPathFromReg()
        {
            using (var HKCU = Registry.CurrentUser) {
                using (var mabiRegKey = HKCU.OpenSubKey(@"SOFTWARE\Nexon\Mabinogi")) {
                    return mabiRegKey.GetValue("ExecutablePath").ToString();
                }
            }
        }

        /// <summary>
        /// EnumWindows에 들어 갈 콜백함수
        /// </summary>
        /// <param name="hwnd">현재 ENUM중인 핸들 값</param>
        /// <param name="lParam">유저 Param</param>
        /// <returns></returns>
        private static bool CB_FindTargetHwndFromPID(IntPtr hwnd, int lParam)
        {
            uint curPID;

            /* HWND를 통해 이 HWND가 어느 PID에 소속되어있는지 PID값을 얻어낸다. */
            WinAPI.GetWindowThreadProcessId(hwnd, out curPID);
            if (curPID == (uint)lParam) {
                /*PID가 동일한 대상을 찾았다.*/
                StringBuilder sb=new StringBuilder();
                WinAPI.GetWindowText(hwnd, sb, 256);
                /*
                 * return false를 제거하고 ( 항상 return true;처리 )
                 * pParam이 지정한 모든 윈도우 핸들을 SW_SHOW 시켜보면
                 * 마비노기 창 이외에도, 플레이오네 출력창, 디버그용 출력창 등
                 * 여러가지 창들이 활성화된다...
                 * 
                 * (하나의 프로세스안에 여러 윈도우(컨트롤 말고 창)가 있어서 여러 핸들이 존재하는 경우임!)
                 * 
                 * 마비노기 창만 관심있으므로 마비노기가 아니면 암것도 안하고
                 * 마비노기 라면 SHOW처리하고 중단한다...
                 * 그런데 테스트 결과 보통은 Enum첫번째 들어오자마자 만나는 윈도우 한들이 마비노기 창이다.
                 * Debug.WriteLine("WINDOW TITLE = [" + sb.ToString() + "]");*/
                if( sb.ToString().Equals("마비노기")) {
                    WinAPI.ShowWindow(hwnd, SwindOp.SW_SHOW);
                    return false; /*ENUM중단.*/
                }
            }
            return true;
        }

        /// <summary>
        /// 주어진 PID를 통해 숨겨졌던 창을 다시 활성화 시킨다..
        /// </summary>
        /// <param name="pid"></param>
        public static void UnSetHideWindow(int pid)
        {
            WinAPI.EnumWindows(CB_FindTargetHwndFromPID, pid);
        }
    }
}
