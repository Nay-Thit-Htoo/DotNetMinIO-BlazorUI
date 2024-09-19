namespace DoNetMinIO.Api.Model.Response
{
    public class ResultDto<T>
    {
        public string MessageCode { get; set; } = nameof(Utilities.MessageStatus.Success);
        public string Message { get; set; } = "Success";
        public T? Result { get; set; }
    }
}
