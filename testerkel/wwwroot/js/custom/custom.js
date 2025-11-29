// Source - https://stackoverflow.com/a
// Posted by Nick Craver
// Retrieved 2025-11-25, License - CC BY-SA 2.5

jQuery.extend(jQuery.validator.messages, {
    required: "Bu alan zorunludur.",
    remote: "Lütfen bu alanı düzeltiniz.",
    email: "Lütfen geçerli bir email adresi giriniz.",
    url: "Lütfen geçerli bir URL giriniz.",
    date: "Lütfen geçerli bir tarih giriniz.",
    dateISO: "Lütfen geçerli bir tarih giriniz (ISO).",
    number: "Lütfen geçerli bir sayı giriniz.",
    digits: "Lütfen sadece rakam giriniz",
    creditcard: "Lütfen geçerli bir kredi kartı numarası giriniz.",
    equalTo: "Aynı değeri tekrar giriniz",
    accept: "Lütfen geçerli uzantılı bir değer giriniz",
    maxlength: jQuery.validator.format("Lütfen en fazla {0} karakter giriniz."),
    minlength: jQuery.validator.format("Lütfen en az {0} karakter giriniz."),
    rangelength: jQuery.validator.format("Karakter sayısı {0} ve {1} aralığında olmalı."),
    range: jQuery.validator.format("{0} ve {1} arası bir değer girin."),
    max: jQuery.validator.format("Girebileceğiniz azami değer: {0}."),
    min: jQuery.validator.format("Girebileceğiniz asgari değer:  {0}.")
});
