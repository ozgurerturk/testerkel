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
            var res = await fetch('/Account/ChangePassword', {
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