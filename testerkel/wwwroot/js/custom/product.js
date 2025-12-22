// product.js

// Sadece Products/Index sayfası varsa çalışan init fonksiyonu
function initProductIndexPage() {
    // Bu sayfa değilse hiçbir şey yapma
    var pageRoot = document.getElementById("product-index-page");
    if (!pageRoot) {
        return;
    }

    console.log("Product Index page init");

    // Delete butonları için click handler
    $(document).on("click", ".js-product-delete", function (e) {
        e.preventDefault();

        var button = $(this);
        var id = button.data("id");

        Swal.fire({
            title: "Emin misiniz?",
            text: "Bu ürünü silmek üzeresiniz. İşlem geri alınamaz.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Evet, sil",
            cancelButtonText: "İptal"
        }).then(function (result) {
            if (!result.isConfirmed) {
                return;
            }

            // Evet dendi → AJAX ile delete
            $.ajax({
                url: "/Products/DeleteAjax",
                type: "POST",
                data: { id: id },
                success: function (resp) {
                    if (resp && resp.success) {
                        // Satırı DOM'dan kaldır
                        var row = button.closest("tr");
                        row.fadeOut(200, function () {
                            row.remove();
                        });

                        Swal.fire({
                            icon: "success",
                            title: "Silindi",
                            text: "Ürün başarıyla silindi.",
                            timer: 1200,
                            showConfirmButton: false
                        });
                    } else {
                        Swal.fire({
                            icon: "error",
                            title: "Hata",
                            text: resp && resp.message ? resp.message : "Silme işlemi başarısız oldu."
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: "error",
                        title: "Hata",
                        text: "Sunucuya erişilirken bir sorun oluştu."
                    });
                }
            });
        });
    });

    var dt = new DataTable("#productTable", {
        processing: true,
        serverSide: true,
        searching: true,
        ordering: true,
        pageLength: 25,
        lengthMenu: [10, 25, 50, 100],

        ajax: {
            url: "/Products/Data",
            type: "POST"
        },

        columns: [
            { data: "code" },
            { data: "name" },
            { data: "unit" },
            { data: "actions", orderable: false, searchable: false }
        ],

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

// Sayfa yüklendiğinde sadece "ilgili" init fonksiyonlarını çağır
document.addEventListener("DOMContentLoaded", function () {
    // İleride başka sayfa init fonksiyonları da ekleyebilirsin:
    // initCustomerIndexPage(), initOrderCreatePage(), vs.
    // Şu an sadece product index init çalışacak:
    initProductIndexPage();
});
