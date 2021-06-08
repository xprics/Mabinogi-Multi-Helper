using System.Diagnostics;
using System.Windows;

namespace CPU_Preference_Changer.UI.InfoForm {
    /// <summary>
    /// ProgramInfo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgramInfo : Window {
        public ProgramInfo(Window parent)
        {
            this.Owner = parent;
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
