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
        $('.sub-link').removeClass('active');
        $('#linkInThisSection').addClass('active');

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

    $('#linkInThisSection').on('click', function () {
       
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
        
        $.ajax({
            url: '/SelfOnlineFlow/LoadSummary',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkPersonalData').on('click', function () {
       
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

    //$('#linkIncomeLiableTax').on('click', function () {
    //    $.ajax({
    //        url: '/SelfOnlineFlow/LoadIncomeLiableTax',
    //        type: 'GET',
    //        success: function (data) {
    //            $('#in-this-section-container').html(data);
    //        },
    //        error: function () {
    //            alert("Error loading section content.");
    //        }
    //    });
    //});

    $('#linkEmploymentDetails').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadEmploymentDetails',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkTerminalBenefits').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadETerminalBenefits',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkExemptAmounts').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadExemptAmounts',
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