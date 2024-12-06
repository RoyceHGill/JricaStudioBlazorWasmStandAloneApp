using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Constants
{
    public struct FileResources
    {
        const string imagesBasePath = "/Images" ;
        const string accessor = "/";

        const string serviceImaageFileName = "/ServiceImages";

        const string productImageFileName = "/ProductImages";

        public const string serviceImageFilePath = imagesBasePath + serviceImaageFileName + accessor;

        public const string productImageFilePath = imagesBasePath + productImageFileName + accessor;

        public const int maxuploadSize = 5000 * 1024;
    }
}
