function initElements() {
    $('#productSelect').select2({
        theme: "classic",
        width: '100%'
    }
    );

    var dt = new DataTable("#warehouseTable", {
        searching: true,
        ordering: true,
        pageLength: 25,
        lengthMenu: [10, 25, 50, 100],

        language: {
            search: "Ara:",
            emptyTable: "Gösterilecek kayıt yok",
            info: "_TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
            infoEmpty: "Kayıt yok",
            infoFiltered: "(_MAX_ kayıttan filtrelendi)",
            lengthMenu: "Sayfada _MENU_ kayıt göster",
            loadingRecords: "Yükleniyor...",
            processing: "İşleniyor...",
            zeroRecords: "Eşleşen kayıt bulunamadı",
            paginate: {
                first: "İlk Sayfa",
                last: "Son Sayfa",
                next: "Sonraki",
                previous: "Önceki"
            },
            aria: {
                orderable: ": sütunu sıralamak için tıklayın",
                orderableReverse: ": sütunu sıralamayı kaldırmak için tıklayın"
            }
        }
    });
}

function getProducts() {
    $('#productSelect')
        .on('select2:open', function () {
            var $search = $('.select2-search__field');

            $search
                .off('input.productSearch')
                .on('input.productSearch', function () {
                    doSearch($(this).val());
                });
        })
        .on('select2:selecting', function () {
            suppressSearch = true;
        })
        .on('select2:select', function () {
            setTimeout(function () { suppressSearch = false; }, 50);
        })
        .on('select2:close', function () {
            suppressSearch = false;
        });
}
function debounce(func, wait) {
    var timeout;
    return function () {
        var context = this;
        var args = arguments;
        clearTimeout(timeout);
        timeout = setTimeout(function () {
            func.apply(context, args);
        }, wait);
    };
}

var lastXhr = null;
var suppressSearch = false;
var debounceMs = 400;

var doSearch = debounce(function (term) {
    if (suppressSearch) return;

    var $select = $('#productSelect');
    var isOpen = $select.data('select2') && $select.data('select2').isOpen();

    if (lastXhr && lastXhr.readyState !== 4) lastXhr.abort();

    lastXhr = $.ajax({
        type: 'POST',
        url: '/Stocks/FillProducts',
        dataType: 'json',
        data: { searchTerm: term },
        success: function (list) {
            var selectedVal = $select.val();

            $select.find('option').not(':selected').remove();

            var existing = {};
            $select.find('option').each(function () { existing[this.value] = true; });

            list.forEach(function (item) {
                var id = String(item.id);
                if (!existing[id]) {
                    $select.append(new Option(item.text, id, false, false));
                    existing[id] = true;
                }
            });

            if (selectedVal) $select.val(selectedVal);

            $select.trigger('change');

            if (isOpen) {
                var $search = $('.select2-search__field');
                $search.val(term);

                $search.focus();
                var el = $search.get(0);
                if (el) el.setSelectionRange(term.length, term.length);

                $select.trigger('select2:select');
            }
        },
        error: function (xhr, status) {
            if (status !== 'abort') {
                console.log('http:', xhr.status, xhr.statusText);
                console.log('responseText:', xhr.responseText);
            }
        }
    });
}, debounceMs);

document.addEventListener("DOMContentLoaded", function () {
    initElements();
    getProducts();
});