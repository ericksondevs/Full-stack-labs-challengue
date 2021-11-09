using Shyjus.BrowserDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork.Configuration
{
    public class Browser : IBrowser
    {
        public string DeviceType { get; set; }

        public string Name { get; set; }

        public string OS { get; set; }

        public string Version { get; set; }
    }
}
