
function escapeHtml(text) {
    if (!text) return '';
    return $('<div>').text(text).html();
}

function OpenVendorModal(id) {
    var titleText = id == 0 ? "Add New Vendor" : "Edit Vendor Details";

    $.get("/Vendors/CreateUpdate/?id=" + id, function (res) {
        $("#CommonModalBodyId").html(res);
        $('#CommonModalId .modal-title').text(titleText);
        $.validator.unobtrusive.parse("#VendorForm");

        bootstrap.Modal.getOrCreateInstance(document.getElementById("CommonModalId")).show();
    }).fail(function (error) {
        Swal.fire({
            icon: 'error',
            title: 'Failed to Load Vendor',
            text: 'Unable to retrieve vendor details from the server.',
            width: '360px',
            confirmButtonColor: '#4f46e5'
        });
    });
}

$(document).on('click', '#vendorSubmitBtn', function (e) {
    e.preventDefault();

    var $form = $("#VendorForm");
    if ($form.length && !$form.valid()) {
        return;
    }

    var $submitBtn = $(this);
    var originalBtnText = $submitBtn.html();
    $submitBtn.prop('disabled', true).html('<i class="fa-solid fa-spinner fa-spin me-1"></i> Saving...');

    var formElement = document.getElementById("VendorForm");
    var data = new FormData(formElement);

    $.ajax({
        url: "/Vendors/CreateUpdate",
        type: "POST",
        data: data,
        contentType: false,
        processData: false,
        success: function (response) {
            $submitBtn.prop('disabled', false).html(originalBtnText);

            if (typeof response === "object") {
                if (response.rowchanged > 0) {
                    Swal.fire({
                        icon: 'success',
                        title: response.vendorId > 0 ? 'Vendor Updated!' : 'Vendor Created!',
                        text: response.vendorId > 0 ? 'Vendor details have been updated successfully.' : 'New vendor has been registered successfully.',
                        width: '360px',
                        timer: 2000,
                        showConfirmButton: false,
                        timerProgressBar: true
                    }).then(() => {
                        $('#vendorDetailsDatatable').DataTable().ajax.reload(null, false);
                        bootstrap.Modal.getOrCreateInstance(document.getElementById("CommonModalId")).hide();
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Save Unsuccessful',
                        text: response.message || 'Unable to save the vendor. Please verify details and try again.',
                        width: '360px',
                        confirmButtonColor: '#4f46e5'
                    });
                }
            } else {
                $("#CommonModalBodyId").html(response);
                $.validator.unobtrusive.parse("#VendorForm");
                bootstrap.Modal.getOrCreateInstance(document.getElementById("CommonModalId")).show();
            }
        },
        error: function (err) {
            $submitBtn.prop('disabled', false).html(originalBtnText);
            Swal.fire({
                icon: 'error',
                title: 'Server Error',
                text: err.statusText || 'A network error occurred while submitting the form.',
                width: '360px',
                confirmButtonColor: '#4f46e5'
            });
        }
    });
});

$(document).on("click", "#DeleteBtn", function () {
    var id = $(this).attr('data-id');

    Swal.fire({
        title: "Delete Vendor?",
        text: "Comfirm for delete record..",
        icon: "warning",
        width: '360px',
        showCancelButton: true,
        confirmButtonColor: "#ef4444",
        cancelButtonColor: "#64748b",
        confirmButtonText: '<i class="fa-solid fa-trash-can me-1"></i> Yes, Delete',
        cancelButtonText: "Cancel",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/Vendors/DeleteConfirmed?id=" + id,
                type: "POST",
                dataType: "json",
                success: function (response) {
                    if (response.rowChanged > 0) {
                        Swal.fire({
                            icon: "success",
                            title: "Vendor Deleted",
                            text: "The vendor record was removed successfully.",
                            width: '360px',
                            showConfirmButton: false,
                            timer: 2000,
                            timerProgressBar: true
                        });
                        $('#vendorDetailsDatatable').DataTable().ajax.reload(null, false);
                    } else {
                        Swal.fire({
                            icon: "error",
                            title: "Delete Failed",
                            text: "Unable to delete vendor record.",
                            width: '360px',
                            confirmButtonColor: '#4f46e5'
                        });
                    }
                },
                error: function (err) {
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: "An error occurred while deleting the record: " + err.status,
                        width: '360px',
                        confirmButtonColor: '#4f46e5'
                    });
                }
            });
        }
    });
});

