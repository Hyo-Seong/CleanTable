using System;
using System.Diagnostics;

namespace CleanTable.Core
{
    internal class CheckFood
    {
        public bool IsEmpty()
        {
            ProcessStartInfo cmd = new ProcessStartInfo();
            Process process = new Process();
            cmd.FileName = @"cmd";
            cmd.WindowStyle = ProcessWindowStyle.Hidden;             // cmd창이 숨겨지도록 하기
            cmd.CreateNoWindow = true;                               // cmd창을 띄우지 안도록 하기

            cmd.UseShellExecute = false;
            cmd.RedirectStandardOutput = true;        // cmd창에서 데이터를 가져오기
            cmd.RedirectStandardInput = true;          // cmd창으로 데이터 보내기
            cmd.RedirectStandardError = true;          // cmd창에서 오류 내용 가져오기

            process.EnableRaisingEvents = false;
            process.StartInfo = cmd;
            process.Start();
            var psinfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = @"cmd",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,           // cmd창으로 데이터 보내기
                RedirectStandardError = true,           // cmd창에서 오류 내용 가져오기
                CreateNoWindow = true,                  //실행 시 콘솔창을 숨기려면 true로 설정한다.
            };

            var p = System.Diagnostics.Process.Start(psinfo);
            p.StandardInput.Write(@"python predict.py ../../Picture/temp.jpg" + Environment.NewLine);
            p.StandardInput.Close();

            string result = p.StandardOutput.ReadToEnd();

            string[] temp = result.Split('\n');
            string category = "";
            int accuracy = 0;
            float loadingTime = 0f;
            try
            {
                category = temp[5].Trim();
                accuracy = Int32.Parse(temp[6]);
                loadingTime = float.Parse(temp[7]);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
            if (accuracy >= 80)
            {
                Debug.WriteLine("[result]  소요시간 : " + loadingTime);
                if (category.Equals("cat"))
                {
                    Debug.WriteLine("This is Cat");
                    return true;
                }
                else
                {
                    Debug.WriteLine("This is Dog");
                    return false;
                }
            }
            else
            {
                Debug.WriteLine("정확하지 않은 사진.");
            }
            return false;
        }
    }
}