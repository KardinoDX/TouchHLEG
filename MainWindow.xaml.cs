using System.Windows;
using System.IO;
using System.Linq;

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

        // Manages Dialogue for obtaining IPA Files
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

        // Manages Dialogue for obtaining APP Folders
        private void btnOpenFolder(object sender, RoutedEventArgs e)
        {

            while (true)
            {
                var FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult selected = FolderBrowser.ShowDialog();
                string folderPath = FolderBrowser.SelectedPath;
                if (selected == System.Windows.Forms.DialogResult.OK && (folderPath.Contains(".app") || folderPath.Contains(".APP")))
                {
                    FilePath.Content = folderPath;
                    FilePathTxt.Text = folderPath;
                    LaunchBtn.IsEnabled = true;
                    return;

                }
                else if (selected == System.Windows.Forms.DialogResult.OK)
                {
                    MessageBox.Show("Please Select a valid .APP folder.",
                                    "Invalid Folder", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else return;
            }
        }

        private void LaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            process.StartInfo.FileName = "cmd";
            string CommandString = "touchHLE.exe \"" + FilePathTxt.Text + "\"";

            // Sets Orientations
                if (OrientationBox.SelectedIndex == 1) CommandString += " --landscape-left ";
                else if (OrientationBox.SelectedIndex == 2) CommandString += " --landscape-right ";

           // Sets Scale
                switch (ScaleBox.SelectedIndex)
                {
                    case (1):
                        CommandString += "--scale-hack=2 ";
                        break;
                    case (2):
                        CommandString += "--scale-hack=3 ";
                        break;
                    case (3):
                        CommandString += "--scale-hack=4 ";
                        break;
                    default:
                        break;
                }

           // Sets X-tilt offset
                if (string.IsNullOrEmpty(XTiltTxt.Text) == false) CommandString += " --x-tilt-offset=" + System.Convert.ToString(System.Convert.ToInt16(XTiltTxt.Text));
           // Sets Y-tilt offset
                if (string.IsNullOrEmpty(YTiltTxt.Text) == false) CommandString += " --y-tilt-offset=" + System.Convert.ToString(System.Convert.ToInt16(YTiltTxt.Text));
            
           // Sets X-range offset
                if (string.IsNullOrEmpty(XRangeTxt.Text) == false) CommandString += " --x-tilt-range=" + System.Convert.ToString(System.Convert.ToInt16(XRangeTxt.Text));
           // Sets Y-range offset
                if (string.IsNullOrEmpty(YRangeTxt.Text) == false) CommandString += " --y-tilt-range=" + System.Convert.ToString(System.Convert.ToInt16(YRangeTxt.Text));


            // Sets Dead Zone Percentage
            if (string.IsNullOrEmpty(DeadZoneTxt.Text) == false) CommandString += " --deadzone=" + System.Convert.ToString(System.Convert.ToInt16(DeadZoneTxt.Text) / 100);


           // Launches touchHLE
            process.StartInfo.Arguments = "/c " + CommandString;
            process.Start();
        }

        // Attempts to Load Settings from File
        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sline = File.ReadLines(Directory.GetCurrentDirectory() + @"\GSettings.csv").Skip(GameBox.SelectedIndex + 1).Take(1).First();
                string[] settings = sline.Split(',');
                this.OrientationBox.SelectedIndex = System.Convert.ToInt16(settings[1]);
                this.ScaleBox.SelectedIndex = System.Convert.ToInt16(settings[2]);
                this.XTiltTxt.Text = settings[3];
                this.YTiltTxt.Text = settings[4];
                this.XRangeTxt.Text = settings[5];
                this.YRangeTxt.Text = settings[6];
                this.DeadZoneTxt.Text = settings[7];
            }
            catch
            {
                MessageBox.Show("There was a problem loading the default settings.\n" +
                                "Please ensure GSettings.csv is in the current directory.\n" +
                                "If it is, it will need to be redownloaded and replaced.",
                                "Error Accessing Settings File", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        // Attempts to Save Settings to File
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show("This will Overwrite Default Settings for " + GameBox.SelectedValue.ToString() + ".\n" +
                             "Are you sure you want to do this?", "Confirm Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirmation == MessageBoxResult.Yes)
            {
                try
                {
                     string[] newSettings = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\GSettings.csv");
                     newSettings[GameBox.SelectedIndex + 1] =
                     GameBox.SelectedValue.ToString() + ',' + 
                     OrientationBox.SelectedIndex + ',' + 
                     ScaleBox.SelectedIndex + ',' + 
                     XTiltTxt.Text + ',' + 
                     YTiltTxt.Text + ',' + 
                     XRangeTxt.Text + "," + 
                     YRangeTxt.Text + ',' + 
                     DeadZoneTxt.Text;
                     File.WriteAllLines(Directory.GetCurrentDirectory() + @"\GSettings.csv", newSettings);
                     MessageBox.Show("Default Settings have been saved for " + GameBox.SelectedValue.ToString() + ".",
                                     "Successfully Saved Settings", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                     MessageBox.Show("There was a problem saving the default settings.\n" +
                                     "Please ensure GSettings.csv is in the directory.\n" +
                                     "If it is, it will need to be redownloaded and replaced.",
                                     "Error Accessing Settings File", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
