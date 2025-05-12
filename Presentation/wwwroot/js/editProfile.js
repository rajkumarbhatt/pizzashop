$(document).ready(function () {
    
    // Get states based on selected country
    $('#select-country').change(function () {
        // empty the state and city dropdowns
        $('#select-state').empty();
        $('#select-city').empty();
        $('#select-state').append($('<option>').text('Select State').val(''));
        $('#select-city').append($('<option>').text('Select City').val(''));

        var countryId = $(this).val();
        $.ajax({
            url: '/Profile/GetStates',
            type: 'GET',
            data: { countryId: countryId },
            success: function (data) {
                $.each(data, function (i, state) {
                    $('#select-state').append($('<option>').text(state.name).val(state.id));
                });
            }
        });
    });
        
    // Get cities based on selected state
    $('#select-state').change(function () {
        var stateId = $(this).val();
        $.ajax({
            url: '/Profile/GetCities',
            type: 'GET',
            data: { stateId: stateId },
            success: function (data) {
                $('#select-city').empty();
                $('#select-city').append($('<option>').text('Select City').val(''));
                $.each(data, function (i, city) {
                    $('#select-city').append($('<option>').text(city.name).val(city.id));
                });
            }
        });
    });

    
    
    // submit form
    $('#edit-profile-form').submit(function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var form = $(this)[0]; 
        var formData = new FormData(form);
        $('.loader-container').removeClass('d-none');
        $.ajax({
            url: '/Profile/EditProfile',
            type: 'POST',
            data: formData,
            processData: false, 
            contentType: false, 
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    setTimeout(function () {
                        window.location.href = '/Profile';
                        $('.loader-container').addClass('d-none');
                    }, 1000);
                } else {
                    toastr.error(data.message);
                    $('.loader-container').addClass('d-none');
                }
            },
            error: function (data) {
                toastr.error('An error occurred. Please try again.');
                $('.loader-container').addClass('d-none');
            }
        });
    });

});    

document.getElementById('file-upload').addEventListener('change', (e) => {
    const file = e.target.files[0];
    if (file && !file.type.startsWith('image/')) {
        toastr.error('Please select an image file.');
        return;
    }
    if (file) {
        document.getElementById('profile-pic').src = URL.createObjectURL(file);
    }
});
