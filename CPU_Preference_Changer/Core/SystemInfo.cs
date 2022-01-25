using CPU_Preference_Changer.Core.Logger;
using System;
using System.Management;

namespace CPU_Preference_Changer.Core
{
    class SystemInfo
    {
        /// <summary>
        /// CPU이름 얻기.
        /// </summary>
        /// <returns></returns>
        public static string GetCpuName(ILogWriter iLogger = null)
        {
            try {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                foreach (ManagementObject x in searcher.Get()) {
                    //대다수는 CPU가 하나 부착되어있으므로 한번만에 찾아진것 반환
                    return x["Name"].ToString();
                }
                return "";
            } catch (Exception err) {
                iLogger?.writeLog(err);
                return "";
            }
        }

        /// <summary>
        /// int value로 얻기
        /// </summary>
        /// <returns></returns>
        public static int GetCpuCoreCnt(ILogWriter iLogger = null)
        {
            try {
                return Environment.ProcessorCount;
            } catch (Exception err) {
                iLogger?.writeLog(err);
                return 0;
            }
        }

        /// <summary>
        /// 논리적 프로세서 수량 얻기 (하이퍼 스레딩 포함)
        /// </summary>
        /// <returns></returns>
        public static string GetCpuCoreCntStr(ILogWriter iLogger = null)
        {
            try {
                return Environment.ProcessorCount + "코어";
            } catch (Exception err) {
                iLogger?.writeLog(err);
                return "???";
            }
        }
    }
}
