$(function () {

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

        let selfOnlineEmploymentIncomeId = $("#hiddenInvestmentIncomeId").val();
        let investmentIncome = $("#dpdInvestmentIncome").val();
        let remuneration = $("#txtSRemuneration").val();
        let gainsProfits = $("#txtGainsProfits").val();
        let txtinvestmentIncome = $("#txtInvestmentIncome").val();
        let bankName = $("#bankInput").val();
        let bankBranch = $("#branchInput").val();
        let accountNo = $("#txtAccountNo").val();
        let amountInvested = $("#txtAmountInvested").val();
        let interest = $("#txtInterest").val();
        let openingBalance = $("#txtOpeningBalance").val();
        let balance = $("#txtBalance").val();

        let isValid = true;

        // Remove old validation messages
        $(".validation-error").remove();


        if (!investmentIncome) {
            $("#dpdInvestmentIncome").after('<div class="text-danger validation-error">Please select Type of Investment Income.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!bankName) {
            $("#bankInput").after('<div class="text-danger validation-error">Please select Name of Bank Name</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!accountNo.trim()) {
            $("#txtAccountNo").after('<div class="text-danger validation-error">Account No is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!amountInvested.trim()) {
            $("#txtAmountInvested").after('<div class="text-danger validation-error">Amount Invested is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!balance.trim()) {
            $("#txtBalance").after('<div class="text-danger validation-error">Balance  is required.</div>');
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
                Category: "Savings",
                InvestmentIncomeType: investmentIncome,
                Remuneration: remuneration,
                GainsProfits: gainsProfits,
                TotalInvestmentIncome: txtinvestmentIncome,
                BankName: bankName,
                BankBranch: bankBranch,
                AccountNo: accountNo,
                AmountInvested: amountInvested,
                Interest: interest,
                OpeningBalance: openingBalance,
                Balance: balance

            }
            $.ajax({
                url: '/SelfOnlineFlow/UpdateInvestmentIncomeDetails',
                type: 'POST',
                data: employSavingsADD,
                success: function (response) {
                    $btn.prop("disabled", false);

                    notifySuccess("", "Update successfully");

                    $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                        $('#SavingsGrid').html($(html).find('#SavingsGrid').html()); // Direct replace

                        //$("#taxTotal").text(newTotal);
                    });

                    $("#dpdInvestmentIncome").val("");
                    $("#txtSRemuneration").val("");
                    $("#txtGainsProfits").val("");
                    $("#txtInvestmentIncome").val("");
                    $("#bankInput").val("");
                    $("#branchInput").val("");
                    $("#txtAccountNo").val("");
                    $("#txtAmountInvested").val("");
                    $("#txtInterest").val("");
                    $("#txtOpeningBalance").val("");
                    $("#txtBalance").val("");


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
                Category: "Savings",
                InvestmentIncomeType: investmentIncome,
                Remuneration: remuneration,
                GainsProfits: gainsProfits,
                TotalInvestmentIncome: txtinvestmentIncome,
                BankName: bankName,
                BankBranch: bankBranch,
                AccountNo: accountNo,
                AmountInvested: amountInvested,
                Interest: interest,
                OpeningBalance: openingBalance,
                Balance: balance

            }
            $.ajax({
                url: '/SelfOnlineFlow/AddInvestmentIncomeDetails',
                type: 'POST',
                data: employSavingsEdit,
                success: function (response) {
                    $btn.prop("disabled", false);

                    notifySuccess("", "Saved successfully");

                    $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                        $('#SavingsGrid').html($(html).find('#SavingsGrid').html()); // Direct replace
                    
                        //$("#taxTotal").text(newTotal);
                    });

                    $("#dpdInvestmentIncome").val("");
                    $("#txtRemuneration").val("");
                    $("#txtGainsProfits").val("");
                    $("#txtInvestmentIncome").val("");
                    $("#bankInput").val("");
                    $("#branchInput").val("");
                    $("#txtAccountNo").val("");
                    $("#txtAmountInvested").val("");
                    $("#txtInterest").val("");
                    $("#txtOpeningBalance").val("");
                    $("#txtBalance").val("");


                },
                error: function () {
                    $btn.prop("disabled", false);
                    alert("Error saving .");
                }
            });
        }

    });


    $('.savingsDetails-editbtn').on('click', function () {

        $(".validation-error").remove();
        // Get row data from button attributes
        var id = $(this).data("id");
        var incomeType = $(this).data("income-type");
        var remuneration = $(this).data("remuneration");
        var gains = $(this).data("gains");
        var total = $(this).data("total");
        var bankName = $(this).data("bank-name");
        var bankBranch = $(this).data("bank-branch");
        var accountNo = $(this).data("account-no");
        var amountInvested = $(this).data("amount-invested");
        var interest = $(this).data("interest");
        var openingBalance = $(this).data("opening-balance");
        var balance = $(this).data("balance");

        // Fill modal or form fields
        $("#hndSavingsId").val(id); // hidden field for ID
        $("#dpdInvestmentIncome").val(incomeType);
        $("#txtRemuneration").val(remuneration);
        $("#txtGainsProfits").val(gains);
        $("#txtInvestmentIncome").val(total);
        $("#bankInput").val(bankName);
        $("#branchInput").val(bankBranch);
        $("#txtAccountNo").val(accountNo);
        $("#txtAmountInvested").val(amountInvested);
        $("#txtInterest").val(interest);
        $("#txtOpeningBalance").val(openingBalance);
        $("#txtBalance").val(balance);

        $("#hiddenInvestmentIncomeId").val(id);
        $("#btnDetailsInvestmentSavings").text("Update");

    });
    let deleteInvestmentId = null;
    let deleteCategoryName = null;

    $('.savingsDetails-deletebtn').on('click', function () {

        deleteInvestmentId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });

    $('#selfonline_confirmDeleteBtn').on('click', function () {
        if (!deleteInvestmentId) return;

        var deleteId = {
            investmentIncomeId: deleteInvestmentId,
            categoryName: deleteCategoryName
        };


        $.ajax({
            url: '/SelfOnlineFlow/DeleteInvestmentIncomeDetail',
            type: 'POST',
            data: deleteId,
            success: function (response) {
                $('#selfonline_confirmDeleteModal').modal('hide');

                $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                    $('#SavingsGrid').html($(html).find('#SavingsGrid').html()); // Direct replace

                    //$("#taxTotal").text(newTotal);
                });
            },
            error: function () {
                alert("Error deleting.");
            }
        });
    });

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

        let selfOnlineEmploymentIncomeId = $("#hiddenInvestmentIncomeId").val();
        let investmentIncome = $("#dpdFDInvestmentIncome").val();
        let remuneration = $("#txtFDRemuneration").val();
        let gainsProfits = $("#txtFDGainsProfits").val();
        let txtinvestmentIncome = $("#txtFDInvestmentIncome").val();
        let bankName = $("#FDbankInput").val();
        let bankBranch = $("#FDbranchInput").val();
        let accountNo = $("#txtFDAccountNo").val();
        let amountInvested = $("#txtFDAmountInvested").val();
        let interest = $("#txtFDInterest").val();
        let openingBalance = $("#txtFDOpeningBalance").val();
        let balance = $("#txtFDBalance").val();

        let isValid = true;

        // Remove old validation messages
        $(".validation-error").remove();


        if (!investmentIncome) {
            $("#dpdFDInvestmentIncome").after('<div class="text-danger validation-error">Please select Type of Investment Income.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!bankName) {
            $("#FDbankInput").after('<div class="text-danger validation-error">Please select Name of Bank Name</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!accountNo.trim()) {
            $("#txtFDAccountNo").after('<div class="text-danger validation-error">Account No is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!amountInvested.trim()) {
            $("#txtFDAmountInvested").after('<div class="text-danger validation-error">Amount Invested is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!balance.trim()) {
            $("#txtFDBalance").after('<div class="text-danger validation-error">Balance  is required.</div>');
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
                Category: "FixedDeposit",
                InvestmentIncomeType: investmentIncome,
                Remuneration: remuneration,
                GainsProfits: gainsProfits,
                TotalInvestmentIncome: txtinvestmentIncome,
                BankName: bankName,
                BankBranch: bankBranch,
                AccountNo: accountNo,
                AmountInvested: amountInvested,
                Interest: interest,
                OpeningBalance: openingBalance,
                Balance: balance

            }
            $.ajax({
                url: '/SelfOnlineFlow/UpdateInvestmentIncomeDetails',
                type: 'POST',
                data: employSavingsADD,
                success: function (response) {
                    $btn.prop("disabled", false);

                    notifySuccess("", "Update successfully");

                    $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                        $('#FDGrid').html($(html).find('#FDGrid').html()); // Direct replace

                        //$("#taxTotal").text(newTotal);
                    });

                    $("#dpdFDInvestmentIncome").val("");
                    $("#txtFDRemuneration").val("");
                    $("#txtFDGainsProfits").val("");
                    $("#txtFDInvestmentIncome").val("");
                    $("#FDbankInput").val("");
                    $("#FDbranchInput").val("");
                    $("#txtFDAccountNo").val("");
                    $("#txtFDAmountInvested").val("");
                    $("#txtFDInterest").val("");
                    $("#txtFDOpeningBalance").val("");
                    $("#txtFDBalance").val("");


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
                Category: "FixedDeposit",
                InvestmentIncomeType: investmentIncome,
                Remuneration: remuneration,
                GainsProfits: gainsProfits,
                TotalInvestmentIncome: txtinvestmentIncome,
                BankName: bankName,
                BankBranch: bankBranch,
                AccountNo: accountNo,
                AmountInvested: amountInvested,
                Interest: interest,
                OpeningBalance: openingBalance,
                Balance: balance

            }
            $.ajax({
                url: '/SelfOnlineFlow/AddInvestmentIncomeDetails',
                type: 'POST',
                data: employSavingsEdit,
                success: function (response) {
                    $btn.prop("disabled", false);

                    notifySuccess("", "Saved successfully");

                    $.get('/SelfOnlineFlow/LoadInvestment_Detailsinvestment', function (html) {
                        $('#FDGrid').html($(html).find('#FDGrid').html()); // Direct replace

                        //$("#taxTotal").text(newTotal);
                    });

                    $("#dpdFDInvestmentIncome").val("");
                    $("#txtFDRemuneration").val("");
                    $("#txtFDGainsProfits").val("");
                    $("#txtFDInvestmentIncome").val("");
                    $("#FDbankInput").val("");
                    $("#FDbranchInput").val("");
                    $("#txtFDAccountNo").val("");
                    $("#txtFDAmountInvested").val("");
                    $("#txtFDInterest").val("");
                    $("#txtFDOpeningBalance").val("");
                    $("#txtFDBalance").val("");


                },
                error: function () {
                    $btn.prop("disabled", false);
                    alert("Error saving .");
                }
            });
        }

    });

    $('.fixedDepositDetails-editbtn').on('click', function () {
   
        $(".validation-error").remove();
        // Get row data from button attributes
        var id = $(this).data("id");
        var incomeType = $(this).data("income-type");
        var remuneration = $(this).data("remuneration");
        var gains = $(this).data("gains");
        var total = $(this).data("total");
        var bankName = $(this).data("bank-name");
        var bankBranch = $(this).data("bank-branch");
        var accountNo = $(this).data("account-no");
        var amountInvested = $(this).data("amount-invested");
        var interest = $(this).data("interest");
        var openingBalance = $(this).data("opening-balance");
        var balance = $(this).data("balance");

        // Fill modal or form fields
        $("#hiddenInvestmentIncomeId").val(id); // hidden field for ID
        $("#dpdFDInvestmentIncome").val(incomeType);
        $("#txtFDRemuneration").val(remuneration);
        $("#txtFDGainsProfits").val(gains);
        $("#txtFDInvestmentIncome").val(total);
        $("#FDbankInput").val(bankName);
        $("#FDbranchInput").val(bankBranch);
        $("#txtFDAccountNo").val(accountNo);
        $("#txtFDAmountInvested").val(amountInvested);
        $("#txtFDInterest").val(interest);
        $("#txtFDOpeningBalance").val(openingBalance);
        $("#txtFDBalance").val(balance);

        $("#btnDetailsInvestmentFD").text("Update");

    });


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