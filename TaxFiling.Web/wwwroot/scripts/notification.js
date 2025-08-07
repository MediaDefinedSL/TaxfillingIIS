toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
  /*  "positionClass": "toast-top-right",*/
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "5000",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

function notifySuccess(title, message) {
    toastr["success"](message, title);
}

function notifyError(title, message) {
    toastr["error"](message, title);
}

function notifyWarning(title, message) {
    toastr["warning"](message, title);
}

function notifyInfo(title, message) {
    toastr["info"](message, title);
}

function notifyConfirm(title, message, onConfirm, onCancel) {
    const toastId = 'confirm-toast-' + new Date().getTime();

    
    const originalOptions = { ...toastr.options };

    
    toastr.options = {
        closeButton: true,
        tapToDismiss: false,
        timeOut: 0,
        extendedTimeOut: 0,
        escapeHtml: false,
        allowHtml: true,
        positionClass: "toast-top-right"
    };

    
    toastr.clear();

    const content = `
        <div id="${toastId}">
            ${message}
            <div class="mt-2 text-end">
                <button type="button" class="btn btn-sm btn-danger me-2" id="${toastId}-yes">Yes</button>
                <button type="button" class="btn btn-sm btn-secondary" id="${toastId}-no">No</button>
            </div>
        </div>
    `;

    const $toastElement = toastr.info(content, title);

    $(document).off('click', `#${toastId}-yes`).on('click', `#${toastId}-yes`, function () {
       
        toastr.clear();
        $(`#${toastId}`).closest('.toast').remove();
        toastr.options = originalOptions;
        if (typeof onConfirm === 'function') onConfirm();
    });

    $(document).off('click', `#${toastId}-no`).on('click', `#${toastId}-no`, function () {
        
        toastr.clear();
        $(`#${toastId}`).closest('.toast').remove();
        toastr.options = originalOptions;
        if (typeof onCancel === 'function') onCancel();
    });
}
