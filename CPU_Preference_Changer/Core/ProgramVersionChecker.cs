using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

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
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(validateServerCertificate);

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
                        /* by LT인척하는엘프 2021.07.16 
                         * 아래와 같이 sr.EndOfStream과 sr.ReadLine()을 이용하여
                           구현했던 버전에서 무슨 이유인지 모르지만 Release모드에서
                            아주 낮은 확률로 EndOfStream이 영원히 false인 현상이 발견되어서
                            String을 모두 받아내고 개행문자단위로 구분하여 찾게함!
                        =======================================================
                        while(!sr.EndOfStream){
                            string curLine = sr.readLine();
                            if ( curLine.Contains(targetStr) )
                                targetLst.Add(curLine);
                        }
                        =======================================================
                         */
                        string readStr = sr.ReadToEnd();
                        string[] sepLine = readStr.Split(new char[] {'\r','\n' });
                        foreach(string curLine in sepLine) {
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
        private readonly static string version_date;
        private readonly static string version_revValue;
        public readonly static string currentVersion;
        private readonly static string parsingStr = "^^#$";

        /// <summary>
        /// 하드코딩할것. 현재 버전 값
        /// ( 새 버전으로 올릴 경우 GIT페이지에서 버전 값 수정 필수!!)
        /// </summary>
        static ProgramVersionChecker()
        {
            version_date = "2021.07.16";
            version_revValue = "1.000"; /* 소수점 3자리 비어있더라도 3자리까지 꽉채울 것!*/
            currentVersion = string.Format("{0}_REV_{1}",version_date,version_revValue);

            if (isValidVer(currentVersion) == false) {
                /*심각한에러.*/
                throw new Exception("Version값 에러");
            }
        }

        /// <summary>
        /// 버전 정보 중 YMD얻기
        /// </summary>
        /// <param name="verStr"></param>
        /// <returns></returns>
        private static string getCurVerDate(string verStr)
        {
            /*YYYY.mm.dd를 포함한다면 최소 10글자..*/
            if (verStr == null || isValidVer(verStr) == false)
                return "";
            /* yyyy.mm.dd로 시작되기때문에 앞부분 10글자 짤라냄.*/
            return verStr.Substring(0,10);
        }

        /// <summary>
        /// 버전 글자가 맞는지 검사..
        /// 이게 틀릴 이유는 없다고보는데... 일단 대충 만들어둠.
        /// </summary>
        /// <param name="verStr"></param>
        /// <returns></returns>
        private static bool isValidVer(string verStr)
        {
            /*1. 20글자가 아니면 버전정보 아님*/
            if (verStr.Length != 20) return false;
            /*2. YYYY.MM.DD이후, _REV_가 발견되어야 함*/
            if (verStr.Contains("_REV_") == false) return false;
            /*3. _REV_가 11번째 (idx로는10)에서 발견되어야 함.*/
            if (verStr.IndexOf("_REV_") != 10) return false;
            /*---------------------------------------------------------------*/
            /*날짜형식 및 REV버전 형식(소수점3자리) 확인해야하지만.. 
             *   귀찮으니 패스하자....*/
            return true;
        }

        /// <summary>
        /// rev버전 값 얻기
        /// </summary>
        /// <param name="verStr"></param>
        /// <returns></returns>
        private static string getRevVer(string verStr)
        {
            if (verStr == null || isValidVer(verStr) == false)
                return "";
            /*버전값에 의하면 REV버전 값은 16번째부터 나온다!*/
            return verStr.Substring(15, verStr.Length - 15);
        }

        /// <summary>
        /// 프로그램 버전 검사 함수
        /// </summary>
        /// <param name="curVerseion"></param>
        /// <returns>-1 : 인자로 주어진 버전이 더 옛날 버전
        ///           0 : 동일한 버전
        ///           1 : 인자로 주어진 버전이 더 미래의 버전 ( 프로그램 배포 전에 발생 or Git의 버전 정보를 갱신하지 않아서 발생 )</returns>
        private static int versionCompare(string curVer)
        {
            /*두 버전이 단순히 동일한지..*/
            if (curVer.ToUpper().Equals(currentVersion.ToUpper()))
                return 0;
            /*날짜 파트 검사.*/
            string curVer_Date, curVer_rev;

            curVer_Date = getCurVerDate(curVer);
            curVer_rev = getRevVer(curVer);

            int i = string.Compare(version_date, curVer_Date);
            if (i != 0) return i;
            /* 날짜까지 같다면 rev버전 비교*/
            return string.Compare(version_revValue, curVer_rev);
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
                    /*버전 정보는 YYYY.MM.DD_REV_1.001처럼 되어있다.
                       따라서,,
                    날짜를 파싱하고, 뒤에 REV버전을 파싱하여
                    나보다 높은지 낮은지 판단할 수 있음!*/
                    if (versionCompare(webVersion.ToUpper())<0) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } catch { }
            /*뭔가 문제있어서 버전 확인에 실패한 경우.,... 새 버전 없다고친다.*/
            return false;
        }
    }
}
