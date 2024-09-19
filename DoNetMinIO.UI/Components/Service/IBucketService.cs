using DoNetMinIO.Api.Model.Request;
using DoNetMinIO.Api.Model.Response;
using DoNetMinIO.Domain.Model.Response;


namespace DoNetMinIO.UI.Components.Service
{
    public interface IBucketService
    {
        Task<ResultDto<String>> CreateBuckets(string bucketName);
        Task<ResultDto<IEnumerable<BucketResponseDto>>> GetBuckets();
    }
}
