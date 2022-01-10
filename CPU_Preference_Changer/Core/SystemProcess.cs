using System;

namespace CPU_Preference_Changer.Core
{
    /// <summary>
    /// System Process class
    /// </summary>
    class SystemProcess
    {
        #region ShutDown
        /// <summary>
        /// Shutdown computer
        /// </summary>
        /// <param name="seconds">int second</param>
        public static bool Shutdown(int seconds)
        {
            return Shutdown(seconds.ToString());
        }

        /// <summary>
        /// Shutdown computer
        /// </summary>
        /// <param name="seconds">uint second</param>
        public static bool Shutdown(uint seconds)
        {
            return Shutdown(seconds.ToString());
        }

        /// <summary>
        /// Shutdown computer
        /// </summary>
        /// <param name="seconds">string second</param>
        public static bool Shutdown(string seconds)
        {
            try {
                System.Diagnostics.Process.Start("shutdown", "-s -f -t " + seconds).Dispose();
                return true;
            } catch (Exception err) {
#if DEBUG
                SingleTonTemplate.MMHGlobalInstance<MMHGlobal>.GetInstance().dbgLogger.writeLog(err);
#endif
                return false;
            }
        }

        /// <summary>
        /// 즉시 종료
        /// </summary>
        public static bool ShutdownNow()
        {
            return Shutdown(1);
        }
        #endregion

        #region Task Kill
        /// <summary>
        /// 관리자 작업 종료. Process.Kill이 안먹힐 경우 사용.
        /// 근데 커널 메모리에 올라가있는건 얘도 Access Deined 발생.
        /// </summary>
        /// <param name="pid">종료할 프로세스 pid(int)</param>
        /// <returns></returns>
        public static bool TaskKill(int pid)
        {
            return TaskKill(pid.ToString());
        }

        /// <summary>
        /// 관리자 작업 종료. Process.Kill이 안먹힐 경우 사용.
        /// 근데 커널 메모리에 올라가있는건 얘도 Access Deined 발생.
        /// </summary>
        /// <param name="pid">종료할 프로세스 pid(uint)</param>
        /// <returns></returns>
        public static bool TaskKill(uint pid)
        {
            return TaskKill(pid.ToString());
        }
        /// <summary>
        /// 관리자 작업 종료. Process.Kill이 안먹힐 경우 사용.
        /// 근데 커널 메모리에 올라가있는건 얘도 Access Deined 발생.
        /// </summary>
        /// <param name="pid">종료할 프로세스 pid(string)</param>
        /// <returns></returns>
        public static bool TaskKill(string pid)
        {
            try {
                System.Diagnostics.Process.Start("taskkill", "/T /F /PID " + pid).Dispose();
                return true;
            } catch (Exception err) {
#if DEBUG
                SingleTonTemplate.MMHGlobalInstance<MMHGlobal>.GetInstance().dbgLogger.writeLog(err);
#endif
                return false;
            }
        }
        #endregion

        #region WMIC Process Delete
        /// <summary>
        /// WMIC 윈도우 관리 도구 CLI 환경으로 process 아예 통짜로 날려버리기
        /// </summary>
        /// <param name="pid">종료할 프로세스 pid(int)</param>
        /// <returns></returns>
        public static bool WMICProcessKill(int pid)
        {
            return WMICProcessKill(pid.ToString());
        }

        /// <summary>
        /// WMIC 윈도우 관리 도구 CLI 환경으로 process 아예 통짜로 날려버리기
        /// </summary>
        /// <param name="pid">종료할 프로세스 pid(uint)</param>
        /// <returns></returns>
        public static bool WMICProcessKill(uint pid)
        {
            return WMICProcessKill(pid.ToString());
        }

        /// <summary>
        /// WMIC 윈도우 관리 도구 CLI 환경으로 process 아예 통짜로 날려버리기
        /// </summary>
        /// <param name="pid">종료할 프로세스 pid(string)</param>
        /// <returns></returns>
        public static bool WMICProcessKill(string pid)
        {
            try
            {
                System.Diagnostics.Process.Start("WMIC", "PROCESS WHERE ProcessID=" + pid + " DELETE").Dispose();
                return true;
            }
            catch (Exception err)
            {
#if DEBUG
                SingleTonTemplate.MMHGlobalInstance<MMHGlobal>.GetInstance().dbgLogger.writeLog(err);
#endif
                return false;
            }
        }

        #endregion

        #region WMI Process Terminate
        /// <summary>
        /// WMI Query를 이용해 Process Terminate
        /// </summary>
        /// <param name="pid">종료할 프로세스 pid(int)</param>
        /// <returns></returns>
        public static bool WMIProcessTerminate(int pid)
        {
            return WMIProcessTerminate(pid.ToString());
        }

        /// <summary>
        /// WMI Query를 이용해 Process Terminate
        /// </summary>
        /// <param name="pid">종료할 프로세스 pid(int)</param>
        /// <returns></returns>
        public static bool WMIProcessTerminate(uint pid)
        {
            return WMIProcessTerminate(pid.ToString());
        }

        /// <summary>
        /// WMI Query를 이용해 Process Terminate
        /// </summary>
        /// <param name="pid">종료할 프로세스 pid(int)</param>
        /// <returns></returns>
        public static bool WMIProcessTerminate(string pid)
        {
            System.Management.ManagementObjectSearcher searcher = null;
            try {
                searcher = new System.Management.ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Process WHERE ProcessID=" + pid);
                System.Management.ManagementObjectCollection collect = searcher.Get();

                if (collect.Count <= 0)
                    return false;

                foreach (System.Management.ManagementObject obj in collect)
                {
                    _ = obj.InvokeMethod("Terminate", null);
                }
                return true;
            } catch (Exception err) {
#if DEBUG
                SingleTonTemplate.MMHGlobalInstance<MMHGlobal>.GetInstance().dbgLogger.writeLog(err);
#endif
                return false;
            }
            finally {
                searcher.Dispose();
            }
        }
        #endregion
    }
}
