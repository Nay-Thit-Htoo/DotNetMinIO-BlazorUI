namespace DoNetMinIO.Api.Model.Request
{
    public class CommonRequestDto
    {
        public string BucketName { get; set; }
        public string ObjectName { get; set; }
        
        public string? ObjectFilePath { get; set; }

        public string FilePath { get; set; }   
        public string? ObjectPrefixName { get; set; }    

    }
}
