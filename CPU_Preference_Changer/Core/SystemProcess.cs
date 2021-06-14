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

    public class ProcessKillRunner
    {
        #region Private Class PrioritySortedDictionary

        // thread safy(maybe) sorted dictionary class
        private class PrioritySortedDictionary : SortedDictionary<ulong, int>
        {
            // thread safe lock
            private object lockObj = new object();

            public PrioritySortedDictionary() { }

            /// <summary>
            /// Push data on Unix Timestamp 
            /// </summary>
            /// <param name="endTimestamp">Unix Timestamp</param>
            /// <param name="pid">process pid</param>
            public void Push(UInt64 endTimestamp, int pid)
            {
                lock (this.lockObj)
                {
                    while (true)
                    {
                        try
                        {
                            // add catch is have same Key
                            this.Add(endTimestamp, pid);

                            // break while
                            break;
                        }
                        catch
                        {
                            endTimestamp++;
                        }
                    }
                }
            }

            /// <summary>
            /// Pop Data
            /// </summary>
            /// <param name="timestamp">timestamp</param>
            /// <param name="pid">process pid</param>
            public void Pop(out UInt64 timestamp, out int pid)
            {
                lock (this.lockObj)
                {
                    try
                    {
                        KeyValuePair<ulong, int> pair = this.First();
                        this.Remove(pair.Key);
                        timestamp = pair.Key;
                        pid = pair.Value;
                    }
                    catch
                    {
                        timestamp = 0;
                        pid = -1;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// singletone instance
        /// </summary>
        public static ProcessKillRunner Instance { get { return instance.Value; } }

        // lazy instance
        private static readonly Lazy<ProcessKillRunner> instance = new Lazy<ProcessKillRunner>(() => new ProcessKillRunner());

        private PrioritySortedDictionary dict = null;
        private System.Threading.Timer threadTimer = null;

        // constructor
        private ProcessKillRunner() { }

        /// <summary>
        /// Start process kill runner
        /// </summary>
        /// <param name="interval_ms">interval ms. Default 500ms(0.5 second)</param>
        public void Start(int interval_ms = 500)
        {
            // allocation dictionary
            this.dict = new PrioritySortedDictionary();

            // local timer tick callback
            Func<int> timerCallback = () =>
            {
                UInt64 timestamp;
                int pid;

                // pop
                this.dict.Pop(out timestamp, out pid);
                if (pid == -1)
                    return 1;

                // now timestamp
                UInt64 nowTimestamp = Convert.ToUInt64(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

                // check
                if (nowTimestamp >= timestamp)
                {
                    try
                    {
                        // get process
                        using (System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(pid))
                        {
                            try
                            {
                                // kill process
                                if (p != null)
                                    p.Kill();
                            }
                            catch
                            {
                                // kill exception or access exception
                                p.Dispose();
                                throw;
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    // is too early
                    this.dict.Push(timestamp, pid);
                }

                return 0;
            };

            // thread timer
            this.threadTimer = new System.Threading.Timer((object state) => { (state as Func<int>)?.Invoke(); }, timerCallback, 0, interval_ms);
        }

        public void Stop()
        {
            if (this.threadTimer != null)
                this.threadTimer.Dispose();
            this.threadTimer = null;
        }

        /// <summary>
        /// Push data on string (yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="endDateTime">(yyyy-MM-dd HH:mm:ss)</param>
        /// <param name="pid">process pid</param>
        public void Push(string endDateTime, int pid)
        {
            // DateTime str2DateTime = DateTime.ParseExact(endDateTime, "yyyy-MM-dd HH:mm:ss", null);
            // this.Push(str2DateTime, pid);
            this.Push(DateTime.ParseExact(endDateTime, "yyyy-MM-dd HH:mm:ss", null), pid);
        }

        /// <summary>
        /// Push data on DateTime
        /// </summary>
        /// <param name="endDateTime">DateTime</param>
        /// <param name="pid">process pid</param>
        public void Push(DateTime endDateTime, int pid)
        {
            this.Push(Convert.ToUInt64(endDateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds), pid);
        }

        /// <summary>
        /// Push data on Unix Timestamp 
        /// </summary>
        /// <param name="endTimestamp">Unix Timestamp</param>
        /// <param name="pid">process pid</param>
        public void Push(UInt64 endTimestamp, int pid)
        {
            if (this.dict == null) this.dict = new PrioritySortedDictionary();
            this.dict.Push(endTimestamp, pid);
        }
    }
}
