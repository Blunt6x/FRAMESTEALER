using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using System.Diagnostics;
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

        private async Task CaptureSequence()
        {
            int fps = 10;
            int duration = 3;
            int totalFrames = fps * duration;

            string outputFolder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "FrameForge_Frames"
            );

            Directory.CreateDirectory(outputFolder);

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < totalFrames; i++)
            {
                int targetTime = i * 1000 / fps;

                int remainingTime = targetTime -
                    (int)stopwatch.ElapsedMilliseconds;

                if (remainingTime > 0)
                {
                    await Task.Delay(remainingTime);
                }

                string framePath = System.IO.Path.Combine(
                    outputFolder,
                    $"frame_{i:D5}.png"
                );

                using (var file = File.Create(framePath))
                {
                    await AnimationPreview.CoreWebView2.CapturePreviewAsync(
                        Microsoft.Web.WebView2.Core.CoreWebView2CapturePreviewImageFormat.Png,
                        file
                    );
                }
            }

            stopwatch.Stop();

            MessageBox.Show(
                $"Captured {totalFrames} frames!\nSaved to: {outputFolder}"
            );
        }

        private async void CaptureSequence_Click(object sender, RoutedEventArgs e)
        {
            if (AnimationPreview.CoreWebView2 == null)
            {
                MessageBox.Show("Please import an HTML animation first.");
                return;
            }

            await CaptureSequence();
        }



        private async Task ExportVideo()
        {
            string inputFolder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "FrameForge_Frames"
            );

            string outputPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "FrameForge_Test.mp4"
            );

            string firstFrame = System.IO.Path.Combine(
                inputFolder,
                "frame_00000.png"
            );

            if (!File.Exists(firstFrame))
            {
                MessageBox.Show("No captured frames found!");
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = @"C:\Users\User\AppData\Local\Microsoft\WinGet\Packages\Gyan.FFmpeg.Shared_Microsoft.Winget.Source_8wekyb3d8bbwe\ffmpeg-9.0.2-full_build-shared\bin\ffmpeg.exe",
                WorkingDirectory = inputFolder,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            startInfo.ArgumentList.Add("-y");
            startInfo.ArgumentList.Add("-framerate");
            startInfo.ArgumentList.Add("10");
            startInfo.ArgumentList.Add("-i");
            startInfo.ArgumentList.Add("frame_%05d.png");
            startInfo.ArgumentList.Add("-c:v");
            startInfo.ArgumentList.Add("libx264");
            startInfo.ArgumentList.Add("-pix_fmt");
            startInfo.ArgumentList.Add("yuv420p");
            startInfo.ArgumentList.Add("-movflags");
            startInfo.ArgumentList.Add("+faststart");
            startInfo.ArgumentList.Add(outputPath);

            using Process process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();

            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                MessageBox.Show(
                    "Video exported successfully!\n\n" + outputPath
                );
            }
            else
            {
                MessageBox.Show("FFmpeg export failed:\n\n" + error);
            }
        }


        private async void ExportVideo_Click(object sender, RoutedEventArgs e)
        {
            await ExportVideo();
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