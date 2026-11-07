using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.Web.WebView2.Wpf;
using System.IO;
using System.Threading.Tasks;

namespace FRAMESTEALER
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private async Task CaptureFrame()
        {
            string outputPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "FrameForge_Test.png"
            );

            using (var file = System.IO.File.Create(outputPath))
            {
                await AnimationPreview.CoreWebView2.CapturePreviewAsync(
                    Microsoft.Web.WebView2.Core.CoreWebView2CapturePreviewImageFormat.Png,
                    file
                );
            }

            MessageBox.Show("Frame captured successfully!");
        }


        private async void CaptureFrame_Click(object sender, RoutedEventArgs e)
        {
            if (AnimationPreview.CoreWebView2 == null)
            {
                MessageBox.Show("Please import an HTML animation first.");
                return;
            }

            await CaptureFrame();
        }



        private void ImportHtml_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Select an HTML animation";

            dialog.Filter = "HTML Files (*.html;*.htm)|*.html;*.htm|All Files (*.*)|*.*";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                
                AnimationPreview.Source = new Uri(dialog.FileName);
            }

        }
    }
}