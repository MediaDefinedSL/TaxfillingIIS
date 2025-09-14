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

    //$('#btnLoadSection').on('click', function () {        
    //    $('.sub-link').removeClass('active');
    //    $('#linkInThisSection').addClass('active');

    //    $.ajax({
    //        url: '/SelfOnlineFlow/LoadInThisSection',
    //        type: 'GET',
    //        success: function (data) {
    //            $('#in-this-section-container').html(data);
    //        },
    //        error: function () {
    //            alert("Error loading section content.");
    //        }
    //    });
    //});

    //$('#linkInThisSection').on('click', function () {
       
    //    $.ajax({
    //        url: '/SelfOnlineFlow/LoadInThisSection', 
    //        type: 'GET',
    //        success: function (data) {
    //            $('#in-this-section-container').html(data);
    //        },
    //        error: function () {
    //            alert("Error loading section content.");
    //        }
    //    });
    //});
    $('#btnLoadSection').on('click', function () {

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

    $('#btnIncomeTaxCredits').on('click', function () {
        $('.sub-link').removeClass('active');

        $('#linkEmploymentIncome').addClass('active');

        $.ajax({
            url: '/SelfOnlineFlow/LoadEmploymentIncome',
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
        $('.sub-link').removeClass('active');

        $('#linkEmploymentIncome').addClass('active');

        $.ajax({
            url: '/SelfOnlineFlow/LoadEmploymentIncome',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });


    $('#linkInvestmentIncome').on('click', function () {
        $('.sub-link').removeClass('active');
        $('#linkDetailsinvestment').addClass('active');

        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_Detailsinvestment',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkEmploymentIncome').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadEmploymentIncome',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

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

    // Investment Income

    $('#linkDetailsinvestment').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_Detailsinvestment',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });
    $('#linkPartnerInvestment').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_PartnerInvestment',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });
    $('#linkBeneficiaryInvestment').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_BeneficiaryInvestment',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });
    $('#linkInvestmentExemptAmounts').on('click', function () {
    
        $.ajax({
            url: '/SelfOnlineFlow/LoadInvestment_ExemptAmounts',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#btnSummarySection').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadSummarySection',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkDeductions').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadDeductions',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });
    $('#linkAssets').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadAssets',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });

    $('#linkLiabilities').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadLiabilities',
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

let allBranches = [];

function showMessage(message, type = "success", autoClose = true, autoCloseTime = 5000) {
    const modal = document.getElementById("enhancedModal");
    const modalContent = modal.querySelector(".enhanced-modal-content");
    const msgElem = document.getElementById("enhancedModalMessage");
    const closeBtn = document.getElementById("enhancedModalClose");

    // Set message text
    msgElem.textContent = message;

    // Remove previous type classes
    modalContent.classList.remove("modal-success", "modal-error", "modal-info");

    // Add current type class
    if (type === "success") modalContent.classList.add("modal-success");
    else if (type === "error") modalContent.classList.add("modal-error");
    else modalContent.classList.add("modal-info");

    // Show modal
    modal.style.display = "flex";

    // Close function
    function closeModal() {
        modal.style.display = "none";
        // Cleanup event listeners to avoid duplicates if called again
        closeBtn.removeEventListener("click", closeModal);
        window.removeEventListener("click", outsideClick);
        if (autoCloseTimer) clearTimeout(autoCloseTimer);
    }

    // Close when clicking close button
    closeBtn.addEventListener("click", closeModal);

    // Close when clicking outside modal content
    function outsideClick(e) {
        if (e.target === modal) closeModal();
    }
    window.addEventListener("click", outsideClick);

    // Auto-close timer if enabled
    let autoCloseTimer;
    if (autoClose) {
        autoCloseTimer = setTimeout(closeModal, autoCloseTime);
    }
}

