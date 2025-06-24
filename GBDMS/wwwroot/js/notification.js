window.showToastr = function (type, message) {
    // Use our custom green toast system instead of toastr
    if (type == "success") {
        window.showToast(message, "success");
    }
    if (type == "error") {
        window.showToast(message, "error");
    }
    if (type == "warning") {
        window.showToast(message, "warning");
    }
    if (type == "info") {
        window.showToast(message, "info");
    }
}