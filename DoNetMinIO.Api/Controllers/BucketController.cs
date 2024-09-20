using DoNetMinIO.Api.Model.Request;
using DoNetMinIO.Api.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoNetMinIO.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketController : ControllerBase
    {
        private readonly IMinIoService _minIoService;
        public BucketController(IMinIoService minIoService)
        {
            _minIoService = minIoService;
        }


        [HttpGet("GetBuckets")]
        public async Task<IActionResult> Buckets()
        {              
            return Ok(await _minIoService.GetBuckets());
        }

        [HttpPost("CreateBucket")]
        public async Task<IActionResult> CreateBucket(string bucketName)
        {
            var request=new CommonRequestDto() { BucketName = bucketName };
            return Ok(await _minIoService.CreateBuckets(request));
        }

        [HttpPost("GetObjectListByBucketName")]
        public async Task<IActionResult> GetObjectListByBucketName(string bucketName,string? filePrefixName)
        {
            var request = new CommonRequestDto() { BucketName = bucketName,ObjectPrefixName= filePrefixName };
            return Ok(await _minIoService.GetBucketObjectList(request));
        }

        [HttpPost("CheckBucketStatus")]
        public async Task<IActionResult> CheckBucketStatus(string bucketName)
        {
            var request = new CommonRequestDto() { BucketName = bucketName };
            return Ok(await _minIoService.CheckBucketStatus(request));
        }

        [HttpPost("RemoveBucket")]
        public async Task<IActionResult> RemoveBucket(string bucketName)
        {
            var request = new CommonRequestDto() { BucketName = bucketName };
            return Ok(await _minIoService.RemoveBucket(request));
        }
    }
}
