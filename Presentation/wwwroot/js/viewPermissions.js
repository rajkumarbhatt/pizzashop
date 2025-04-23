var parentCheckbox = document.getElementById('flexCheckIndeterminate');
parentCheckbox.addEventListener('change', e => {
    document.querySelectorAll('.permission-checkbox').forEach(checkbox => {
        checkbox.checked = e.target.checked;
    });
});

document.querySelectorAll('.permission-checkbox').forEach(checkbox => {
    checkbox.addEventListener('change', () => {
        var tbodyCheckbox = document.querySelectorAll('.permission-checkbox').length;
        var tbodyCheckedbox = document.querySelectorAll('.permission-checkbox:checked').length;
        if (tbodyCheckbox === tbodyCheckedbox) {
            // All selected
            parentCheckbox.indeterminate = false;
            parentCheckbox.checked = true;
        } else if (tbodyCheckedbox > 0) {
            // Some selected
            parentCheckbox.indeterminate = true;
            parentCheckbox.checked = false;
        } else {
            // None selected
            parentCheckbox.indeterminate = false;
            parentCheckbox.checked = false;
        }
    });
});

var changedPermissions = [];


function changePermission (roleId, permissionId, permissionName) {
    var checked = document.getElementById(permissionName + roleId + permissionId).checked;
    var permission = {
        roleId: roleId,
        permissionId: permissionId,
        permissionName: permissionName,
        checked: checked
    };
    var index = changedPermissions.findIndex(p => p.roleId === roleId && p.permissionId === permissionId && p.permissionName === permissionName);
    if (index === -1) {
        changedPermissions.push(permission);
    } else {
        changedPermissions[index] = permission;
    }
    console.log(changedPermissions);
}

function savePermissions() {
    $('.loader-container').removeClass('d-none');    
    $.ajax({
        type: 'POST',
        url: '/RoleAndPermission/EditPermissions',
        contentType: 'application/json',
        data: JSON.stringify(changedPermissions),
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                setTimeout(() => {
                    window.location.reload();
                    $('.loader-container').addClass('d-none');
                }, 1000);
            } else {
                toastr.error(response.message);
                $('.loader-container').addClass('d-none');
            }
        },
        error: function (response) {
            toastr.error(response.message);
            $('.loader-container').addClass('d-none');
        }
    });
}


function enableAll(id) {
    if ($('#' + id).is(':checked')) {
        $('.' + id).removeAttr('disabled');
    } else {
        $('.' + id).attr('disabled', 'disabled');
    }
}

function checkAll() {
    if ($('.intermediate-checkbox').is(':checked')) {
        $('.permission-checkbox').each(function() {
            $(this).prop('checked', true);
            enableAll($(this).val());
        });
    } else {
        $('.permission-checkbox').each(function() {
            $(this).prop('checked', false);
            enableAll($(this).val());
        });
    }
}