function triggerMultiSelect() {
    try {
        const selectElement = document.getElementById('SelectModifierGroup');
        const customMultiSelect = document.querySelector('.custom-multi-select');
        const dropdownOptions = customMultiSelect.querySelector('.dropdown-options');
        const selectedTagsContainer = customMultiSelect.querySelector('.selected-tags');
        const searchInput = customMultiSelect.querySelector('.search-input');
        const clearAllButton = customMultiSelect.querySelector('.clear-all');
        const dropdownToggle = customMultiSelect.querySelector('.dropdown-toggle');

        dropdownOptions.innerHTML = '';

        selectElement.querySelectorAll('option').forEach(option => {
            try {
                const value = option.value;
                const text = option.textContent;

                const customOption = document.createElement('div');
                customOption.className = 'option';
                customOption.setAttribute('data-value', value);
                customOption.innerHTML = `
                    <input type="checkbox" id=${value} onchange="addModifierGroupToList(id)">
                    <label for="custom-option-${value}">${text}</label>
                `;

                dropdownOptions.appendChild(customOption);

                const checkbox = customOption.querySelector('input[type="checkbox"]');
                checkbox.addEventListener('change', () => {
                    try {
                        if (checkbox.checked) {
                            option.selected = true;
                            addTag(value, text);
                        } else {
                            option.selected = false;
                            removeTag(value);
                        }
                    } catch (error) {
                        console.error("Error in checkbox change event:", error);
                    }
                });
            } catch (error) {
                console.error("Error in option processing:", error);
            }
        });

        function addTag(value, text) {
            try {
                const tag = document.createElement('div');
                tag.className = 'tag';
                tag.setAttribute('data-value', value);
                tag.innerHTML = `
                    ${text}
                    <span class="tag-delete"></span>
                `;

                selectedTagsContainer.appendChild(tag);

                tag.querySelector('.tag-delete').addEventListener('click', () => {
                    try {
                        tag.remove();
                        const option = selectElement.querySelector(`option[value="${value}"]`);
                        if (option) {
                            option.selected = false;
                        }
                        const checkbox = dropdownOptions.querySelector(`input[id="custom-option-${value}"]`);
                        if (checkbox) {
                            checkbox.checked = false;
                        }
                    } catch (error) {
                        console.error("Error in tag delete event:", error);
                    }
                });
            } catch (error) {
                console.error("Error in addTag function:", error);
            }
        }

        function removeTag(value) {
            try {
                const tag = selectedTagsContainer.querySelector(`[data-value="${value}"]`);
                if (tag) {
                    tag.remove();
                }
            } catch (error) {
                console.error("Error in removeTag function:", error);
            }
        }

        clearAllButton.addEventListener('click', () => {
            try {
                selectedTagsContainer.innerHTML = '';
                selectElement.querySelectorAll('option').forEach(option => {
                    option.selected = false;
                });
                dropdownOptions.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                    checkbox.checked = false;
                });
            } catch (error) {
                console.error("Error in clearAllButton click event:", error);
            }
        });

        searchInput.addEventListener('focus', () => {
            try {
                dropdownOptions.style.display = 'block';
            } catch (error) {
                console.error("Error in searchInput focus event:", error);
            }
        });

        searchInput.addEventListener('blur', () => {
            try {
                setTimeout(() => {
                    dropdownOptions.style.display = 'none';
                }, 200);
            } catch (error) {
                console.error("Error in searchInput blur event:", error);
            }
        });
    } catch (error) {
        console.error("Error in triggerMultiSelect function:", error);
    }
}

function untriggerMultiSelect() {
    try {
        const customMultiSelect = document.querySelector('.custom-multi-select');
        customMultiSelect.innerHTML = '';
    } catch (error) {
        console.error("Error in untriggerMultiSelect function:", error);
    }
}
