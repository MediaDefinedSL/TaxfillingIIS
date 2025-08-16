$(function () {
    var dropdown = document.getElementsByClassName("dropdown-btn");
    var i;

    for (i = 0; i < dropdown.length; i++) {
        dropdown[i].addEventListener("click", function () {
            this.classList.toggle("active");
            var dropdownContent = this.nextElementSibling;
            if (dropdownContent.style.display === "block") {
                dropdownContent.style.display = "none";
            } else {
                dropdownContent.style.display = "block";
            }
        });
    }


    document.querySelectorAll('.sub-link').forEach(link => {
        link.addEventListener('click', function () {
            document.querySelectorAll('.sub-link').forEach(l => l.classList.remove('active'));
            this.classList.add('active');
        });
    });

    $('#btndashboardSection').on('click', function () {

        $('#in-this-section-container').show();
        hideAllSections();
        $.ajax({
            url: '/SelfOnlineFlow/LoadDashboardSection',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#btnLoadSection').on('click', function () {
       
        $('#in-this-section-container').show();
        hideAllSections();
        $('.sub-link').removeClass('active');
        $('#linkInThisSection').addClass('active');

        $.ajax({
            url: '/SelfOnlineFlow/LoadTaxAssistedInThisSection',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkInThisSection').on('click', function () {
        $('#in-this-section-container').show();
        hideAllSections();
        $.ajax({
            url: '/SelfOnlineFlow/LoadInThisSection',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkTaxPayer').on('click', function () {
        $('#in-this-section-container').show();
        hideAllSections();
        $.ajax({
            url: '/SelfOnlineFlow/LoadTaxPayer',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkMaritalStatus').on('click', function () {
        hideAllSections();
        $('#in-this-section-container').show();
        $.ajax({
            url: '/SelfOnlineFlow/LoadMaritalStatus',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkLastYear').on('click', function () {
        hideAllSections();
        $('#in-this-section-container').show();
        $.ajax({
            url: '/SelfOnlineFlow/LoadTaxReturnLastyear',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkIdentification').on('click', function () {
        hideAllSections();

        $('#in-this-section-container').show();
        $.ajax({
            url: '/SelfOnlineFlow/LoadIdentification',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkContactInformation').on('click', function () {
        hideAllSections();

        $('#in-this-section-container').show();
        $.ajax({
            url: '/SelfOnlineFlow/LoadContactInformation',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkSummary').on('click', function () {
        hideAllSections();
        
        $('#in-this-section-container').show();
        $.ajax({
            url: '/SelfOnlineFlow/LoadSummary',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
                $('#divSummaryAssistedNext').css("display", "block"); 
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkPersonalData').on('click', function () {
        hideAllSections();

        $('#in-this-section-container').show();
        $.ajax({
            url: '/SelfOnlineFlow/PersonalData',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkIncomeLiableTax').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadIncomeLiableTax',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

});

function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    sidebar.classList.toggle('collapsed');
}

function hideAllSections() {
    $('#employment-section, #documents-section, #bank-section, #assets-section, #other-section, #declaration-section').hide();
}