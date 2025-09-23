
const sidebarLinks = document.querySelectorAll(".sidebar-link");
const contentPanels = document.querySelectorAll(".content-panel");


let selectedFile = null;
let selectedCategoryName = ""; // set this based on button click
let allDocs = [];
let globalUploadCount = 0;

const fileInput = document.getElementById('fileInput');
const submitBtn = document.getElementById('submitBtn');
const submitBankBtn = document.getElementById('submitBankBtn');
const submitOtherBtn = document.getElementById('submitOtherBtn');

const excelRadio = document.getElementById("optionExcel");
const manualRadio = document.getElementById("optionManual");
const excelSection = document.getElementById("excelSection");
const manualSection = document.getElementById("manualSection");

const collapse = document.getElementById('uploadedFilesCollapse');
const icon = document.getElementById('toggleIcon');
function showLoadMask() {
    //     document.getElementById('loadMask').style.display = 'block';
    document.getElementById("loadMask").style.display = "flex";
}
function hideLoadMask() {
    document.getElementById("loadMask").style.display = "none";
}

collapse.addEventListener('show.bs.collapse', () => {
    icon.classList.remove('bi-chevron-down');
    icon.classList.add('bi-chevron-up');
});

collapse.addEventListener('hide.bs.collapse', () => {
    icon.classList.remove('bi-chevron-up');
    icon.classList.add('bi-chevron-down');
});


const select = document.getElementById('AnyOtherType');
const message = document.getElementById('otherTypeAlert');

select.addEventListener('change', function () {
    if (select.value === 'Otherbenefits') {
        message.style.display = 'block'; // show message
    } else {
        message.style.display = 'none'; // hide message
    }
});


const toggleButton = document.getElementById("financialToggle");
const collapseIcon = document.getElementById("collapseIcon");
const collapseTarget = document.getElementById("financialDetailsCollapse");



function incrementUploadCount() {
    globalUploadCount++;
    $('#globalUploadCount').text(globalUploadCount);

    if (globalUploadCount > 0) {
        // const payload = {
        //      userId: userId,
        //      uploadedDocumentStatus: 1

        //  };
        const payload = {
            userId: userId,
            year: new Date().getFullYear(),
            docStatus: 1,
            isPersonalInfoCompleted: null,
            isIncomeTaxCreditsCompleted: null,
            updatedDate: new Date().toISOString()

        };


        //fetch(`${baseApiUrl}api/users/update-document-status`, {
        fetch(`${baseApiUrl}api/users/update-user-document-status`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    // Update status to NEW
                    const row = document.querySelector('#assessment-row-2024');
                    // Find the badge by ID
                    const badgeRow = document.querySelector('#status-badge-2024');
                    const badge = document.querySelector('#statusBadge');
                    //const removeBtn = document.getElementById("btnRemoveDraft");
                    const removeBtn = document.getElementById("btnRemoveDft");
                    // Update text and classes
                    if (badge) {
                        badge.textContent = 'DRAFT';
                        badge.classList.remove('bg-info');   // remove old status color
                        badge.classList.add('bg-warning');   // example new color


                    }
                    if (badgeRow) {
                        badgeRow.textContent = 'DRAFT';
                        badgeRow.classList.remove('bg-info');   // remove old status color
                        badgeRow.classList.add('bg-warning');   // example new color

                    }

                    if (removeBtn) {
                        removeBtn.style.display = "block";
                    }
                    updateProgressHeader(2024, "DRAFT", "bg-warning");



                }
                else
                    alert(data.message);
            })
            .catch(err => alert('Error updating status.'));
    }
}

function decrementUploadCount() {
    globalUploadCount--;
    $('#globalUploadCount').text(globalUploadCount);
}

function updateGlobalUploadCount() {
    if (globalUploadCount == 0) {
        globalUploadCount = allDocs.length;
    }

    $('#globalUploadCount').text(globalUploadCount);
    document.getElementById("fileCountLink").textContent = globalUploadCount + " files";
}

$("#btnRemoveDraft, #btnRemoveDft").click(function () {
    // Show confirmation modal
    $("#deleteConfirmationModal").modal("show");
});

async function deleteAllFilefromServer() {

    try {
        let year = new Date().getFullYear();
        const response = await fetch(`${fileSiteUrl}delete-all/${userId}/${year}`, {
            method: "DELETE"
        });

        if (!response.ok) {
            const errMsg = await response.text();
            showMessage("❌ Failed to delete document. " + errorText, "error");
            //throw new Error("Delete failed: " + errMsg);
        }
        else {
            const message = await response.text();
        }

        // Optionally refresh document list or remove the row from the table
    } catch (error) {
        console.error(error);
        alert("Error: " + error.message);
    }
}



$("#confirmDeleteDocsBtn").click(async function () {
    $("#deleteConfirmationModal").modal("hide");
    showLoadMask();

    $("tr[id^='assessment-row-']").each(async function () {
        const row = $(this);
        const statusBadge = row.find("span[id^='status-badge-']");
        const status = statusBadge.text().trim();

        if (status === "DRAFT") {
            const assessmentText = row.find("td:nth-child(2)").text().trim();
            const assessmentYear = assessmentText.split("/")[1]; // e.g., "2025"

            try {
                // Delete uploaded documents
                await fetch(`${baseApiUrl}api/UserUploadTaxAssistedDoc/DeleteAllByUserAndYear?userId=${encodeURIComponent(userId)}&assessmentYear=${encodeURIComponent(assessmentYear)}`, {
                    method: "DELETE"
                });

                // Delete draft data
                await fetch(`${baseApiUrl}api/UserTaxAssistedOtherAssetsDetails/DeleteDraftOtherTaxByUserAndYear?userId=${encodeURIComponent(userId)}&assessmentYear=${encodeURIComponent(assessmentText)}`, {
                    method: "DELETE"
                });

                deleteAllFilefromServer();

                // const payload = {
                //        userId: userId,
                //        uploadedDocumentStatus: 0
                //    };

                const payload = {
                    userId: userId,
                    year: new Date().getFullYear(),
                    docStatus: 0,
                    isPersonalInfoCompleted: null,
                    isIncomeTaxCreditsCompleted: null,
                    updatedDate: new Date().toISOString()

                };
                // fetch(`${baseApiUrl}api/users/update-document-status`, {
                fetch(`${baseApiUrl}api/users/update-user-document-status`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(payload)
                })
                    .then(res => res.json())
                    .then(data => {
                        if (data.success) {
                            globalUploadCount = 0;
                            // Update status to NEW
                            loadUploadedDocs(userId, null);
                            statusBadge.text("NEW")
                                .removeClass("bg-warning")
                                .addClass("bg-info text-dark");
                            $("#statusBadge")
                                .text("NEW")
                                .removeClass("bg-warning")
                                .addClass("bg-info text-dark");
                            $('#globalUploadCount').text("0");
                            updateRemoveBtn();
                            updateChecklistStatuses([], 0);
                            const btnRemoveDft = document.getElementById("btnRemoveDft");
                            btnRemoveDft.style.display = "none";
                            document.getElementById("fileCountLink").textContent = "0 files";

                            updateProgressHeader(2024, "NEW", "bg-info text-dark");

                            hideLoadMask();


                        }
                        else
                            alert(data.message);
                    })
                    .catch(err => alert('Error updating status.'));





                // Uncheck the checkbox
                // row.find("input[type='checkbox']").prop("checked", false);

            } catch (err) {
                console.error(`Failed to delete draft for ${assessmentYear}:`, err);
                alert(`Failed to delete draft data for ${assessmentYear}`);
            }
        }
    });

    //modalInstance.hide(); // Close modal after processing
});

function handleExcelFileChange(event) {
    const fileInput = event.target;
    const file = fileInput.files[0];
    const fileNameDisplay = document.getElementById('uploadedFileName');
    const validationMessage = document.getElementById('fileValidationMessage');
    const submitBtn = document.getElementById('submitExcelBtn');

    fileNameDisplay.textContent = "";
    validationMessage.textContent = "";
    submitBtn.disabled = true;

    if (!file) return;

    const fileName = file.name;
    const validExtension = /\.xlsx$/i;

    if (!validExtension.test(fileName)) {
        validationMessage.textContent = "Invalid file type. Please upload a .xlsx Excel file.";
        fileInput.value = ""; // Clear file
    } else {
        fileNameDisplay.textContent = "Selected file: " + fileName;
        submitBtn.disabled = false;
    }
}

function toggleManualTab(tab) {
    const localTab = document.getElementById('manualLocalTab');
    const foreignTab = document.getElementById('manualForeignTab');
    const btnLocal = document.getElementById('tabLocal');
    const btnForeign = document.getElementById('tabForeign');

    btnLocal.classList.remove('active');
    btnForeign.classList.remove('active');

    localTab.style.display = 'none';
    foreignTab.style.display = 'none';

    if (tab === 'local') {
        btnLocal.classList.add('active');
        localTab.style.display = 'block';
        localTab.classList.add('show', 'active');
        foreignTab.classList.remove('show', 'active');

        btnLocal.classList.add('active');
        btnLocal.classList.remove('btn-outline-primary');
        btnLocal.classList.add('btn-primary');

        btnForeign.classList.remove('active');
        btnForeign.classList.remove('btn-primary');
        btnForeign.classList.add('btn-outline-primary');
    } else {
        btnForeign.classList.add('active');
        foreignTab.style.display = 'block';
        foreignTab.classList.add('show', 'active');
        localTab.classList.remove('show', 'active');

        btnForeign.classList.add('active');
        btnForeign.classList.remove('btn-outline-primary');
        btnForeign.classList.add('btn-primary');

        btnLocal.classList.remove('active');
        btnLocal.classList.remove('btn-primary');
        btnLocal.classList.add('btn-outline-primary');
    }

}

function addMoreInstitutionTypeNote(index) {
    const container = document.getElementById(`institutionTypeNoteContainer-${index}`);

    const wrapper = document.createElement("div");
    wrapper.className = "institution-note-group mb-2 d-flex gap-2";

    const select = document.createElement("select");
    select.className = "form-select";
    select.name = `assets[${index}].InstitutionType`;
    select.innerHTML = `
    <option value="">Select Institution</option>
    <option>Share balances on the Sri Lanka Stock Exchange</option>
    <option>Investments in Treasury bills</option>
    <option>Investments in Treasury bonds</option>
    <option>Foreign investments</option>
    <option>Any other investments</option>
    `;

    const input = document.createElement("input");
    input.type = "text";
    input.name = `assets[${index}].Description`;
    input.className = "form-control";
    input.placeholder = "Value / Description";

    const removeBtn = document.createElement("button");
    removeBtn.type = "button";
    removeBtn.className = "btn btn-outline-danger btn-sm";
    removeBtn.innerHTML = "✖";
    removeBtn.onclick = function () {
        removeInstitutionNoteInput(removeBtn);
    };

    wrapper.appendChild(select);
    wrapper.appendChild(input);
    wrapper.appendChild(removeBtn);
    container.appendChild(wrapper);
}

function removeInstitutionNoteInput(button) {
    const wrapper = button.closest(".institution-note-group");
    if (wrapper) wrapper.remove();
}

function addMoreVehicleNote(index) {
    const container = document.getElementById(`vehicleNoteContainer-${index}`);

    const wrapper = document.createElement("div");
    wrapper.className = "vehicle-note-group mb-2 d-flex gap-2";

    const select = document.createElement("select");
    select.className = "form-select";
    select.name = `assets[${index}].vehicleTypes`;
    select.innerHTML = `
    <option value="">Select Vehicle Type</option>
    <option>Motor Car</option>
    <option>Van</option>
    <option>Bike</option>
    <option>Other</option>
    `;

    const input = document.createElement("input");
    input.type = "text";
    input.name = `assets[${index}].Description`;
    input.className = "form-control";
    input.placeholder = "Value / Description";

    const removeBtn = document.createElement("button");
    removeBtn.type = "button";
    removeBtn.className = "btn btn-outline-danger btn-sm";
    removeBtn.innerHTML = "✖";
    removeBtn.onclick = function () {
        removeVehicleNoteInput(removeBtn);
    };

    wrapper.appendChild(select);
    wrapper.appendChild(input);
    wrapper.appendChild(removeBtn);
    container.appendChild(wrapper);
}

