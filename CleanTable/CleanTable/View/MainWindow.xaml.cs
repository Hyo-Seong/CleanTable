using CleanTable.Common;
using CleanTable.Core;
using CleanTable.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
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
        WebCam webcam;
        DataManager dataManager = new DataManager();

        public MainWindow()
        {
            InitializeComponent();
            lvImage.ItemsSource = App.snapShotViewModel.Items;
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            dataManager.LoadData();

            try
            {
                InitCam();
            }
            catch(Exception ex)
            {
                string msg = "카메라 초기화에 실패하였습니다. 프로그램을 다시 실행해주세요\n" + ex.Message;
                MessageBox.Show(msg);

                this.Close();
            }
        }

        private void InitCam()
        {
            webcam = new WebCam();
            webcam.Initialize(ref previewImageCtrl);
            webcam.InitResolution(1024, 768);

            webcam.Start();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            webcam.Start();
        }

        private async void Window_Closing(object sender, EventArgs e)
        {
            webcam.Stop();
            await dataManager.SaveData();
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            EnableRecog(true);
            Capture();
        }

        private async void Capture()
        {
            SnapShot snapShot = new SnapShot();

            string rootPath = dataManager.ValidPath();
            string filename = DateTime.Now.ToString(ComDef.FILENAME_DATETIMEFORMAT);
            string imageFullPath = Path.Combine(rootPath, filename) + ComDef.FILEEXTENSION;

            if (string.IsNullOrEmpty(imageFullPath))
                return;

#if true
            ImageSource source = previewImageCtrl.Source;
            webcam.SaveImageCapture((BitmapSource)source, imageFullPath);

            image.Source = new BitmapImage(new Uri(imageFullPath, UriKind.Absolute));
#else
            BitmapSource bitmapSource = webcam.SaveImageCapture(imageFullPath); 
            image.Source = bitmapSource;
#endif
            snapShot = await CheckDish(imageFullPath);
            snapShot.CaptureDateTime = DateTime.Now.ToString("G");
            snapShot.Id = filename;
            snapShot.Path = imageFullPath;
            
            App.snapShotViewModel.Add(snapShot);

            EnableRecog(false);

            return;
        }

        private async Task<SnapShot> CheckDish(string filename)
        {
            CheckFood checkFood = new CheckFood();
            SnapShot snapShot = await checkFood.IsDishEmptyAsync(filename);

            return snapShot;
        }

        private void EnableRecog(bool isRecog)
        {
            return; //only test
            if(isRecog)
            {
                btnCapture.Content = "인식중...";
                btnCapture.IsEnabled = false;
            }
            else
            {
                btnCapture.Content = "인식";
                btnCapture.IsEnabled = true;
            }
        }

        private void lvImage_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SnapShot snapShot = lvImage.SelectedItem as SnapShot;

            if(snapShot != null)
            {
                try
                {
                    image.Source = new BitmapImage(new Uri(snapShot.Path));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

#if false //NOTUSED

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

        private async void Capture()
        {
            SnapShot snapShot = new SnapShot();

            string now = ComUtil.GetSnapshotDir();
            string savePath = ComUtil.GetProcessRootPath() + ComDef.SAVEIMAGEDIR;

            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            savePath += now;

            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }


            string filename = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            string imagePath = savePath + "\\" + filename;
            string strResult = string.Empty;//WebcamViewer.TakeSnapshot(imagePath);

            if (string.IsNullOrEmpty(strResult))
                return;

            image.Source = new BitmapImage(new Uri(strResult, UriKind.Absolute));
            snapShot = await CheckDish(strResult);
            snapShot.CaptureDateTime = DateTime.Now.ToString("G");
            snapShot.Id = filename;
            snapShot.Path = strResult;
            

            App.snapShotViewModel.Add(snapShot);

            EnableRecog(false);

            return;
        }
#endif

    }
}