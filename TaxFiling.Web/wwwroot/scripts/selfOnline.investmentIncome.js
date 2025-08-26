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
        alert(123);
        var $btn = $(this);
        $btn.prop("disabled", true);

        let selfOnlineEmploymentIncomeId = $("#hiddenInvestmentIncomeId").val();
        let investmentIncome = $("#dpdInvestmentIncome").val();
        let remuneration = $("#txtRemuneration").val();
        let gainsProfits = $("#txtGainsProfits").val();
        let txtinvestmentIncome = $("#txtInvestmentIncome").val();
        let bankName = $("#dpdBankName").val();
        let bankBranch = $("#dpdBankBranch").val();
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
        //if (!bankInstitution) {
        //    $("#dpdBankInstitution").after('<div class="text-danger validation-error">Please select Name of bank/financial institution</div>');
        //    $btn.prop("disabled", false);
        //    isValid = false;
        //}
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
        


        var employInvestment = {
            SelfOnlineInvestmentId: selfOnlineEmploymentIncomeId,
            Category: "DetailsInvestment",
            TotalInvestmentIncome: investmentIncome ,
            Remuneration: remuneration,
            GainsProfits: gainsProfits,
            TotalInvestmentIncome: txtinvestmentIncome,
            BankName: bankName,
            BankBranch: bankBranch,
            AccountNo: accountNo,
            AmountInvested: amountInvested ,
            Interest: interest,
            OpeningBalance: openingBalance,
            Balance: balance

          }
          
              $.ajax({
                  url: '/SelfOnlineFlow/AddInvestmentIncomeDetails',
                  type: 'POST',
                  data: employInvestment,
                  success: function (response) {
                      $btn.prop("disabled", false);
  
                      notifySuccess("", "Saved successfully");
  
                      $.get('/SelfOnlineFlow/LoadIncomeLiableTax', function (html) {
                          //$('#employmentDetailsGrid').html($(html).find('#employmentDetailsGrid').html()); // Direct replace
                          //var newTotal = $(html).find("#spnEmploymentIncomeTotal").text();
                          //   $("#spnEmploymentIncomeTotal").text(newTotal);
                          // var newTaxTotal = $(html).find("#taxTotal").text();
                         // $("#taxTotal").text(newTotal);
                      });
  
                      //$("#drpTypeEmployment").val("Primary");
                      //$("#txtRemuneration").val("");
                      //$("#txtEmpDetailsECName").val("");
                      //$("#txtAPITPrimaryEmployment").val("");
                      //$("#txtTINEmployer").val("");
                      //$("#txtAPITSecondaryEmployment").val("");
  
                  },
                  error: function () {
                      $btn.prop("disabled", false);
                      alert("Error saving .");
                  }
              });

    });

   

});