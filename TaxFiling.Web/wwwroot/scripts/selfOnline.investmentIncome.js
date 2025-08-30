$(function () {

    document.getElementById("ddlSTypeInvestment").addEventListener("change", function () {
        var selectedValue = this.value;
        document.getElementById("txtSActivityCode").value = selectedValue;
    });

    document.getElementById("ddlFDTypeInvestment").addEventListener("change", function () {
        var FDselectedValue = this.value;
        document.getElementById("txtFDActivityCode").value = FDselectedValue;
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


    /* ============= Divident  */

    $(document).on("click", "#btnDetailsInvestmentDivident", function () {

        var $btn = $(this);
        $btn.prop("disabled", true);

        let selfOnlineEmploymentIncomeId = $("#hiddenInvestmentIncomeId").val();
        let investmentIncome = $("#dpdDInvestmentIncome").val();
        let remuneration = $("#txtDRemuneration").val();
        let gainsProfits = $("#txtDGainsProfits").val();
        let txtinvestmentIncome = $("#txtDInvestmentIncome").val();

        let companyInstitution = $("#dpdDCompanyInstitution").val();
        let sharesStocks = $("#txtDSharesStocks").val();
        let acquisitiondate = $("#txtDAcquisition").val();
        let costAcquisitionMarket = $("#txtDCostAcquisitionMarket").val();
        let netDividend = $("#txtDNetDividend").val();
    

        let isValid = true;

        // Remove old validation messages
        $(".validation-error").remove();


        if (!investmentIncome) {
            $("#dpdFDInvestmentIncome").after('<div class="text-danger validation-error">Please select Type of Investment Income.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!companyInstitution) {
            $("#dpdDCompanyInstitution").after('<div class="text-danger validation-error">Please select Name of Company/Institution</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!netDividend.trim()) {
            $("#txtDNetDividend").after('<div class="text-danger validation-error">Net Dividend Income is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
       

        if (!isValid) {
            return;
        }


        if (selfOnlineEmploymentIncomeId) {

            var employSavingsADD = {
                SelfOnlineInvestmentId: selfOnlineEmploymentIncomeId,
                TransactionType: "Edit",
                Category: "Dividend",
                InvestmentIncomeType: investmentIncome,
                Remuneration: remuneration,
                GainsProfits: gainsProfits,
                TotalInvestmentIncome: txtinvestmentIncome,
                CompanyInstitution: companyInstitution,
                SharesStocks: sharesStocks,
                AcquisitionDate: acquisitiondate,
                CostAcquisition: costAcquisitionMarket,
                NetDividendIncome: netDividend
               

            }
            $.ajax({
                url: '/SelfOnlineFlow/UpdateInvestmentIncomeDetails',
                type: 'POST',
                data: employSavingsADD,
                success: function (response) {
                    $btn.prop("disabled", false);

                    notifySuccess("", "Update successfully");

                    $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                        $('#DividentGrid').html($(html).find('#DividentGrid').html()); // Direct replace

                        //$("#taxTotal").text(newTotal);
                    });

                    $("#dpdDInvestmentIncome").val("");
                    $("#txtDRemuneration").val("");
                    $("#txtFDGainsProfits").val("");
                    $("#txtDInvestmentIncome").val("");
                    $("#dpdDCompanyInstitution").val("");
                    $("#txtDSharesStocks").val("");
                    $("#txtDAcquisition").val("");
                    $("#txtDCostAcquisitionMarket").val("");
                    $("#txtDNetDividend").val("");
                    


                },
                error: function () {
                    $btn.prop("disabled", false);
                    alert("Error saving .");
                }
            });

        }
        else {
            var employSavingsEdit = {
                SelfOnlineInvestmentId: selfOnlineEmploymentIncomeId,
                TransactionType: "Add",
                Category: "Dividend",
                InvestmentIncomeType: investmentIncome,
                Remuneration: remuneration,
                GainsProfits: gainsProfits,
                TotalInvestmentIncome: txtinvestmentIncome,
                CompanyInstitution: companyInstitution,
                SharesStocks: sharesStocks,
                AcquisitionDate: acquisitiondate,
                CostAcquisition: costAcquisitionMarket,
                NetDividendIncome: netDividend

            }
            $.ajax({
                url: '/SelfOnlineFlow/AddInvestmentIncomeDetails',
                type: 'POST',
                data: employSavingsEdit,
                success: function (response) {
                    $btn.prop("disabled", false);

                    notifySuccess("", "Saved successfully");

                    $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                        $('#DividentGrid').html($(html).find('#DividentGrid').html()); // Direct replace

                        //$("#taxTotal").text(newTotal);
                    });

                    $("#dpdDInvestmentIncome").val("");
                    $("#txtDRemuneration").val("");
                    $("#txtFDGainsProfits").val("");
                    $("#txtDInvestmentIncome").val("");
                    $("#dpdDCompanyInstitution").val("");
                    $("#txtDSharesStocks").val("");
                    $("#txtDAcquisition").val("");
                    $("#txtDCostAcquisitionMarket").val("");
                    $("#txtDNetDividend").val("");


                },
                error: function () {
                    $btn.prop("disabled", false);
                    alert("Error saving .");
                }
            });
        }

    });

    $('.dividend-editbtn').on('click', function () {
    
        $(".validation-error").remove();
        // Get row data from button attributes
        var id = $(this).data("id");
        var incomeType = $(this).data("income-type");
        var remuneration = $(this).data("remuneration");
        var gains = $(this).data("gains");
        var total = $(this).data("total");
        var company = $(this).data("company");
        var shares = $(this).data("shares");
        var acquisition = $(this).data("acquisition");
        var cost = $(this).data("cost");
        var netDividend = $(this).data("net-dividend");
       

        // Fill modal or form fields
        $("#hiddenInvestmentIncomeId").val(id); // hidden field for ID
        $("#dpdDInvestmentIncome").val(incomeType);
        $("#txtDRemuneration").val(remuneration);
        $("#txtDGainsProfits").val(gains);
        $("#txtDInvestmentIncome").val(total);
        $("#dpdDCompanyInstitution").val(company);
        $("#txtDSharesStocks").val(shares);

        if (acquisition) {
            // Format date for input[type="date"]
            let date = new Date(acquisition);
            let formatted = date.toISOString().split('T')[0];
            $("#txtDAcquisition").val(formatted);
        } else {
            $("#txtDAcquisition").val("");
        }

        $("#txtDCostAcquisitionMarket").val(cost);
        $("#txtDNetDividend").val(netDividend);
       
        $("#btnDetailsInvestmentDivident").text("Update");

    });


});