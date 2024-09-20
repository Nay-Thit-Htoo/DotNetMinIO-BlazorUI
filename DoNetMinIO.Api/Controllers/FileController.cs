using DoNetMinIO.Api.Model.Request;
using DoNetMinIO.Api.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoNetMinIO.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IMinIoService _minIoService;
        public FileController(IMinIoService minIoService)
        {
            _minIoService = minIoService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(string bucketName,string? objectFilePath,IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is missing");

            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var request = new CommonRequestDto() {BucketName=bucketName, ObjectFilePath=objectFilePath,ObjectName=file.FileName,FilePath=filePath };
            await _minIoService.UploadFileAsync(request);

            return Ok("File uploaded successfully.");
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string bucketName,string fileName)
        {

            var destinationPath = Path.Combine(Path.GetTempPath(), fileName);
            var request = new CommonRequestDto() {BucketName=bucketName,ObjectName = fileName, FilePath = destinationPath };
            await _minIoService.DownloadFileAsync(request);

            var fileBytes = System.IO.File.ReadAllBytes(destinationPath);
            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
