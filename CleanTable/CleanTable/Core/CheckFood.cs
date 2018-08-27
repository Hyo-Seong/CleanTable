using CleanTable.Model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CleanTable.Core
{
    public class CheckFood
    {
        public async Task<SnapShot> IsDishEmptyAsync(string fileName)
        {
            SnapShot snapShot = new SnapShot();

            bool bEmpty = await Task.Run(() =>
            {
                var psInfo = new ProcessStartInfo
                {
                    FileName = @"cmd",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,           // cmd창으로 데이터 보내기
                    RedirectStandardError = true,           // cmd창에서 오류 내용 가져오기
                    CreateNoWindow = true,                  // 실행 시 콘솔창을 숨기려면 true로 설정한다.
                };

                var process = Process.Start(psInfo);
                string command = @"python predict.py " + fileName + Environment.NewLine;
                process.StandardInput.Write(command); //../../Picture/temp.jpg
                process.StandardInput.Close();

                string result = process.StandardOutput.ReadToEnd();
                Debug.WriteLine(result);
                string[] temp = result.Split('\n');
                string category = "";
                int accuracy = 0;
                float loadingTime = 0f;

                try
                {
                    category = temp[5].Trim();
                    accuracy = Int32.Parse(temp[6]);
                    loadingTime = float.Parse(temp[7]);

                    snapShot.Category = category;
                    snapShot.Accuracy = accuracy;
                    snapShot.LoadingTime = loadingTime;
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    snapShot.Message = exception.Message;
                    return false;
                }
                if (accuracy >= 80) // 정확도80% 이상이면 신뢰할 수 있는 데이터
                {
                    Debug.WriteLine("[result]  소요시간 : " + loadingTime);
                    if (category.Equals("filled"))
                    {
                        Debug.WriteLine("filled");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine("empty");
                        return false;
                    }
                }
                else
                {
                    Debug.WriteLine("정확하지 않은 사진.");
                }
                return false;
            });

            return snapShot;
        }
    }
}