function removeVehicleNoteInput(button) {
    const wrapper = button.closest(".vehicle-note-group");
    if (wrapper) wrapper.remove();
}

function addMoreNotes(index) {
    const container = document.getElementById(`notesContainer-${index}`);

    const wrapper = document.createElement("div");
    wrapper.className = "note-input-group mb-2 d-flex gap-2";

    const input = document.createElement("input");
    input.type = "text";
    input.name = `assets[${index}].Description`;
    input.className = "form-control";
    input.placeholder = "Enter value or notes";

    const removeBtn = document.createElement("button");
    removeBtn.type = "button";
    removeBtn.className = "btn btn-outline-danger btn-sm";
    removeBtn.innerHTML = "✖";
    removeBtn.onclick = function () {
        removeNoteInput(removeBtn);
    };

    wrapper.appendChild(input);
    wrapper.appendChild(removeBtn);
    container.appendChild(wrapper);
}

function removeNoteInput(button) {
    const wrapper = button.closest(".note-input-group");
    if (wrapper) wrapper.remove();
}

function addMoreFiles(rowId) {
    const container = document.getElementById(`extraFileInputs-${rowId}`);
    const fileInput = document.createElement("div");
    fileInput.classList.add("file-input-group", "mb-2");
    fileInput.innerHTML = `
    <div class="d-flex">
        <input type="file" name="assets[${rowId}].Files" class="form-control me-2" />
        <button type="button" class="btn btn-outline-danger btn-sm" onclick="this.parentElement.parentElement.remove()">
            ✖
        </button>
    </div>`;
    container.appendChild(fileInput);
}

function submitLocalAssets() {

    const isLocal = document.getElementById('tabLocal').classList.contains('active');
    const formData = new FormData();
    let assetIndex = 0;

    function processRows(containerSelector, isLocal) {
        let type = isLocal ? 1 : 2;
        const rows = $(`${containerSelector} table tbody tr`);

        rows.each(function (index) {
            const row = $(this);
            formData.append(`assets[${assetIndex}].UserId`, userId);
            formData.append(`assets[${assetIndex}].AssestType`, type);
            const assetCategory = row.find('input.AssetCategory[type="hidden"]').val();
            formData.append(`assets[${assetIndex}].AssetCategory`, assetCategory);
            // Description
            //const description = row.find('input[type="text"], select').first().val() || '';
            // formData.append(`assets[${assetIndex}].AssetNote`, description);

            // Vehicle Type (optional)
            // const vehicleType = row.find(`select[name="assets[${assetIndex}].vehicleTypes"]`).val();
            //if (vehicleType)
            //       formData.append(`assets[${assetIndex}].AssetVehicleType`, vehicleType);

            row.find(`select[name="assets[${assetIndex}].vehicleTypes"]`).each(function (typeIndex) {
                const typeVal = $(this).val();
                if (typeVal && typeVal.trim() !== "") {
                    formData.append(`assets[${assetIndex}].AssetVehicleTypes[${typeIndex}]`, typeVal);
                }
            });

            //const assestInstitution = row.find(`select[name="assets[${assetIndex}].InstitutionType"]`).val();
            //  if (assestInstitution)
            //     formData.append(`assets[${assetIndex}].AssetInstitution`, assestInstitution);
            row.find(`select[name="assets[${assetIndex}].InstitutionType"]`).each(function (typeIndex) {
                const typeVal = $(this).val();
                if (typeVal && typeVal.trim() !== "") {
                    formData.append(`assets[${assetIndex}].AssetInstitutions[${typeIndex}]`, typeVal);
                }
            });


            // Files
            row.find('input[type="file"]').each(function () {
                const input = this;
                if (input.files && input.files.length > 0) {
                    for (let i = 0; i < input.files.length; i++) {
                        formData.append(`assets[${assetIndex}].Files`, input.files[i]);
                    }
                }
            });
            //     row.find('input[name^="assets["][name$=".Description"]').each(function (noteIndex) {
            //     formData.append(`assets[${assetIndex}].AssetNotes[${noteIndex}]`, $(this).val());
            // });

            row.find(`input[name="assets[${assetIndex}].Description"]`).each(function (noteIndex) {
                const noteVal = $(this).val();
                if (noteVal && noteVal.trim() !== "") {
                    formData.append(`assets[${assetIndex}].AssetNotes[${noteIndex}]`, noteVal);
                }
            });

            assetIndex++;
        });

    }

    processRows('#manualLocalTab', true);  // Local assets
    processRows('#manualForeignTab', false); // Foreign assets

    $.ajax({
        url: `${baseApiUrl}api/UserUploadTaxAssistedDoc/SubmitAssets`, // Adjust path if needed
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            showMessage("Assets saved successfully!", "success");
            //alert("Assets saved successfully!");
            loadAssestsUploadedFiles(userId);
        },
        error: function (xhr) {
            alert("Error saving assets.");
            console.log(xhr.responseText);
        }
    });
}

document.getElementById('saveFinancialDetailsBtn').addEventListener('click', async () => {
    const container = document.getElementById('income-expenses');
    const dataArray = [];

    // Validation: Check mandatory fields
    const requiredFields = ["Cash in hand", "Living Expenses"];
    let isValid = true;
    let missingFields = [];

    requiredFields.forEach(fieldLabel => {
        const input = Array.from(container.querySelectorAll('label.form-label'))
            .find(label => label.innerText.replace(/^\*\s*/, '').trim() === fieldLabel)
            ?.closest('.col-md-6')
            ?.querySelector('input');

        if (!input || input.value.trim() === "") {
            isValid = false;
            missingFields.push(fieldLabel);
        }
    });

    if (!isValid) {
        //alert("Please fill in the required fields: " + missingFields.join(", "));
        showMessage("Please fill in the required fields: " + missingFields.join(", "), "error")
        return;
    }

    container.querySelectorAll('input').forEach(input => {
        const parentDiv = input.closest('.col-md-6');
        if (parentDiv) {
            const label = parentDiv.querySelector('label.form-label');
            if (label) {
                const value = input.value.trim();
                if (value !== "") {  // only add if not empty
                    dataArray.push({
                        Category: label.innerText.replace(/^\*\s*/, '').trim(), // remove leading * and spaces
                        Value: value
                    });
                }
            }
        }
    });

    // Add any other info you want, e.g., userId, assessmentYear
    const payload = {
        UserId: userId, // dynamically set
        AssessmentYear: '2024/2025',
        Details: dataArray
    };

    try {
        const response = await fetch(`${baseApiUrl}api/UserTaxAssistedOtherAssetsDetails/SaveUserOtherTaxDetails`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload),
        });

        if (!response.ok) {
            const err = await response.text();
            alert('Error: ' + err);
            return;
        }

        if (document.getElementById('saveFinancialDetailsBtn').innerText == "Save") {
            showMessage("Saved successfully!", "success");
            document.getElementById('saveFinancialDetailsBtn').innerText = "Update";
        }
        else {
            showMessage("Updated Successfully!", "success");
        }
    }
    catch (err) {
        alert('Failed to submit: ' + err.message);
    }
});

async function loadFinancialDetails(userId, assessmentYear) {
    try {
        const url = `${baseApiUrl}api/UserTaxAssistedOtherAssetsDetails/GetUserOtherTaxDetails?userId=${encodeURIComponent(userId)}&assessmentYear=${encodeURIComponent(assessmentYear)}`;
        const response = await fetch(url);

        //  if (!response.ok) {
        //     alert('Failed to load data');
        //     return;
        // }
        const data = await response.json();

        if (data && data.details && data.details.length > 0) {
            document.getElementById('saveFinancialDetailsBtn').innerText = "Update"; // change to update
        } else {
            document.getElementById('saveFinancialDetailsBtn').innerText = "Save";
        }

        const docStatus = document.getElementById("docUploadStatus")?.value;

        if (docStatus === "2") {
            document.getElementById('saveFinancialDetailsBtn').disabled = true;
        }

        // Clear all inputs first
        document.querySelectorAll('#income-expenses .col-md-6 input').forEach(input => {
            input.value = '';      // clear old values
            input.disabled = false; // re-enable unless docStatus disables again later
        });

        // Populate inputs by matching label text with category
        data.details.forEach(detail => {
            document.querySelectorAll('#income-expenses .col-md-6').forEach(div => {
                const label = div.querySelector('label.form-label');
                const input = div.querySelector('input');
                if (label && input) {
                    const labelText = label.innerText.replace("*", "").trim(); // remove * if present
                    if (labelText === detail.category) {
                        if (!isNaN(detail.value)) {
                            input.value = Number(detail.value).toLocaleString('en-US');
                        } else {
                            input.value = detail.value;
                        }

                        if (docStatus === "2") {
                            input.disabled = true;
                        }
                    } else if (docStatus === "2") {
                        input.disabled = true;
                    }
                }

            });
        });
    } catch (err) {
        alert('Error loading data: ' + err.message);
    }
}


function uploadAsset(index, assetType) {
    // Get the note input
    showLoadMask();
    const noteInput = $(`input[name="assets[${index}].Description"]`);
    const note = noteInput.val().trim();

    // Get the file input
    const fileInput = $(`input[name="assets[${index}].Files"]`)[0];
    const file = fileInput.files[0];

    // Vehicle Type (optional)
    const vehicleType = $(`select[name="assets[${index}].vehicleTypes"]`).val();
    const assestInstitution = $(`select[name="assets[${index}].InstitutionType"]`).val();
    const vehicleTypeSelect = $(`select[name="assets[${index}].vehicleTypes"]`);
    const institutionSelect = $(`select[name="assets[${index}].InstitutionType"]`);

    if (vehicleTypeSelect.length > 0) {   // check if dropdown exists                            
        if (!vehicleType || vehicleType.trim() === "") {
            hideLoadMask();
            isValid = false;
            vehicleTypeSelect.addClass("error"); // highlight
            showMessage("Please select vehicle Type", "error");
            return;
        }
        else {
            vehicleTypeSelect.removeClass("error");
        }
    }

    if (institutionSelect.length > 0) {   // check if dropdown exists

        if (!assestInstitution || assestInstitution.trim() === "") {
            hideLoadMask();
            isValid = false;
            institutionSelect.addClass("error");
            showMessage("Please select Institution Type", "error");
            return;
        }
        else {
            institutionSelect.removeClass("error");
        }
    }

    // Basic validation
    if (!note) {
        hideLoadMask();
        showMessage("Please enter a note or value.", "error");
        noteInput.addClass("error");
        noteInput.focus();
        return;
    }
    else {
        noteInput.removeClass("error");
    }
    if (!file) {
        hideLoadMask();
        showMessage("Please select a file to upload.", "error");
        $(fileInput).addClass("error");
        fileInput.focus();
        return;
    }
    else {
        $(fileInput).removeClass("error");
    }


    // Prepare form data for AJAX
    const formData = new FormData();
    // formData.append(`assets[${index}].AssetNote`, note);
    formData.append(`AssetNote`, note);
    formData.append(`Files`, file);
    formData.append(`AssetCategory`, $(".AssetCategory").eq(index).val()); // category hidden input
    formData.append(`UserId`, userId);
    formData.append(`AssestType`, assetType);
    if (vehicleType)
        formData.append(`AssetVehicleType`, vehicleType);
    if (assestInstitution)
        formData.append(`AssetInstitution`, assestInstitution);
    // Disable button to prevent multiple clicks
    const btn = $(`button[onclick='uploadAsset(${index})']`);
    btn.prop("disabled", true).text("Uploading...");

    // AJAX upload call (adjust URL to your API endpoint)
    $.ajax({
        url: `${baseApiUrl}api/UserUploadTaxAssistedDoc/SubmitAssets`,
        //url:`https://localhost:7119/api/UserUploadTaxAssistedDoc/SubmitAssets`,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            showMessage("Assets saved successfully!", "success");
            incrementUploadCount();
            //alert("Assets saved successfully!");
            loadAssestsUploadedFiles(userId);
            // Clear inputs or update UI as needed
            noteInput.val("");
            fileInput.value = "";
            vehicleTypeSelect.prop('selectedIndex', 0); // Reset vehicle type dropdown
            institutionSelect.prop('selectedIndex', 0); // Reset institution dropdown
        },
        error: function (xhr, status, error) {
            alert("Upload failed: " + error);
        },
        complete: function () {

            hideLoadMask();

            btn.prop("disabled", false).html('<i class="bi bi-cloud-upload-fill me-1"></i> Upload');
        },
    });
}


