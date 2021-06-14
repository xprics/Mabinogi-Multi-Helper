using System;
using System.Collections.Generic;
using System.Linq;

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
        public static void Shutdown(int seconds)
        {
            Shutdown(seconds.ToString());
        }

        /// <summary>
        /// Shutdown computer
        /// </summary>
        /// <param name="seconds">uint second</param>
        public static void Shutdown(uint seconds)
        {
            Shutdown(seconds.ToString());
        }

        /// <summary>
        /// Shutdown computer
        /// </summary>
        /// <param name="seconds">string second</param>
        public static void Shutdown(string seconds)
        {
            System.Diagnostics.Process.Start("shutdown.exe", "-s -f -t " + seconds);
        }

        public static void ShutdownNow()
        {
            System.Diagnostics.Process.Start("shutdown.exe", "-s -f");
        }
    }
}
