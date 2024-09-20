
using DoNetMinIO.Api.Model;
using DoNetMinIO.Api.Model.Request;
using DoNetMinIO.Api.Model.Response;
using DoNetMinIO.Domain.Model.Response;
using DoNetMinIO.Domains.Model.Response;
using Microsoft.AspNetCore.Mvc.Formatters;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Result;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace DoNetMinIO.Api.Service
{
    public class MinIoService : IMinIoService
    {
        private readonly IMinioClient _minioClient;
        private readonly IConfiguration _configuration;   
        public MinIoService(IConfiguration configuration,IMinioClient minioClient)
        {            
            _configuration = configuration;
            _minioClient = minioClient;           
        }

        public async Task<ResultDto<string>> TestMinioConnectionAsync()
        {
            ResultDto<string> resultDto = new ResultDto<string>();           
            try
            {
                var bucketLst = await _minioClient.ListBucketsAsync();
                resultDto.Message = "Successfully Connected with MinIO";
                goto Result;
            }
            catch (MinioException e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = $"MinIO Exception: {e.Message}";
                goto Result;
            }
            catch (Exception e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = $"General Exception: {e.Message}";
                goto Result;
            }
       
        Result:
            return resultDto;
        }

        public async Task<ResultDto<IEnumerable<BucketResponseDto>>> GetBuckets()
        {
            ResultDto<IEnumerable<BucketResponseDto>> resultDto = new ResultDto<IEnumerable<BucketResponseDto>>();
            try
            {                
                var bucketLst = await _minioClient.ListBucketsAsync();
                if (bucketLst is null)
                {
                    resultDto.MessageCode = nameof(Utilities.MessageStatus.NotFound);
                    resultDto.Message = "There is no any buckets!";
                }                     
                else
                    resultDto.Result = bucketLst.Buckets.Select((x)=> new BucketResponseDto( x.Name,x.CreationDate));
                goto Result;
            }
            catch (Exception e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = e.Message;
                goto Result;
            }

        Result:
            return resultDto;
        }

        public async Task<ResultDto<String>> CreateBuckets(CommonRequestDto requestDto)
        {
            if (string.IsNullOrEmpty(requestDto.BucketName))
                throw new ArgumentNullException(nameof(requestDto.BucketName));

            ResultDto<String> resultDto = new ResultDto<String>();        

            try
            {
                bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(requestDto.BucketName));
                if (!bucketExists)
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(requestDto.BucketName));
                    resultDto.Result = $"{requestDto.BucketName} Successfully Created!";
                    goto Result;
                }
                else
                {
                    resultDto.MessageCode= nameof(Utilities.MessageStatus.Error);
                    resultDto.Message = $"{requestDto.BucketName} Already Exist!";
                    goto Result;
                }              
            }
            catch (MinioException e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = e.Message;
                goto Result;
            }

        Result:
            return resultDto;
        }

        public async Task DownloadFileAsync(CommonRequestDto requestDto)
        {
            if (string.IsNullOrEmpty(requestDto.BucketName))
                throw new ArgumentNullException(nameof(requestDto.BucketName));

            if (string.IsNullOrEmpty(requestDto.ObjectName))
                throw new ArgumentNullException(nameof(requestDto.ObjectName));

            if (string.IsNullOrEmpty(requestDto.FilePath))
                throw new ArgumentNullException(nameof(requestDto.FilePath));

            try
            {
                await _minioClient.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(requestDto.BucketName)
                    .WithObject(requestDto.ObjectName)
                    .WithFile(requestDto.FilePath));
            }
            catch (MinioException e)
            {
                Console.WriteLine($"[MinIO] Error: {e.Message}");
            }
        }

        public async Task<ResultDto<string>> UploadFileAsync(CommonRequestDto requestDto)
        {
            if (string.IsNullOrEmpty(requestDto.BucketName))
                throw new ArgumentNullException(nameof(requestDto.BucketName));

            if (string.IsNullOrEmpty(requestDto.ObjectName))
                throw new ArgumentNullException(nameof(requestDto.ObjectName));

            if (string.IsNullOrEmpty(requestDto.FilePath))
                throw new ArgumentNullException(nameof(requestDto.FilePath));

            ResultDto<string>  result=new ResultDto<string>();

            try
            {
                bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(requestDto.BucketName));
                if (!bucketExists)
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(requestDto.BucketName));
                }

                if (String.IsNullOrEmpty(requestDto.ObjectFilePath))
                    await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(requestDto.BucketName)
                    .WithObject(requestDto.ObjectName)
                    .WithFileName(requestDto.FilePath));
                else
                    await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(requestDto.BucketName)
                    .WithObject($"{requestDto.ObjectFilePath}/{requestDto.ObjectName}")
                    .WithFileName(requestDto.FilePath));
            }
            catch (MinioException e)
            {
                result.MessageCode = nameof(Utilities.MessageStatus.Error);
                result.Message = e.Message;
            }

            Result:
             return result;
        }

        public async Task<ResultDto<IEnumerable<BucketObjectResponseDto>>>GetBucketObjectList(CommonRequestDto requestDto)
        {
            if (string.IsNullOrEmpty(requestDto.BucketName))
                throw new ArgumentNullException(nameof(requestDto.BucketName));            

            ResultDto<IEnumerable<BucketObjectResponseDto>> resultDto = new ResultDto<IEnumerable<BucketObjectResponseDto>>();
            try
            {
                var listArgs = (String.IsNullOrEmpty(requestDto.ObjectPrefixName)) ?
                    new ListObjectsArgs()
                .WithBucket(requestDto.BucketName)
                .WithRecursive(true) :
                 new ListObjectsArgs()
                .WithBucket(requestDto.BucketName)
                .WithPrefix(requestDto.ObjectPrefixName)
                .WithRecursive(true);

                var bucketObjectList = _minioClient.ListObjectsEnumAsync(listArgs);
                if (bucketObjectList is null)
                {
                    resultDto.MessageCode = nameof(Utilities.MessageStatus.NotFound);
                    resultDto.Message = $"There is no any objects for {requestDto.BucketName}!";
                }
                else
                {
                    List<BucketObjectResponseDto> objectLst =new List<BucketObjectResponseDto>();
                     await foreach (Item item in bucketObjectList)
                    {
                        objectLst.Add(new BucketObjectResponseDto()
                        {
                            BucketName = requestDto.BucketName,
                            ObjectFileName=item.Key,
                            ContentType = item.ContentType,
                            FileSize =(long)item.Size,
                            ModifiedDate = item.LastModifiedDateTime.ToString()!
                        });
                    }
                     resultDto.Result = objectLst;
                }            
                goto Result;
            }
            catch (Exception e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = e.Message;
                goto Result;
            }

        Result:
            return resultDto;
        }

        public async Task<ResultDto<string>> CheckBucketStatus(CommonRequestDto requestDto)
        {
            if (string.IsNullOrEmpty(requestDto.BucketName))
                throw new ArgumentNullException(nameof(requestDto.BucketName));

            ResultDto<string> resultDto = new ResultDto<string>();
            try
            {
                bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(requestDto.BucketName));
                resultDto.Message = (bucketExists) ? "Already Exist" : "Not Exist!";
                goto Result;
            }
            catch (MinioException e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = $"MinIO Exception: {e.Message}";
                goto Result;
            }
            catch (Exception e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = $"General Exception: {e.Message}";
                goto Result;
            }

        Result:
            return resultDto;
        }

        public async Task<ResultDto<string>> RemoveBucket(CommonRequestDto requestDto)
        {
            ResultDto<string> resultDto = new ResultDto<string>();
            if(string.IsNullOrEmpty(requestDto.BucketName))
                 throw new ArgumentNullException(nameof(requestDto.BucketName));

            try
            {
                await _minioClient.RemoveBucketAsync(
                  new RemoveBucketArgs()
                      .WithBucket(requestDto.BucketName));
                resultDto.Message = $"{requestDto.BucketName} Successfully Deleted!";
                goto Result;
            }
            catch (MinioException e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = $"MinIO Exception: {e.Message}";
                goto Result;
            }
            catch (Exception e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = $"General Exception: {e.Message}";
                goto Result;
            }

        Result:
            return resultDto;
        }

        public Task<ResultDto<string>> RemoveBucketObject(CommonRequestDto requestDto)
        {
            ResultDto<string> resultDto = new ResultDto<string>();
            if (string.IsNullOrEmpty(requestDto.BucketName))
                throw new ArgumentNullException(nameof(requestDto.BucketName));

            if (string.IsNullOrEmpty(requestDto.ObjectName))
                throw new ArgumentNullException(nameof(requestDto.ObjectName));

            try
            {
                var args = new RemoveObjectArgs()
                 .WithBucket(requestDto.BucketName)
                 .WithObject(requestDto.ObjectName);
                resultDto.Message = $"{requestDto.BucketName}/{requestDto.ObjectName} Successfully Deleted!";
                goto Result;
            }
            catch (MinioException e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = $"MinIO Exception: {e.Message}";
                goto Result;
            }
            catch (Exception e)
            {
                resultDto.MessageCode = nameof(Utilities.MessageStatus.Error);
                resultDto.Message = $"General Exception: {e.Message}";
                goto Result;
            }

        Result:
            return Task.FromResult(resultDto);
        }

    }
}
