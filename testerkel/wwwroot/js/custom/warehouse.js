// wwwroot/js/custom/warehouse.js
(function () {

    // Edit/View mode + Save buton davranışı
    window.initWarehouseEditToggle = function (options) {
        var formId = options && options.formId ? options.formId : "warehouseForm";
        var form = document.getElementById(formId);
        var editBtn = document.getElementById(options.editButtonId || "btnEdit");
        var saveBtn = document.getElementById(options.saveButtonId || "btnSave");
        var cancelBtn = document.getElementById(options.cancelButtonId || "btnCancel");
        var modeBadge = document.getElementById(options.modeBadgeId || "modeBadge");

        if (!form || !editBtn || !saveBtn || !cancelBtn || !modeBadge) {
            return;
        }

        var editableFields = form.querySelectorAll("[data-editable='true']");
        var isEditMode = false;
        var initialValues = {};

        // İlk değerleri sakla (Cancel için)
        editableFields.forEach(function (el) {
            initialValues[el.name] = el.type === "checkbox" ? el.checked : el.value;
        });

        function setEditMode(editMode) {
            isEditMode = editMode;

            editableFields.forEach(function (el) {
                if (editMode) {
                    el.removeAttribute("readonly");
                    el.removeAttribute("disabled");
                } else {
                    if (el.tagName === "SELECT" || el.type === "checkbox") {
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

            editableFields.forEach(function (el) {
                if (initialValues.hasOwnProperty(el.name)) {
                    if (el.type === "checkbox") {
                        el.checked = initialValues[el.name];
                    } else {
                        el.value = initialValues[el.name];
                    }
                }
            });

            setEditMode(false);
        });

        // Save: valid değilse disable etme, validse formu gönder
        saveBtn.addEventListener("click", function (e) {
            e.preventDefault();

            if (window.jQuery && $.validator && $(form).data("validator")) {
                var isValid = $(form).valid();
                if (!isValid) {
                    saveBtn.disabled = false;
                    return;
                }
            }

            saveBtn.disabled = true;
            form.submit();
        });
    };

    // Depodaki ürünleri ayrı tab'da AJAX ile getir
    window.initWarehouseProductsTab = function (options) {
        var productsTabId = options.productsTabId || "products-tab";
        var containerId = options.productsContainerId || "warehouseProductsContainer";
        var warehouseId = options.warehouseId;
        var getProductsUrl = options.getProductsUrl; // base URL, örn: /Warehouses/GetProducts

        var productsTab = document.getElementById(productsTabId);
        var container = document.getElementById(containerId);
        var loaded = false;

        if (!productsTab || !container || !warehouseId || !getProductsUrl) {
            return;
        }

        productsTab.addEventListener("shown.bs.tab", function () {
            if (loaded) return;

            var url = getProductsUrl + "?warehouseId=" + warehouseId;

            fetch(url)
                .then(function (r) { return r.json(); })
                .then(function (data) {
                    loaded = true;

                    if (!data || !data.length) {
                        container.innerHTML = '<div class="text-muted">Bu depoda ürün bulunmuyor.</div>';
                        return;
                    }

                    var html = '';
                    html += '<table class="table table-sm table-bordered table-striped mb-0">';
                    html += '<thead class="table-light">';
                    html += '<tr>';
                    html += '<th>Kod</th>';
                    html += '<th>Ürün Adı</th>';
                    html += '<th>Birim</th>';
                    html += '<th class="text-end">Fiyat</th>';
                    html += '<th class="text-end">Miktar</th>';
                    html += '</tr>';
                    html += '</thead>';
                    html += '<tbody>';

                    data.forEach(function (p) {
                        html += '<tr>';
                        html += '<td>' + (p.code ?? '') + '</td>';
                        html += '<td>' + (p.name ?? '') + '</td>';
                        html += '<td>' + (p.unit ?? '') + '</td>';
                        html += '<td class="text-end">' + (p.price != null ? Number(p.price).toFixed(2) : '') + '</td>';
                        html += '<td class="text-end">' + (p.quantity != null ? p.quantity : '') + '</td>';
                        html += '</tr>';
                    });

                    html += '</tbody></table>';

                    container.innerHTML = html;
                })
                .catch(function () {
                    container.innerHTML = '<div class="text-danger">Ürünler yüklenirken bir hata oluştu.</div>';
                });
        });
    };

    // Bu sayfaya özel toplu init (Details sayfasında sadece bunu çağırırsın)
    window.warehouseDetailsInit = function (options) {
        options = options || {};
        window.initWarehouseEditToggle(options);
        window.initWarehouseProductsTab(options);
    };

})();
