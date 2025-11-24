namespace testerkel.Models
{
    public enum UnitType : byte
    {
        Piece = 1,     // Adet
        Kilogram = 2,  // Kg
        Liter = 3,     // Litre
        Box = 4,       // Kutu
        Meter = 5,     // Metre
        Hour = 6       // Saat (hizmet/çalışma süresi)
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