document.getElementById("fileInput").addEventListener("change", function () {
    if (this.files.length > 0) {
        selectedFile = this.files[0];

    }
});
document.getElementById("fileInputTerminal").addEventListener("change", function () {
    if (this.files.length > 0) {
        selectedFile = this.files[0];

    }
});
document.getElementById("fileInputAnyOther").addEventListener("change", function () {
    if (this.files.length > 0) {
        selectedFile = this.files[0];
    }
});
document.getElementById("fileInputBank").addEventListener("change", function () {
    if (this.files.length > 0) {
        selectedFile = this.files[0];
    }
});
document.getElementById("fileInputOther").addEventListener("change", function () {
    if (this.files.length > 0) {
        selectedFile = this.files[0];
    }
});

document.querySelectorAll('.category-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        document.querySelectorAll('.category-btn').forEach(b => b.classList.remove('active'));
        this.classList.add('active');
        const category = this.getAttribute('data-category');

        document.getElementById('t10-form').style.display = (category === 'T10') ? 'block' : 'none';
        document.getElementById('terminal-form').style.display = (category === 'TerminalBenefit') ? 'block' : 'none';
        document.getElementById('AnyOther-form').style.display = (category === 'AnyOther') ? 'block' : 'none';

        const categoryText = this.innerText.trim();
        selectedCategoryName = categoryText;

        const selectedCategory = this.dataset.category;
        const displayMap = {
            "T10": "T10",
            "TerminalBenefit": "Terminal Benefit",
            "AnyOther": "Any Other"
        };

        const readableCategory = displayMap[selectedCategory] || selectedCategory;
        document.getElementById("categoryHeading").innerHTML =
            `<span class="text-muted">Your Uploaded</span> <span class="badge bg-primary">${readableCategory}</span> <span class="text-muted">Documents for 2024/2025</span>`;
        renderDocsTable(categoryText); // Only filter already loaded docs
    });
});

// Listen for Bootstrap tab change event
document.querySelectorAll('#employmentTab button[data-bs-toggle="tab"]').forEach(btn => {
    btn.addEventListener('shown.bs.tab', function (event) {
        console.log("Now active tab:", event.target.id);       // New active tab
        console.log("Previously active tab:", event.relatedTarget?.id); // Old tab
        const selectedtab = this.getAttribute('data-bs-target').replace('#', ''); // t10, terminal, anyother

        // Map IDs to readable labels
        const displayMap = {
            "t10": "T10",
            "terminal": "Terminal Benefit",
            "anyother": "Any Other"
        };

        const selectedCategory = displayMap[selectedtab] || selectedtab;
        selectedCategoryName = selectedCategory;
        console.log(selectedCategory);
        // Update heading text
        document.getElementById("categoryHeading").innerHTML =
            `<span class="text-muted">Your Uploaded</span> <span class="badge bg-primary">${selectedCategory}</span> <span class="text-muted">Documents for 2024/2025</span>`;

        // Call your existing renderDocsTable function with proper label
        renderDocsTable(selectedCategory);
    });
});

function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    sidebar.classList.toggle('collapsed');
}
function showPanel(panelId) {

    document.getElementById('in-this-section-container').style.display = 'none';
    contentPanels.forEach(panel => {
        panel.style.display = (panel.id === panelId) ? "block" : "none";
    });

    // Remove "active" from all sidebar and sub-links
    document.querySelectorAll('.sidebar-link, .sub-link').forEach(link => {
        link.classList.remove("active");
    });

    // Remove "active" from all links
    //sidebarLinks.forEach(link => {
    //link.classList.remove("active");
    // });
    // Add "active" to current clicked link
    const activeLink = document.querySelector(`.sidebar-link[data-target="${panelId}"]`);
    if (activeLink) {
        activeLink.classList.add("active");
    }
    // ✅ If employment-section, auto-select T10
    if (panelId === "employment-section") {
        const t10Btn = document.querySelector(`.category-btn[data-category="T10"]`);
        if (t10Btn) t10Btn.click();
    }
}
sidebarLinks.forEach(link => {
    link.addEventListener("click", function (e) {
        e.preventDefault();
        const targetId = this.getAttribute("data-target");
        const panelId = this.dataset.target;

        showPanel(targetId);

        if (panelId === "bank-section") {
            selectedCategoryName = 'Bank Confirmation';
            loadUploadedDocs(userId, 'Bank Confirmation'); // userId must be available in this scope
        }
        if (panelId === "employment-section") {
            selectedCategoryName = 'T10';
            loadUploadedDocs(userId, 'T10'); // userId must be available in this scope
        }
        else if (panelId === "assets-section") {
            selectedCategoryName = 'Assets Liabilities';
            loadAssestsUploadedFiles(userId);// userId must be available in this scope
            loadExcelAssestsUploadedFiles(userId);
        }
        else if (panelId === "other-section") {
            selectedCategoryName = 'Other Documents';
            loadUploadedDocs(userId, 'Other Documents'); // userId must be available in this scope
            loadFinancialDetails(userId, '2024/2025');
        }
        else if (panelId === "declaration-section") {
            selectedCategoryName = 'Declaration';
            loadDeclarationDocs(userId);
        }

    });
});

function loadAssestsUploadedFiles(userId) {
    $.ajax({
        url: `${baseApiUrl}api/UserUploadTaxAssistedDoc/GetUploadedDocsByUser?userId=${userId}`,
        type: 'GET',
        success: function (data) {
            const collapseEl = document.getElementById('uploadedFilesCollapse');
            const collapseInstance = new bootstrap.Collapse(collapseEl, {
                show: true,
                toggle: false
            });
            collapseInstance.show();
            const tbody = $("#uploadedAssetsTable tbody");
            tbody.empty();
            // Filter only Asset or Liability categories in JS
            const filteredDocs = data.filter(doc =>
                doc.categoryName?.trim().toLowerCase() === "assets liabilities"
            );


            if (filteredDocs.length === 0) {
                tbody.append(`<tr><td colspan="7" class="text-center">No Assets/Liabilities uploaded.</td></tr>`);
                return;
            }

            filteredDocs.forEach((doc, index) => {
                const dateOnly = doc.uploadDate.split("T")[0];
                const assetTypeLabel = doc.assestType === 1 ? "Local" : "Foreign";
                const row = `
                                              <tr>
                                                  <td>${index + 1}</td>
                                                     <td>${doc.assetCategory}</td>
                                                     <td>${assetTypeLabel}</td>
                                                      <td>
                                                        ${doc.assetNote || ""}
                                                        ${doc.assetVehicleType ? ' - ' + doc.assetVehicleType : ''}
                                                                    ${!doc.assetVehicleType && doc.assetInstitution ? ' - ' + doc.assetInstitution : ''}
                                                      </td>
                                                        <td>${doc.originalName || ""}</td>
                                                           <td>${dateOnly}</td>
                                                  <td>
                                                       <button class="btn btn-primary btn-sm" onclick="viewDoc('${doc.fileName}', '${doc.decryptionKey}')">View</button>
                                                           <button class="btn btn-danger btn-sm delete-btn" onclick="showDeleteConfirm('${doc.userUploadId}', '${doc.uploadId}')">Delete</button>
                                                  </td>
                                              </tr>
                                          `;

                tbody.append(row);


                const docStatus = document.getElementById("docUploadStatus")?.value;
                updateChecklistStatuses(data, docStatus);

                if (docStatus === "2") {
                    // Disable all delete buttons after they're dynamically added
                    document.querySelectorAll(".delete-btn").forEach(btn => {
                        btn.disabled = true;
                        btn.title = "You cannot delete after submission";
                        btn.removeAttribute("onclick");
                    });

                }
            });
        },
        error: function (xhr) {
            console.error("Error loading uploaded documents", xhr.responseText);
        }
    });

}

function loadExcelAssestsUploadedFiles(userId) {
    $.ajax({
        url: `${baseApiUrl}api/UserUploadTaxAssistedDoc/GetUploadedDocsByUser?userId=${userId}`,
        type: 'GET',
        success: function (data) {
            // Filter only Asset or Liability categories in JS
            const filteredDocs = data.filter(doc =>
                doc.categoryName?.trim().toLowerCase() === "assests upload excel"
            );
            const tbody = $("#uploadedExcelAssetsTable tbody");
            tbody.empty();
            if (filteredDocs.length === 0) {
                tbody.append(`<tr><td colspan="5" class="text-center">No Assets/Liabilities uploaded.</td></tr>`);
                return;
            }

            filteredDocs.forEach((doc, index) => {
                const dateOnly = doc.uploadDate.split("T")[0];

                const row = `
                                                    <tr>
                                                        <td>${index + 1}</td>
                                                           <td>${doc.originalName}</td>
                                                           <td>${dateOnly}</td>
                                                        <td>
                                                             <button class="btn btn-primary btn-sm" onclick="viewDoc('${doc.fileName}', '${doc.decryptionKey}')">View</button>
                                                                 <button class="btn btn-danger btn-sm delete-btn" onclick="showDeleteConfirm('${doc.userUploadId}', '${doc.uploadId}')">Delete</button>
                                                        </td>
                                                    </tr>
                                                `;

                tbody.append(row);


                const docStatus = document.getElementById("docUploadStatus")?.value;
                updateChecklistStatuses(data, docStatus);

                if (docStatus === "2") {
                    // Disable all delete buttons after they're dynamically added
                    document.querySelectorAll(".delete-btn").forEach(btn => {
                        btn.disabled = true;
                        btn.title = "You cannot delete after submission";
                        btn.removeAttribute("onclick");
                    });

                }
            });
        },
        error: function (xhr) {
            console.error("Error loading uploaded documents", xhr.responseText);
        }
    });

}

function showBankConfirmationColumns() {
    // Hide default columns
    $('.col-employer-name').addClass('d-none');

    // Show bank-specific columns
    $('.col-confirmation-type, .col-bank-name').removeClass('d-none');
}

function showDefaultColumns() {
    // Show default columns
    $('.col-employer-name').removeClass('d-none');

    // Hide bank-specific
    $('.col-confirmation-type, .col-bank-name').addClass('d-none');
}

async function loadUploadedDocs(userId, selectedCategory = null) {
    //alert("2");
    if (selectedCategory == 'Bank Confirmation') {
        document.getElementById("uploadedBankDocs").style.display = "block";
    }
    else if (selectedCategory == 'Other Documents') {
        document.getElementById("uploadedOtherDocs").style.display = "block";
    }
    else {
        document.getElementById("uploadedDocs").style.display = "block";
    }


    try {
        const res = await fetch(`${baseApiUrl}api/UserUploadTaxAssistedDoc/GetUploadedDocsByUser?userId=${userId}`);
        if (!res.ok) {
            alert("Failed to load uploaded documents");
            return;
        }

        allDocs = await res.json();
        updateGlobalUploadCount();
        renderDocsTable(selectedCategory);
        updateChecklistStatuses(allDocs, docStatus);
    } catch (err) {
        console.error("Error loading documents:", err);
        alert("An error occurred while loading documents");
    }

}

function updateProgressHeader(year, newStatusText, newStatusClass) {
    const header = document.getElementById("progressHeader");
    // update text
    header.textContent = `${year}/${year + 1} – ${newStatusText}`;
    // reset classes (keep "card-header")
    header.className = "card-header " + newStatusClass;
}