//Datatable
$(document).ready(function () {
    $("#vendorDetailsDatatable").DataTable({
        processing: true,
        serverSide: true,
        responsive: true,
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search vendor, GST, contact...",
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ suppliers",
            infoEmpty: "No vendors registered yet",
            zeroRecords: "No matching vendors found",
            paginate: {
                previous: '<i class="fa-solid fa-chevron-left"></i>',
                next: '<i class="fa-solid fa-chevron-right"></i>'
            }
        },
        ajax: {
            url: "/Vendors/VendorDatatable",
            type: "POST",
            dataType: "json"
        },
        columns: [
            {
                data: "vendorName",
                name: "vendorName",
                render: function (data, type, row) {
                    if (!data) return '<span class="text-muted">N/A</span>';
                    var initials = data.trim().substring(0, 2).toUpperCase();
                    return `<div class="vendor-name-cell">
                        <div class="vendor-avatar">${initials}</div>
                        <div>
                            <div class="vendor-name-text">${escapeHtml(data)}</div>
                            <div class="vendor-id-sub">Vendor #${row.vendorId || ''}</div>
                        </div>
                    </div>`;
                }
            },
            {
                data: "emails",
                name: "emails",
                render: function (data) {
                    if (!data) return '<span class="text-muted">N/A</span>';
                    return `<a href="mailto:${data}" class="text-decoration-none text-secondary d-inline-flex align-items-center gap-1">
                        <i class="fa-regular fa-envelope text-primary"></i>
                        <span>${escapeHtml(data)}</span>
                    </a>`;
                }
            },
            {
                data: "contactPerson",
                name: "contactPerson",
                render: function (data) {
                    if (!data) return '<span class="text-muted">N/A</span>';
                    return `<span class="d-inline-flex align-items-center gap-1 text-dark fw-medium">
                        <i class="fa-regular fa-user text-muted"></i>
                        <span>${escapeHtml(data)}</span>
                    </span>`;
                }
            },
            {
                data: "mobileNumbers",
                name: "mobileNumbers",
                render: function (data) {
                    if (!data) return '<span class="text-muted">N/A</span>';
                    return `<span class="d-inline-flex align-items-center gap-1 text-secondary font-monospace">
                        <i class="fa-solid fa-phone text-muted"></i>
                        <span>${escapeHtml(data)}</span>
                    </span>`;
                }
            },
            {
                data: "gstNumber",
                name: "gstNumber",
                render: function (data) {
                    if (!data) return '<span class="text-muted">N/A</span>';
                    return `<span class="badge-gst"><i class="fa-solid fa-receipt text-secondary"></i>${escapeHtml(data)}</span>`;
                }
            },
            {
                data: "vendorsCategoryName",
                name: "vendorsCategoryName",
                render: function (data) {
                    if (!data || data === 'No Assign') return '<span class="badge bg-light text-secondary border">Unassigned</span>';
                    return `<span class="badge-category"><i class="fa-solid fa-tag"></i>${escapeHtml(data)}</span>`;
                }
            },
            {
                data: "status",
                name: "status",
                render: function (data) {
                    if (data === "Active") {
                        return `<span class="badge-status-active"><span class="status-dot-sm"></span>Active</span>`;
                    }
                    return `<span class="badge-status-inactive"><span class="status-dot-sm"></span>Inactive</span>`;
                }
            },
            {
                data: "vendorId",
                orderable: false,
                className: "text-center",
                render: function (data) {
                    return `<div class="action-btn-group">
                        <button type="button" class="btn-action btn-action-edit" onclick="OpenVendorModal(${data})" title="Edit Vendor">
                            <i class="fa-solid fa-pen-to-square"></i>
                        </button>
                        <button type="button" class="btn-action btn-action-delete" id="DeleteBtn" data-id="${data}" title="Delete Vendor">
                            <i class="fa-solid fa-trash-can"></i>
                        </button>
                    </div>`;
                }
            }
        ]
    });
});

//  show file name which upload 
$(document).on('change', '#vendorFiles', function () {
    var $list = $('#fileListid');
    $list.empty();

    var files = this.files;
    if (files.length === 0) {
        $list.html('<small class="text-muted fst-italic" id="noFilesText">No files selected</small>');
        return;
    }

    for (var i = 0; i < files.length; i++) {
        var sizeMB = (files[i].size / (1024 * 1024)).toFixed(2);
        var fileName = files[i].name;
        var shortName = fileName.length > 18 ? fileName.substring(0, 15) + '...' : fileName;
        var itemHtml = `
            <span class="badge bg-white text-primary border d-inline-flex align-items-center gap-1 py-1 px-2 shadow-xs" title="${escapeHtml(fileName)} (${sizeMB} MB)" style="font-size: 0.75rem;">
                <i class="fa-solid fa-file-lines text-primary"></i>
                <span>${escapeHtml(shortName)}</span>
                <span class="text-muted small">(${sizeMB}MB)</span>
            </span>`;
        $list.append(itemHtml);
    }
});

// Remove Existing Document Handler
$(document).on('click', '.btn-remove-doc', function () {
    $(this).closest('.doc-item').fadeOut(150, function () {
        $(this).remove();
    });
});