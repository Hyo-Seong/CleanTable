using CleanTable.Common;
using CleanTable.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CleanTable
{
    public class DataManager
    {
        public void LoadData()
        {
            if (System.IO.File.Exists(ComDef.LOGFILENAME))
            {
                StreamReader sr = new StreamReader(ComDef.LOGFILENAME, Encoding.GetEncoding("UTF-8"));
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
            else
            {
                return;
            }
        }

        public async Task SaveData()
        {
            string filePath = Path.Combine(ComUtil.GetProcessRootPath(), ComDef.LOGFILENAME);
            await Task.Run(() =>
            {
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

                            sw.Close();
                            fs.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        string msg = "인식 기록 저장 시 에러가 발생하였습니다\n" + ex.Message;
                        MessageBox.Show(msg);
                    });
                }
            });

            return;
        }

        public string ValidPath()
        {
            string strTodayDir = ComUtil.GetSnapshotDir();
            string savePath = ComUtil.GetProcessRootPath() + ComDef.SAVEIMAGEDIR;

            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            savePath += strTodayDir;

            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            return savePath;
        }
    }
}
