using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTable.Common
{
    public class ComUtil
    {
        public static string GetProcessRootPath()
        {
            string path = string.Empty;

            path = Directory.GetCurrentDirectory();
            return path;
        }

        public static string GetSnapshotDir()
        {
            string now = "\\";
            now += DateTime.Now.ToString("yyyyMMdd");

            return now;
        }
    }
}