function updateChecklistStatuses(docs, docStatus = null) {
    let totalItems = 5;
    let completed = 0;
    // employment income
    const hasEmploymentIncome = docs.some(d => d.categoryName === "T10");
    setStatus("employmentIncomeStatus", hasEmploymentIncome);
    if (hasEmploymentIncome) completed++;

    // bank confirmation
    const hasBankConfirmation = docs.some(d => d.categoryName === "Bank Confirmation");
    setStatus("bankConfirmationStatus", hasBankConfirmation);
    if (hasBankConfirmation) {
        completed++;
    }

    // assets
    const hasAssets = docs.some(d =>
        d.categoryName === "Assets Liabilities" ||
        d.categoryName === "Assests Upload Excel"
    );
    setStatus("assetsStatus", hasAssets);
    if (hasAssets) completed++;

    // other docs
    const hasOther = docs.some(d => d.categoryName === "Other Documents");
    setStatus("otherDocsStatus", hasOther);
    if (hasOther) completed++;

    // personal info (dummy logic, adapt as needed)
    const PersonalInfoVal = document.getElementById("personalInfoCompleted").value;
    const hasPersonalInfo = PersonalInfoVal == 1 ? true : false;
    setStatus("personalInfoStatus", hasPersonalInfo);
    if (hasPersonalInfo == 1) completed++;
    //setStatus("personalInfoStatus", hasPersonalInfo);

    let progress = 0;
    //const docStatus = document.getElementById("docUploadStatus")?.value;
    if (completed === totalItems && docStatus === 2) {
        progress = 100;
    }
    else if (completed === totalItems) {
        progress = 90; // all uploaded but not submitted
    } else if (completed === 1) {
        progress = 18; // one completed
    } else {
        progress = Math.round((completed / totalItems) * 90);
    }
    if (docStatus === 2) {
        progress = 100;
    }
    const bar = document.getElementById("progressBar");
    if (bar) {
        bar.style.width = progress + "%";
        bar.textContent = progress + "%";
    }
    updateCurrentAssessementProgressbar("2024", progress);

}

function updateCurrentAssessementProgressbar(year, progress) {
    const progressBar = document.querySelector(`#assessment-row-${year} .progress-bar`);
    if (progressBar) {
        progressBar.style.width = progress + "%";
        //progressBar.textContent = percent + "%"; // optional
    }
    if (progress == 100 && year == 2024)
        document.getElementById("btnViewSubmittedDocs").style.display = "block";
    else
        document.getElementById("btnViewSubmittedDocs").style.display = "none";

}



function setStatus(elementId, isUploaded) {
    const el = document.getElementById(elementId);
    if (!el) return;

    if (isUploaded) {
        el.textContent = "✓";
        el.className = "text-success";
    } else {
        el.textContent = "Missing";
        el.className = "text-danger";
    }
    //setStepsIndicatorProgress(elementId);
}



function renderDocsTable(categoryFilter = null) {
    let tableBody;
    if (categoryFilter === 'Bank Confirmation')
        tableBody = document.getElementById("documentsTableBankBody");
    else if (categoryFilter === 'Other Documents')
        tableBody = document.getElementById("documentsTableOtherBody");
    else
        tableBody = document.getElementById("documentsTableBody");

    // ❗️Check if the target element exists
    if (!tableBody) {
        console.error("Table body element not found for the selected category.");
        return;
    }

    tableBody.innerHTML = "";

    const filteredDocs = categoryFilter
        ? allDocs.filter(doc => doc.categoryName === categoryFilter)
        : allDocs;

    filteredDocs.forEach((doc, index) => {
        const dateOnly = doc.uploadDate.split("T")[0];
        let employmentValue = "";
        let bankConfirmationType = "";
        let bankName = "";

        if (doc.categoryName === "T10") {
            employmentValue = doc.t10EmployerName;
        } else if (doc.categoryName === "Terminal Benefit") {
            employmentValue = doc.terminalEmployerName;
        } else if (doc.categoryName === "Any Other") {
            employmentValue = doc.anyOtherType;
        } else if (doc.categoryName === "Other Documents") {
            employmentValue = doc.otherDocumentName;
        }

        employmentValue = employmentValue ?? "";
        const row = document.createElement("tr");

        bankConfirmationType = doc.bankConfirmationType ?? "";
        bankName = doc.bankName ?? "";

        row.innerHTML = `
    <td>${index + 1}</td>
    <td>${doc.originalName}</td>
    <td>${doc.categoryName}</td>
    <td class="col-employer-name d-none">${employmentValue}</td>
    <td class="col-confirmation-type d-none">${bankConfirmationType}</td>
    <td class="col-bank-name d-none">${bankName}</td>
    <td>${dateOnly}</td>
    <td>
        <button class="btn btn-primary btn-sm" onclick="viewDoc('${doc.fileName}', '${doc.decryptionKey}')">View</button>
        <button class="btn btn-danger btn-sm delete-btn" onclick="showDeleteConfirm('${doc.userUploadId}', '${doc.uploadId}')">Delete</button>
    </td>
    `;
        tableBody.appendChild(row);
    });

    if (categoryFilter === 'Bank Confirmation')
        showBankConfirmationColumns();
    else
        showDefaultColumns();

    const docStatus = document.getElementById("docUploadStatus")?.value;

    if (docStatus === "2") {
        // Disable all delete buttons after they're dynamically added
        document.querySelectorAll(".delete-btn").forEach(btn => {
            btn.disabled = true;
            btn.title = "You cannot delete after submission";
            btn.removeAttribute("onclick");
        });

    }

}
async function viewDoc(fileName, decryptionKey) {
    const width = 800;
    const height = 600;
    const left = (screen.width / 2) - (width / 2);
    const top = (screen.height / 2) - (height / 2);

    const formData = new FormData();
    formData.append("filename", fileName); // Match the API's expected field
    formData.append("decryptionKey", decryptionKey);
    formData.append("year", new Date().getFullYear().toString());

    const viewRes = await fetch(`${fileSiteUrl}view`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            filename: fileName,
            decryptionKey: decryptionKey,
            userId: userId,
            year: new Date().getFullYear().toString()
        })

    });

    if (!viewRes.ok) {
        const err = await viewRes.text();
        console.error("View API error:", err);
        throw new Error("External load failed: " + err);
    }
    const contentType = viewRes.headers.get("Content-Type") || "application/octet-stream";
    const blob = await viewRes.blob();
    const fileURL = URL.createObjectURL(new Blob([blob], { type: contentType }));

    window.open(
        fileURL,
        '_blank',
        `toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=${width},height=${height},top=${top},left=${left}`
    );
}
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

function showDeleteConfirm(userUploadId, uploadId) {

    const modalElement = document.getElementById("confirmDeleteModal");
    const originalDeleteBtn = document.getElementById("confirmDeleteBtn");

    const newDeleteBtn = originalDeleteBtn.cloneNode(true);
    originalDeleteBtn.parentNode.replaceChild(newDeleteBtn, originalDeleteBtn);

    newDeleteBtn.addEventListener("click", () => {
        const modalInstance = bootstrap.Modal.getInstance(modalElement);
        modalInstance.hide();

        setTimeout(() => {
            document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
            document.body.classList.remove('modal-open');
        }, 500);

        deleteDoc(userUploadId);
        deleteFilefromServer(uploadId);

    });

    const modal = new bootstrap.Modal(modalElement);
    modal.show();
}

async function deleteFilefromServer(uploadId) {

    try {
        let year = new Date().getFullYear();
        const response = await fetch(`${fileSiteUrl}file/${uploadId}/${userId}/${year}`, {
            method: "DELETE"
        });

        if (!response.ok) {
            const errMsg = await response.text();
            showMessage("❌ Failed to delete document. " + errorText, "danger");
            //throw new Error("Delete failed: " + errMsg);
        }
        else {
            const message = await response.text();
        }

        // Optionally refresh document list or remove the row from the table
    } catch (error) {
        console.error(error);
        alert("Error: " + error.message);
    }
}

async function deleteDoc(userUploadId) {

    try {
        const response = await fetch(`${baseApiUrl}api/UserUploadTaxAssistedDoc/DeleteDoc/${userUploadId}`, {
            method: "DELETE"
        });

        if (!response.ok) {
            const errMsg = await response.text();
            showMessage("❌ Failed to delete document. " + errorText, "error");
            //throw new Error("Delete failed: " + errMsg);
        }
        else {
            showMessage("✅ Document deleted successfully.", "success");
            const userId = window.AppConfig.userId;//'@User.FindFirst("UserID")?.Value';
            decrementUploadCount();
            loadUploadedDocs(userId, selectedCategoryName);
            if (selectedCategoryName == "Assets Liabilities" || selectedCategoryName == "Assests Upload Excel") {
                loadExcelAssestsUploadedFiles(userId);
                loadAssestsUploadedFiles(userId);
            }

        }

        // Optionally refresh document list or remove the row from the table
    } catch (error) {
        console.error(error);
        alert("Error: " + error.message);
    }
}

const acceptCheckbox = document.getElementById("acceptDeclaration");
const submitDeclareBtn = document.getElementById("submitDeclarationBtn");
const printBtn = document.getElementById("printDeclarationBtn");
const declarationSection = document.getElementById("declaration-section");

acceptCheckbox.addEventListener("change", function () {
    submitDeclareBtn.disabled = !this.checked;

});

submitDeclareBtn.addEventListener("click", function () {
    const bar = document.getElementById("progressBar");
    const widthValue = progressBar.style.width; // "54%"
    const progress = parseInt(widthValue, 10);
    if (acceptCheckbox.checked && progress == 90) {

        const userId = '@userId';
        const status = 2; // SUBMITTED
        const currentYear = new Date().getFullYear() - 1;
        const badgeId = `status-badge-${currentYear}`;
        const badge = document.getElementById(badgeId);
        // const payload = {
        //     userId: userId,
        //     uploadedDocumentStatus: 2
        // };
        const payload = {
            userId: userId,
            year: new Date().getFullYear(),
            docStatus: 2,
            isPersonalInfoCompleted: null,
            isIncomeTaxCreditsCompleted: null,
            updatedDate: new Date().toISOString()

        };

        //fetch(`${baseApiUrl}api/users/update-document-status`, {
        fetch(`${baseApiUrl}api/users/update-user-document-status`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    document.getElementById("docUploadStatus").value = status;
                    acceptCheckbox.disabled = true;
                    document.getElementById("btnMainDashboardNext").style.display = "block";
                    document.getElementById('saveFinancialDetailsBtn').style.display = "none";
                    if (badge) {
                        badge.innerText = "SUBMITTED";
                        badge.className = "badge bg-success";
                    }
                    //Disable delete and submit buttons
                    document.querySelectorAll('.delete-btn, .submit-btn').forEach(btn => {
                        btn.disabled = true;
                        btn.title = "Disabled after submission";

                        // Optional: remove onclick handler
                        btn.onclick = function (e) {
                            e.preventDefault();
                            e.stopPropagation();
                            return false;
                        };
                    });

                    updateProgressHeader(2024, "SUBMITTED", "bg-success text-white");
                    const btnContinue = document.getElementById("btnContinue");
                    const btnUploadDocs = document.getElementById("btnUploadDocs");
                    const btnRemoveDft = document.getElementById("btnRemoveDft");
                    const btnViewSubmittedDocs = document.getElementById("btnViewSubmittedDocs");
                    const bar = document.getElementById("progressBar");
                    if (bar) {
                        bar.style.width = "100%";
                        bar.textContent = "100%";
                    }
                    if (btnContinue)
                        btnContinue.style.display = "none";
                    if (btnUploadDocs)
                        btnUploadDocs.style.display = "none";
                    if (btnRemoveDft)
                        btnRemoveDft.style.display = "none";
                    if (btnViewSubmittedDocs)
                        btnViewSubmittedDocs.style.display = "block";

                    updateChecklistStatuses(allDocs, 2);
                    updateCurrentAssessementProgressbar(2024, 100);
                    $("#statusBadge")
                        .text("SUBMITTED")
                        .removeClass("bg-warning")
                        .addClass("badge bg-success");


                    this.disabled = true;

                    this.innerText = "Terms Accepted";
                    const steps = document.querySelectorAll('.taxAssistedSteps span');
                    if (!steps.length) return;
                    steps[5].classList.add('completed');
                    steps[6].classList.add('completed');
                    //alert("Your Documents submitted successfully");
                    showMessage("Your Documents submitted successfully and Tax admins are contact you ASAP!", "success");
                    printBtn.disabled = false;




                }
                else
                    alert(data.message);
            })
            .catch(err => alert('Error updating status.'));

    }
    else {
        showMessage("Please check all sections and upload all relevant files before submit", "error")
        acceptCheckbox.checked = false;
        this.disabled = true;

    }
});

printBtn.addEventListener("click", function () {
    window.print();
});

