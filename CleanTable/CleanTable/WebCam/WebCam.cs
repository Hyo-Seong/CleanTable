using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace CleanTable
{
    /// <summary>
    /// https://stackoverflow.com/questions/39450823/c-sharp-opencvsharp-3-camcorder-capture-on-windows-form
    /// </summary>
    public class WebCam
    {
        VideoCapture capture;
        Mat frame;
        Bitmap bitmap;

        OpenCvSharp.Size size;

        int isCameraRunning = 0;

        private System.Windows.Controls.Image _ImageControl;
        //private int FrameNumber = 30;


        public void Initialize(ref System.Windows.Controls.Image ImageControl)
        {
            _ImageControl = ImageControl;

            size.Width = 640;
            size.Height = 480;
        }

        public void InitResolution(int width, int height)
        {
            size.Width = width;
            size.Height = height;
        }

        public async void Start()
        {
            isCameraRunning = 1;

            await Task.Run(() =>
            {
                frame = new Mat(size, MatType.CV_8UC3);
                capture = new VideoCapture();
                //TODO: 고해상도 하려면 어떻게 해야 할지 고민해보자
                capture.Set(CaptureProperty.FrameWidth, size.Width);
                capture.Set(CaptureProperty.FrameHeight, size.Height);

                capture.Open(0);

                while (isCameraRunning == 1)
                {
                    capture.Read(frame);
                    bitmap = BitmapConverter.ToBitmap(frame);
                    using (var memory = new MemoryStream())
                    {
                        bitmap.Save(memory, ImageFormat.Jpeg);
                        memory.Position = 0;

                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            _ImageControl.Source = bitmapImage;
                        });

                        //Thread.Sleep(30);
                    }
                }
            });
        }

        public void Stop()
        {
            isCameraRunning = 0;
        }

        public void Continue()
        {
            isCameraRunning = 1;
        }

        public void SaveImageCapture(BitmapSource bitmap, string filename)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.QualityLevel = 100;

            using (FileStream fstream = new FileStream(filename, FileMode.Create))
            {
                encoder.Save(fstream);
                fstream.Close();
            };
        }

        public BitmapSource SaveImageCapture(string filename)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            BitmapSource bitmapSource = GetBitmapSource(bitmap);
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.QualityLevel = 100;

            using (FileStream fstream = new FileStream(filename, FileMode.Create))
            {
                encoder.Save(fstream);
                fstream.Close();
            };

            return bitmapSource;
        }

        public BitmapSource GetBitmapSource(Bitmap bitmap)
        {
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap
            (
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            return bitmapSource;
        }
    }
}
