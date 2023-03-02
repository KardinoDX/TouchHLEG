using System.Windows;
using System.IO;


namespace TouchHLE_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        System.Diagnostics.Process process = new System.Diagnostics.Process();

        private void btnOpenFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialogue = new Microsoft.Win32.OpenFileDialog();
            openFileDialogue.Filter = "Decrypted IPA Files (*.ipa)|*.ipa";
            if (openFileDialogue.ShowDialog() == true)
            {
                FilePath.Content = openFileDialogue.FileName;
                FilePathTxt.Text = openFileDialogue.FileName; 
                LaunchBtn.IsEnabled = true;
            }
        }

        private void LaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            process.StartInfo.FileName = "cmd";
            string CommandString = "touchHLE.exe \"" + FilePathTxt.Text + "\"";

            if (OrientationBox.SelectedIndex == 1) CommandString += " --landscape-left " ;
            else if (OrientationBox.SelectedIndex == 2) CommandString += " --landscape-right " ;

            switch (ScaleBox.SelectedIndex)
            {
                case (1):
                    CommandString += "--scale-hack=2";
                    break;
                case (2):
                    CommandString += "--scale-hack=3";
                    break;
                case (3):
                    CommandString += "--scale-hack=4";
                    break;
                default:
                    break;
            }

            process.StartInfo.Arguments = "/c " + CommandString;
            process.Start();
        }
    }
}
