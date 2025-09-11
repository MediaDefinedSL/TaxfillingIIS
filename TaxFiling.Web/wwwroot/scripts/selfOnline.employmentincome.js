
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

    $("#divAPITPrimaryEmployment").show();
    $("#divAPITSecondaryEmployment").hide();

    $(document).on("click", "#btnEmploymentIncome", function () {
        // e.preventDefault();

        var $btn = $(this);
        $btn.prop("disabled", true);

        let residency = $("input[name='Residency']:checked").val();
        let seniorCitizen = $("#seniorCitizen").val();

        if (!residency) {
            alert("Please select a Residency option.");
            $btn.setButtonDisabled(false);
            return;
        }

        var employmentIncome = {
            Residency: residency,
            SeniorCitizen: seniorCitizen
        }


        $.ajax({
            url: '/SelfOnlineFlow/AddEmploymentIncome',
            type: 'POST',
            data: employmentIncome,
            success: function (response) {
                $btn.prop("disabled", false);
                //notifySuccess("", "Saved successfully");
                showMessage("Saved successfully.", "success");
            },
            error: function () {
                $btn.setButtonDisabled(false);
                alert("Error saving .");
            }
        });
    });

    $(document).off("click", "#btnEmploymentDetails").on("click", "#btnEmploymentDetails", function () {
    
        // e.preventDefault();
        var id = $("#hiddenEmploymentDetailsId").val();
        var $btn = $(this);
        $btn.prop("disabled", true);

        let selfOnlineEmploymentIncomeId = $("#hndSelfOnlineEmploymentIncomeId").val();
        let typeEmployment = $("#drpTypeEmployment").val();
        let remuneration = $("#txtRemuneration").val();
        let empDetailsECName = $("#txtEmpDetailsECName").val();
        let APITPrimaryEmployment = $("#txtAPITPrimaryEmployment").val();
        let TINEmployer = $("#txtTINEmployer").val();
        let APITSecondaryEmployment = $("#txtAPITSecondaryEmployment").val();
        let residency = $("input[name='Residency']:checked").val();
        let seniorCitizen = $("#rdbSeniorCitizen").prop("checked");

        let isValid = true;

        // Remove old validation messages
        $(".validation-error").remove();


        if (!typeEmployment) {
            $("#drpTypeEmployment").after('<div class="text-danger validation-error">Please select Type of Employment.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!empDetailsECName.trim()) {
            $("#txtEmpDetailsECName").after('<div class="text-danger validation-error">Name is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!remuneration.trim()) {
            $("#txtRemuneration").after('<div class="text-danger validation-error">Remuneration is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }


        if (!isValid) {
            return;
        }


        var employIncome = {
            SelfOnlineEmploymentDetailsId: id,
            Residency: residency,
            SeniorCitizen: seniorCitizen,
            CategoryName: "EmploymentDetails",
            TypeOfName: typeEmployment,
            EmployerORCompanyName: empDetailsECName,
            TINOfEmployer: TINEmployer,
            Remuneration: remuneration,
            APITPrimaryEmployment: APITPrimaryEmployment,
            APITSecondaryEmployment: APITSecondaryEmployment
        }
        if (id) {
            $.ajax({
                url: '/SelfOnlineFlow/UpdateEmploymentIncomeDetails',
                type: 'POST',
                data: employIncome,
                success: function (response) {
                    $btn.prop("disabled", false);
                    showMessage("Employment income details updated successfully", "success");

                    $.get('/SelfOnlineFlow/LoadEmploymentDetails', function (html) {
                        $('#employmentDetails1Grid').html($(html).find('#employmentDetails1Grid').html()); // Direct replace
                        var newTotal = parseFloat($(html).find("#hiddentotal").val() || 0);
                        
                        $("#taxTotal").text(newTotal);
                    });

                    $("#drpTypeEmployment").val("Primary");
                    $("#txtRemuneration").val("");
                    $("#txtEmpDetailsECName").val("");
                    $("#txtAPITPrimaryEmployment").val("");
                    $("#txtTINEmployer").val("");
                    $("#txtAPITSecondaryEmployment").val("");
                    $("#hndSelfOnlineEmploymentIncomeId").val("");
                    $("#rdbSeniorCitizen").prop("checked", false);
                    $("input[name='Residency']").prop("checked", false);
                    $("#hiddenEmploymentDetailsId").val("");
                    $("#btnEmploymentDetails").text("Submit");

                },
                error: function () {
                    $btn.setButtonDisabled(false);
                    alert("Error saving .");
                }
            });
        }
        else {
            
            $.ajax({
                url: '/SelfOnlineFlow/AddEmploymentIncomeDetails',
                type: 'POST',
                data: employIncome,
                success: function (response) {
                 
                    $btn.prop("disabled", false);
                   // notifySuccess("", "save successfully");
                    showMessage("Employment income details save successfully.", "success");
                    $.get('/SelfOnlineFlow/LoadEmploymentDetails', function (html) {
                        
                        $('#employmentDetails1Grid').html($(html).find('#employmentDetails1Grid').html());
                        var newTotal = parseFloat($(html).find("#hiddentotal").val() || 0);
                        $("#taxTotal").text(newTotal);
                    });


                    $("#drpTypeEmployment").val("Primary");
                    $("#txtRemuneration").val("");
                    $("#rdbSeniorCitizen").prop("checked", false);
                    $("input[name='Residency']").prop("checked", false);
                    $("#txtEmpDetailsECName").val("");
                    $("#txtAPITPrimaryEmployment").val("");
                    $("#txtTINEmployer").val("");
                    $("#txtAPITSecondaryEmployment").val("");
                    $("#hndSelfOnlineEmploymentIncomeId").val("");
                    $("#hiddenEmploymentDetailsId").val("");
                    $("#btnEmploymentDetails").text("Submit");
                   

                },
                error: function () {
                    $btn.prop("disabled", false);
                    alert("Error saving .");
                }
            });
        }
    });

    $(document).on("click", "#btnEmploymentDetailsClear", function () {

        $("#drpTypeEmployment").val("Primary");
        $("#txtRemuneration").val("");
        $("#rdbSeniorCitizen").prop("checked", false);
        $("input[name='Residency']").prop("checked", false);
        $("#txtEmpDetailsECName").val("");
        $("#txtAPITPrimaryEmployment").val("");
        $("#txtTINEmployer").val("");
        $("#txtAPITSecondaryEmployment").val("");
        $("#hiddenEmploymentDetailsId").val(0);
        $("#btnEmploymentDetails").text("Submit");
        $("#hndSelfOnlineEmploymentIncomeId").val("");

    });

    $(document).off("click", "#btnTerminalSubmit").on("click", "#btnTerminalSubmit", function () {
   
        // e.preventDefault();
        var id = $("#hiddenTerminalId").val();

        var $btn = $(this);
        $btn.prop("disabled", true);

        let selfOnlineEmploymentIncomeId = $("#hndSelfOnlineEmploymentIncomeId").val();
        let typeTerminal = $("#dpdTypeTerminal").val();
        let terminalECName = $("#txtTerminalECName").val();
        let TINTerminal = $("#txtTerminalTINNo").val();
        let terminalBenefits = $("#txtTerminalBenefits").val();

        let isValid = true;

        // Remove old validation messages
        $(".validation-error").remove();

        if (!typeTerminal) {
            $("#dpdTypeTerminal").after('<div class="text-danger validation-error">Please select Type of Terminal Benefit.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!terminalECName.trim()) {
            $("#txtTerminalECName").after('<div class="text-danger validation-error">Employer/Company Name is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!terminalBenefits.trim()) {
            $("#txtTerminalBenefits").after('<div class="text-danger validation-error">Terminal Benefits (Rs.) is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }

        if (!isValid) {
            return;
        }

        var terminalIncome = {
            SelfOnlineEmploymentDetailsId: id,
            CategoryName: "TerminalBenefits",
            TypeOfName: typeTerminal,
            EmployerORCompanyName: terminalECName,
            TINOfEmployer: TINTerminal,
            TerminalBenefits: terminalBenefits
        }
        if (id) {
            $.ajax({
                url: '/SelfOnlineFlow/UpdateEmploymentIncomeDetails',
                type: 'POST',
                data: terminalIncome,
                success: function (response) {
                    $btn.prop("disabled", false);
                   // notifySuccess("", "Update successfully");
                    showMessage("Update successfully.", "success");
                    $.get('/SelfOnlineFlow/LoadETerminalBenefits', function (html) {
                        $('#terminalDetailsGrid').html($(html).find('#terminalDetailsGrid').html());
                        var newTotal = parseFloat($(html).find("#hiddenBtotal").val() || 0); 
                        $("#taxTotal").text(newTotal);
                    });

                    $("#dpdTypeTerminal").val("Primary");
                    $("#txtTerminalECName").val("");
                    $("#txtTerminalTINNo").val("");
                    $("#txtTerminalBenefits").val("");
                    $("#btnTerminalSubmit").text("Submit");
                    $("#hiddenTerminalId").val("")

                },
                error: function () {
                    $btn.setButtonDisabled(false);
                    alert("Error saving .");
                }
            });
        }
        else {

            $.ajax({
                url: '/SelfOnlineFlow/AddEmploymentIncomeDetails',
                type: 'POST',
                data: terminalIncome,
                success: function (response) {
                    $btn.prop("disabled", false);
                   // notifySuccess("", "Saved successfully");
                    showMessage("Saved successfully.", "success");
                    $.get('/SelfOnlineFlow/LoadETerminalBenefits', function (html) {
                        $('#terminalDetailsGrid').html($(html).find('#terminalDetailsGrid').html());

                        var newTotal = parseFloat($(html).find("#hiddenBtotal").val() || 0); 
                        $("#taxTotal").text(newTotal);
                    });

                    $("#dpdTypeTerminal").val("Primary");
                    $("#txtTerminalECName").val("");
                    $("#txtTerminalTINNo").val("");
                    $("#txtTerminalBenefits").val("");
                    $("#btnTerminalSubmit").text("Submit");
                    $("#hiddenTerminalId").val("")

                },
                error: function () {
                    $btn.setButtonDisabled(false);
                    alert("Error saving .");
                }
            });
        }
    });
    $(document).on("click", "#btnTerminalClear", function () {

        $("#dpdTypeTerminal").val("Primary");
        $("#txtTerminalECName").val("");
        $("#txtTerminalTINNo").val("");
        $("#txtTerminalBenefits").val("");
        $("#btnTerminalSubmit").text("Submit");
        $("#hiddenTerminalId").val("")

    });

    $(document).off("click", "#btnExemptSubmit").on("click", "#btnExemptSubmit", function () {
      // e.preventDefault();
        var id = $("#hiddenExemptId").val();
        var $btn = $(this);
        $btn.prop("disabled", true);

        let selfOnlineEmploymentIncomeId = $("#hndSelfOnlineEmploymentIncomeId").val();
        let exemptType = $("#dpdExemptType").val();
        let exemptTinEmployerName = $("#txtExemptTinEmployerName").val();
        let TINExempt = $("#txtExemptTin").val();
        let exemptAmount = $("#txtExemptAmount").val();

        let isValid = true;

        // Remove old validation messages
        $(".validation-error").remove();

        if (!exemptType) {
            $("#dpdExemptType").after('<div class="text-danger validation-error">Please select Type of Exempt or Excluded Income.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!exemptTinEmployerName.trim()) {
            $("#txtExemptTinEmployerName").after('<div class="text-danger validation-error">Employer/Company Name is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!exemptAmount.trim()) {
            $("#txtExemptAmount").after('<div class="text-danger validation-error">Amount is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }

        if (!isValid) {
            return;
        }

        var exemptIncome = {
            SelfOnlineEmploymentDetailsId: id,
            CategoryName: "ExemptAmounts",
            TypeOfName: exemptType,
            EmployerORCompanyName: exemptTinEmployerName,
            TINOfEmployer: TINExempt,
            Amount: exemptAmount
        }
        if (id) {
            $.ajax({
                url: '/SelfOnlineFlow/UpdateEmploymentIncomeDetails',
                type: 'POST',
                data: exemptIncome,
                success: function (response) {
                    $btn.prop("disabled", false);
                  //  notifySuccess("", "Update successfully");
                    showMessage("Update successfully.", "success");
                    $.get('/SelfOnlineFlow/LoadExemptAmounts', function (html) {
                        $('#exemptDetailsGrid').html($(html).find('#exemptDetailsGrid').html());
                        var newTotal = parseFloat($(html).find("#hiddenEtotal").val() || 0);
                        $("#taxTotal").text(newTotal);
                    });

                    $("#dpdExemptType").val("");
                    $("#txtExemptTinEmployerName").val("");
                    $("#txtExemptTin").val("");
                    $("#txtExemptAmount").val("");
                    $("#btnExemptSubmit").text("Submit");
                    $("#hiddenExemptId").val("");

                },
                error: function () {
                    $btn.setButtonDisabled(false);
                    alert("Error saving .");
                }
            });
        }
        else {

            $.ajax({
                url: '/SelfOnlineFlow/AddEmploymentIncomeDetails',
                type: 'POST',
                data: exemptIncome,
                success: function (response) {
                    $btn.prop("disabled", false);
                    //notifySuccess("", "Saved successfully");
                    showMessage("Saved successfully.", "success");

                    $.get('/SelfOnlineFlow/LoadExemptAmounts', function (html) {
                        $('#exemptDetailsGrid').html($(html).find('#exemptDetailsGrid').html());
                        var newTotal = parseFloat($(html).find("#hiddenEtotal").val() || 0);
                        $("#taxTotal").text(newTotal);
                    });

                    $("#dpdExemptType").val("");
                    $("#txtExemptTinEmployerName").val("");
                    $("#txtExemptTin").val("");
                    $("#txtExemptAmount").val("");
                    $("#btnExemptSubmit").text("Submit");
                    $("#hiddenExemptId").val("");
                    

                },
                error: function () {
                    $btn.setButtonDisabled(false);
                    alert("Error saving .");
                }
            });
        }
    });

    $(document).on("click", "#btnExemptClear", function () {


        $("#dpdExemptType").val("Primary");
        $("#txtExemptTinEmployerName").val("");
        $("#txtExemptTin").val("");
        $("#txtExemptAmount").val("");
        $("#hiddenExemptId").val(0);
        $("#btnExemptSubmit").text("Submit");

    });

    $("#drpTypeEmployment").change(function () {
        var selectedValue = $(this).val();

        if (selectedValue === "Primary") {
            $("#divAPITPrimaryEmployment").show();
            $("#divAPITSecondaryEmployment").hide();
        } else if (selectedValue === "Secondary") {
            $("#divAPITPrimaryEmployment").hide();
            $("#divAPITSecondaryEmployment").show();
        }
    });

    $('#linkTerminalBenefitsContinue').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadETerminalBenefits',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });
    $('#linkEmploymentDetailsContinue').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadEmploymentDetails',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkExemptAmountsContinue').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadExemptAmounts',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $(document).off("click", ".employmentDetails-editbtn").on("click", ".employmentDetails-editbtn", function () {
 
        $(".validation-error").remove();
        $("#rdbSeniorCitizen").prop("checked", false);
        $("input[name='Residency']").prop("checked", false);

        var id = $(this).data("id");
        var residency = $(this).data("residency");
        var type = $(this).data("type");
        var name = $(this).data("name");
        var tin = $(this).data("tin");
        var remuneration = $(this).data("remuneration");
        var primary = $(this).data("primary");
        var secondary = $(this).data("secondary");
        var seniorcitizen = String($(this).data("seniorcitizen")).toLowerCase() === "true";

        $(".rdbresidency[value='" + residency + "']").prop("checked", true);
        $("#rdbSeniorCitizen").prop("checked", seniorcitizen);
        $("#drpTypeEmployment").val(type);
        $("#txtEmpDetailsECName").val(name);
        $("#txtTINEmployer").val(tin);
        $("#txtRemuneration").val(remuneration);
        $("#txtAPITPrimaryEmployment").val(primary);
        $("#txtAPITSecondaryEmployment").val(secondary);

        // Store id in hidden field for update
        $("#hiddenEmploymentDetailsId").val(id);
        $("#btnEmploymentDetails").text("Update");
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });

    $(document).off("click", ".terminalbenefits-editbtn").on("click", ".terminalbenefits-editbtn", function () {
   
        var id = $(this).data("id");
        var type = $(this).data("type");
        var name = $(this).data("name");
        var tin = $(this).data("tin");
        var benefit = $(this).data("benefit");

        // Fill form fields with selected row data
        $("#dpdTypeTerminal").val(type);
        $("#txtTerminalECName").val(name);
        $("#txtTerminalTINNo").val(tin);
        $("#txtTerminalBenefits").val(benefit);

        // Store id in hidden field for update
        $("#hiddenTerminalId").val(id);
        $("#btnTerminalSubmit").text("Update");
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });

    $(document).off("click", ".exemptamounts-editbtn").on("click", ".exemptamounts-editbtn", function () {
   

        var id = $(this).data("id");
        var type = $(this).data("type");
        var name = $(this).data("name");
        var tin = $(this).data("tin");
        var amount = $(this).data("amount");

        // Fill form fields with selected row data
        $("#dpdExemptType").val(type);
        $("#txtExemptTinEmployerName").val(name);
        $("#txtExemptTin").val(tin);
        $("#txtExemptAmount").val(amount);

        // Store id in hidden field for update
        $("#hiddenExemptId").val(id);
        $("#btnExemptSubmit").text("Update");
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });


    let deleteEmploymentDetailsId = null;
    let deleteEmploymentDetailsName = null;

    $(document).off("click", ".employmentdetails-deletebtn").on("click", ".employmentdetails-deletebtn", function () {

        deleteEmploymentDetailsId = $(this).data("id");
        deleteEmploymentDetailsName = $(this).data("name");

        // $("#hiddenTerminalId").val(deleteEmploymentDetailsId);
        $('#selfonline_confirmDeleteModal').modal('show');
        

    });

    $(document).off("click", "#selfonline_confirmDeleteBtn").on("click", "#selfonline_confirmDeleteBtn", function () {
  
        if (!deleteEmploymentDetailsId) return;

        var deleteId = {
            employmentDetailsId: deleteEmploymentDetailsId,
            employmentDetailsName: deleteEmploymentDetailsName
        };

      
        $.ajax({
            url: '/SelfOnlineFlow/DeleteEmploymentIncomeDetail',
            type: 'POST',
            data: deleteId,
            success: function (response) {
                $('#selfonline_confirmDeleteModal').modal('hide');
 
                if (deleteEmploymentDetailsName == "EmploymentDetails") {
                    $.get('/SelfOnlineFlow/LoadEmploymentDetails', function (html) {
                        $('#employmentDetails1Grid').html($(html).find('#employmentDetails1Grid').html()); // Direct replace
                        var newTotal = parseFloat($(html).find("#hiddentotal").val() || 0);

                        $("#taxTotal").text(newTotal);
                    });
                }
                if (deleteEmploymentDetailsName == "TerminalBenefits") {
                    $.get('/SelfOnlineFlow/LoadETerminalBenefits', function (html) {
                        $('#terminalDetailsGrid').html($(html).find('#terminalDetailsGrid').html());
                        var newTotal = parseFloat($(html).find("#hiddenBtotal").val() || 0);
                        $("#taxTotal").text(newTotal);
                    });
                }
                if (deleteEmploymentDetailsName == "ExemptAmounts") {
                    $.get('/SelfOnlineFlow/LoadExemptAmounts', function (html) {
                        $('#exemptDetailsGrid').html($(html).find('#exemptDetailsGrid').html());
                        var newTotal = parseFloat($(html).find("#hiddenEtotal").val() || 0);
                        $("#taxTotal").text(newTotal);
                    });
                }
             
            },
            error: function () {
                alert("Error deleting.");
            }
        });
    });


    $('#etf').on('show.bs.collapse', function () {

        let selfOnlineEmploymentIncomeId = $("#hndSelfOnlineEmploymentIncomeId").val();

        var terminalBenefits = {
            employmentIncomeId: selfOnlineEmploymentIncomeId,
            terminalBenefits: true
        }
        $.ajax({
            url: '/SelfOnlineFlow/UpdateEmploymentIncomeTerminalBenefits',
            type: 'POST',
            data: terminalBenefits,
            success: function (response) {
                //  notifySuccess("", "Saved successfully");
            },
            error: function () {
                alert("Error saving .");
            }
        });

    });
    let deleteTargetId = null;

    $('#etf').on('hide.bs.collapse', function (e) {

        //   e.preventDefault(); 
        deleteTargetId = $("#hndSelfOnlineEmploymentIncomeId").val();
        $('#confirmDeleteModal').modal('show');
    });


    $('#confirmDeleteBtn').on('click', function () {
        if (!deleteTargetId) return;

        var terminalBenefits = {
            employmentIncomeId: deleteTargetId,
            terminalBenefits: false
        };


        $.ajax({
            url: '/SelfOnlineFlow/UpdateEmploymentIncomeTerminalBenefits',
            type: 'POST',
            data: terminalBenefits,
            success: function (response) {
                $('#confirmDeleteModal').modal('hide');
                $('#etf').off('hide.bs.collapse').collapse('hide');

                $.get('/SelfOnlineFlow/LoadIncomeLiableTax', function (html) {
                    $('#exemptDetailsGrid').html($(html).find('#exemptDetailsGrid').html());
                    var newTotal = $(html).find("#spnEmploymentIncomeTotal").text();
                    $("#spnEmploymentIncomeTotal").text(newTotal);
                    $("#taxTotal").text(newTotal);
                });
            },
            error: function () {
                alert("Error deleting.");
            }
        });
    });
    $('#Exemptt').on('show.bs.collapse', function () {

        let selfOnlineEmploymentIncomeId = $("#hndSelfOnlineEmploymentIncomeId").val();

        var terminalBenefits = {
            employmentIncomeId: selfOnlineEmploymentIncomeId,
            exemptAmounts: true
        }
        $.ajax({
            url: '/SelfOnlineFlow/UpdateEmploymentIncomeExemptAmounts',
            type: 'POST',
            data: terminalBenefits,
            success: function (response) {
                // notifySuccess("", "Saved successfully");
            },
            error: function () {
                alert("Error saving .");
            }
        });

    });


    $('#Exemptt').on('hide.bs.collapse', function () {

        // e.preventDefault();
        deleteTargetId = $("#hndSelfOnlineEmploymentIncomeId").val();
        $('#exemptConfirmDeleteModal').modal('show');


    });

    $('#exemptConfirmDeleteBtn').on('click', function () {
        if (!deleteTargetId) return;

        var exempt = {
            employmentIncomeId: deleteTargetId,
            terminalBenefits: false
        };


        $.ajax({
            url: '/SelfOnlineFlow/UpdateEmploymentIncomeExemptAmounts',
            type: 'POST',
            data: exempt,
            success: function (response) {
                $('#exemptConfirmDeleteModal').modal('hide');
                $('#Exemptt').off('hide.bs.collapse').collapse('hide');

                $.get('/SelfOnlineFlow/LoadIncomeLiableTax', function (html) {
                    $('#exemptDetailsGrid').html($(html).find('#exemptDetailsGrid').html());
                    var newTotal = $(html).find("#spnEmploymentIncomeTotal").text();
                    $("#spnEmploymentIncomeTotal").text(newTotal);
                    $("#taxTotal").text(newTotal);
                });

            },
            error: function () {
                alert("Error deleting.");
            }
        });
    });

});


