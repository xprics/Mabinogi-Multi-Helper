namespace CPU_Preference_Changer.Core
{
    class SystemProcess
    {
        public static void Shutdown(int seconds)
        {
            Shutdown(seconds.ToString());
        }
        public static void Shutdown(uint seconds)
        {
            Shutdown(seconds.ToString());
        }

        public static void Shutdown(string seconds)
        {
            System.Diagnostics.Process.Start("shutdown.exe", "-s -f -t " + seconds);
        }
    }
}
