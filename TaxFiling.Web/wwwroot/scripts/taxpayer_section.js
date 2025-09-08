$(function () {
   

    $('#linkTaxPayerContinue').on('click', function () {
        var selectedId = $("input[name='imgbackground']:checked").val();
        var selectedName = $("input[name='imgbackground']:checked").data("name");

        if (!selectedId) {
            showMessage("Please select a taxpayer.","error");
            return;
        }

        var payload = {
            TaxpayerId: selectedId,
            Name: selectedName,
            SpouseName: $("#SpouseName").val(),
            SpouseTINNo: $("#SpouseTINNo").val(),
            SpouseNIC: $("#SpouseNIC").val(),
            SomeoneName: $("input[name='OtherName']").val(),
            Relationship: $("input[name='OtherRelationship']").val(),
            SomeoneTINNo: $("input[name='OtherTIN']").val()
        };

        $.ajax({
            url: '/SelfOnlineFlow/UpdateTaxPayerID',
            type: 'PUT',
            data: payload,
            success: function (response) {
                $.ajax({
                    url: '/SelfOnlineFlow/LoadMaritalStatus',
                    type: 'GET',
                    success: function (data) {
                        $('#in-this-section-container').html(data);
                        $('.sub-link').removeClass('active');
                        $('#linkMaritalStatus').addClass('active');
                        $("html, body").animate({ scrollTop: 0 }, "smooth");
                    }
                });
            },
            error: function () {
                alert("Error saving taxpayer.");
            }
        });
    });

    function setStepsIndicatorProgress(element) {

        const el = document.getElementById(element);
        if (!el) return;
        const steps = document.querySelectorAll('.taxAssistedSteps span');
        if (!steps.length) return;

        switch (element) {

            case "employmentIncomeStatus":
                if (el.className == "text-success") {
                    steps[1].classList.add('completed');
                    console.log(steps[1]);
                }
                break;
            case "bankConfirmationStatus":
                if (el.className == "text-success") {
                    steps[2].classList.add('completed');
                }
                break;
            case "assetsStatus":
                if (el.className == "text-success") {
                    steps[3].classList.add('completed');
                }
                break;
            case "otherDocsStatus":
                if (el.className == "text-success") {
                    steps[4].classList.add('completed');
                }
                break;
            case "Declare":
                if (el.className == "text-success") {
                    steps[5].classList.add('completed');
                }
                break;
            case "submission":
                steps[6].classList.add('completed');
                break;
        }

    }


    $('#linkTaxPayerNext').on('click', function () {

        if (document.getElementById("docUploadStatus")) {

            $('#in-this-section-container').show();
            hideAllSections();
            $('.sub-link').removeClass('active');
            $('#linkInThisSection').addClass('active');

            $.ajax({
                url: '/SelfOnlineFlow/LoadTaxAssistedInThisSection',
                type: 'GET',
                success: function (data) {
                    $('#in-this-section-container').html(data);
                    setStepsIndicatorProgress("employmentIncomeStatus");
                    setStepsIndicatorProgress("bankConfirmationStatus");
                    setStepsIndicatorProgress("assetsStatus");
                    setStepsIndicatorProgress("otherDocsStatus");
                    setStepsIndicatorProgress("Declare");
                    setStepsIndicatorProgress("submission");
                    
                },
                error: function () {
                    alert("Error loading section content.");
                }
            });
        }
        else {
            $('#in-this-section-container').show();
            $.ajax({
                url: '/SelfOnlineFlow/LoadInThisSection',
                type: 'GET',
                success: function (data) {
                    $('#in-this-section-container').html(data);
                    $('.sub-link').removeClass('active');
                    $('#linkInThisSection').addClass('active');
                    $("html, body").animate({ scrollTop: 0 }, "smooth");
                },
                error: function () {
                    alert("Error loading section content.");
                }
            });
        }
        

    });

  
 
        const radios = document.querySelectorAll(".imgbgchk");
       // const spouseSection = document.getElementById("spouseDetails");
        const someoneElseSection = document.getElementById("someoneElseDetails");

        function toggleSections(selectedName) {
            //
            //spouseSection.style.display = "none";
            someoneElseSection.style.display = "none";

            //if (selectedName === "myself and my spouse") {
            //    spouseSection.style.display = "block";
            //}
            //else
            if (selectedName === "someone else") {
                someoneElseSection.style.display = "block";
            }
        }

        radios.forEach(radio => {
            radio.addEventListener("change", function () {
                toggleSections(this.dataset.name.trim().toLowerCase());
            });
        });

        // Show correct section if pre-selected
        const selected = document.querySelector(".imgbgchk:checked");
        if (selected) {
            toggleSections(selected.dataset.name.trim().toLowerCase());
        }
   

   

});

