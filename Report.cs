using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoLE_Gauss
{
    internal class Report
    {
        public int size { get; set; }
        public int Threads { get; set; }
        public TimeSpan time { get; set; }
        public Report(int size, int Threads, TimeSpan time)
        {
            this.size = size;
            this.Threads = Threads;
            this.time = time;
        }
    }
}
