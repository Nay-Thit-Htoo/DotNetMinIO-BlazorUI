using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DoNetMinIO.Domains.Model.Response
{
    public class BucketObjectResponseDto
    {
        public string BucketName { get; set; }
        public string ObjectFileName { get; set; }
        public string ModifiedDate { get; set; }
        public long FileSize { get; set; }   
        public string ContentType { get; set; }
    }
}
