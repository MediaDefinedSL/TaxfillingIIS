$(function () {
    //-----------------------------Partner Income
    $(document).off("click", "#btnPartnerInvestmentSubmit").on("click", "#btnPartnerInvestmentSubmit", function () {

        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collecting values from your form
        let selfOnlineInvestmentPartnerId = $("#hiddenInvestmentPartnerIncomeId").val();   // hidden field for edit/update
        let totalInvestment = $("#txtTotalInvestment").val();
        let activityCode = $("#txtActivityCodePartnership").val();
        let partnershipName = $("#txtPartnershipName").val();
        let partnershipTin = $("#txtPartnershipTin").val();
        let gainsProfits = $("#txtGainsProfits").val();
        let totalInvestmentPartnership = $("#txtTotalInvestmentPartnership").val();


        let isValid = true;
        $(".validation-error").remove();

        // === Validation ===

        if (!totalInvestment) {
            $("#txtRProperty").after('<div class="text-danger validation-error">Total Investment Income is required.</div>');
            isValid = false;
        }
        if (!partnershipName.trim()) {
            $("#partnershipName").after('<div class="text-danger validation-error">Partnership Name is required.</div>');
            isValid = false;
        }

        if (!isValid) {
            $btn.prop("disabled", false);
            return;
        }

        // === Data Object ===
        let partnerInvestmentData = {
            InvestmentIncomePBEId: selfOnlineInvestmentPartnerId,
            TransactionType: selfOnlineInvestmentPartnerId ? "Edit" : "Add",
            Category: "Partner",
            TotalInvestmentIncome: totalInvestment,
            ActivityCode: activityCode,
            PartnershipName: partnershipName,
            TINNO: partnershipTin,
            GainsProfits: gainsProfits,
            TotalInvestmentIncomePartnership: totalInvestmentPartnership

        };

        // === AJAX URL ===
        var url = selfOnlineInvestmentPartnerId
            ? '/SelfOnlineFlow/UpdateSelfOnlineInvestmentPartnerBeneficiaryExemptDetails'
            : '/SelfOnlineFlow/AddSelfOnlineInvestmentPartnerBeneficiaryExemptDetails';

        $.ajax({
            url: url,
            type: 'POST',
            data: partnerInvestmentData,
            success: function (response) {
                $btn.prop("disabled", false);

                notifySuccess("", selfOnlineInvestmentPartnerId ? "Update successfully" : "Saved successfully");

                // Reload rent grid
                $.get('/SelfOnlineFlow/LoadInvestment_PartnerInvestment', function (html) {
                    $('#partnerInvestmentsGrid').html($(html).find('#partnerInvestmentsGrid').html());
                });

               // resetFormRent();
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving.");
            }
        });
    });


    $(document).on('click', '.partnerDetails-editbtn', function () {
  
        $(".validation-error").remove();

        let id = $(this).data("id");
        let category = $(this).data("category");
        let totalInvestment = $(this).data("total");
        let code = $(this).data("code");
        let partnershipName = $(this).data("partnership");
        let ptinNo = $(this).data("ptinNo");
        let gainsProfits = $(this).data("gains");
        let totalPartnership = $(this).data("totalpartnership");
        alert(ptinNo);

        // Now fill the modal/form inputs
        $("#hiddenInvestmentPartnerIncomeId").val(id);
        $("#txtCategory").val(category);
        $("#txtTotalInvestment").val(totalInvestment);
        $("#txtActivityCodePartnership").val(code);
        $("#txtPartnershipName").val(partnershipName);
        $("#txtPartnershipTin").val(ptinNo);
        $("#txtGainsProfits").val(gainsProfits);
        $("#txtTotalInvestmentPartnership").val(totalPartnership);

        // Open modal
        // $("#partnerDetailsModal").modal("show");
        $("#hiddenInvestmentPartnerIncomeId").val(id);
        $("#btnPartnerInvestmentSubmit").text("Update");
    });

    let deleteInvestmentId = null;
    let deleteCategoryName = null;

    $(document).on("click", ".PartnerBeneficiaryExemptDetails-deletebtn", function () {

        deleteInvestmentId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });
    $(document).on("click", "#selfonline_confirmDeleteBtn", function () {

        if (!deleteInvestmentId) return;

        var deleteId = {
            investmentIncomeId: deleteInvestmentId,
            categoryName: deleteCategoryName
        };


        $.ajax({
            url: '/SelfOnlineFlow/DeleteSelfOnlineInvestmentPartnerBeneficiaryExemptDetails',
            type: 'POST',
            data: deleteId,
            success: function (response) {
                $('#selfonline_confirmDeleteModal').modal('hide');

                if (deleteCategoryName == "Partner") {
                    $.get('/SelfOnlineFlow/LoadInvestment_PartnerInvestment', function (html) {
                            $('#partnerInvestmentsGrid').html($(html).find('#partnerInvestmentsGrid').html());
                    });
                }


            },
            error: function () {
                alert("Error deleting.");
            }
        });
    });

});