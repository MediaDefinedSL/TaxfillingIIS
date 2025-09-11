$(function () {

    document.querySelectorAll(".numeric-input").forEach(function (input) {
        input.addEventListener("input", function (e) {
            // Remove everything except digits and decimal point
            let value = e.target.value.replace(/[^0-9.]/g, "");

            // Only allow one decimal point
            let parts = value.split(".");
            if (parts.length > 2) {
                value = parts[0] + "." + parts.slice(1).join("");
            }

            // Format integer part with commas
            if (parts.length > 1) {
                // Has decimals
                let integerPart = parts[0];
                let decimalPart = parts[1].substring(0, 2); // limit to 2 decimals
                integerPart = integerPart ? parseInt(integerPart, 10).toLocaleString("en-US") : "0";
                value = integerPart + "." + decimalPart;
            } else {
                // No decimals
                if (value) {
                    value = parseInt(value, 10).toLocaleString("en-US");
                }
            }

            e.target.value = value;
        });

        input.addEventListener("blur", function (e) {
            let value = e.target.value.replace(/,/g, "");
            if (value) {
                let num = parseFloat(value);
                e.target.value = num.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }
        });

        // Prevent entering multiple dots directly
        input.addEventListener("keypress", function (e) {
            if (!/[0-9.]/.test(e.key)) {
                e.preventDefault();
            }
            // Prevent typing a second dot
            if (e.key === "." && e.target.value.includes(".")) {
                e.preventDefault();
            }
        });
    });


    $(document).on("input", "#txtSActivityCode", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });
    $(document).on("input", "#txtFDActivityCode", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });


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

               // notifySuccess("", selfOnlineInvestmentPartnerId ? "Update successfully" : "Saved successfully");
                showMessage(selfOnlineInvestmentPartnerId ? "Update successfully." : "Saved successfully", "success");
                // Reload rent grid
                $.get('/SelfOnlineFlow/LoadInvestment_PartnerInvestment', function (html) {
                    $('#partnerInvestmentsGrid').html($(html).find('#partnerInvestmentsGrid').html());
                });

                resetFormPartner();
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving.");
            }
        });
    });

    function resetFormPartner() {

        $("#hiddenInvestmentPartnerIncomeId").val("");   
        $("#txtTotalInvestment").val("");
        $("#txtActivityCodePartnership").val("");
        $("#txtPartnershipName").val("");
        $("#txtPartnershipTin").val("");
        $("#txtGainsProfits").val("");
        $("#txtTotalInvestmentPartnership").val("");
    }

    $(document).off("click", ".partnerDetails-editbtn").on("click", ".partnerDetails-editbtn", function () {

        $(".validation-error").remove();

        let id = $(this).data("id");
        let category = $(this).data("category");
        let totalInvestment = $(this).data("total");
        let code = $(this).data("code");
        let partnershipName = $(this).data("partnership");
        let ptinNo = $(this).data("ptin");
        let gainsProfits = $(this).data("gains");
        let totalPartnership = $(this).data("totalpartnership");
       
        // Now fill the modal/form inputs
        $("#hiddenInvestmentPartnerIncomeId").val(id);
      //  $("#txtCategory").val(category);
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

    $(document).on("click", "#btnPartnerInvestmentClear", function () {

        resetFormPartner();
        $("#btnPartnerInvestmentSubmit").text("Submit");

    });

    let deleteInvestmentId = null;
    let deleteCategoryName = null;

    $(document).on("click", ".PartnerBeneficiaryExemptDetails-deletebtn", function () {

        deleteInvestmentId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });

    $(document).off("click", "#selfonline_confirmDeleteBtn").on("click", "#selfonline_confirmDeleteBtn", function () {

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
                if (deleteCategoryName == "Beneficiary") {
                    $.get('/SelfOnlineFlow/LoadInvestment_BeneficiaryInvestment', function (html) {
                        $('#beneficiaryDetailsGrid').html($(html).find('#beneficiaryDetailsGrid').html());
                    });
                }



            },
            error: function () {
                alert("Error deleting.");
            }
        });
    });

    $('#linkDetailsContinue').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_Detailsinvestment',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkBeneficiaryContinue').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_BeneficiaryInvestment',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });


    //---------------Beneficiary

    $(document).off("click", "#btnBeneficiarySubmit").on("click", "#btnBeneficiarySubmit", function () {

        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collecting values from your form
        let selfOnlineInvestmentPartnerId = $("#hiddenInvestmentBeneficiaryIncomeId").val();   // hidden field for edit/update
        let totalInvestment = $("#txtBTotalInvestment").val();
        let activityCode = $("#txtBActivityCode").val();
        let trustName = $("#txtBTrustName").val();
        let trustTin = $("#txtBTINTrust").val();
        let gainsProfits = $("#txtBGainsProfits").val();
        let totalInvestmenttrust = $("#txtBTotalInvestmentTrust").val();


        let isValid = true;
        $(".validation-error").remove();

        // === Validation ===

        if (!totalInvestment) {
            $("#txtBTotalInvestment").after('<div class="text-danger validation-error">Total Investment Income is required.</div>');
            isValid = false;
        }
        if (!trustName.trim()) {
            $("#txtBTrustName").after('<div class="text-danger validation-error">Trust Name is required.</div>');
            isValid = false;
        }

        if (!isValid) {
            $btn.prop("disabled", false);
            return;
        }

        // === Data Object ===
        let beneficiaryInvestmentData = {
            InvestmentIncomePBEId: selfOnlineInvestmentPartnerId,
            TransactionType: selfOnlineInvestmentPartnerId ? "Edit" : "Add",
            Category: "Beneficiary",
            TotalInvestmentIncome: totalInvestment,
            ActivityCode: activityCode,
            TrustName: trustName,
            TINNO: trustTin,
            GainsProfits: gainsProfits,
            TotalInvestmentIncomeTrust: totalInvestmenttrust

        };

        // === AJAX URL ===
        var url = selfOnlineInvestmentPartnerId
            ? '/SelfOnlineFlow/UpdateSelfOnlineInvestmentPartnerBeneficiaryExemptDetails'
            : '/SelfOnlineFlow/AddSelfOnlineInvestmentPartnerBeneficiaryExemptDetails';

        $.ajax({
            url: url,
            type: 'POST',
            data: beneficiaryInvestmentData,
            success: function (response) {
                $btn.prop("disabled", false);

                //notifySuccess("", selfOnlineInvestmentPartnerId ? "Update successfully" : "Saved successfully");
                showMessage(selfOnlineInvestmentPartnerId ? "Update successfully." : "Saved successfully", "success");
                // Reload rent grid
                $.get('/SelfOnlineFlow/LoadInvestment_BeneficiaryInvestment', function (html) {
                    $('#beneficiaryDetailsGrid').html($(html).find('#beneficiaryDetailsGrid').html());
                });

                resetBeneficiaryForm();
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving.");
            }
        });
    });

    function resetBeneficiaryForm() {

         $("#hiddenInvestmentBeneficiaryIncomeId").val("");   
         $("#txtBTotalInvestment").val("");
         $("#txtBActivityCode").val("");
         $("#txtBTrustName").val("");
         $("#txtBTINTrust").val("");
         $("#txtBGainsProfits").val("");
        $("#txtBTotalInvestmentTrust").val("");
    }

    $(document).on("click", "#btnBeneficiaryClear", function () {

        resetBeneficiaryForm();
        $("#btnBeneficiarySubmit").text("Submit");

    });

    $(document).off("click", ".beneficiaryDetails-editbtn").on("click", ".beneficiaryDetails-editbtn", function () {

        $(".validation-error").remove();

        let id = $(this).data("id");
        let category = $(this).data("category");
        let totalInvestment = $(this).data("total");
        let code = $(this).data("code");
        let trustname = $(this).data("trustname");
        let ptinNo = $(this).data("btin");
        let gainsProfits = $(this).data("gains");
        let totaltrust = $(this).data("totaltrust");
      
        // Now fill the modal/form inputs
        $("#hiddenInvestmentPartnerIncomeId").val(id);
        $("#txtBTotalInvestment").val(totalInvestment);
        $("#txtBActivityCode").val(code);
        $("#txtBTrustName").val(trustname);
        $("#txtBTINTrust").val(ptinNo);
        $("#txtBGainsProfits").val(gainsProfits);
        $("#txtBTotalInvestmentTrust").val(totaltrust);

        // Open modal
        // $("#partnerDetailsModal").modal("show");
        $("#hiddenInvestmentBeneficiaryIncomeId").val(id);
        $("#btnBeneficiarySubmit").text("Update");
    });

    $('#linkExemptContinue').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_ExemptAmounts',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkPartnerDetailsContinue').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_PartnerInvestment',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });


    //---------------Exempt

    $(document).off("click", "#btnExemptAmountsSubmit").on("click", "#btnExemptAmountsSubmit", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);

        let selfOnlineInvestmentPartnerId = $("#hiddenInvestmentExemptIncomeId").val();
        let profitsInvestment = $("#rdbTProfitsInvestment").is(":checked");
        let excludedAmount = $("#rdbTExcludedAmount").is(":checked");
        let exempt = $("#txtTExempt").val();

        let isValid = true;
        $(".validation-error").remove();

        if (!exempt.trim()) {
            $("#txtTExempt").after('<div class="text-danger validation-error">Exempt/Excluded Income is required.</div>');
            isValid = false;
        }

        if (!isValid) {
            $btn.prop("disabled", false);
            return;
        }

        let excludedInvestmentData = {
            InvestmentIncomePBEId: selfOnlineInvestmentPartnerId,
            TransactionType: selfOnlineInvestmentPartnerId ? "Edit" : "Add",
            Category: "Exempt",
            IsExemptAmountA: profitsInvestment,
            IsExcludedAmountB: excludedAmount,
            ExemptExcludedIncome: exempt
        };

        var url = selfOnlineInvestmentPartnerId
            ? '/SelfOnlineFlow/UpdateSelfOnlineInvestmentPartnerBeneficiaryExemptDetails'
            : '/SelfOnlineFlow/AddSelfOnlineInvestmentPartnerBeneficiaryExemptDetails';

        $.ajax({
            url: url,
            type: 'POST',
            data: excludedInvestmentData,
            success: function (response) {
                $btn.prop("disabled", false);

               // notifySuccess("", selfOnlineInvestmentPartnerId ? "Update successfully" : "Saved successfully");
                showMessage(selfOnlineInvestmentPartnerId ? "Update successfully." : "Saved successfully", "success");
                $.get('/SelfOnlineFlow/LoadInvestment_ExemptAmounts', function (html) {
                    // refresh grid here if needed
                });
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving.");
            }
        });
    });


    $('#linkExemptAmountsBack').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_BeneficiaryInvestment',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

});