
$(function () {

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
                notifySuccess("", "Saved successfully");
            },
            error: function () {
                $btn.setButtonDisabled(false);
                alert("Error saving .");
            }
        });
    });

    $(document).on("click", "#btnEmploymentDetails", function () {
        // e.preventDefault();

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
        let seniorCitizen = $("#seniorCitizen").val();

       

        
        let isValid = true;

        // Remove old validation messages
        $(".validation-error").remove();

        //if (!residency) {
        //    $(".clsresidency").after('<div class="text-danger validation-error">Please select Type of Employment.</div>');
        //    $btn.prop("disabled", false);
        //    isValid = false;
        //}

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


        $.ajax({
            url: '/SelfOnlineFlow/AddEmploymentIncomeDetails',
            type: 'POST',
            data: employIncome,
            success: function (response) {
                $btn.prop("disabled", false); 

                notifySuccess("", "Saved successfully");

                $.get('/SelfOnlineFlow/LoadIncomeLiableTax', function (html) {
                    $('#employmentDetailsGrid').html($(html).find('#employmentDetailsGrid').html()); // Direct replace
                    var newTotal = $(html).find("#spnEmploymentIncomeTotal").text();
                 //   $("#spnEmploymentIncomeTotal").text(newTotal);
                   // var newTaxTotal = $(html).find("#taxTotal").text();
                    $("#taxTotal").text(newTotal);
                });

                $("#drpTypeEmployment").val("Primary");
                $("#txtRemuneration").val("");
                $("#txtEmpDetailsECName").val("");
                $("#txtAPITPrimaryEmployment").val("");
                $("#txtTINEmployer").val("");
                $("#txtAPITSecondaryEmployment").val("");
                
            },
            error: function () {
                $btn.prop("disabled", false); 
                alert("Error saving .");
            }
        });
    });

    $(document).on("click", "#btnTerminalSubmit", function () {
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
                    notifySuccess("", "Update successfully");

                    $.get('/SelfOnlineFlow/LoadIncomeLiableTax', function (html) {
                        $('#terminalDetailsGrid').html($(html).find('#terminalDetailsGrid').html());

                        var newTotal = $(html).find("#spnEmploymentIncomeTotal").text();
                       // $("#spnEmploymentIncomeTotal").text(newTotal);
                        $("#taxTotal").text(newTotal);
                    });

                    $("#dpdTypeTerminal").val("Primary");
                    $("#txtTerminalECName").val("");
                    $("#txtTerminalTINNo").val("");
                    $("#txtTerminalBenefits").val("");

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
                    notifySuccess("", "Saved successfully");

                    $.get('/SelfOnlineFlow/LoadIncomeLiableTax', function (html) {
                        $('#terminalDetailsGrid').html($(html).find('#terminalDetailsGrid').html());

                        var newTotal = $(html).find("#spnEmploymentIncomeTotal").text();
                        $("#spnEmploymentIncomeTotal").text(newTotal);
                        $("#taxTotal").text(newTotal);
                    });

                    $("#dpdTypeTerminal").val("Primary");
                    $("#txtTerminalECName").val("");
                    $("#txtTerminalTINNo").val("");
                    $("#txtTerminalBenefits").val("");

                },
                error: function () {
                    $btn.setButtonDisabled(false);
                    alert("Error saving .");
                }
            });
        }
    });

    $(document).on("click", "#btnExemptSubmit", function () {
        // e.preventDefault();

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
            //SelfOnlineEmploymentIncomeId: selfOnlineEmploymentIncomeId,
            CategoryName: "ExemptAmounts",
            TypeOfName: exemptType,
            EmployerORCompanyName: exemptTinEmployerName,
            TINOfEmployer: TINExempt,
            Amount: exemptAmount
        }


        $.ajax({
            url: '/SelfOnlineFlow/AddEmploymentIncomeDetails',
            type: 'POST',
            data: exemptIncome,
            success: function (response) {
                $btn.prop("disabled", false);
                notifySuccess("", "Saved successfully");

                $.get('/SelfOnlineFlow/LoadIncomeLiableTax', function (html) {
                    $('#exemptDetailsGrid').html($(html).find('#exemptDetailsGrid').html()); 
                    var newTotal = $(html).find("#spnEmploymentIncomeTotal").text();
                    $("#spnEmploymentIncomeTotal").text(newTotal);
                    $("#taxTotal").text(newTotal);
                });

                $("#dpdExemptType").val("");
                $("#txtExemptTinEmployerName").val("");
                $("#txtExemptTin").val("");
                $("#txtExemptAmount").val("");

            },
            error: function () {
                $btn.setButtonDisabled(false);
                alert("Error saving .");
            }
        });
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

    $('.terminalbenefits-editbtn').on('click', function () {

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
    });

    let deleteEmploymentDetailsId = null;

    $('.terminalbenefits-deletebtn').on('click', function () {

        deleteEmploymentDetailsId = $(this).data("id");

        $("#hiddenTerminalId").val(deleteEmploymentDetailsId);
        $('#selfonline_confirmDeleteModal').modal('show');
      
    });

    
    $('#selfonline_confirmDeleteBtn').on('click', function () {
        if (!deleteEmploymentDetailsId) return;

        var deleteId = {
            employmentDetailsId: deleteEmploymentDetailsId,
        };


        $.ajax({
            url: '/SelfOnlineFlow/DeleteEmploymentIncomeDetail',
            type: 'POST',
            data: deleteId,
            success: function (response) {
                $('#selfonline_confirmDeleteModal').modal('hide');
               
                $.get('/SelfOnlineFlow/LoadIncomeLiableTax', function (html) {
                    $('#terminalDetailsGrid').html($(html).find('#terminalDetailsGrid').html());

                    var newTotal = $(html).find("#spnEmploymentIncomeTotal").text();
                    // $("#spnEmploymentIncomeTotal").text(newTotal);
                    $("#taxTotal").text(newTotal);
                });
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


