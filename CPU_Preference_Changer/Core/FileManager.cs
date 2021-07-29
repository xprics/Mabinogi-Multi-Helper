using CPU_Preference_Changer.WinAPI_Wrapper;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CPU_Preference_Changer.Core
{
    class FileManager
    {

        /// <summary>
        /// 귀찮아서... MSDN에서 예제 가져옴 주어진 경로 아래에 모든 파일 or 디렉토리를 재귀적으로 이동하며 복사함..
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }


        /// <summary>
        /// 원본 디렉토리 하위의 내용물을 dst경로 하위에 그대로 복사함!
        /// 귀찮아서 MSDN예제를 적당히 썼다....
        /// </summary>
        /// <param name="src">원본 디렉토리 경로</param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static bool copyDirectory(string src, string dst)
        {
            if (src == null || dst == null || src.Equals("") || dst.Equals(""))
                return false;

            /*소스 경로 없는건 말이안됨*/
            if (!Directory.Exists(src)) 
                return false;

            DirectoryInfo srcDir = new DirectoryInfo(src);
            DirectoryInfo dstDir = new DirectoryInfo(dst);

            try {
                CopyAll(srcDir, dstDir);
            } catch (Exception err) {
                /*디스크 용량부족하거나 권한없거나 다양한 이유로 실패할수도 있음.*/
                throw err;
            }
            return true;
        }

        /// <summary>
        /// 주어진 디렉토리를 삭제하여 하위에 위치한 모든 파일을 삭제합니다.
        /// </summary>
        public static void deleteDirectory(string path)
        {
            if( Directory.Exists(path))
                Directory.Delete(path, true);
        }



        /// <summary>
        /// 심볼릭 링크 생성하기...
        /// </summary>
        /// <param name="symLinkPath"></param>
        /// <param name="linkTargetPath"></param>
        public static void CreateDirectorySymbolicLink(string symLinkPath, string linkTargetPath)
        {
            if (symLinkPath == null || linkTargetPath == null || symLinkPath.Equals("") || linkTargetPath.Equals(""))
                return ;

            /* 바로가기 만들 대상 폴더가 없는것은 말이안됨... */
            if (!Directory.Exists(linkTargetPath))
                return ;

            /*이미 링크가 존재하는것도 말이 안됨..*/
            if ( File.Exists(symLinkPath) || Directory.Exists(symLinkPath))
            {
                return; 
            }

            WinAPI.CreateSymbolicLink(symLinkPath, linkTargetPath, WinAPI.SYMBOLIC_LINK_FLAG.Directory);
        }
    }
}
