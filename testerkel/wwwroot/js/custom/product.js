
// Kullanıcının isteği: var kullanalım
(function () {
    var form = document.getElementById("productForm");
    var editBtn = document.getElementById("btnEdit");
    var saveBtn = document.getElementById("btnSave");
    var cancelBtn = document.getElementById("btnCancel");
    var modeBadge = document.getElementById("modeBadge");

    if (!form || !editBtn || !saveBtn || !cancelBtn || !modeBadge) {
        return;
    }

    var editableFields = form.querySelectorAll("[data-editable='true']");
    var isEditMode = false;
    var initialValues = {};

    // İlk değerleri sakla (Cancel için)
    editableFields.forEach(function (el) {
        initialValues[el.name] = el.value;
    });

    function setEditMode(editMode) {
        isEditMode = editMode;

        editableFields.forEach(function (el) {
            if (editMode) {
                el.removeAttribute("readonly");
                el.removeAttribute("disabled");
            } else {
                if (el.tagName === "SELECT") {
                    el.setAttribute("disabled", "disabled");
                } else {
                    el.setAttribute("readonly", "readonly");
                }
            }
        });

        if (editMode) {
            editBtn.classList.add("d-none");
            saveBtn.classList.remove("d-none");
            cancelBtn.classList.remove("d-none");
            modeBadge.textContent = "Edit mode";
            modeBadge.classList.remove("bg-secondary");
            modeBadge.classList.add("bg-warning", "text-dark");
        } else {
            editBtn.classList.remove("d-none");
            saveBtn.classList.add("d-none");
            cancelBtn.classList.add("d-none");
            modeBadge.textContent = "View mode";
            modeBadge.classList.remove("bg-warning", "text-dark");
            modeBadge.classList.add("bg-secondary");
        }
    }

    // Başlangıç: View mode
    setEditMode(false);

    editBtn.addEventListener("click", function (e) {
        e.preventDefault();
        setEditMode(true);
    });

    cancelBtn.addEventListener("click", function (e) {
        e.preventDefault();

        // Alanları başlangıç değerlerine geri al
        editableFields.forEach(function (el) {
            if (initialValues.hasOwnProperty(el.name)) {
                el.value = initialValues[el.name];
            }
        });

        setEditMode(false);
    });

    form.addEventListener("submit", function () {
        // Double click önleme
        saveBtn.disabled = true;
    });
})();