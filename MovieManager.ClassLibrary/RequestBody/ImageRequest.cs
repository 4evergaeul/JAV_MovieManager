using System;
using System.Collections.Generic;
using System.Text;

namespace MovieManager.ClassLibrary.RequestBody
{
    public class ImageRequest
    {
        /// <summary>
        /// 0: poster, 1: fanart
        /// 10: actor thumbsnail, 11: actor large portrait
        /// </summary>
        public int ImageType { get; set; }
        public string Id { get; set; }
    }
}
