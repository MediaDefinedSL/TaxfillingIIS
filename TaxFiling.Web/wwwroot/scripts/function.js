$.fn.setButtonDisabled = function (disable) {
    return this.each(function () {
        $(this).prop('disabled', disable);
    });
};
