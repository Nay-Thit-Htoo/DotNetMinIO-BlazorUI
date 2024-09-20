using DoNetMinIO.Api.Model.Request;
using DoNetMinIO.Api.Model.Response;
using DoNetMinIO.Domain.Model.Response;
using Newtonsoft.Json;
namespace DoNetMinIO.UI.Components.Service
{
    public class BucketService :IBucketService
    {
        private readonly HttpClient httpClient;    
        public BucketService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ResultDto<IEnumerable<BucketResponseDto>>> GetBuckets()
        {
            return await httpClient.GetFromJsonAsync<ResultDto<IEnumerable<BucketResponseDto>>>("api/Bucket/GetBuckets");
        }

        public async Task<ResultDto<string>> CreateBuckets(string bucketName)
        {            
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/Bucket/CreateBucket/?bucketName="+bucketName,bucketName);      
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultDto<string>>(responseContent);
            return result!;
                     
        }

        public async Task<ResultDto<string>> DeleteBuckets(string bucketName)
        {
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/Bucket/RemoveBucket?bucketName=" + bucketName, bucketName);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultDto<string>>(responseContent);
            return result!;

        }

    }
}
