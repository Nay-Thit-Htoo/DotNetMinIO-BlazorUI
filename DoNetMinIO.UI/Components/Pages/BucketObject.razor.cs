using DoNetMinIO.Api.Model;
using DoNetMinIO.Domain.Model.Response;
using DoNetMinIO.UI.Components.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace DoNetMinIO.UI.Components.Pages
{
    public partial class BucketObject : Microsoft.AspNetCore.Components.ComponentBase
    {
        #region Injects
        [Inject]
        public required IBucketService bucketService { get; set; }

        [Inject]
        public required MudBlazor.ISnackbar Snackbar { get; set; }
        #endregion

        #region Properties
        IEnumerable<BucketResponseDto> buckets { get; set; }

        BucketResponseDto selectedBucket { get; set; }

        private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
        private string _dragClass = DefaultDragClass;
        private readonly List<string> _fileNames = new();
        private MudFileUpload<IReadOnlyList<IBrowserFile>>? _fileUpload;

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
            _fileNames.Clear();
            ClearDragClass();
        }

        private Task OpenFilePickerAsync()
            => _fileUpload?.OpenFilePickerAsync() ?? Task.CompletedTask;

        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();
            var files = e.GetMultipleFiles();
            foreach (var file in files)
            {
                _fileNames.Add(file.Name);
            }
        }

        private void Upload()
        {
            // Upload the files here
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("TODO: Upload your files!");
        }

        private void SetDragClass()
            => _dragClass = $"{DefaultDragClass} mud-border-primary";

        private void ClearDragClass()
            => _dragClass = DefaultDragClass;


        #region DropDown
        private void BucketDropDown_Change(BucketResponseDto bucketResponseDto)
        {

        }
        #endregion


    }
}
