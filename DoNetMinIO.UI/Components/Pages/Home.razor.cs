using DoNetMinIO.Api.Model;
using DoNetMinIO.Api.Model.Request;
using DoNetMinIO.Domain.Model.Response;
using DoNetMinIO.UI.Components.Service;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DoNetMinIO.UI.Components.Pages
{
    public partial class Home : Microsoft.AspNetCore.Components.ComponentBase
    {

        [Inject]
        public required IBucketService bucketService { get; set; }

        [Inject]
        public required MudBlazor.ISnackbar Snackbar { get; set; }


        #region Properties
        IEnumerable<BucketResponseDto> buckets { get; set; }
        private string txtBucketName { get; set; }

        private MudTextField<string> txtFieldBucketName;
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

        private async void CreateBtnClick()
        {
           if(String.IsNullOrEmpty(txtBucketName))
            {
                await txtFieldBucketName.FocusAsync();
                return;
            }

            var result = await bucketService.CreateBuckets(txtBucketName);
            if(result.MessageCode== nameof(Utilities.MessageStatus.Success))
            {
                Snackbar.Add(result.Message, Severity.Success);
                txtBucketName=String.Empty;
                GetBucketList();
            }
            else
            {
                Snackbar.Add(result.Message, Severity.Error);
            }
        }
    }
}
