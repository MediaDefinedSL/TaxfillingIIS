$(function () {

    document.getElementById("ddlSTypeInvestment").addEventListener("change", function () {
        var selectedValue = this.value;
        document.getElementById("txtSActivityCode").value = selectedValue;
    });

    document.getElementById("ddlFDTypeInvestment").addEventListener("change", function () {
        var FDselectedValue = this.value;
        document.getElementById("txtFDActivityCode").value = FDselectedValue;
    });
    document.getElementById("ddlDTypeInvestment").addEventListener("change", function () {
        var DselectedValue = this.value;
        document.getElementById("txtDActivityCode").value = DselectedValue;
    });


    //----------------------- Interest Income from Savings Accounts
    let allBanks = [];
    let selectedBank = null;
    let highlightedIndex = -1;
    let highlightedBranchIndex = -1;

    $.getJSON("https://mail.taxfiling.lk/getallbank", function (data) {
        allBanks = data;
    });
  
    $("#bankInput").on("input focus", function () {
        const q = $(this).val().toLowerCase();
        let results = allBanks;

        if (q) {
            results = results.filter(b =>
                (b.bankName && b.bankName.toLowerCase().startsWith(q))
            );
        }

        let html = "";
        if (results.length === 0) html = "<div>No banks match.</div>";
        results.forEach(b => {
            html += `<div data-id="${b.BankCode}" data-long="${b.bankName}">
                                          ${b.bankName} 
                                 </div>`;
        });

        $("#bankDropdown").html(html).show();
    });

    $("#bankDropdown").on("mousedown", "div", function () {
        const bankId = $(this).data("id");

        const bankLong = $(this).data("long");

        $("#bankInput").val(bankLong);
        $("#bank_id").val(bankId);

        selectedBank = bankId;

        $("#branchInput")
            .prop("disabled", false)       // enable input
            .val("")                        // clear previous value
            .attr("placeholder", "Type branch name or code")
            .focus();

        //$("#branchInput").val("").prop("disabled", false).attr("placeholder", "Type branch name or code");
        $("#branch_code, #branch_name").val("");
        $("#bankDropdown").hide();
        $.getJSON(`https://mail.taxfiling.lk/getbranches/${selectedBank}`, (branches) => {
            if (!branches.length) return $("#branchDropdown").hide();

            const html = branches.map(br =>
                `<div data-code="${br.BranchCode}" data-name="${br.BranchName}">${br.BranchName}</div>`
            ).join("");

            $("#branchDropdown").html(html).show();
            $("#branchDropdown div").removeClass("highlight").first().addClass("highlight");
            highlightedBranchIndex = 0; // first item highlighted
        });
    });

    $("#branchDropdown").on("mousedown", "div", function () {
        const brCode = $(this).data("code");
        const brName = $(this).data("name");

        $("#branchInput").val(brName);
        $("#branch_code").val(brCode);
        $("#branch_name").val(brName);

        $("#branchDropdown").hide();
    });

    $(document).on("click", (e) => {
        if (!$(e.target).closest("#bankInput, #bankDropdown").length) {
            $("#bankDropdown").hide();
        }
        if (!$(e.target).closest("#branchInput, #branchDropdown").length) {
            $("#branchDropdown").hide();
        }
    });

    $("#bankInput").on("keydown", function (e) {
        const items = $("#bankDropdown div");
        if (!items.length) return;

        if (e.key === "ArrowDown") {
            e.preventDefault();
            highlightedIndex = (highlightedIndex + 1) % items.length;
            items.removeClass("highlight").eq(highlightedIndex).addClass("highlight");
        }
        else if (e.key === "ArrowUp") {
            e.preventDefault();
            highlightedIndex = (highlightedIndex - 1 + items.length) % items.length;
            items.removeClass("highlight").eq(highlightedIndex).addClass("highlight");
        }
        else if (e.key === "Enter") {
            e.preventDefault();
            if (highlightedIndex >= 0) {
                const selected = items.eq(highlightedIndex);
                const bankId = selected.data("id");
                const bankName = selected.data("long");

                $("#bankInput").val(bankName);
                $("#bank_id").val(bankId);
                selectedBank = bankId;

                $("#branchInput")
                    .prop("disabled", false)       // enable input
                    .val("")                        // clear previous value
                    .attr("placeholder", "Type branch name or code")
                    .focus();

                // reset branch
                //$("#branchInput").val("").prop("disabled", false).attr("placeholder", "Type branch name or code");
                $("#branch_code, #branch_name").val("");

                $("#bankDropdown").hide();

                $.getJSON(`https://mail.taxfiling.lk/getbranches/${selectedBank}`, (branches) => {
                    if (!branches.length) return $("#branchDropdown").hide();

                    const html = branches.map(br =>
                        `<div data-code="${br.BranchCode}" data-name="${br.BranchName}">${br.BranchName}</div>`
                    ).join("");

                    $("#branchDropdown").html(html).show();
                    //highlightedBranchIndex = -1; // reset highlight

                    $("#branchDropdown div").removeClass("highlight").first().addClass("highlight");
                    highlightedBranchIndex = 0; // first item highlighted
                });
            }
        }
    });


    $("#bankDropdown").on("mouseenter", "div", function () {
        $("#bankDropdown div").removeClass("highlight");
        $(this).addClass("highlight");
        highlightedIndex = $(this).index();
    });

    $("#branchInput").on("keydown", function (e) {

        const items = $("#branchDropdown div");
        if (!items.length) return;

        if (e.key === "ArrowDown") {
            e.preventDefault();
            highlightedBranchIndex = (highlightedBranchIndex + 1) % items.length;
            items.removeClass("highlight").eq(highlightedBranchIndex).addClass("highlight");
        }
        else if (e.key === "ArrowUp") {
            e.preventDefault();
            highlightedBranchIndex = (highlightedBranchIndex - 1 + items.length) % items.length;
            items.removeClass("highlight").eq(highlightedBranchIndex).addClass("highlight");
        }
        else if (e.key === "Enter") {
            e.preventDefault();
            if (highlightedBranchIndex >= 0) {
                const selected = items.eq(highlightedBranchIndex);
                const brCode = selected.data("code");
                const brName = selected.data("name");

                $("#branchInput").val(brName);
                $("#branch_code").val(brCode);
                $("#branch_name").val(brName);

                $("#branchDropdown").hide();
                highlightedBranchIndex = -1;
            }
        }
    });

    $("#branchDropdown").on("mouseenter", "div", function () {
        $("#branchDropdown div").removeClass("highlight");
        $(this).addClass("highlight");
        highlightedBranchIndex = $(this).index();
    });

    $("#branchInput").on("focus input", function () {
        if (!selectedBank) return; // No bank selected, do nothing

        const q = $(this).val().toLowerCase();

        $.getJSON(`https://mail.taxfiling.lk/getbranches/${selectedBank}`, (branches) => {
            if (!branches.length) return $("#branchDropdown").hide();

            // Filter if user typed something
            const results = q
                ? branches.filter(br => br.BranchName.toLowerCase().startsWith(q))
                : branches;

            if (!results.length) return $("#branchDropdown").hide();

            const html = results.map(br =>
                `<div data-code="${br.BranchCode}" data-name="${br.BranchName}">${br.BranchName}</div>`
            ).join("");

            $("#branchDropdown").html(html).show();

            // Highlight first item
            $("#branchDropdown div").removeClass("highlight").first().addClass("highlight");
            highlightedBranchIndex = 0;
        });
    });



    $(document).on("click", "#btnDetailsInvestmentSavings", function () {

        var $btn = $(this);
        $btn.prop("disabled", true);

        // Get form values
        let selfOnlineInvestmentId = $("#hiddenInvestmentIncomeId").val();
        let activityCode = $("#txtSActivityCode").val();
        let typeInvestment = $("#ddlSTypeInvestment").val();
        let bankName = $("#bankInput").val();
        let bankBranch = $("#branchInput").val();
        let accountNo = $("#txtSAccountNo").val();
        let whtCertificateNo = $("#txtSWHTCertificateNo").val();
        let amountInvested = $("#txtSAmountInvested").val();
        let interestIncome = $("#txtSInterest").val();
        let whtDeducted = $("#txtSWHTDeducted").val();
        let foreignTaxCredit = $("#txtSForeignTaxCredit").val();
        let openingBalance = $("#txtSOpeningBalance").val();
        let closingBalance = $("#txtSBalance").val();

        let isValid = true;

        // Remove old validation messages
        $(".validation-error").remove();

        // Validation
        if (!typeInvestment) {
            $("#ddlSTypeInvestment").after('<div class="text-danger validation-error">Please select Type of Investment.</div>');
            isValid = false;
        }
        if (!bankName.trim()) {
            $("#bankInput").after('<div class="text-danger validation-error">Bank Name is required.</div>');
            isValid = false;
        }
        if (!accountNo.trim()) {
            $("#txtSAccountNo").after('<div class="text-danger validation-error">Account No is required.</div>');
            isValid = false;
        }
        if (!amountInvested.trim()) {
            $("#txtSAmountInvested").after('<div class="text-danger validation-error">Amount Invested is required.</div>');
            isValid = false;
        }
        

        if (!isValid) {
            $btn.prop("disabled", false);
            return;
        }

        // Prepare data for AJAX
        var fixedDepositData = {
            InvestmentIncomeDetailId: selfOnlineInvestmentId,
            TransactionType: selfOnlineInvestmentId ? "Edit" : "Add",
            Category: "Savings",
            ActivityCode: activityCode,
            TypeOfInvestment: typeInvestment,
            BankName: bankName,
            BankBranch: bankBranch,
            AccountNo: accountNo,
            WHTCertificateNo: whtCertificateNo,
            AmountInvested: amountInvested,
            IncomeAmount: interestIncome,
            WHTDeducted: whtDeducted,
            ForeignTaxCredit: foreignTaxCredit,
            OpeningBalance: openingBalance,
            ClosingBalance: closingBalance
        };
        // === AJAX URL ===
        var url = selfOnlineInvestmentId
            ? '/SelfOnlineFlow/UpdateSelfOnlineInvestmentIncomeDetails'
            : '/SelfOnlineFlow/AddSelfOnlineInvestmentIncomeDetails';

        $.ajax({
            url: url,
            type: 'POST',
            data: fixedDepositData,
            success: function (response) {
                $btn.prop("disabled", false);

                notifySuccess("", selfOnlineInvestmentId ? "Update successfully" : "Saved successfully");

                $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                    $('#SavingsGrid').html($(html).find('#SavingsGrid').html());
                });

                resetForm();
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving.");
            }
        });

  

    });

    function resetForm() {

        $("#hiddenInvestmentIncomeId").val("");
        $("#txtSActivityCode").val("");
        $("#ddlSTypeInvestment").val("");
        $("#bankInput").val("");
        $("#branchInput").val("");
        $("#txtSAccountNo").val("");
        $("#txtSWHTCertificateNo").val("");
        $("#txtSAmountInvested").val("");
        $("#txtSInterest").val("");
        $("#txtSWHTDeducted").val("");
        $("#txtSForeignTaxCredit").val("");
        $("#txtSOpeningBalance").val("");
        $("#txtSBalance").val("");
    }

    $(document).on('click', '.savingsDetails-editbtn', function () {
      
        $(".validation-error").remove();

        // Read all data-* attributes
        var id = $(this).data("id");
        var category = $(this).data("category");
        var activityCode = $(this).data("activitycode");
        var typeOfInvestment = $(this).data("typeofinvestment");
        var bankName = $(this).data("bankname");
        var bankBranch = $(this).data("bankbranch");
        var accountNo = $(this).data("accountno");
        var whtCertificateNo = $(this).data("whtcertificateno");
        var whtDeducted = $(this).data("whtdeducted");
        var openingBalance = $(this).data("openingbalance");
        var closingBalance = $(this).data("closingbalance");
        var incomeAmount = $(this).data("incomeamount");
        var foreignTaxCredit = $(this).data("foreigntaxcredit");
        var amountInvested = $(this).data("amountinvested");

        // Fill your form fields (IDs must match your inputs in the form)
        $("#txtSActivityCode").val(activityCode);
        $("#ddlSTypeInvestment").val(typeOfInvestment);
        $("#bankInput").val(bankName);
        $("#branchInput").prop("disabled", false).val(bankBranch); // enable branch field
        $("#txtSAccountNo").val(accountNo);
        $("#txtSWHTCertificateNo").val(whtCertificateNo);
        $("#txtSWHTDeducted").val(whtDeducted);
        $("#txtSOpeningBalance").val(openingBalance);
        $("#txtSBalance").val(closingBalance);
        $("#txtSInterest").val(incomeAmount);
        $("#txtSForeignTaxCredit").val(foreignTaxCredit);
        $("#txtSAmountInvested").val(amountInvested);

        $("#hiddenInvestmentIncomeId").val(id);
        $("#btnDetailsInvestmentSavings").text("Update");

    });
    $(document).on("click", "#btnDetailsInvestmentClear", function () {

        resetForm();
        $("#btnDetailsInvestmentSavings").text("Submit");

    });
    let deleteInvestmentId = null;
    let deleteCategoryName = null;

    $(document).on("click", ".investmentDetails-deletebtn", function () {
   
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
            url: '/SelfOnlineFlow/DeleteSelfOnlineInvestmentIncomeDetails',
            type: 'POST',
            data: deleteId,
            success: function (response) {
                $('#selfonline_confirmDeleteModal').modal('hide');
               
                $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                    if (deleteCategoryName == "Savings") {
                        $('#SavingsGrid').html($(html).find('#SavingsGrid').html());
                    }
                    if (deleteCategoryName == "FixedDeposit") {
                        $('#FDGrid').html($(html).find('#FDGrid').html());
                    }
                    if (deleteCategoryName == "Dividend") {
                        $('#DividentGrid').html($(html).find('#DividentGrid').html());
                    }
                    if (deleteCategoryName == "Rent") {
                        $('#RentGrid').html($(html).find('#RentGrid').html());
                    }
                    
                    //$("#taxTotal").text(newTotal);
                });
            },
            error: function () {
                alert("Error deleting.");
            }
        });
    });


    //----------------------- Interest Income from Savings Accounts End 


    //==============  FD ============


    $("#FDbankInput").on("input focus", function () {
    
        const q = $(this).val().toLowerCase();
        let results = allBanks;

        if (q) {
            results = results.filter(b =>
                (b.bankName && b.bankName.toLowerCase().startsWith(q))
            );
        }

        let html = "";
        if (results.length === 0) html = "<div>No banks match.</div>";
        results.forEach(b => {
            html += `<div data-id="${b.BankCode}" data-long="${b.bankName}">
                                          ${b.bankName} 
                                 </div>`;
        });

        $("#FDbankDropdown").html(html).show();
    });

    $("#FDbankDropdown").on("mousedown", "div", function () {
        const bankId = $(this).data("id");

        const bankLong = $(this).data("long");

        $("#FDbankInput").val(bankLong);
        $("#bank_id").val(bankId);

        selectedBank = bankId;

        $("#FDbranchInput")
            .prop("disabled", false)       // enable input
            .val("")                        // clear previous value
            .attr("placeholder", "Type branch name or code")
            .focus();

        //$("#branchInput").val("").prop("disabled", false).attr("placeholder", "Type branch name or code");
        $("#branch_code, #branch_name").val("");
        $("#FDbankDropdown").hide();
        $.getJSON(`https://mail.taxfiling.lk/getbranches/${selectedBank}`, (branches) => {
            if (!branches.length) return $("#FDbranchDropdown").hide();

            const html = branches.map(br =>
                `<div data-code="${br.BranchCode}" data-name="${br.BranchName}">${br.BranchName}</div>`
            ).join("");

            $("#FDbranchDropdown").html(html).show();
            $("#FDbranchDropdown div").removeClass("highlight").first().addClass("highlight");
            highlightedBranchIndex = 0; // first item highlighted
        });
    });

    $("#FDbranchDropdown").on("mousedown", "div", function () {
        const brCode = $(this).data("code");
        const brName = $(this).data("name");

        $("#FDbranchInput").val(brName);
        $("#branch_code").val(brCode);
        $("#branch_name").val(brName);

        $("#FDbranchDropdown").hide();
    });

    $(document).on("click", (e) => {
        if (!$(e.target).closest("#FDbankInput, #FDbankDropdown").length) {
            $("#FDbankDropdown").hide();
        }
        if (!$(e.target).closest("#FDbranchInput, #FDbranchDropdown").length) {
            $("#FDbranchDropdown").hide();
        }
    });

    $("#FDbankInput").on("keydown", function (e) {
        const items = $("#FDbankDropdown div");
        if (!items.length) return;

        if (e.key === "ArrowDown") {
            e.preventDefault();
            highlightedIndex = (highlightedIndex + 1) % items.length;
            items.removeClass("highlight").eq(highlightedIndex).addClass("highlight");
        }
        else if (e.key === "ArrowUp") {
            e.preventDefault();
            highlightedIndex = (highlightedIndex - 1 + items.length) % items.length;
            items.removeClass("highlight").eq(highlightedIndex).addClass("highlight");
        }
        else if (e.key === "Enter") {
            e.preventDefault();
            if (highlightedIndex >= 0) {
                const selected = items.eq(highlightedIndex);
                const bankId = selected.data("id");
                const bankName = selected.data("long");

                $("#FDbankInput").val(bankName);
                $("#bank_id").val(bankId);
                selectedBank = bankId;

                $("#FDbranchInput")
                    .prop("disabled", false)       // enable input
                    .val("")                        // clear previous value
                    .attr("placeholder", "Type branch name or code")
                    .focus();

                // reset branch
                //$("#branchInput").val("").prop("disabled", false).attr("placeholder", "Type branch name or code");
                $("#branch_code, #branch_name").val("");

                $("#FDbankDropdown").hide();

                $.getJSON(`https://mail.taxfiling.lk/getbranches/${selectedBank}`, (branches) => {
                    if (!branches.length) return $("#FDbranchDropdown").hide();

                    const html = branches.map(br =>
                        `<div data-code="${br.BranchCode}" data-name="${br.BranchName}">${br.BranchName}</div>`
                    ).join("");

                    $("#FDbranchDropdown").html(html).show();
                    //highlightedBranchIndex = -1; // reset highlight

                    $("#FDbranchDropdown div").removeClass("highlight").first().addClass("highlight");
                    highlightedBranchIndex = 0; // first item highlighted
                });
            }
        }
    });


    $("#FDbankDropdown").on("mouseenter", "div", function () {
        $("#FDbankDropdown div").removeClass("highlight");
        $(this).addClass("highlight");
        highlightedIndex = $(this).index();
    });

    $("#FDbranchInput").on("keydown", function (e) {

        const items = $("#FDbranchDropdown div");
        if (!items.length) return;

        if (e.key === "ArrowDown") {
            e.preventDefault();
            highlightedBranchIndex = (highlightedBranchIndex + 1) % items.length;
            items.removeClass("highlight").eq(highlightedBranchIndex).addClass("highlight");
        }
        else if (e.key === "ArrowUp") {
            e.preventDefault();
            highlightedBranchIndex = (highlightedBranchIndex - 1 + items.length) % items.length;
            items.removeClass("highlight").eq(highlightedBranchIndex).addClass("highlight");
        }
        else if (e.key === "Enter") {
            e.preventDefault();
            if (highlightedBranchIndex >= 0) {
                const selected = items.eq(highlightedBranchIndex);
                const brCode = selected.data("code");
                const brName = selected.data("name");

                $("#FDbranchInput").val(brName);
                $("#branch_code").val(brCode);
                $("#branch_name").val(brName);

                $("#FDbranchDropdown").hide();
                highlightedBranchIndex = -1;
            }
        }
    });

    $("#FDbranchDropdown").on("mouseenter", "div", function () {
        $("#FDbranchDropdown div").removeClass("highlight");
        $(this).addClass("highlight");
        highlightedBranchIndex = $(this).index();
    });

    $("#FDbranchInput").on("focus input", function () {
        if (!selectedBank) return; // No bank selected, do nothing

        const q = $(this).val().toLowerCase();

        $.getJSON(`https://mail.taxfiling.lk/getbranches/${selectedBank}`, (branches) => {
            if (!branches.length) return $("#FDbranchDropdown").hide();

            // Filter if user typed something
            const results = q
                ? branches.filter(br => br.BranchName.toLowerCase().startsWith(q))
                : branches;

            if (!results.length) return $("#FDbranchDropdown").hide();

            const html = results.map(br =>
                `<div data-code="${br.BranchCode}" data-name="${br.BranchName}">${br.BranchName}</div>`
            ).join("");

            $("#FDbranchDropdown").html(html).show();

            // Highlight first item
            $("#FDbranchDropdown div").removeClass("highlight").first().addClass("highlight");
            highlightedBranchIndex = 0;
        });
    });

    $(document).on("click", "#btnDetailsInvestmentFD", function () {

        var $btn = $(this);
        $btn.prop("disabled", true);

        // Get form values
        let selfOnlineInvestmentId = $("#hiddenInvestmentIncomeId").val();
        let activityCode = $("#txtFDActivityCode").val();
        let typeInvestment = $("#ddlFDTypeInvestment").val();
        let bankName = $("#FDbankInput").val();
        let bankBranch = $("#FDbranchInput").val();
        let accountNo = $("#txtFDAccountNo").val();
        let whtCertificateNo = $("#txtFDWHTCertificateNo").val();
        let amountInvested = $("#txtFDAmountInvested").val();
        let interestIncome = $("#txtFDInterest").val();
        let whtDeducted = $("#txtFDWHTDeducted").val();
        let foreignTaxCredit = $("#txtFDForeignTaxCredit").val();
        let openingBalance = $("#txtFDOpeningBalance").val();
        let closingBalance = $("#txtFDBalance").val();

        let isValid = true;

        // Remove old validation messages
        $(".validation-error").remove();

        // Validation
        if (!typeInvestment) {
            $("#ddlFDTypeInvestment").after('<div class="text-danger validation-error">Please select Type of Investment.</div>');
            isValid = false;
        }
        if (!bankName.trim()) {
            $("#FDbankInput").after('<div class="text-danger validation-error">Bank Name is required.</div>');
            isValid = false;
        }
        if (!accountNo.trim()) {
            $("#txtFDAccountNo").after('<div class="text-danger validation-error">Account No is required.</div>');
            isValid = false;
        }
        if (!amountInvested.trim()) {
            $("#txtFDAmountInvested").after('<div class="text-danger validation-error">Amount Invested is required.</div>');
            isValid = false;
        }


        if (!isValid) {
            $btn.prop("disabled", false);
            return;
        }

        // Prepare data for AJAX
        var fixedDepositData = {
            InvestmentIncomeDetailId: selfOnlineInvestmentId,
            TransactionType: selfOnlineInvestmentId ? "Edit" : "Add",
            Category: "FixedDeposit",
            ActivityCode: activityCode,
            TypeOfInvestment: typeInvestment,
            BankName: bankName,
            BankBranch: bankBranch,
            AccountNo: accountNo,
            WHTCertificateNo: whtCertificateNo,
            AmountInvested: amountInvested,
            IncomeAmount: interestIncome,
            WHTDeducted: whtDeducted,
            ForeignTaxCredit: foreignTaxCredit,
            OpeningBalance: openingBalance,
            ClosingBalance: closingBalance
        };
        // === AJAX URL ===
        var url = selfOnlineInvestmentId
            ? '/SelfOnlineFlow/UpdateSelfOnlineInvestmentIncomeDetails'
            : '/SelfOnlineFlow/AddSelfOnlineInvestmentIncomeDetails';

        $.ajax({
            url: url,
            type: 'POST',
            data: fixedDepositData,
            success: function (response) {
                $btn.prop("disabled", false);

                notifySuccess("", selfOnlineInvestmentId ? "Update successfully" : "Saved successfully");

                $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                    $('#FDGrid').html($(html).find('#FDGrid').html());
                });

                resetFormFD();
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving.");
            }
        });


    });
    $(document).on('click', '.fixedDepositDetails-editbtn', function () {
       $(".validation-error").remove();

        // Read all data-* attributes
        var id = $(this).data("id");
        var category = $(this).data("category");
        var activityCode = $(this).data("activitycode");
        var typeOfInvestment = $(this).data("typeofinvestment");
        var bankName = $(this).data("bankname");
        var bankBranch = $(this).data("bankbranch");
        var accountNo = $(this).data("accountno");
        var whtCertificateNo = $(this).data("whtcertificateno");
        var whtDeducted = $(this).data("whtdeducted");
        var openingBalance = $(this).data("openingbalance");
        var closingBalance = $(this).data("closingbalance");
        var incomeAmount = $(this).data("incomeamount");
        var foreignTaxCredit = $(this).data("foreigntaxcredit");
        var amountInvested = $(this).data("amountinvested");

        // Fill your form fields (IDs must match your inputs in the form)
        $("#txtFDActivityCode").val(activityCode);
        $("#ddlFDTypeInvestment").val(typeOfInvestment);
        $("#FDbankInput").val(bankName);
        $("#FDbranchInput").prop("disabled", false).val(bankBranch); // enable branch field
        $("#txtFDAccountNo").val(accountNo);
        $("#txtFDWHTCertificateNo").val(whtCertificateNo);
        $("#txtFDWHTDeducted").val(whtDeducted);
        $("#txtFDOpeningBalance").val(openingBalance);
        $("#txtFDBalance").val(closingBalance);
        $("#txtFDInterest").val(incomeAmount);
        $("#txtFDForeignTaxCredit").val(foreignTaxCredit);
        $("#txtFDAmountInvested").val(amountInvested);

        $("#hiddenInvestmentIncomeId").val(id);
        $("#btnDetailsInvestmentFD").text("Update");

    });

    $(document).on("click", "#btnDetailsInvestmentFDClear", function () {

        resetFormFD();
        $("#btnDetailsInvestmentFD").text("Submit");

    });

    function resetFormFD() {

        $("#hiddenInvestmentIncomeId").val("");
        $("#txtFDActivityCode").val("");
        $("#ddlFDTypeInvestment").val("");
        $("#FDbankInput").val("");
        $("#FDbranchInput").val("");
        $("#txtFDAccountNo").val("");
        $("#txtFDWHTCertificateNo").val("");
        $("#txtFDAmountInvested").val("");
        $("#txtFDInterest").val("");
        $("#txtFDWHTDeducted").val("");
        $("#txtFDForeignTaxCredit").val("");
        $("#txtFDOpeningBalance").val("");
        $("#txtFDBalance").val("");
    }


    /* ============= Divident  =================*/

    $(document).on("click", "#btnDetailsInvestmentDivident", function () {

        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collecting values from your form
        let selfOnlineInvestmentId = $("#hiddenInvestmentIncomeId").val();   // hidden field for edit/update
        let activityCode = $("#txtDActivityCode").val();
        let typeOfInvestment = $("#ddlDTypeInvestment").val();
        let amountInvested = $("#txtDAmountInvested").val();
        let dividendIncome = $("#txtDDividendIncome").val();
        let companyInstitution = $("#txtDNameCompanyInstitution").val();
        let sharesStocks = $("#txtDSharesStocks").val();
        let acquisitionDate = $("#txtDAcquisition").val();
        let costAcquisitionMarket = $("#txtDCostAcquisitionMarket").val();
        let whtDeducted = $("#txtDWHTDeducted").val();
        let foreignTaxCredit = $("#txtDForeignTaxCredit").val();

        let isValid = true;

        $(".validation-error").remove();

        if (!typeOfInvestment) {
            $("#ddlDTypeInvestment").after('<div class="text-danger validation-error">Please select Type of Invemsent.</div>');
            isValid = false;
        }
        if (!amountInvested) {
            $("#txtDAmountInvested").after('<div class="text-danger validation-error">Amount Invested is required</div>');
            isValid = false;
        }
        if (!dividendIncome.trim()) {
            $("#dividendIncome").after('<div class="text-danger validation-error">Dividend Income is required.</div>');
            isValid = false;
        }
       

        if (!isValid) {
            $btn.prop("disabled", false);
            return;
        }

        let dividendData = {
            InvestmentIncomeDetailId: selfOnlineInvestmentId,
            TransactionType: selfOnlineInvestmentId ? "Edit" : "Add",
            Category: "Dividend",
            ActivityCode: activityCode,
            TypeOfInvestment: typeOfInvestment,
            AmountInvested: amountInvested,
            NetDividendIncome: dividendIncome,
            CompanyInstitution: companyInstitution,
            SharesStocks: sharesStocks,
            AcquisitionDate: acquisitionDate,
            CostAcquisition: costAcquisitionMarket,
            WHTDeducted: whtDeducted,
            ForeignTaxCredit: foreignTaxCredit

        };
        // === AJAX URL ===
        var url = selfOnlineInvestmentId
            ? '/SelfOnlineFlow/UpdateSelfOnlineInvestmentIncomeDetails'
            : '/SelfOnlineFlow/AddSelfOnlineInvestmentIncomeDetails';

        $.ajax({
            url: url,
            type: 'POST',
            data: dividendData,
            success: function (response) {
                $btn.prop("disabled", false);

                notifySuccess("", selfOnlineInvestmentId ? "Update successfully" : "Saved successfully");

                $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                    $('#DividentGrid').html($(html).find('#DividentGrid').html());
                });

                resetFormDivident();
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving.");
            }
        });

      

    });

    function resetFormDivident() {

        $("#hiddenInvestmentIncomeId").val("");
        $("#txtDActivityCode").val("");
        $("#ddlDTypeInvestment").val("");
        $("#txtDAmountInvested").val("");
        $("#txtDDividendIncome").val("");
        $("#txtDNameCompanyInstitution").val("");
        $("#txtDSharesStocks").val("");
        $("#txtDAcquisition").val("");
        $("#txtDCostAcquisitionMarket").val("");
        $("#txtDWHTDeducted").val("");
        $("#txtDForeignTaxCredit").val("");

        
    }

    $('.dividend-editbtn').on('click', function () {
    
        $(".validation-error").remove();
        // Get row data from button attributes
        var id = $(this).data("id");
        var category = $(this).data("category");
        var activityCode = $(this).data("activitycode");
        var typeOfInvestment = $(this).data("typeofinvestment");
        var amountinvested = $(this).data("amountinvested");
        var company = $(this).data("company");
        var shares = $(this).data("shares");
        var acquisition = $(this).data("acquisition");
        var cost = $(this).data("cost");
        var netdividend = $(this).data("netdividend");
        var netwhtdeducted = $(this).data("netwhtdeducted");
        var netforeigntaxcredit = $(this).data("netforeigntaxcredit");

        $("#txtDActivityCode").val(activityCode);
        $("#ddlDTypeInvestment").val(typeOfInvestment);
        $("#txtDAmountInvested").val(amountinvested);
        $("#txtDDividendIncome").val(netdividend);
        $("#txtDNameCompanyInstitution").val(company);
        $("#txtDSharesStocks").val(shares);
        $("#txtDCostAcquisitionMarket").val(cost);
        $("#txtDWHTDeducted").val(netwhtdeducted);
        $("#txtDForeignTaxCredit").val(netforeigntaxcredit);

        

        if (acquisition) {
            // Format date for input[type="date"]
            let date = new Date(acquisition);
            let formatted = date.toISOString().split('T')[0];
            $("#txtDAcquisition").val(formatted);
        } else {
            $("#txtDAcquisition").val("");
        }

        $("#hiddenInvestmentIncomeId").val(id);
        $("#btnDetailsInvestmentDivident").text("Update");

    });

    /* ============= Divident  =================*/

    $(document).on("click", "#btnDetailsInvestmentRent", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collecting values from your form
        let selfOnlineInvestmentId = $("#hiddenInvestmentIncomeId").val();   // hidden field for edit/update
        let activityCode = $("#txtRActivityCode").val();
        let typeOfInvestment = $("#ddlRTypeInvestment").val();
        let sProperty = $("#txtRProperty").val();
        let addess = $("#txtRAddess").val();
        let rentIncome = $("#txtRTotalRentIncome").val();
        let ratesLocalAuthority = $("#txtRRatesLocalAuthority").val();
        let acquisitionDate = $("#txtRAcquisitionDate").val();
        let giftInhreted = $("#txtRCostGiftInhreted").val();
        let marketValue = $("#txtRMarketValue").val();
        let freignTaxCredit = $("#txtForeignTaxCredit").val();
       

        let isValid = true;
        $(".validation-error").remove();

        // === Validation ===
        if (!typeOfInvestment) {
            $("#ddlRTypeInvestment").after('<div class="text-danger validation-error">Please select Type of Investment.</div>');
            isValid = false;
        }
        if (!sProperty) {
            $("#txtRProperty").after('<div class="text-danger validation-error">Situation of property is required.</div>');
            isValid = false;
        }
        if (!rentIncome.trim()) {
            $("#txtRTotalRentIncome").after('<div class="text-danger validation-error">Rent Income is required.</div>');
            isValid = false;
        }

        if (!isValid) {
            $btn.prop("disabled", false);
            return;
        }

        // === Data Object ===
        let rentData = {
            InvestmentIncomeDetailId: selfOnlineInvestmentId,
            TransactionType: selfOnlineInvestmentId ? "Edit" : "Add",
            Category: "Rent",
            ActivityCode: activityCode,
            TypeOfInvestment: typeOfInvestment,
            PropertySituation: sProperty,
            PropertyAddress: addess,
            RatesLocalAuthority: ratesLocalAuthority,
            IncomeAmount: rentIncome,
            GiftOrInheritedCost: giftInhreted,
            AcquisitionDate: acquisitionDate,
            MarketValue: marketValue,
            ForeignTaxCredit: freignTaxCredit
          
        };

        // === AJAX URL ===
        var url = selfOnlineInvestmentId
            ? '/SelfOnlineFlow/UpdateSelfOnlineInvestmentIncomeDetails'
            : '/SelfOnlineFlow/AddSelfOnlineInvestmentIncomeDetails';

        $.ajax({
            url: url,
            type: 'POST',
            data: rentData,
            success: function (response) {
                $btn.prop("disabled", false);

                notifySuccess("", selfOnlineInvestmentId ? "Update successfully" : "Saved successfully");

                // Reload rent grid
                $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                    $('#RentGrid').html($(html).find('#RentGrid').html());
                });

                resetFormRent();
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving.");
            }
        });
    });

    function resetFormRent() {

        $("#hiddenInvestmentIncomeId").val("");
        $("#txtRActivityCode").val("");
        $("#ddlRTypeInvestment").val("");
        $("#txtRProperty").val("");
        $("#txtRAddess").val("");
        $("#txtRTotalRentIncome").val("");
        $("#txtRRatesLocalAuthority").val("");
        $("#txtRAcquisitionDate").val("");
        $("#txtRCostGiftInhreted").val("");
        $("#txtRMarketValue").val("");
        $("#txtForeignTaxCredit").val("");

    }
    $(document).on("click", ".rent-editbtn", function () {
    
        $(".validation-error").remove();
        // Get row data from button attributes
       
        var id = $(this).data("id");
        var activitycode = $(this).data("activity");
        var activity = $(this).data("activity");
        var property = $(this).data("property");
        var address = $(this).data("address");
        var income = $(this).data("income");
        var rates = $(this).data("rates");
        var cost = $(this).data("cost");
        var market = $(this).data("market");
        var foreigntax = $(this).data("foreigntax");
        var acquisition = $(this).data("acquisition");

        // Set values back to form
        $("#txtRActivityCode").val(activitycode);
        $("#ddlRTypeInvestment").val(activity);
        $("#txtRProperty").val(property);
        $("#txtRAddess").val(address);
        $("#txtRTotalRentIncome").val(income);
        $("#txtRRatesLocalAuthority").val(rates);
        $("#txtRCostGiftInhreted").val(cost);
        $("#txtRMarketValue").val(market);
        $("#txtForeignTaxCredit").val(foreigntax); // if you add deed field in form



        if (acquisition) {
            // Format date for input[type="date"]
            let date = new Date(acquisition);
            let formatted = date.toISOString().split('T')[0];
            $("#txtRAcquisitionDate").val(formatted);
        } else {
            $("#txtRAcquisitionDate").val("");
        }

        $("#hiddenInvestmentIncomeId").val(id);
        $("#btnDetailsInvestmentRent").text("Update");

    });

   
});