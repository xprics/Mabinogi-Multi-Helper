using CPU_Preference_Changer.Core;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CPU_Preference_Changer.UI.Tool.MabiDocRePath
{
    /// <summary>
    /// MabiDocRePath.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MabiDocRePath : Window
    {
        public MabiDocRePath(Window parent)
        {
            this.Owner = parent;
            InitializeComponent();
            tb_SourcePath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\마비노기";
        }

        private string openDirectorySelDlg(string info)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = info;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }
            return "";
        }

        /// <summary>
        /// 아마 이걸 누를 사람이 있을까 싶지만.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_SelSource_Click(object sender, RoutedEventArgs e)
        {
            tb_SourcePath.Text = openDirectorySelDlg("내문서 -> 마비노기 경로를 골라주세요..");
        }

        /// <summary>
        /// dst선택 버튼 클릭..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_SelDest_Click(object sender, RoutedEventArgs e)
        {
            tb_DestPath.Text = openDirectorySelDlg("새 폴더 경로를 지정해주세요.") + @"\마비노기";
        }

        /// <summary>
        /// 백업 폴더 경로 이름 얻기..
        /// </summary>
        /// <param name="dstFold"></param>
        /// <returns></returns>
        private string getBackupFoldPath(string dstFold)
        {
            return dstFold + "_Backup";
        }

        /// <summary>
        /// 마비노기 옮기기전 백업 만들면서 옮기기위해.. 백업폴더로 옮기기!
        /// </summary>
        /// <param name="dstFold"></param>
        /// <returns></returns>
        private bool mkMabiBackup(string srcPath,string dstFold)
        {
            try
            {
                FileManager.copyDirectory(srcPath, getBackupFoldPath(dstFold));
            }catch (Exception err)
            {
                MessageBox.Show(err.Message,"백업 생성 실패",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 마비노기 옮기기전 백업 만들면서 옮기기위해.. 백업폴더로 옮기기!
        /// </summary>
        /// <param name="dstFold"></param>
        /// <returns></returns>
        private bool mabiFoldCopy(string srcPath, string dstFold)
        {
            try
            {
                FileManager.copyDirectory(srcPath, dstFold);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "백업 생성 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 백업용 폴더 지우기
        /// </summary>
        private void deleteBackupFold(string path)
        {
            if (MessageBoxResult.OK != MessageBox.Show("마비노기 백업 폴더를 삭제하겠습니까?", "안내", MessageBoxButton.YesNo))
                return;
            FileManager.deleteDirectory(path);
        }

        /// <summary>
        /// 버튼 상태 토글..
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="state"></param>
        private void buttonToggleState(Button btn, bool state)
        {
            Dispatcher.Invoke(() =>
            {
                btn.IsEnabled = state;
            });
        }

        /// <summary>
        /// 화면에 메세지 출력하기..
        /// </summary>
        /// <param name="msg"></param>
        private void updateStateMessage(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                label_Message.Content = msg;
            });
        }

        /// <summary>
        /// ui에 존재하는 버튼 잠그기.. 
        /// </summary>
        /// <param name="b">true 잠금, false - 해제</param>
        private void setUiLock(bool b)
        {
            buttonToggleState(bt_Start, !b);
            buttonToggleState(bt_SelDest, !b);
            buttonToggleState(bt_SelSource, !b);
        }

        /// <summary>
        /// 작업 시작..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_Start_Click(object sender, RoutedEventArgs e)
        {
            setUiLock(true);
            string dstPath, srcPath;
            srcPath = tb_SourcePath.Text;
            dstPath = tb_DestPath.Text;

            if (srcPath == null || dstPath == null
                || srcPath.Equals("") || dstPath.Equals(""))
            {
                MessageBox.Show("원본 / 대상 경로를 선택하세요.");
                return;
            }

            Thread th = new Thread(() => {
                if (srcPath[0] == dstPath[0])
                {
                    /*동일한 드라이브로 링크..? */
                    if (MessageBoxResult.OK != MessageBox.Show("동일한 드라이브를 대상으로 링크를 만들고 있으십니다!\n 계속하시겠습니까?", "안내", MessageBoxButton.YesNo)){
                        setUiLock(false);
                        return;
                    }
                }

                /* 1. 백업 파일 생성 */
                /* 타겟이 될 경로에 "마비노기_백업"을 만든다... */
                updateStateMessage("작업을 진행하기 전 백업 파일을 생성하고 있습니다,,,");
                if (false == mkMabiBackup(srcPath, dstPath))
                {
                    setUiLock(false);
                    return;
                }
                /*2. 디렉토리 복사*/
                updateStateMessage("마비노기 폴더 복사 중,,,");
                if (false == mabiFoldCopy(srcPath, dstPath))
                {
                    deleteBackupFold(getBackupFoldPath(dstPath));
                    setUiLock(false);
                    return;
                }
                /*3. 내문서의 마비노기 경로 삭제*/
                updateStateMessage("내문서의 마비노기 삭제,,,");
                FileManager.deleteDirectory(srcPath);

                /*4. 내문서에서 유저가 선택한 dstPath로의 심볼릭 링크를 만들어야한다..*/
                updateStateMessage("심볼릭 링크 생성,,,");
                FileManager.CreateDirectorySymbolicLink(srcPath, dstPath);

                /*5. 완료. 백업 삭제  => 혹시 모르니까 사람이 수동으로 하게 함!
                deleteBackupFold(getBackupFoldPath(dstPath));*/

                updateStateMessage("작업 완료!");
                setUiLock(false);
            });

            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            
        }
    }
}
