using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CPU_Preference_Changer
{
    class MabiProcess
    {
        public delegate void FindMabiProcess(Process p, string pName, string PID, string startTime, string coreState, string runPath, ref object usrParam);

        /// <summary>
        /// 마비노기로 추정되는 프로세스 목록 얻기
        /// </summary>
        /// <returns></returns>
        public static void getAllTargets(FindMabiProcess fnFindMabiProcess,ref object usrParam)
        {
            /*1. 실행 프로세스 목록가져옴*/
            Process[] lst = Process.GetProcesses();
            /*2. 리스트 중 마비노기로 추정되는 자료 찾음*/
            foreach (Process x in lst)
            {
                try
                {
                    /*창 이름이 "마비노기"인 것 찾는다*/
                    if (string.Compare(x.MainWindowTitle, "마비노기") == 0)
                    {
                        /*콜백함수 실행*/
                        fnFindMabiProcess(x,
                                          x.ProcessName,
                                          x.Id + "",
                                          x.StartTime.ToString(),
                                          x.ProcessorAffinity + "",
                                          x.MainModule.FileName,
                                          ref usrParam);
                    }
                } catch {

                }
            }
        }

        /// <summary>
        /// 코어 할당량을 주어진 값에 맞게 한다!
        /// </summary>
        /// <param name="p">적용할 프로세스 (마비노기가 아니여도 작동하긴 함)</param>
        /// <param name="numOfCore">할당할 코어 수 (CPU수보다 많이한들 의미없음..</param>
        public static void setTargetCoreState(Process p, IntPtr Affinity)
        {
            if (p != null) p.ProcessorAffinity = Affinity;
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
        /// Win32 API SetForegroundWindow 선언,, 이걸로해보고 잘안되면 ShowWindow를 써보던가 한다...
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        
    }
}