function toggleUpload(checkboxId, fileInputId) {
    const checkbox = document.getElementById(checkboxId);
    const fileInput = document.getElementById(fileInputId);
    if (checkbox && fileInput) {
        fileInput.style.display = checkbox.checked ? "block" : "none";
    }
}

async function loadDeclarationDocs(userId) {
    try {
        const res = await fetch(`${baseApiUrl}api/UserUploadTaxAssistedDoc/GetUploadedDocsByUser?userId=${userId}`);
        if (!res.ok) {
            alert("Failed to load declaration documents.");
            return;
        }

        const allDocs = await res.json();
        const tbody = $("#declarationDocsTable tbody");
        tbody.empty();
        $("#uploadedDeclarationDocs").show();
        if (document.getElementById("acceptDeclaration").checked)
            document.getElementById("btnMainDashboardNext").style.display = "block";

        if (allDocs && allDocs.length > 0) {
            //$("#uploadedDeclarationDocs").show();
            allDocs.forEach((doc, index) => {
                const uploadDate = doc.uploadDate ? doc.uploadDate.split("T")[0] : "";
                const assetTypeLabel = doc.categoryName === "Assets Liabilities"
                    ? (doc.assestType === 1 ? "Local" : "Foreign") : "";
                const displayName = doc.originalName?.trim() || doc.assetNote;
                let assetCategoryDisplayName = "";
                if (doc.categoryName === "T10")
                    assetCategoryDisplayName = doc.t10EmployerName ?? "";
                else if (doc.categoryName === "Bank Confirmation")
                    assetCategoryDisplayName = `${doc.bankConfirmationType ?? ""} ${doc.bankName ?? ""}`.trim();
                else if (doc.categoryName === "Terminal Benefit")
                    assetCategoryDisplayName = `${doc.terminalEmployerName ?? ""}`;
                else if (doc.categoryName === "Any Other")
                    assetCategoryDisplayName = doc.anyOtherType;
                else if (doc.categoryName === "Assets Liabilities")
                    assetCategoryDisplayName = doc.assetCategory;
                else if (doc.categoryName === "Other Documents")
                    assetCategoryDisplayName = `${doc.otherDocumentName ?? ""}`;
                const row = `
                                          <tr>
                                              <td>${index + 1}</td>
                                                <td>${doc.categoryName}</td>
                                                    <td>${assetCategoryDisplayName || "NA"}</td>
                                                    <td class="no-wrap">${uploadDate}</td>

                                          </tr>
                                      `;
                tbody.append(row);
            });
        } else {
            tbody.append(`
                                       <tr>
                                           <td colspan="4" class="text-center">No files uploaded.</td>
                                       </tr>
                                   `);
            //$("#uploadedDeclarationDocs").hide();
        }
        const assessmentYear = "2024/2025";
        const otherTbody = $("#otherTaxPaymentsTable tbody");
        otherTbody.empty();
        // --- Second API: Other Tax Details ---
        const resOtherAssets = await fetch(`${baseApiUrl}api/UserTaxAssistedOtherAssetsDetails/GetUserOtherTaxDetails?userId=${encodeURIComponent(userId)}&assessmentYear=${encodeURIComponent(assessmentYear)}`);
        if (resOtherAssets.ok) {
            const otherAssets = await resOtherAssets.json();
            if (otherAssets && otherAssets.details.length > 0) {
                document.getElementById("otherTaxHeader").style.display = "block";
                // $("#uploadedDeclarationDocs").show();
                $("#otherTaxPaymentsTable").show();
                otherAssets.details.forEach((doc, index) => {
                    const row = `
    <tr>
        <td>${otherTbody.children().length + 1}</td>
        <td>${doc.category}</td>
        <td>${Number(doc.value).toLocaleString('en-US')}</td>
        <td class="no-wrap">${doc.createdDate.split("T")[0]}</td>
    </tr>
    `;
                    otherTbody.append(row);
                });
            } else {
                document.getElementById("otherTaxHeader").style.display = "none";
                $("#otherTaxPaymentsTable").hide();
            }
        } else {
            alert("Failed to load other tax details.");
        }

    } catch (err) {
        console.error("Error loading declaration docs:", err);
        alert("An error occurred while fetching declaration documents.");
    }
}

function updateRemoveBtn() {
    // find the checked row
    const checked = document.querySelector(".checkbox-lg:checked");
    const removeBtn = document.getElementById("btnRemoveDraft");
    if (checked) {
        const row = checked.closest("tr");
        const status = row.querySelector("span.badge").innerText.trim();

        // disable remove button if status is SUBMITTED or NEW
        if (status === "SUBMITTED" || status === "NEW") {
            removeBtn.disabled = true;
        } else {
            removeBtn.disabled = false;
        }
    } else {
        removeBtn.disabled = true; // nothing selected
    }

}

function updateSidebar(status) {
    if (status === -1) { // incomplete
        // $(".sidebar-link").not("#btnLoadSection").addClass("disabled-link").off("click").on("click", function(e){
        //     e.preventDefault();
        //     showMessage("Please complete Personal Information before accessing other sections.","error");
        // });

        $("#documents-submenu a").addClass("disabled-link").off("click").on("click", function (e) {
            e.preventDefault();
            showMessage("Please complete Personal Information first.", "error");
        });
    } else { // complete
        $(".sidebar-link").removeClass("disabled-link").off("click");
        $("#documents-submenu a").removeClass("disabled-link").off("click");
    }
}



