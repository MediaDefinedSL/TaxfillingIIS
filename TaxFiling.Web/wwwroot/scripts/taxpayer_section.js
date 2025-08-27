$(function () {
   

    $('#linkTaxPayerContinue').on('click', function () {
        var selectedId = $("input[name='imgbackground']:checked").val();
        var selectedName = $("input[name='imgbackground']:checked").data("name");

        if (!selectedId) {
            alert("Please select a taxpayer.");
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
                    }
                });
            },
            error: function () {
                alert("Error saving taxpayer.");
            }
        });
    });


    $('#linkTaxPayerNext').on('click', function () {

        $.ajax({
            url: '/SelfOnlineFlow/LoadInThisSection',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
                $('.sub-link').removeClass('active');
                $('#linkInThisSection').addClass('active');
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

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

