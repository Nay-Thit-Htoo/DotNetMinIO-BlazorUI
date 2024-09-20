using DoNetMinIO.Api.Model.Request;
using DoNetMinIO.Api.Model.Response;
using DoNetMinIO.Domain.Model.Response;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Result;
using System.Collections.Generic;

namespace DoNetMinIO.Api.Service
{
    public interface IMinIoService
    {
        Task<ResultDto<string>> TestMinioConnectionAsync();
        Task<ResultDto<IEnumerable<BucketResponseDto>>> GetBuckets();
        Task<ResultDto<String>> CreateBuckets(CommonRequestDto requestDto);
        Task<ResultDto<string>> CheckBucketStatus(CommonRequestDto requestDto);
        Task<ResultDto<string>> RemoveBucket(CommonRequestDto requestDto);
        Task<ResultDto<string>> UploadFileAsync(CommonRequestDto requestDto);
        Task DownloadFileAsync(CommonRequestDto requestDto);
        Task<ResultDto<IAsyncEnumerable<Item>>> GetBucketObjectList(CommonRequestDto requestDto);     
        Task<ResultDto<string>> RemoveBucketObject(CommonRequestDto requestDto);
    }
}
