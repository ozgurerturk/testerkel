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
}

// Sayfa yüklendiğinde sadece "ilgili" init fonksiyonlarını çağır
document.addEventListener("DOMContentLoaded", function () {
    // İleride başka sayfa init fonksiyonları da ekleyebilirsin:
    // initCustomerIndexPage(), initOrderCreatePage(), vs.
    // Şu an sadece product index init çalışacak:
    initProductIndexPage();
});
