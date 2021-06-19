namespace CPU_Preference_Changer.Core
{
    /// <summary>
    /// System Process class
    /// </summary>
    class SystemProcess
    {
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
                System.Diagnostics.Process.Start("shutdown.exe", "-s -f -t " + seconds);
                return true;
            } catch {
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
    }
}
