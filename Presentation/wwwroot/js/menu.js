

$(document).ready(function() {

    function showSearchBar () {
      document.getElementById("mobile-search-bar").classList.remove("d-none");
      document.getElementById("search-icon").classList.add("d-none");
    }

    window.addEventListener('mouseup',function(event){
        var mobileSearchBar = document.getElementById('mobile-search-bar');
        if(event.target != mobileSearchBar && event.target.parentNode != mobileSearchBar){
            mobileSearchBar.classList.add('d-none');
            document.getElementById("search-icon").classList.remove("d-none");
        }
    });  
    $("#AddCategoryModal").click(function() {
        var Name = $("#CategoryName").val();
        var Description = $("#CategoryDescription").val();
        $.ajax({
            url: '/Menu/AddCategory',
            type: 'POST',
            data: { categoryName: Name, categoryDescription: Description },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    setTimeout(function() {
                        window.location.href = "/Menu"; 
                    }, 1000);
                } else {
                    if(data.message != null) {
                        toastr.error(data.message);
                    } else {
                        toastr.error("Not Authorized");
                    }
                }
            },
            error: function (data) {
                toastr.error(data.message);
            }
        });
    });

    $("#EditCategoryModal").click(function() {
        var Id = $("#EditCategoryId").val();
        var Name = $("#EditCategoryName").val();
        var Description = $("#EditCategoryDescription").val();
        $.ajax({
            url: '/Menu/UpdateCategory',
            type: 'PUT',
            data: { categoryId: Id, categoryName: Name, categoryDescription: Description },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    setTimeout(function() {
                        window.location.href = "/Menu"; 
                    }, 1000);
                } else {
                    if(data.message != null) {
                        toastr.error(data.message);
                    } else {
                        toastr.error("Not Authorized");
                    }
                }
            },
            error: function (data) {
                toastr.error(data.message);
            }
        });
    });
});


function triggerMultiSelect(){
        const selectElement = document.getElementById('SelectModifierGroup');
        const customMultiSelect = document.querySelector('.custom-multi-select');
        const dropdownOptions = customMultiSelect.querySelector('.dropdown-options');
        const selectedTagsContainer = customMultiSelect.querySelector('.selected-tags');
        const searchInput = customMultiSelect.querySelector('.search-input');
        const clearAllButton = customMultiSelect.querySelector('.clear-all');
        const dropdownToggle = customMultiSelect.querySelector('.dropdown-toggle');

        // Build the custom dropdown options from the <select> element
        selectElement.querySelectorAll('option').forEach(option => {
            const value = option.value;
            const text = option.textContent;

            // Create a custom option
            const customOption = document.createElement('div');
            customOption.className = 'option';
            customOption.setAttribute('data-value', value);
            customOption.innerHTML = `
      <input type="checkbox" id=${value} onchange="addModifierGroupToList(id)">
      <label for="custom-option-${value}">${text}</label>
    `;

            // Add the custom option to the dropdown
            dropdownOptions.appendChild(customOption);

            // Sync the selected state with the <select> element
            const checkbox = customOption.querySelector('input[type="checkbox"]');
            checkbox.addEventListener('change', () => {
                if (checkbox.checked) {
                    option.selected = true; // Sync with <select>
                    addTag(value, text); // Add tag
                } else {
                    option.selected = false; // Sync with <select>
                    removeTag(value); // Remove tag
                }
            });
        });

        // Function to add a tag
        function addTag(value, text) {
            const tag = document.createElement('div');
            tag.className = 'tag';
            tag.setAttribute('data-value', value);
            tag.innerHTML = `
      ${text}
      <span class="tag-delete"></span>
    `;

            // Add tag to the selected tags container
            selectedTagsContainer.appendChild(tag);

            // Remove tag when delete button is clicked
            tag.querySelector('.tag-delete').addEventListener('click', () => {
                tag.remove();
                const option = selectElement.querySelector(`option[value="${value}"]`);
                if (option) {
                    option.selected = false; // Sync with <select>
                }
                const checkbox = dropdownOptions.querySelector(`input[id="custom-option-${value}"]`);
                if (checkbox) {
                    checkbox.checked = false; // Uncheck the checkbox
                }
            });
        }

        // Function to remove a tag
        function removeTag(value) {
            const tag = selectedTagsContainer.querySelector(`[data-value="${value}"]`);
            if (tag) {
                tag.remove();
            }
        }

        // Clear all selected tags
        clearAllButton.addEventListener('click', () => {
            selectedTagsContainer.innerHTML = '';
            selectElement.querySelectorAll('option').forEach(option => {
                option.selected = false; // Unselect all options in <select>
            });
            dropdownOptions.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                checkbox.checked = false; // Uncheck all checkboxes
            });
        });
        // Toggle dropdown visibility
        searchInput.addEventListener('focus', () => {
            dropdownOptions.style.display = 'block';
        });

        searchInput.addEventListener('blur', () => {
            setTimeout(() => {
                dropdownOptions.style.display = 'none';
            }, 200);
        });
}
