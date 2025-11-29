namespace testerkel.Models
{
    public enum UnitType : byte
    {
        Adet = 1,     // Adet
        Kilogram = 2,  // Kg
        Litre = 3,     // Litre
        Kutu = 4,       // Kutu
        Metre = 5,     // Metre
        Saat = 6       // Saat (hizmet/çalışma süresi)
    }


    public enum StockDirection : byte
    {
        In = 1,
        Out = 2
    }

    public enum StockMovementType : byte
    {
        PurchaseIn = 1,   // Satınalma girişi
        TransferIn = 2,   // Depodan depoya transfer girişi

        ConsumptionOut = 10, // Sarf
        SalesOut = 11, // Satış
        TransferOut = 12  // Depodan depoya transfer çıkışı
    }

}