//page load
document.addEventListener("DOMContentLoaded", function () {



    //$(document).on("personalInfoCompleted", function(e, newStatus) {                        
    //updateSidebar(newStatus);
    //});

    //var status = parseInt($("#personalInfoCompleted").val() || 0);
    //updateSidebar(status);


    const checkboxes = document.querySelectorAll(".checkbox-lg");
    const removeBtn = document.getElementById("btnRemoveDraft");
    const sidebarLinks = document.querySelectorAll(".sidebar-link");

    checkboxes.forEach(cb => {
        cb.addEventListener("change", function () {
            if (this.checked) {
                // uncheck all others
                checkboxes.forEach(other => {
                    if (other !== this) other.checked = false;
                });
                setStatus("employmentIncomeStatus", true);
                setStatus("bankConfirmationStatus", true);
                setStatus("assetsStatus", true);
                setStatus("otherDocsStatus", true);
                const row = this.closest("tr");
                const yearText = parseInt(row.cells[1].textContent.trim().split("/")[0], 10); // 2024
                const currentYear = new Date().getFullYear();
                const statusText = row.cells[4].textContent.trim();
                const statusBadge = row.querySelector("span.badge");
                const newStatusClass = statusBadge.className.replace("badge", "").trim();
                const progressBar = row.querySelector(".progress-bar");
                const widthValue = progressBar.style.width; // "54%"
                const progress = parseInt(widthValue, 10);

                if (yearText + 1 != currentYear) {
                    sidebarLinks.forEach(link => {
                        link.classList.add("disabled");
                        link.style.pointerEvents = "none";  // block clicks
                        link.style.opacity = "0.5";         // dim look
                    });

                    updateProgressHeader(yearText, "SUBMITTED", "bg-success text-white");
                    const btnContinue = document.getElementById("btnContinue");
                    const btnUploadDocs = document.getElementById("btnUploadDocs");
                    const btnRemoveDft = document.getElementById("btnRemoveDft");
                    const btnViewSubmittedDocs = document.getElementById("btnViewSubmittedDocs");
                    const bar = document.getElementById("progressBar");
                    if (bar) {
                        bar.style.width = progress + "%";
                        bar.textContent = progress + "%";
                    }
                    if (btnContinue)
                        btnContinue.style.display = "none";
                    if (btnUploadDocs)
                        btnUploadDocs.style.display = "none";
                    if (btnRemoveDft)
                        btnRemoveDft.style.display = "none";
                    if (btnViewSubmittedDocs)
                        btnViewSubmittedDocs.style.display = "none";

                }
                else {

                    sidebarLinks.forEach(link => {
                        link.classList.remove("disabled");
                        link.style.pointerEvents = "auto";
                        link.style.opacity = "1";
                    });

                    updateProgressHeader(yearText, statusText, newStatusClass);
                    updateChecklistStatuses(allDocs, parseInt(document.getElementById("docUploadStatus").value, 10));
                    const bar = document.getElementById("progressBar");
                    if (bar) {
                        bar.style.width = progress + "%";
                        bar.textContent = progress + "%";
                    }
                    if (parseInt(document.getElementById("docUploadStatus").value, 10) == 1) {
                        if (btnContinue)
                            btnContinue.style.display = "block";
                        if (btnUploadDocs)
                            btnUploadDocs.style.display = "block";
                        if (btnRemoveDft)
                            btnRemoveDft.style.display = "block";
                        if (btnViewSubmittedDocs)
                            btnViewSubmittedDocs.style.display = "none";


                    }
                    else if (parseInt(document.getElementById("docUploadStatus").value, 10) == 0) {
                        if (btnContinue)
                            btnContinue.style.display = "block";
                        if (btnUploadDocs)
                            btnUploadDocs.style.display = "block";

                        if (btnRemoveDft)
                            btnRemoveDft.style.display = "none";
                        if (btnViewSubmittedDocs)
                            btnViewSubmittedDocs.style.display = "none";


                    }
                    else if (parseInt(document.getElementById("docUploadStatus").value, 10) == 2) {

                        if (btnContinue)
                            btnContinue.style.display = "none";
                        if (btnUploadDocs)
                            btnUploadDocs.style.display = "none";

                        if (btnRemoveDft)
                            btnRemoveDft.style.display = "none";
                        if (btnViewSubmittedDocs)
                            btnViewSubmittedDocs.style.display = "block";



                    }

                }




            }

            updateRemoveBtn();
        });
    });

    // Allow only numeric input
    $(".numeric-input").on("keypress", function (e) {
        var charCode = (e.which) ? e.which : e.keyCode;
        // Allow numbers, backspace, delete
        if (charCode != 8 && charCode != 46 && (charCode < 48 || charCode > 57)) {
            e.preventDefault();
        }
    });

    // Auto-format number on blur (e.g., 1000 → 1,000)
    $(".numeric-input").on("blur", function () {
        var value = $(this).val().replace(/,/g, '');
        if (value) {
            $(this).val(Number(value).toLocaleString('en-US'));
        }
    });

    // Remove formatting on focus for easy editing
    $(".numeric-input").on("focus", function () {
        $(this).val($(this).val().replace(/,/g, ''));
    });

    // Initial state: disable remove button until a valid row is selected
    updateRemoveBtn();

    //let allBranches = [];

    // // Load all branches once
    // $.getJSON("https://mail.taxfiling.lk/bank-branches", function(data) {
    //     allBranches = data.map(branch => ({
    //         id: branch.BranchCode,
    //        // text: branch.BranchName + " (" + branch.bankName + ")" // show both branch and bank
    //          text: branch.BranchName
    //     }));

    //     $('#BankName').select2({
    //         placeholder: "Select bank",
    //         minimumInputLength: 0, // allow opening full list without typing
    //         data: allBranches,
    //         matcher: function(params, data) {
    //             // If no search term, show all
    //             if ($.trim(params.term) === '') {
    //                 return data;
    //             }

    //             // Match text anywhere (contains match, not just startsWith)
    //             if (data.text.toLowerCase().startsWith(params.term.toLowerCase())) {
    //                 return data;
    //             }

    //             return null;
    //         },
    //         dropdownAutoWidth: true,
    //         width: 'resolve',
    //         allowClear: true
    //     });
    // });

    //  $('#BankName').on('select2:open', function() {
    //     let searchBox = document.querySelector('.select2-container--open .select2-search__field');
    //     if (searchBox) {
    //         searchBox.setAttribute('placeholder', 'Filter your choice');
    //     }
    // });

    //                                                                    $('#BankName').select2({
    //     placeholder: "Type full name or short name (HNB, BOC)",
    //     ajax: {
    //         url: "https://mail.taxfiling.lk/getallbank",
    //         dataType: 'json',
    //         delay: 250,
    //         processResults: function (data) {
    //             return {
    //                 results: data.map(function(bank) {
    //                     return { id: bank.bankCode, text: bank.bankName  };
    //                 })
    //             };
    //         }
    //     },

    // minimumInputLength: 1,          // ✅ search using main box
    // minimumResultsForSearch: Infinity // ✅ removes inner search box
    // });

    // // 2. When bank is selected → load branches
    //    $("#BankName").on("change", function () {
    //     let bankCode = $(this).val();
    //     if (!bankCode) {
    //         $("#BranchName").prop("disabled", true).empty();
    //         return;
    //     }

    //     $.getJSON("https://mail.taxfiling.lk/getbranches/" + bankCode, function (branches) {
    //         let branchList = branches.map(br => ({
    //             id: br.BranchCode,
    //             text: br.BranchName
    //         }));

    //            $("#BranchName").prop("disabled", false).empty().select2({
    //             placeholder: "Select a Branch",
    //             data: branchList,
    //             allowClear: true,
    //             width: 'resolve'
    //         });
    //     });
    // });

    let allBanks = [];
    let selectedBank = null;
    let highlightedIndex = -1;
    let highlightedBranchIndex = -1;

    $.getJSON(`${window.AppConfig.mailUrl}getallbank`, function (data) {
        allBanks = data;
    });
    $("#bankInput").on("focus", function () {
        // Show all banks on focus
        renderBanks(allBanks, true);
    });

    $("#bankInput").on("input", function () {
        const q = $(this).val().toLowerCase();
        let results = allBanks;

        if (q) {
            results = results.filter(b =>
                b.bankName && b.bankName.toLowerCase().startsWith(q)
            );
        }

        renderBanks(results, true);
    });

    function renderBanks(results, highlightSelected) {
        let html = results.length === 0 ? "<div>No banks match.</div>" : "";
        results.forEach(b => {
            html += `<div data-id="${b.BankCode}" data-long="${b.bankName}">${b.bankName}</div>`;
        });

        $("#bankDropdown").html(html).show();

        // Highlight previously selected bank if present
        highlightedIndex = -1;

        if (highlightSelected && selectedBank) {

            $("#bankDropdown div").each(function (i) {
                if ($(this).data("id") == selectedBank) {
                    console.log($(this).data("id"))
                    $(this).addClass("highlight");
                    highlightedIndex = i;
                    this.scrollIntoView({ block: "nearest", behavior: "smooth" });
                    return false; // stop loop
                }
            });
        }
    }



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
        $.getJSON(`${window.AppConfig.mailUrl}getbranches/${selectedBank}`, (branches) => {
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

                $.getJSON(`${window.AppConfig.mailUrl}getbranches/${selectedBank}`, (branches) => {
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

        $.getJSON(`${window.AppConfig.mailUrl}getbranches/${selectedBank}`, (branches) => {
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
    
    const userId = window.AppConfig.userId;
    //const userId = userId;//'@User.FindFirst("UserID")?.Value';
    loadUploadedDocs(userId, 'T10');
    toggleManualTab('local');
    const docStatus = document.getElementById("docUploadStatus")?.value;

    if (docStatus === "2") {

        document.querySelectorAll('.submit-btn').forEach(btn => {
            btn.disabled = true;
            btn.title = "Disabled after submission";
        });
        document.getElementById("status-badge-2024").innerText = "SUBMITTED";
        document.getElementById("status-badge-2024").className = "badge bg-success";
        const acceptCheckbox = document.getElementById("acceptDeclaration");
        acceptCheckbox.checked = true;
        acceptCheckbox.disabled = true;
    }

    const message = '@TempData["SuccessMessage"]';
    if (message) {
        //alert(message); // You can use SweetAlert or Bootstrap Toast instead of `alert`

        // Optional: Scroll to a specific section
        // const target = document.getElementById("assets-section");
        //   if (target) {
        //       target.scrollIntoView({ behavior: 'smooth' });
        //   }
        showPanel("assets-section");
    }

    const radioExcel = document.getElementById("optionExcel");
    const radioManual = document.getElementById("optionManual");
    const excelSection = document.getElementById("excelUploadSection");
    const manualSection = document.getElementById("manualTableSection");

    function toggleInputMode() {
        if (radioExcel.checked) {
            excelSection.style.display = "block";
            manualSection.style.display = "none";
        } else {
            excelSection.style.display = "none";
            manualSection.style.display = "block";
        }
    }

    radioExcel.addEventListener("change", toggleInputMode);
    radioManual.addEventListener("change", toggleInputMode);
    toggleInputMode(); // on load

    const firstLink = document.querySelector(".sidebar-link[data-target]");
    if (firstLink) {
        const firstTarget = firstLink.getAttribute("data-target");
        // const firstButton = buttons[0];
        showPanel(firstTarget);
        selectedCategoryName = 'T10'; //firstButton.innerText.trim();
    }

    //scroll to top of the page
    $("#start-btn, #btnContinue, #btnNext, #btnAssestsNext, #btnDocNext,#btnOtherNext, #btnDeclareNext,#btnOtherPrevious,#btnAssetsPrevious, #btnBankPrevious, #btnEmployementPrevious, #btnPrevious,#btnDocPrevious, #linkTaxPayerNext,#divSummaryAssistedNext,#linkTaxPayerContinue").on("click", function () {
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });

    function checkUploadedFileExists(tableName) {
        let uploadedTable = document.getElementById(tableName);

        // Check if table has any rows in tbody
        let rows = uploadedTable.querySelectorAll("tbody tr");

        let validRows = Array.from(rows).filter(row => {
            let text = row.innerText.trim();
            return text !== "" && text !== "No Assets/Liabilities uploaded.";
        });

        if (validRows.length === 0) {
            //alert("Please upload at least one file before proceeding.");
            return false;
        }

        return true;
    }

    const startBtn = document.getElementById("start-btn");
    const btnContinue = document.getElementById("btnContinue");
    [startBtn, btnContinue].forEach(btn => {
        if (btn) {
            btn.addEventListener("click", function () {
                document.getElementById("btnLoadSection").click();
                // showPanel("documents-section");
            });
        }
    });

    const confirmModal = new bootstrap.Modal(document.getElementById("confirmModal"));
    let confirmCallback = null; // store callback for Yes button

    // Show modal with optional callback
    function showConfirmModal(callback) {
        confirmCallback = callback;
        confirmModal.show();
    }

    document.getElementById("confirmYes").addEventListener("click", function () {
        confirmModal.hide();
        if (confirmCallback) confirmCallback();
    });

    const nextBtn = document.getElementById("btnNext");
    if (nextBtn) {
        nextBtn.addEventListener("click", function (e) {

            if (!checkUploadedFileExists("documentsTableBody")) {
                e.preventDefault(); // stop default action
                showConfirmModal(function () {

                    selectedCategoryName = 'Bank Confirmation';
                    showPanel("bank-section");
                    loadUploadedDocs(userId, 'Bank Confirmation');
                });
            } else {
                // Proceed normally if file uploaded
                selectedCategoryName = 'Bank Confirmation';
                showPanel("bank-section");
                loadUploadedDocs(userId, 'Bank Confirmation');
            }

        });
    }

    const nextAssestBtn = document.getElementById("btnAssestsNext");
    if (nextAssestBtn) {
        nextAssestBtn.addEventListener("click", function (e) {

            if (!checkUploadedFileExists("documentsTableBankBody")) {
                e.preventDefault(); // stop default action
                showConfirmModal(function () {

                    showPanel("assets-section");
                    loadExcelAssestsUploadedFiles(userId);
                    loadAssestsUploadedFiles(userId);
                });
            } else {
                showPanel("assets-section");
                loadExcelAssestsUploadedFiles(userId);
                loadAssestsUploadedFiles(userId);
            }

        });
    }

    const nextOtherBtn = document.getElementById("btnOtherNext");
    if (nextOtherBtn) {
        nextOtherBtn.addEventListener("click", function (e) {

            if (!checkUploadedFileExists("uploadedExcelAssetsTable") && !checkUploadedFileExists("uploadedAssetsTable")) {
                e.preventDefault(); // stop default action
                showConfirmModal(function () {

                    selectedCategoryName = 'Other Documents';
                    showPanel("other-section");
                    loadUploadedDocs(userId, 'Other Documents');
                    loadFinancialDetails(userId, '2024/2025');
                });
            } else {
                selectedCategoryName = 'Other Documents';
                showPanel("other-section");
                loadUploadedDocs(userId, 'Other Documents');
                loadFinancialDetails(userId, '2024/2025');
            }
        });
    }

    const nextDeclareBtn = document.getElementById("btnDeclareNext");
    if (nextDeclareBtn) {
        nextDeclareBtn.addEventListener("click", function (e) {
            const container = document.getElementById('income-expenses');
            // const dataArray = [];

            // Validation: Check mandatory fields
            const requiredFields = ["Cash in hand", "Living Expenses"];
            let isValid = true;
            // let missingFields = [];

            requiredFields.forEach(fieldLabel => {
                const input = Array.from(container.querySelectorAll('label.form-label'))
                    .find(label => label.innerText.replace(/^\*\s*/, '').trim() === fieldLabel)
                    ?.closest('.col-md-6')
                    ?.querySelector('input');

                if (!input || input.value.trim() === "") {
                    isValid = false;
                    //missingFields.push(fieldLabel);
                }
            });

            if (!isValid) {
                showMessage("Please enter Living Expenses and Cash In Hand in Expenses and Balances", "error")
            }
            else if (!checkUploadedFileExists("documentsTableOtherBody")) {
                e.preventDefault(); // stop default action
                showConfirmModal(function () {

                    showPanel("declaration-section");
                    loadDeclarationDocs(userId);
                });
            } else {
                showPanel("declaration-section");
                loadDeclarationDocs(userId);
            }

        });
    }
    const btnDocNext = document.getElementById("btnDocNext");

    if (btnDocNext) {
        btnDocNext.addEventListener("click", function () {
            showPanel("employment-section");

        });
    }
    const btnMainDashboardNext = document.getElementById("btnMainDashboardNext");

    if (btnMainDashboardNext) {
        btnMainDashboardNext.addEventListener("click", function () {
            showPanel("dashboard-section"); // default

        });
    }

    const btnDocPrevious = document.getElementById("btnDocPrevious");

    if (btnDocPrevious) {
        btnDocPrevious.addEventListener("click", function () {
            $('#linkSummary').click();

        });
    }



    const previousBtn = document.getElementById("btnPrevious");
    const btnUploadDocs = document.getElementById("btnUploadDocs");
    const btnViewSubmittedDocs = document.getElementById("btnViewSubmittedDocs");

    [previousBtn, btnUploadDocs, btnViewSubmittedDocs].forEach(btn => {
        if (btn) {
            btn.addEventListener("click", function () {
                showPanel("documents-section");
            });
        }
    });
    if (previousBtn || btnUploadDocs) {
        previousBtn.addEventListener("click", function () {
            showPanel("documents-section");
        });
    }

    const previousEmploymentBtn = document.getElementById("btnEmployementPrevious");
    if (previousEmploymentBtn) {
        previousEmploymentBtn.addEventListener("click", function () {
            showPanel("employment-section");
            loadUploadedDocs(userId, 'T10');

        });
    }

    const previousBankBtn = document.getElementById("btnBankPrevious");
    if (previousBankBtn) {
        previousBankBtn.addEventListener("click", function () {
            selectedCategoryName = 'Bank Confirmation';
            showPanel("bank-section");
            loadUploadedDocs(userId, 'Bank Confirmation');

        });
    }

    const previousOtherBtn = document.getElementById("btnOtherPrevious");
    if (previousOtherBtn) {
        previousOtherBtn.addEventListener("click", function () {
            selectedCategoryName = 'Other Documents';
            showPanel("other-section");
            loadUploadedDocs(userId, 'Other Documents');

        });
    }

    const previousAssetsBtn = document.getElementById("btnAssetsPrevious");
    if (previousAssetsBtn) {
        previousAssetsBtn.addEventListener("click", function () {
            showPanel("assets-section");
            loadExcelAssestsUploadedFiles(userId);
            loadAssestsUploadedFiles(userId);
        });
    }

    const uploadDocBtn = document.getElementById("uploadDoc-btn");
    if (uploadDocBtn) {
        uploadDocBtn.addEventListener("click", function () {
            showPanel("employment-section");
            const documentsSubmenu = document.getElementById("documents-submenu");
            if (documentsSubmenu && documentsSubmenu.classList.contains("collapse")) {
                documentsSubmenu.classList.add("show");
            }
        });
    }

    // Activate tab from URL parameter
    const urlParams = new URLSearchParams(window.location.search);
    const section = urlParams.get("section");
    if (section) {

        if (section === "assets") {
            const manualOption = document.getElementById("optionManual");
            if (manualOption) {
                manualOption.checked = true;
                toggleInputMode();
                // document.querySelector('label[for="optionManual"]')?.classList.add("active");
                //document.querySelector('label[for="optionExcel"]')?.classList.remove("active");
            }
        }
        showPanel(`${section}-section`);
    } else {
        showPanel("dashboard-section"); // default
    }
});

submitBtn.addEventListener("click", async function () {
    let isValid = true;
    let messages = [];


    if (selectedCategoryName == "T10") {
        // Employer Name
        const employerName = document.getElementById("T10EmployerName");
        if (!employerName.value.trim()) {
            isValid = false;
            messages.push("T10 Employer Name is required.");
            employerName.classList.add("is-invalid");
        } else {
            employerName.classList.remove("is-invalid");
        }

        if (!isValid) {
            //alert(messages.join("\n"));
            showMessage(messages.join("\n"), "error");
            return;
        }

    }
    else if (selectedCategoryName == "Terminal Benefit") {
        isValid = true;
        messages = [];
        // Employer Name
        const TBemployerName = document.getElementById("TerminalEmployerName");
        if (!TBemployerName.value.trim()) {
            isValid = false;
            messages.push("Terminal Benefit Employer Name is required.");
            TBemployerName.classList.add("is-invalid");
        } else {
            TBemployerName.classList.remove("is-invalid");
        }

        if (!isValid) {
            //alert(messages.join("\n"));
            showMessage(messages.join("\n"), "error");
            return;
        }

    }

    else if (selectedCategoryName == "Any Other") {
        isValid = true;
        messages = [];
        // Employer Name
        const anyOtherType = document.getElementById("AnyOtherType");
        if (!anyOtherType.value.trim()) {
            isValid = false;
            messages.push("Please select Any Other Type.");
            anyOtherType.classList.add("is-invalid");
        } else {
            anyOtherType.classList.remove("is-invalid");
        }

        if (!isValid) {
            //alert(messages.join("\n"));
            showMessage(messages.join("\n"), "error");
            return;
        }

    }


    if (!selectedFile || !selectedCategoryName) {
        Swal.fire({
            icon: 'warning',
            title: 'Upload file',
            text: 'Please upload a file before submitting.',
            customClass: {
                popup: 'small-swal-popup'
            }
        });

        return;
    }
    showLoadMask();

    try {
        // 1. Upload to external API
        const formData = new FormData();
        formData.append("file", selectedFile);
        formData.append("userId", userId);
        formData.append("year", new Date().getFullYear().toString());

        const uploadRes = await fetch(`${fileSiteUrl}upload`, {
            method: "POST",
            body: formData
        });

        // if (!uploadRes.ok)
        //{
        //  showMessage("❌ Failed to upload document - " + uploadRes.error, "danger");
        //throw new Error("External upload failed");
        // }

        const uploadResult = await uploadRes.json();

        if (!uploadResult.success || !uploadResult.data) {
            // throw new Error("Upload failed: " + uploadResult.error);
            showMessage("❌ Failed to upload document - " + uploadResult.error, "error");
            selectedFile = null;
            const fileInput = document.getElementById("fileInput");
            if (fileInput) fileInput.value = "";
            const fileInputTerminal = document.getElementById("fileInputTerminal");
            if (fileInputTerminal) fileInputTerminal.value = "";
            const fileInputAnyOther = document.getElementById("fileInputAnyOther");
            if (fileInputAnyOther) fileInputAnyOther.value = "";

            let el;
            (el = document.getElementById("T10EmployerName")) && (el.value = "");
            (el = document.getElementById("TerminalEmployerName")) && (el.value = "");
            (el = document.getElementById("AnyOtherType")) && (el.selectedIndex = 0);
            return;
            // throw new Error("Upload failed: " + uploadResult.error);
        }

        const data = uploadResult.data;

        console.log("Employement" + selectedCategoryName);

        // 2. Call your internal API to save metadata
        const saveResponse = await fetch(`${baseApiUrl}api/UserUploadTaxAssistedDoc/SaveUploadedDocs`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: '@userId',
                categoryName: selectedCategoryName,
                uploadedFileName: data.originalName,
                fileName: data.filename,
                location: data.location,
                uploadTime: data.uploadTime,
                decryptionKey: data.decryptionKey,
                uploadId: data.uploadId,
                originalName: data.originalName,
                // Optional fields based on category
                t10EmployerName: document.getElementById("T10EmployerName")?.value || null,
                terminalEmployerName: document.getElementById("TerminalEmployerName")?.value || null,
                anyOtherType: document.getElementById("AnyOtherType")?.value || null,
                bankConfirmationType: document.getElementById("BankConfirmationType")?.value || null,
                bankName: null,//document.getElementById("BankName")?.value || null,
                otherDocumentName: document.getElementById("otherDocName")?.value || null
            })
        });

        if (!saveResponse.ok) throw new Error("Failed to save metadata");
        incrementUploadCount();
        const saveResult = await saveResponse.json();
        const userUploadId = saveResult.userUploadId;
        showMessage("File Submitted successfully!", "success");

        // alert("File submitted successfully!");

        // Optional: add to table UI
        const documentsTable = document.getElementById("documentsTableBody");
        const fileSizeKB = (selectedFile.size / 1024).toFixed(2); // Convert bytes to KB
        document.getElementById("uploadedDocs").style.display = "block";
        let employmentValue = "";
        let bankConfirmationType = "";
        let bankName = "";

        if (selectedCategoryName === "T10") {
            employmentValue = document.getElementById("T10EmployerName")?.value || null;
        } else if (selectedCategoryName === "Terminal Benefit") {
            employmentValue = document.getElementById("TerminalEmployerName")?.value || null;
        } else if (selectedCategoryName === "Any Other") {
            employmentValue = document.getElementById("AnyOtherType")?.value || null;
        }

        employmentValue = employmentValue ?? "";
        bankConfirmationType = document.getElementById("BankConfirmationType")?.value || "";
        bankName = "";//document.getElementById("BankName")?.value || "";

        const tableBody = document.querySelector("#documentsTableBody");
        const currentCount = tableBody.querySelectorAll("tr").length;
        const index = currentCount + 1; // next index

        const row = document.createElement("tr");
        row.innerHTML = `
    <td>${index}</td>
    <td>${data.originalName}</td>
    <td>${selectedCategoryName}</td>
    <td class="col-employer-name ">${employmentValue}</td>
    <td class="col-confirmation-type d-none">${bankConfirmationType}</td>
    <td class="col-bank-name d-none">${bankName}</td>
    <td>${new Date(data.uploadTime).toISOString().split("T")[0]}</td>
    <td>
        <button class="btn btn-primary btn-sm" onclick="viewDoc('${data.filename}', '${data.decryptionKey}')">View</button>
        <button class="btn btn-danger btn-sm delete-btn" onclick="showDeleteConfirm('${userUploadId}','${data.uploadId}')">Delete</button>
    </td>`;

        documentsTable.appendChild(row);

        loadUploadedDocs(userId, selectedCategoryName);
        // Reset UI
        selectedFile = null;
        const fileInput = document.getElementById("fileInput");
        if (fileInput) fileInput.value = "";
        const fileInputTerminal = document.getElementById("fileInputTerminal");
        if (fileInputTerminal) fileInputTerminal.value = "";
        const fileInputAnyOther = document.getElementById("fileInputAnyOther");
        if (fileInputAnyOther) fileInputAnyOther.value = "";

        let el;
        (el = document.getElementById("T10EmployerName")) && (el.value = "");
        (el = document.getElementById("TerminalEmployerName")) && (el.value = "");
        (el = document.getElementById("AnyOtherType")) && (el.selectedIndex = 0);
        updateChecklistStatuses(allDocs, docStatus);
        //document.getElementById("file-name").textContent = "";
        //document.getElementById("submitBtn").style.display = "none";

    } catch (err) {
        console.error(err);
        alert("Upload failed: " + err.message);
    }
    finally {
        hideLoadMask();
    }
});

submitBankBtn.addEventListener("click", async function () {

    let isValid = true;
    let messages = [];

    // Bank Confirmation Type
    const bankType = document.getElementById("BankConfirmationType");
    if (!bankType.value.trim()) {
        isValid = false;
        messages.push("Bank Confirmation Type is required.");
        bankType.classList.add("is-invalid");
    } else {
        bankType.classList.remove("is-invalid");
    }

    // Bank Name
    const bankName = document.getElementById("bankInput");
    if (!bankName.value.trim()) {
        isValid = false;
        messages.push("Bank Name is required.");
        bankName.classList.add("is-invalid");
    } else {
        bankName.classList.remove("is-invalid");
    }

    // Branch
    const branch = document.getElementById("branchInput");
    if (!branch.value.trim()) {
        isValid = false;
        messages.push("Branch Name is required.");
        branch.classList.add("is-invalid");
    } else {
        branch.classList.remove("is-invalid");
    }

    if (!isValid) {
        //alert(messages.join("\n"));
        showMessage(messages.join("\n"), "error");
        return;
    }


    if (!selectedFile || !selectedCategoryName) {
        Swal.fire({
            icon: 'warning',
            title: 'Upload file',
            text: 'Please upload a file before submitting.',
            customClass: {
                popup: 'small-swal-popup'
            }
        });

        return;
    }
    showLoadMask();
    try {
        // 1. Upload to external API
        const formData = new FormData();
        formData.append("file", selectedFile);
        formData.append("userId", userId);
        formData.append("year", new Date().getFullYear().toString());

        const uploadRes = await fetch(`${fileSiteUrl}upload`, {
            method: "POST",
            body: formData
        });

        //if (!uploadRes.ok) throw new Error("External upload failed");

        const uploadResult = await uploadRes.json();

        if (!uploadResult.success || !uploadResult.data) {
            showMessage("❌ Failed to upload document - " + uploadResult.error, "error");
            selectedFile = null;
            document.getElementById("BankConfirmationType").selectedIndex = 0;
            //document.getElementById("BankName").value = "";
            //$('#BankName').val(null).trigger('change');
            $("#bankInput").val("");
            $("#bank_id").val("");
            $("#branchInput").val("").prop("disabled", true).attr("placeholder", "Select a bank first");
            $("#branch_code, #branch_name").val("");
            $("#branchDropdown").hide();
            $("#bankDropdown").hide();

            selectedBank = null;
            highlightedBranchIndex = -1;
            document.getElementById("fileInputBank").value = "";
            return;
            //throw new Error("Upload failed: " + uploadResult.message);
        }

        const data = uploadResult.data;
        console.log("Bank" + selectedCategoryName);
        // 2. Call your internal API to save metadata
        const saveResponse = await fetch(`${baseApiUrl}api/UserUploadTaxAssistedDoc/SaveUploadedDocs`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: '@userId',
                categoryName: selectedCategoryName,
                uploadedFileName: data.originalName,
                fileName: data.filename,
                location: data.location,
                uploadTime: data.uploadTime,
                decryptionKey: data.decryptionKey,
                uploadId: data.uploadId,
                originalName: data.originalName,
                // Optional fields based on category
                t10EmployerName: document.getElementById("T10EmployerName")?.value || null,
                terminalEmployerName: document.getElementById("TerminalEmployerName")?.value || null,
                anyOtherType: document.getElementById("AnyOtherType")?.value || null,
                bankConfirmationType: document.getElementById("BankConfirmationType")?.value || null,
                // bankName: $('#BankName').select2('data')[0].text,//document.getElementById("BankName")?.value || null,
                bankName: $("#bankInput").val() + "-" + $("#branchInput").val(),
                otherDocumentName: document.getElementById("otherDocName")?.value || null
            })
        });

        if (!saveResponse.ok) throw new Error("Failed to save metadata");
        incrementUploadCount();
        const saveResult = await saveResponse.json();
        const userUploadId = saveResult.userUploadId;
        showMessage("File Submitted successfully!", "success");
        //alert("File submitted successfully!");

        // Optional: add to table UI
        const documentsTable = document.getElementById("documentsTableBankBody");
        const fileSizeKB = (selectedFile.size / 1024).toFixed(2); // Convert bytes to KB
        document.getElementById("uploadedBankDocs").style.display = "block";

        let employmentValue = "";
        let bankConfirmationType = "";
        let bankName = "";

        if (selectedCategoryName === "T10") {
            employmentValue = document.getElementById("T10EmployerName")?.value || null;
        } else if (selectedCategoryName === "Terminal Benefit") {
            employmentValue = document.getElementById("TerminalEmployerName")?.value || null;
        } else {
            employmentValue = document.getElementById("otherDocName")?.value || null;
        }

        employmentValue = employmentValue ?? "";
        bankConfirmationType = document.getElementById("BankConfirmationType")?.value || "";
        bankName = $("#bankInput").val() + "-" + $("#branchInput").val();//document.getElementById("BankName")?.value || "";

        const tableBody = document.querySelector("#documentsTableBankBody");
        const currentCount = tableBody.querySelectorAll("tr").length;
        const index = currentCount + 1; // next index
        console.log("bank index" + index + "," + currentCount)
        const row = document.createElement("tr");
        row.innerHTML = `
    <td>${index}</td>
    <td>${data.originalName}</td>
    <td>${selectedCategoryName}</td>
    <td class="col-employer-name d-none">${employmentValue}</td>
    <td class="col-confirmation-type ">${bankConfirmationType}</td>
    <td class="col-bank-name ">${bankName}</td>
    <td>${new Date(data.uploadTime).toISOString().split("T")[0]}</td>
    <td>
        <button class="btn btn-primary btn-sm" onclick="viewDoc('${data.filename}', '${data.decryptionKey}')">View</button>
        <button class="btn btn-danger btn-sm delete-btn" onclick="showDeleteConfirm('${userUploadId}','${data.uploadId}')">Delete</button>
    </td>`;

        documentsTable.appendChild(row);

        loadUploadedDocs(userId, selectedCategoryName);
        // Reset UI
        selectedFile = null;
        document.getElementById("BankConfirmationType").selectedIndex = 0;
        //document.getElementById("BankName").value = "";
        //$('#BankName').val(null).trigger('change');
        $("#bankInput").val("");
        $("#bank_id").val("");
        $("#branchInput").val("").prop("disabled", true).attr("placeholder", "Select a bank first");
        $("#branch_code, #branch_name").val("");
        $("#branchDropdown").hide();
        $("#bankDropdown").hide();

        selectedBank = null;
        highlightedBranchIndex = -1;
        document.getElementById("fileInputBank").value = "";
        updateChecklistStatuses(allDocs, docStatus);
        //document.getElementById("file-name").textContent = "";
        //document.getElementById("submitBtn").style.display = "none";

    } catch (err) {
        console.error(err);
        alert("Upload failed: " + err.message);
    }
    finally {
        hideLoadMask();
    }
});

