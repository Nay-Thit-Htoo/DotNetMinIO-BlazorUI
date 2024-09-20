using DoNetMinIO.Api.Model.Request;
using DoNetMinIO.Api.Model.Response;
using DoNetMinIO.Domain.Model.Response;
using DoNetMinIO.Domains.Model.Response;


namespace DoNetMinIO.UI.Components.Service
{
    public interface IBucketService
    {
        Task<ResultDto<String>> CreateBuckets(string bucketName);
        Task<ResultDto<IEnumerable<BucketResponseDto>>> GetBuckets();
        Task<ResultDto<string>> DeleteBuckets(string bucketName);
        Task<ResultDto<IEnumerable<BucketObjectResponseDto>>> GetBucketsObjectList(string bucketName,string? objectFilePrefixName);
        Task<ResultDto<string>> UploadBucketFile(string bucketName,string? objBucketNewFilePath,MultipartFormDataContent fileContent);

    }
}
