using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTable.Core
{
    public class RecogInfo
    {
        public bool isEmpty { get; set; }
        public string category { get; set; }
        public int accuracy { get; set; }
        public float loadingTime { get; set; }
        public string message { get; set; }
    }
}