submitExcelBtn.addEventListener("click", async function () {

    const fileInput = document.getElementById('assetsExcelFile');
    const selectedFile = fileInput.files[0];
    const selected = document.querySelector('input[name="assetInputOption"]:checked');

    if (!selectedFile) {
        alert("Please select a file before submitting.");
        return;
    }

    showLoadMask();

    try {

        // 1. Upload to external API
        const formData = new FormData();
        formData.append("file", selectedFile);
        formData.append("userId", userId);
        formData.append("year", new Date().getFullYear().toString());

        const uploadRes = await fetch(`${fileSiteUrl}upload`, {
            method: "POST",
            body: formData
        });

        if (!uploadRes.ok) throw new Error("External upload failed");

        const uploadResult = await uploadRes.json();

        if (!uploadResult.success || !uploadResult.data) {
            throw new Error("Upload failed: " + uploadResult.message);
        }

        const data = uploadResult.data;
        // 2. Call your internal API to save metadata
        const saveResponse = await fetch(`${baseApiUrl}api/UserUploadTaxAssistedDoc/SaveUploadedDocs`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: '@userId',
                categoryName: 'Assests Upload Excel',
                uploadedFileName: data.originalName,
                fileName: data.filename,
                location: data.location,
                uploadTime: data.uploadTime,
                decryptionKey: data.decryptionKey,
                uploadId: data.uploadId,
                originalName: data.originalName,
                // Optional fields based on category
                t10EmployerName: document.getElementById("T10EmployerName")?.value || null,
                terminalEmployerName: document.getElementById("TerminalEmployerName")?.value || null,
                anyOtherType: document.getElementById("AnyOtherType")?.value || null,
                bankConfirmationType: document.getElementById("BankConfirmationType")?.value || null,
                bankName: null,//document.getElementById("BankName")?.value || null,
                otherDocumentName: document.getElementById("otherDocName")?.value || null,
                assestOptionType: selected.value,
                AssestsUploadExcelSheetName: data.originalName
            })
        });

        if (!saveResponse.ok) throw new Error("Failed to save metadata");
        incrementUploadCount();
        const saveResult = await saveResponse.json();
        const userUploadId = saveResult.userUploadId;
        showMessage("File Submitted successfully!", "success");
        loadExcelAssestsUploadedFiles(userId);
        //alert("File submitted successfully!");

        // // Optional: add to table UI
        // const documentsTable = document.getElementById("documentsTableOtherBody");
        // const fileSizeKB = (selectedFile.size / 1024).toFixed(2); // Convert bytes to KB
        // document.getElementById("uploadedOtherDocs").style.display = "block";

        // const row = document.createElement("tr");
        // row.innerHTML = `
        //     <td>${data.originalName}</td>

        //     <td>${new Date(data.uploadTime).toISOString().split("T")[0]}</td>
        //     <td>
        //         <button class="btn btn-primary btn-sm" onclick="viewDoc('${data.filename}', '${data.decryptionKey}')">View</button>
        //         <button class="btn btn-danger btn-sm" onclick="showDeleteConfirm('${userUploadId}')">Delete</button>
        //     </td>`;

        // documentsTable.appendChild(row);

        // loadUploadedDocs(userId,selectedCategoryName);
        // // Reset UI
        // selectedFile = null;
        document.getElementById("assetsExcelFile").value = ""; // clear file input
        document.getElementById("uploadedFileName").textContent = ""; // clear filename

        //document.getElementById("file-name").textContent = "";
        //document.getElementById("submitBtn").style.display = "none";

    } catch (err) {
        console.error(err);
        alert("Upload failed: " + err.message);
    }
    finally {
        hideLoadMask();
    }
});


