using System;
using System.Linq;

using IBackgroundFreqTask = CPU_Preference_Changer.Core.BackgroundFreqTaskManager.IBackgroundFreqTask;

namespace CPU_Preference_Changer.Core
{
    /// <summary>
    /// System Process class
    /// </summary>
    class SystemProcess
    {
        #region Shutdown
        /// <summary>
        /// Shutdown computer
        /// </summary>
        /// <param name="seconds">int second</param>
        /// <returns>shutdown booking success or not</returns>
        public static bool Shutdown(int seconds)
        {
            return Shutdown(seconds.ToString());
        }

        /// <summary>
        /// Shutdown computer
        /// </summary>
        /// <param name="seconds">uint second</param>
        /// <returns>shutdown booking success or not</returns>
        public static bool Shutdown(uint seconds)
        {
            return Shutdown(seconds.ToString());
        }

        /// <summary>
        /// Shutdown computer
        /// </summary>
        /// <param name="seconds">string second</param>
        /// <returns>shutdown booking success or not</returns>
        public static bool Shutdown(string seconds)
        {
            try
            {
                System.Diagnostics.Process.Start("shutdown.exe", "-s -f -t " + seconds);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region process kill
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns>kill success or not</returns>
        public static bool Kill(int pid)
        {
            try
            {
                System.Diagnostics.Process.GetProcessById(pid).Kill();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }

    public class ProcessKillTask : IBackgroundFreqTask
    {
        #region Private Class PrioritySortedDictionary

        // thread safy(maybe) sorted dictionary class
        private class PrioritySortedDictionary : System.Collections.Generic.SortedDictionary<ulong, int>
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
                            this.Add(endTimestamp, pid);

                            // break while
                            break;
                        }
                        catch
                        {
                            // catch is have same Key
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
            /// <returns></returns>
            public bool Pop(ref UInt64 timestamp, ref int pid)
            {
                bool popResult = false;
                lock (this.lockObj)
                {
                    try
                    {
                        popResult = false;
                        if (this.Count > 0)
                        {
                            System.Collections.Generic.KeyValuePair<ulong, int> pair = this.First();
                            this.Remove(pair.Key);
                            timestamp = pair.Key;
                            pid = pair.Value;
                            return true;
                        }
                    }
                    catch { }
                }
                return popResult;
            }
        }
        #endregion

        private PrioritySortedDictionary dict = null;
        private const ulong freqTick = 500;

        // constructor
        public ProcessKillTask() {
            this.dict = new PrioritySortedDictionary();
        }

        // IBackgroundFreqTask interface
        public ulong getFreqTick() => freqTick;

        public void runFreqWork(object param)
        {
            UInt64 timestamp = 0;
            int pid = 0;

            // pop
            if (this.dict.Pop(ref timestamp, ref pid))
            {
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
                            catch (Exception err)
                            {
                                // kill exception or access exception
                                p.Dispose();
                                throw err;
                            }
                        }
                    }
                    catch
                    {
                        // 뭐라 처리해주긴 해야하는데...
                    }
                }
                else
                {
                    // is too early
                    this.dict.Push(timestamp, pid);
                }
            }
        }

        #region Push overloading
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
            if (this.dict == null)
                this.dict = new PrioritySortedDictionary();

            this.dict.Push(endTimestamp, pid);
        }
        #endregion
    }
}
