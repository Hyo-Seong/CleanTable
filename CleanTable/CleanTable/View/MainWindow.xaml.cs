using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CleanTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        VideoCapture capture;
        Mat frame;
        WriteableBitmap wb;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            try
            {
                capture = VideoCapture.FromCamera(CaptureDevice.Any, 0);
                capture.FrameWidth = 480;
                capture.FrameHeight = 320;
                capture.Open(0);
                wb = new WriteableBitmap(capture.FrameWidth, capture.FrameHeight, 96, 96, PixelFormats.Bgr24, null);
                image.Source = wb;
            }

            catch
            {
                MessageBox.Show("카메라 초기화에 실패하였습니다. 프로그램을 다시 실행해주세요.");
                this.Close();
            }
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            frame.Dispose();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            frame = new Mat();

            capture.Read(frame);
            frame = frame.Flip(FlipMode.Y); // 좌우 반전

            // var process = Process.GetCurrentProcess();
            // string savePath = process.MainModule.FileName + "\\SaveImage";

            string savePath = Directory.GetCurrentDirectory() + "\\SaveImage";

            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            WriteableBitmapConverter.ToWriteableBitmap(frame, wb);


            DateTime now = new DateTime();
            now = DateTime.Now;
            

            string imagePath = savePath + "\\" + now.ToString("yyyyMMdd_HHmmss") + ".jpg"; 

            OpenCvSharp.Cv2.ImWrite(imagePath, frame); // 경로 지정 후 jpg 형식으로 저장(파일명은 저장일 + 저장 시간)

            image.Source = wb;


        }
    }
}