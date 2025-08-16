$('#linkTaxPayerNext').on('click', function () {  
    $('#in-this-section-container').show();
    $.ajax({
        url: '/SelfOnlineFlow/LoadContactInformation',
        type: 'GET',
        success: function (data) {
            $('#in-this-section-container').html(data);
            $('.sub-link').removeClass('active');
            $('#linkContactInformation').addClass('active');
        },
        error: function () {
            alert("Error loading section content.");
        }
    });

});

$('#linkTaxPayerContinue').on('click', function () {        
    $('#linkSummary').removeClass('active');    
    $('.dropdown-btn').each(function () {
        var dropdownContent = this.nextElementSibling;
        if (dropdownContent && dropdownContent.classList.contains("dropdown-container")) {
            dropdownContent.style.display = "none";
        }
    });
    showPanel("documents-section");

});

