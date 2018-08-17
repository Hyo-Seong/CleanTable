using CleanTable.Common;
using CleanTable.Core;
using CleanTable.Model;
using Microsoft.Expression.Encoder.Devices;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        CheckFood checkFood = new CheckFood();

        public Collection<EncoderDevice> VideoDevices { get; set; }
        //public Collection<EncoderDevice> AudioDevices { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            lvImage.ItemsSource = App.snapShotViewModel.Items;
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            LoadData();

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

        private async void Window_Closing(object sender, EventArgs e)
        {
            //TODO: csv 저장
            await SaveData();
        }

        private void LoadData()
        {
            if (System.IO.File.Exists("Snapshot.csv"))
            {
                StreamReader sr = new StreamReader("Snapshot.csv", Encoding.GetEncoding("UTF-8"));
                while (!sr.EndOfStream)
                {
                    string data = sr.ReadLine();
                    string[] dataArray = data.Split(ComDef.SEPARATECHAT);

                    SnapShot snapShot = new SnapShot
                    {
                        Id = dataArray[0],
                        CaptureDateTime = dataArray[1],
                        Path = dataArray[2],
                        Category = dataArray[3],
                        IsEmpty = bool.Parse(dataArray[4]),
                        Accuracy = int.Parse(dataArray[5]),
                        Message = dataArray[6],
                        LoadingTime = float.Parse(dataArray[7])
                    };

                    App.snapShotViewModel.Add(snapShot);
                }
            }
            else return;
        }

        private async Task SaveData()
        {
            await Task.Run(() =>
            {
            string filePath = ComUtil.GetProcessRootPath() + "\\Snapshot.csv";
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                        {
                            List<SnapShot> lstSnapShot = new List<SnapShot>(App.snapShotViewModel.Items);

                            StringBuilder dat = new StringBuilder();
                            foreach (SnapShot item in lstSnapShot)
                            {
                                dat.Append(item.Id + ComDef.SEPARATECHAT.ToString());
                                dat.Append(item.CaptureDateTime + ComDef.SEPARATECHAT.ToString());
                                dat.Append(item.Path + ComDef.SEPARATECHAT.ToString());
                                dat.Append(item.Category + ComDef.SEPARATECHAT.ToString());
                                dat.Append(item.IsEmpty + ComDef.SEPARATECHAT.ToString());
                                dat.Append(item.Accuracy + ComDef.SEPARATECHAT.ToString());
                                dat.Append(item.Message + ComDef.SEPARATECHAT.ToString());
                                dat.Append(item.LoadingTime + Environment.NewLine);

                            }

                            string strData = dat.ToString();
                            sw.Write(strData);

                            //string line = string.Join(",", App.snapShotViewModel.Items);
                            sw.Close();
                            fs.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message);
                    });
                    
                }
            }); 

            return;
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
            EnableRecog(true);
            Capture();
            //Capture(strResult);

            //string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";
            
            //frame = Capture();
            //string filename = string.Empty;
            //bool bSaved = SaveImage(frame, ref filename);
            //if(bSaved)
            //{
            //    CheckDish(filename);
            //}
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
            string strResult = WebcamViewer.TakeSnapshot(imagePath);

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

        private async Task<SnapShot> CheckDish(string filename)
        {
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
                image.Source = new BitmapImage(new Uri(snapShot.Path));
            }
        }
    }
}