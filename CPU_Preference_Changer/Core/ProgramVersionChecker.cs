using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CPU_Preference_Changer.Core {

    /// <summary>
    /// 웹 페이지(HTTPS)에서 특정 문자열을 포함하는 String을 얻는 클래스
    /// </summary>
    public class HttpsGetHiddenValue {
        private string targetStr;
        private string URL;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="hiddenText"></param>
        public HttpsGetHiddenValue(string URL, string hiddenText)
        {
            this.targetStr = hiddenText;
            this.URL = URL;
        }

        /// <summary>
        /// SSL연결을 위해 필요하다함
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certifi"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErr"></param>
        /// <returns></returns>
        private bool validateServerCertificate(object sender, X509Certificate certifi,X509Chain chain, SslPolicyErrors sslPolicyErr)
        {
            return true;
        }

        /// <summary>
        /// 웹 URL에 접속하고 targetStr을 포함하는 목록을 얻는다.
        /// </summary>
        /// <returns></returns>
        private List<string> getHiddenStrFromHtmlDocument()
        {
            HttpWebRequest req;
            HttpWebResponse response;

            try {
                req =  (HttpWebRequest)WebRequest.Create(URL);
                req.Proxy = null;
                req.Credentials = CredentialCache.DefaultCredentials;
                ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(validateServerCertificate);

                req.CookieContainer = new CookieContainer();
                req.Method = "GET";

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                response = (HttpWebResponse)req.GetResponse();

                List<string> targetLst = new List<string>();
                if ( response.StatusCode == HttpStatusCode.OK) {
                    using (var x = response.GetResponseStream()) {
                        StreamReader sr = new StreamReader(x);
                        while (!sr.EndOfStream) {
                            var curLine = sr.ReadLine();
                            if ( curLine.Contains(targetStr) )
                                targetLst.Add(curLine);
                        }
                    }
                }
                response.Close();
                return targetLst;
            } catch (Exception err){
                Debug.WriteLine(err.Message);
            }
            return null;
        }

        /// <summary>
        /// 숨겨진 문자열이 발견되었다면 발견된것들 중 첫 번째 요소 반환
        /// 없다면 null반환
        /// </summary>
        /// <returns></returns>
        public string getFirstHiddenStr()
        {
            var doc = getHiddenStrFromHtmlDocument();
            if (doc != null)
                return doc[0];
            return null;
        }
    
    }

    /// <summary>
    /// 프로그램 버전 체크를 위한 클래스
    /// </summary>
    class ProgramVersionChecker {
        public readonly static string currentVersion;
        private readonly static string parsingStr = "^^#$";

        /// <summary>
        /// 하드코딩할것. 현재 버전 값
        /// ( 새 버전으로 올릴 경우 GIT페이지에서 버전 값 수정 필수!!)
        /// </summary>
        static ProgramVersionChecker()
        {
            currentVersion = "2021.06.15_REV_1.001";
        }

        /// <summary>
        /// 프로그램의 새 버전이 존재하는지 검사한다.
        /// </summary>
        /// <returns></returns>
        public static bool isNewVersionExist()
        {
            /*버전 확인용 서버를 올릴수는 없으니까... (돈이 필요해진다...)
             * 이 프로그램 소스코드가 올려진 GIT페이지에 써진 버전 정보와 
             * 지금 정보를 비교해서 판단한다!*/
            const string gitURL = "https://github.com/xprics/Mabinogi-Multi-Helper";
            try {
                HttpsGetHiddenValue getter = new HttpsGetHiddenValue(gitURL, "^^#$");
                var str = getter.getFirstHiddenStr();
                if (str != null) {
                    string webVersion = str.Substring(parsingStr.Length);
                    if (false == webVersion.ToUpper().Equals(currentVersion.ToUpper())) {
                        return true;
                    }
                }
            } catch { }
            return false;
        }
    }
}
