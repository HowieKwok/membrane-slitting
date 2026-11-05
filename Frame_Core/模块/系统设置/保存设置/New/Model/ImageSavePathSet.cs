using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{

    public class ImageSavePathSet
    {
        private string _Path = "";
        public string Path
        {
            get => _Path;
            set { _Path = value; }
        }


        private string _FileName = "";
        public string FileName
        {
            get => _FileName;
            set { _FileName = value; }
        }


        private string _SuffixName = "";
        public string SuffixName
        {
            get => _SuffixName;
            set { _SuffixName = value; }
        }
    }
}
