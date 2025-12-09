namespace testerkel.Models
{
    public enum UnitType : byte
    {
        Adet = 1,
        Kilogram = 2,
        Litre = 3,
        Kutu = 4,
        Metre = 5,
        Saat = 6,
        AdTl = 7,
        Cift = 8,
        MetreKare = 9,
        MetreKup = 10,
        Paket = 11,
        Takım = 12,
        Ton = 13,
        Top = 14
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
