using CleanTable.Common;
using CleanTable.Core;
using Microsoft.Expression.Encoder.Devices;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
        Mat frame = null;
        WriteableBitmap wb;
        CheckFood checkFood = new CheckFood();

        public Collection<EncoderDevice> VideoDevices { get; set; }
        //public Collection<EncoderDevice> AudioDevices { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            try
            {
                //capture = VideoCapture.FromCamera(CaptureDevice.Any, 0);
                //capture.FrameWidth = 640;
                //capture.FrameHeight = 480;
                //capture.Open(0);
                //wb = new WriteableBitmap(capture.FrameWidth, capture.FrameHeight, 96, 96, PixelFormats.Bgr24, null);
                //image.Source = wb;

                InitWebCam();
            }
            catch
            {
                MessageBox.Show("카메라 초기화에 실패하였습니다. 프로그램을 다시 실행해주세요.");
                this.Close();
            }
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            if(frame != null)
            {
                frame.Dispose();
            }
        }

        private void InitWebCam()
        {
            VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            //AudioDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);

            foreach(EncoderDevice device in VideoDevices)
            {
                if(device.Name != "Screen Capture Source")
                {
                    VidDevices.Items.Add(device);
                    break;
                }
            }

            StartPreview();
        }

        private void StartPreview()
        {
            try
            {
                // Display webcam video
                WebcamViewer.StartPreview();
            }
            catch (Microsoft.Expression.Encoder.SystemErrorException ex)
            {
                MessageBox.Show("Device is in use by another application");
            }
        }

        private void StopPreview()
        {
            try
            {
                WebcamViewer.StopPreview();
            }
            catch (Microsoft.Expression.Encoder.SystemErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            //string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";
            string savePath = ComUtil.GetProcessRootPath() + "\\SaveImage";
            DateTime now = new DateTime();
            now = DateTime.Now;

            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            string imagePath = savePath + "\\" + now.ToString("yyyyMMdd_HHmmssfff");
            //filename = imagePath;
            string strResult =  WebcamViewer.TakeSnapshot(imagePath);
            CheckDish(strResult);

            image.Source = new BitmapImage(new Uri(strResult, UriKind.Absolute));

            //frame = Capture();
            //string filename = string.Empty;
            //bool bSaved = SaveImage(frame, ref filename);
            //if(bSaved)
            //{
            //    CheckDish(filename);
            //}
        }
        private async void CheckDish(string filename)
        {
            bool isEmpty = await checkFood.IsDishEmptyAsync(filename);
        }

        private Mat Capture()
        {
            Mat mat = new Mat();

            capture.Read(mat);
            mat = mat.Flip(FlipMode.Y); // 좌우 반전

            WriteableBitmapConverter.ToWriteableBitmap(mat, wb);
            image.Source = wb;

            return mat;
        }

        private bool SaveImage(Mat frame, ref string filename)
        {
            bool isSaved = false;
            string savePath = ComUtil.GetProcessRootPath() + "\\SaveImage";
            DateTime now = new DateTime();
            now = DateTime.Now;

            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            string imagePath = savePath + "\\" + now.ToString("yyyyMMdd_HHmmss") + ".jpg";
            filename = imagePath;

            isSaved = OpenCvSharp.Cv2.ImWrite(imagePath, frame); //경로 지정 후 jpg 형식으로 저장(파일명은 저장일 + 저장 시간)
            return isSaved;
        }

#if false //NOTUSED
        private void PreviewWebcam()
        {
            Mat mat = new Mat();
            Cv2.NamedWindow("1", WindowMode.AutoSize);
            loop = true;

            while (loop)
            {
                if (capture.Read(mat))
                {
                    Cv2.ImShow("1", mat);
                    mat = mat.Flip(FlipMode.Y);
                    WriteableBitmapConverter.ToWriteableBitmap(mat, wb);
                    PreviewImage.Source = wb;
                }

                int c = Cv2.WaitKey(10);
                if (c != -1)
                    break;    
            }
        }
#endif
    }
}