submitOtherBtn.addEventListener("click", async function () {

    let isValid = true;
    let messages = [];
    // Doc Name
    const otherDocName = document.getElementById("otherDocName");
    if (!otherDocName.value.trim()) {
        isValid = false;
        messages.push("Other Document Name is required.");
        otherDocName.classList.add("is-invalid");
    } else {
        otherDocName.classList.remove("is-invalid");
    }

    if (!isValid) {
        //alert(messages.join("\n"));
        showMessage(messages.join("\n"), "error");
        return;
    }


    if (!selectedFile || !selectedCategoryName) {
        Swal.fire({
            icon: 'warning',
            title: 'Upload file',
            text: 'Please upload a file before submitting.',
            customClass: {
                popup: 'small-swal-popup'
            }
        });

        return;
    }

    showLoadMask();

    try {

        // 1. Upload to external API
        const formData = new FormData();
        formData.append("file", selectedFile);
        formData.append("userId", userId);
        formData.append("year", new Date().getFullYear().toString());

        const uploadRes = await fetch(`${fileSiteUrl}upload`, {
            method: "POST",
            body: formData
        });

        // if (!uploadRes.ok) throw new Error("External upload failed");

        const uploadResult = await uploadRes.json();

        if (!uploadResult.success || !uploadResult.data) {
            showMessage("❌ Failed to upload document - " + uploadResult.error, "error");
            selectedFile = null;
            document.getElementById("otherDocName").value = "";
            document.getElementById("fileInputOther").value = "";
            return;
            //throw new Error("Upload failed: " + uploadResult.message);
        }

        const data = uploadResult.data;
        console.log("Other" + selectedCategoryName);
        // 2. Call your internal API to save metadata
        const saveResponse = await fetch(`${baseApiUrl}api/UserUploadTaxAssistedDoc/SaveUploadedDocs`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: '@userId',
                categoryName: selectedCategoryName,
                uploadedFileName: data.originalName,
                fileName: data.filename,
                location: data.location,
                uploadTime: data.uploadTime,
                decryptionKey: data.decryptionKey,
                uploadId: data.uploadId,
                originalName: data.originalName,
                // Optional fields based on category
                t10EmployerName: document.getElementById("T10EmployerName")?.value || null,
                terminalEmployerName: document.getElementById("TerminalEmployerName")?.value || null,
                anyOtherType: document.getElementById("AnyOtherType")?.value || null,
                bankConfirmationType: document.getElementById("BankConfirmationType")?.value || null,
                bankName: null,//document.getElementById("BankName")?.value || null,
                otherDocumentName: document.getElementById("otherDocName")?.value || null
            })
        });

        if (!saveResponse.ok) throw new Error("Failed to save metadata");
        incrementUploadCount();
        const saveResult = await saveResponse.json();
        const userUploadId = saveResult.userUploadId;
        showMessage("File Submitted successfully!", "success");
        // alert("File submitted successfully!");

        // Optional: add to table UI
        const documentsTable = document.getElementById("documentsTableOtherBody");
        const fileSizeKB = (selectedFile.size / 1024).toFixed(2); // Convert bytes to KB
        document.getElementById("uploadedOtherDocs").style.display = "block";

        let employmentValue = "";
        let bankConfirmationType = "";
        let bankName = "";


        if (selectedCategoryName === "T10") {
            employmentValue = document.getElementById("T10EmployerName")?.value || null;
        } else if (selectedCategoryName === "Terminal Benefit") {
            employmentValue = document.getElementById("TerminalEmployerName")?.value || null;
        } else {
            employmentValue = document.getElementById("otherDocName")?.value || null;
        }

        employmentValue = employmentValue ?? "";
        bankConfirmationType = document.getElementById("BankConfirmationType")?.value || "";
        bankName = "";// document.getElementById("BankName")?.value || "";


        const tableBody = document.querySelector("#documentsTableOtherBody");
        const currentCount = tableBody.querySelectorAll("tr").length;
        const index = currentCount + 1; // next index

        const row = document.createElement("tr");
        row.innerHTML = `
    <td>${index}</td>
    <td>${data.originalName}</td>
    <td>${selectedCategoryName}</td>
    <td class="col-employer-name">${employmentValue}</td>
    <td class="col-confirmation-type d-none">${bankConfirmationType}</td>
    <td class="col-bank-name d-none">${bankName}</td>
    <td>${new Date(data.uploadTime).toISOString().split("T")[0]}</td>
    <td>
        <button class="btn btn-primary btn-sm" onclick="viewDoc('${data.filename}', '${data.decryptionKey}')">View</button>
        <button class="btn btn-danger btn-sm delete-btn" onclick="showDeleteConfirm('${userUploadId}','${data.uploadId}')">Delete</button>
    </td>`;

        documentsTable.appendChild(row);

        loadUploadedDocs(userId, selectedCategoryName);
        // Reset UI
        selectedFile = null;
        document.getElementById("otherDocName").value = "";
        document.getElementById("fileInputOther").value = "";
        updateChecklistStatuses(allDocs, docStatus);
        //document.getElementById("file-name").textContent = "";
        //document.getElementById("submitBtn").style.display = "none";

    } catch (err) {
        console.error(err);
        alert("Upload failed: " + err.message);
    }
    finally {
        hideLoadMask();
    }
});

$(document).on("input", "input[name^='assets'][name$='Description']", function () {
    let val = $(this).val();
    if (val !== "" && !isNaN(val) && Number(val) < 0) {
        $(this).val(0); // reset if negative
    }
});

$(document).on("keydown", "input[name^='assets'][name$='Description']", function (e) {
    if (e.key === "-" || e.key === "Subtract") {
        e.preventDefault(); // stop minus key
    }
});


