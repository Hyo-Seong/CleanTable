using CleanTable.Common;
using CleanTable.Core;
using CleanTable.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTable.ViewModel
{
    public class SnapShotViewModel
    {
        public ObservableCollection<SnapShot> Items { get; set; }

        public SnapShotViewModel()
        {
            Items = new ObservableCollection<SnapShot>();
            string directoryPath = SetImageResourcePath();
            //Adds(BindingImages(directoryPath));
        }

        public List<SnapShot> BindingImages(string dirPath)
        {
            List<SnapShot> lstSnapShot = new List<SnapShot>();

            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(dirPath);
            
            foreach(System.IO.FileInfo File in directoryInfo.GetFiles())
            {
                string snapshotId = File.Name.Substring(0, File.Name.Length - 5);
                string snapshotPath = File.FullName;

                lstSnapShot.Add(new SnapShot
                {
                    Path = snapshotPath,
                    Id = snapshotId
                });
            }

            return lstSnapShot;
        }

        public string SetImageResourcePath()
        {
            string now = ComUtil.GetSnapshotDir();
            string resourcePath = ComUtil.GetProcessRootPath() + ComDef.SAVEIMAGEDIR + now;
            return resourcePath;
        }

        public void Adds(List<SnapShot> lstSnapShot)
        {
            foreach (SnapShot item in lstSnapShot)
            {
                Add(item);
            }
        }

        public void Add(SnapShot item)
        {
            Items.Add(item);
        }

        public void Delete(SnapShot image)
        {
            Items.Remove(image);
        }
        
    }
}
