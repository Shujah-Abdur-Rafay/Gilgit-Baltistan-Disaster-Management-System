using Microsoft.JSInterop;

namespace GBDMS.Services
{
    public class ToastService
    {
        private readonly IJSRuntime _jsRuntime;

        public ToastService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task ShowToast(string message, string type = "info", int duration = 3000)
        {
            await _jsRuntime.InvokeVoidAsync("showToast", message, type, duration);
        }

        public async Task ShowSuccess(string message)
        {
            await ShowToast(message, "success");
        }

        public async Task ShowError(string message)
        {
            await ShowToast(message, "error");
        }

        public async Task ShowInfo(string message)
        {
            await ShowToast(message, "info");
        }

        public async Task ShowWarning(string message)
        {
            await ShowToast(message, "warning");
        }
    }
}
