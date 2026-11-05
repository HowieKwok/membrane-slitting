using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Frame_Core
{
    public class ImageSavePathInfo
    {
        public string ScreenshotPath { get; set; } = "";

        public string ScreenshotFileName { get; set; } = "";

        public ImageSaveFormat ScreenshotFileFormat { get; set; } = ImageSaveFormat.JPEG20;
    
        public bool IsHighDefinitionCapture { get; set; } = false;

        public bool IsScreenshot { get; set; } = false;

        public string Path { get; set; } = "";

        public string FileName { get; set; } = "";

        public ImageSaveFormat FileFormat { get; set; } = ImageSaveFormat.JPEG20;

        public bool IsShow { get; set; } = false;

        public bool IsSave { get; set; } = false;
     
    }
}
