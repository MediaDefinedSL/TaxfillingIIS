$(function () {

 
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