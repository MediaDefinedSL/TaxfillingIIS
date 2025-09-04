$(function () {

    $('#linkMaritalNext').on('click', function () {

        var selectedId = $("input[name='imgbackground']:checked").val();
        var selectedName = $("input[name='imgbackground']:checked").data("name");

        if (!selectedId) {
            alert("Please select a taxpayer.");
            return;
        }

        var payload = {
            Id: selectedId,
            Name: selectedName,
            SpouseFullName: $("#SpouseFullName").val(),
            SpouseTINNo: $("#SpouseTINNo").val(),
            SpouseNIC: $("#SpouseNIC").val(),
            NumberOfDependents: $("#NumberOfDependents").val()
            
        };

        if (selectedId) {
            console.log("Selected MaritalStatus ID: " + selectedId);
            $.ajax({
                url: '/SelfOnlineFlow/UpdateMaritalStatus',
                type: 'POST',
                data: payload,
                success: function (response) {
                    $.ajax({
                        url: '/SelfOnlineFlow/LoadTaxReturnLastyear',
                        type: 'GET',
                        success: function (data) {
                            $('#in-this-section-container').html(data);
                            $('.sub-link').removeClass('active');
                            $('#linkLastYear').addClass('active');
                            $("html, body").animate({ scrollTop: 0 }, "smooth");
                        },
                        error: function () {
                            alert("Error loading section content.");
                        }
                    });
                },
                error: function () {
                    alert("Error saving taxpayer.");
                }
            });

        } else {
            alert("Please select a taxpayer.");
        }
    });

    $('#linkMaritalContinue').on('click', function () {
      
        $.ajax({
            url: '/SelfOnlineFlow/LoadTaxPayer',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
                $('.sub-link').removeClass('active');
                $('#linkTaxPayer').addClass('active');
                $("html, body").animate({ scrollTop: 0 }, "smooth");
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

    });
    const radios = document.querySelectorAll(".imgbgchk");
     const spouseSection = document.getElementById("spouseDetails");
   

    function toggleSections(selectedName) {
        spouseSection.style.display = "none";
       
        if (selectedName === "married") {
            spouseSection.style.display = "block";
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