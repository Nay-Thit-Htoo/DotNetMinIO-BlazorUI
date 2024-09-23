using DoNetMinIO.Api.Model;
using DoNetMinIO.Api.Model.Response;
using DoNetMinIO.Domain.Model.Response;
using DoNetMinIO.Domains.Model.Response;
using DoNetMinIO.UI.Components.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using static MudBlazor.CategoryTypes;

namespace DoNetMinIO.UI.Components.Pages
{
    public partial class BucketObject : Microsoft.AspNetCore.Components.ComponentBase
    {
        #region Injects
        [Inject]
        public required IBucketService bucketService { get; set; }

        [Inject]
        public required MudBlazor.ISnackbar Snackbar { get; set; }

        [Inject]
        public required IJSRuntime JS { get; set; }
        #endregion

        #region Properties
        IEnumerable<BucketResponseDto> buckets { get; set; }

        IEnumerable<BucketObjectResponseDto> bucketObjects { get; set; }

        private string txtObjectNewFilePath { get; set; }

        BucketResponseDto selectedBucket { get; set; }

        private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
        private string _dragClass = DefaultDragClass;
        private MudFileUpload<IReadOnlyList<IBrowserFile>>? _fileUpload;
        private MultipartFormDataContent uploadedFileContent;
        private string? uploadedFileName;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GetBucketList();
            }
        }

        #region Private
        private async void GetBucketList()
        {
            var result = await bucketService.GetBuckets();
            if (result.MessageCode == nameof(Utilities.MessageStatus.Success))
            {
                buckets = result.Result;
                StateHasChanged();
            }
        }
        private async Task ClearAsync()
        {
            await (_fileUpload?.ClearAsync() ?? Task.CompletedTask);
            uploadedFileContent =new MultipartFormDataContent();
            StateHasChanged();
            ClearDragClass();
        }

        private Task OpenFilePickerAsync()
            => _fileUpload?.OpenFilePickerAsync() ?? Task.CompletedTask;
        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();           
            var files = e.GetMultipleFiles();

            //Prepare to upload file content
            uploadedFileContent = new MultipartFormDataContent();
            var fileContent = new StreamContent(files.Last().OpenReadStream());
            uploadedFileContent.Add(fileContent, "file", files.Last().Name);
            uploadedFileName=files.Last().Name;
            StateHasChanged();
        }

        private void SetDragClass()
            => _dragClass = $"{DefaultDragClass} mud-border-primary";

        private void ClearDragClass()
            => _dragClass = DefaultDragClass;

        private async void GetBucketObjectFiles()
        {
            var result = await bucketService.GetBucketsObjectList(selectedBucket.Name, String.Empty);
            if (result.MessageCode == nameof(Utilities.MessageStatus.Success))
            {
                bucketObjects = result.Result;
                StateHasChanged();
            }
        }
        #endregion

        #region DropDown
        private async void BucketDropDown_Change(BucketResponseDto bucketResponseDto)
        {
            if(bucketResponseDto is null ||  String.IsNullOrEmpty(bucketResponseDto.Name))
            {
                return;
            }

            selectedBucket=bucketResponseDto as BucketResponseDto;
            GetBucketObjectFiles();
        }
        #endregion

        #region Button Click
        private async void DownloadBtn_Click(BucketObjectResponseDto downObject)
        {
            var result = await bucketService.DownloadBucketFile(downObject.BucketName, downObject.ObjectFileName);
            if (result.MessageCode == nameof(Utilities.MessageStatus.Success))
            {
                var fileBase64 = Convert.ToBase64String(result.Result);
                var fileUrl = $"data:application/octet-stream;base64,{fileBase64}";

                // Trigger file download via JSInterop
                await JS.InvokeVoidAsync("downloadFileFromUrl", downObject.ObjectFileName, fileUrl);
            }

            Snackbar.Add(result.Message, (result.MessageCode == nameof(Utilities.MessageStatus.Success)) ? Severity.Success : Severity.Error);

        }

        private async void UploadBtn_Click()
        {
            if (selectedBucket is null || String.IsNullOrEmpty(selectedBucket.Name))
            {
                Snackbar.Add("Please choose Bucket Name", Severity.Warning);
                return;
            }

            if (uploadedFileContent is null)
            {
                Snackbar.Add("Please choose file", Severity.Warning);
                return;
            }

            var result = await bucketService.UploadBucketFile(selectedBucket.Name, txtObjectNewFilePath, uploadedFileContent);
            Snackbar.Add(result.Message, (result.MessageCode == nameof(Utilities.MessageStatus.Success)) ? Severity.Success : Severity.Error);
            uploadedFileName = String.Empty;
            GetBucketObjectFiles();
        }
        private async void DeleteBucketObjectBtnClick(string bucketName, string objectName)
        {
            if (String.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }
            if (String.IsNullOrEmpty(objectName))
            {
                throw new ArgumentNullException(nameof(objectName));
            }

            var result = await bucketService.DeleteBucketObjects(bucketName, objectName);
            if (result.MessageCode == nameof(Utilities.MessageStatus.Success))
            {
                Snackbar.Add(result.Message, Severity.Success);
                GetBucketObjectFiles();
            }
            else
            {
                Snackbar.Add(result.Message, Severity.Error);
            }
        }
        #endregion

    }
}
