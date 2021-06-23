using System;
using System.IO;

namespace CPU_Preference_Changer.Core.Logger {
    /// <summary>
    /// by LT인척하는엘프 2021.06.20 
    /// 적절히 로그를 찍는 클래스..
    /// </summary>
    class MMH_Logger {
        private string logPath;
        private StreamWriter sw;
        private FileStream fs;
        private object obj = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="logPath">로그파일 경로</param>
        public MMH_Logger (string logPath)
        {
            this.logPath = logPath;
            try {
                openLogFile();
            } catch {
                throw new IOException("로그 파일 Open 실패");
            }
        }

        /// <summary>
        /// 로그 파일 오픈
        /// </summary>
        private void openLogFile()
        {
            fs = File.Open(logPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            sw = new StreamWriter(fs);
        }

        /// <summary>
        /// 주어진 String에 타임스탬프를 덧붙인다.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string mkTimeStampStr(string str)
        {
            return string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff"), str);
        }

        /// <summary>
        /// 로그 기록 ( 기록시간과 개행문자는 알아서 넣어줌 )
        /// </summary>
        /// <param name="str"></param>
        public void writeLog(string str)
        {
            lock (obj) {
                /*멀티스레드 환경에서 이미 닫긴경우 아무것도 안하게 처리..*/
                if (sw == null) return;
                /*Date 기록*/
                sw.WriteLine(mkTimeStampStr(str));
                sw.Flush(); /*로그가 바로바로 기록되도록 Flush한다.*/
            }
        }

        /// <summary>
        /// Exception정보 로그로 쓰기..
        /// </summary>
        /// <param name="err"></param>
        public void writeLog(Exception err)
        {
            lock (obj) {
                sw.WriteLine(mkTimeStampStr(err.Message));
                sw.WriteLine(mkTimeStampStr(err.StackTrace));
                sw.Flush();
            }
        }

        /// <summary>
        /// 로그파일 닫기 ( 이 작업 이후 writeLog불가능 )
        /// </summary>
        public void closeLogFile()
        {
            lock (obj) {
                sw.Close(); /* 하위에 연결된 스트림도 알아서 Close시켜줌*/
                sw = null;
            }
        }
    }
}
