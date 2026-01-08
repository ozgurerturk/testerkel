function initElements() {
    if (document.title !== "Kullanıcılar") {
        return;
    }

    $('#usersTable').DataTable({
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
    })
};

function initIndexMethods() {
    
    if (document.title !== "Kullanıcılar") {
        return;
    }
    // Click handler for delete buttons
    $(document).on("click", ".js-user-delete", function (e) {
        e.preventDefault();

        var button = $(this);
        var id = button.data("id");

        Swal.fire({
            title: "Emin misiniz?",
            text: "Bu üyeyi silmek üzeresiniz. İşlem geri alınamaz.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Evet, sil",
            cancelButtonText: "İptal"
        }).then(function (result) {
            if (!result.isConfirmed) {
                return;
            }

            $.ajax({
                url: "/Users/Delete",
                type: "POST",
                data: { userId: id },
                success: function (resp) {
                    if (resp && resp.result) {
                        var row = button.closest("tr");
                        row.fadeOut(200, function () {
                            row.remove();
                        });

                        var deletedUserEmail = resp.data;

                        Swal.fire({
                            icon: "success",
                            title: "Silindi",
                            text: deletedUserEmail + " kullanıcısı başarıyla silindi.",
                            timer: 1200,
                            showConfirmButton: false
                        });
                    } else {
                        var errorList = "";

                        if (resp && resp.errors) {
                            resp.errors.forEach(err => {
                                errorList += err + "<br>";
                            });
                        }

                        Swal.fire({
                            icon: "error",
                            title: "Hata",
                            html: errorList || "Silme işlemi başarısız oldu."
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
};

function initChangePassword() {
    var changePasswordButton = document.getElementById("btnChangePassword");
    if (!changePasswordButton) return;

    changePasswordButton.addEventListener("click", async function (e) {
        e.preventDefault();

        var tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
        var token = tokenEl ? tokenEl.value : "";

        var currentPassword = $('#txtCurrentPassword').val();
        var newPassword = $('#txtNewPassword').val();
        var newPasswordConfirm = $('#txtNewPasswordConfirm').val();
        var userId = $('#userId').val();

        var confirm = await Swal.fire({
            icon: "question",
            title: "Emin misin?",
            text: "Şifren güncellenecek. Devam etmek istiyor musun?",
            showCancelButton: true,
            confirmButtonText: "Evet, değiştir",
            cancelButtonText: "Vazgeç"
        });

        if (!confirm.isConfirmed) return;

        var dataPasswordChangeVm = {
            CurrentPassword: currentPassword,
            NewPassword: newPassword,
            NewPasswordConfirm: newPasswordConfirm,
            UserId: userId
        };

        try {
            var res = await fetch('/Users/ChangePassword', {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                },
                body: JSON.stringify(dataPasswordChangeVm)
            });

            var data = null;
            try { data = await res.json(); } catch (jsonErr) { }

            if (!res.ok) {
                await Swal.fire({
                    icon: "error",
                    title: "Hata",
                    text: (data && data.message) ? data.message : "İşlem başarısız."
                });
                return;
            }

            if (data && data.result) {
                await Swal.fire({
                    icon: 'success',
                    title: 'Başarılı',
                    text: 'Şifreniz başarıyla değiştirildi.'
                });

                $('#txtCurrentPassword').val('');
                $('#txtNewPassword').val('');
                $('#txtNewPasswordConfirm').val('');
            } else {
                var html = (data && data.errors && data.errors.length)
                    ? data.errors.join('<br>')
                    : 'Şifre değiştirilemedi.';

                await Swal.fire({
                    icon: 'error',
                    title: 'Hata',
                    html: html
                });
            }
        } catch (err) {
            await Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Sunucuya bağlanırken hata oluştu.'
            });
        }
    });
}


document.addEventListener("DOMContentLoaded", function () {
    initElements();
    initIndexMethods();
});