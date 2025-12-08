using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using testerkel.Data;
using testerkel.Models;
using testerkel.ViewModels.Stock;

public class StocksController : Controller
{
    private readonly ErkelErpDbContext _context;

    public StocksController(ErkelErpDbContext context)
    {
        _context = context;
    }

    // ---------------------------------------------------------
    //  STOK İŞLEMLERİ ANA SAYFASI
    // ---------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> Index(int? warehouseId)
    {
        var vm = new StockOperationVm
        {
            WarehouseId = warehouseId ?? 0,
            Operation = StockOperationType.PurchaseIn
        };

        await FillWarehousesAsync();
        return View("/Views/Stocks/Index.cshtml", vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(StockOperationVm vm)
    {
        if (vm.WarehouseId <= 0)
        {
            ModelState.AddModelError(nameof(vm.WarehouseId), "Lütfen bir depo seçiniz.");
        }

        if (!ModelState.IsValid)
        {
            await FillWarehousesAsync();
            return View("/Views/Stocks/Index.cshtml", vm);
        }

        switch (vm.Operation)
        {
            case StockOperationType.PurchaseIn:
                return RedirectToAction(nameof(PurchaseIn), new { warehouseId = vm.WarehouseId });

            case StockOperationType.SalesOut:
                return RedirectToAction(nameof(SalesOut), new { warehouseId = vm.WarehouseId });

            case StockOperationType.ConsumptionOut:
                return RedirectToAction(nameof(ConsumptionOut), new { warehouseId = vm.WarehouseId });

            case StockOperationType.Transfer:
                return RedirectToAction(nameof(Transfer), new { fromWarehouseId = vm.WarehouseId });

            default:
                return RedirectToAction(nameof(Index));
        }
    }


    // ---------------------------------------------------------
    //  STOK GİRİŞİ (PurchaseIn)
    // ---------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> PurchaseIn(int warehouseId)
    {
        var vm = new StockTxnVm
        {
            WarehouseId = warehouseId,
            PageTitle = "Stok Girişi (Satınalma)",
            SubmitText = "Stok Girişi Kaydet"
        };

        await FillProductsAsync();
        return View("/Views/Stocks/StockMove.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PurchaseIn(StockTxnVm vm)
    {
        if (!ModelState.IsValid)
        {
            await FillProductsAsync();
            return View("/Views/Stocks/StockMove.cshtml", vm);
        }

        var txn = new StockTxn
        {
            WarehouseId = vm.WarehouseId,
            ProductId = vm.ProductId,
            Qty = vm.Qty,
            Direction = StockDirection.In,
            MovementType = StockMovementType.PurchaseIn,
            TxnDate = vm.TxnDate
        };

        _context.StockTxns.Add(txn);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Warehouses", new { id = vm.WarehouseId });
    }


    // ---------------------------------------------------------
    //  STOK ÇIKIŞI (SalesOut)
    // ---------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> SalesOut(int warehouseId)
    {
        var vm = new StockTxnVm
        {
            WarehouseId = warehouseId,
            PageTitle = "Stok Çıkışı (Satış)",
            SubmitText = "Stok Çıkışını Kaydet"
        };

        await FillProductsAsync();
        return View("/Views/Stocks/StockMove.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SalesOut(StockTxnVm vm)
    {
        if (!ModelState.IsValid)
        {
            await FillProductsAsync();
            return View("/Views/Stocks/StockMove.cshtml", vm);
        }

        var txn = new StockTxn
        {
            WarehouseId = vm.WarehouseId,
            ProductId = vm.ProductId,
            Qty = vm.Qty,
            Direction = StockDirection.Out,
            MovementType = StockMovementType.SalesOut,
            TxnDate = vm.TxnDate
        };

        _context.StockTxns.Add(txn);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Warehouses", new { id = vm.WarehouseId });
    }


    // ---------------------------------------------------------
    //  SARF / TÜKETİM (ConsumptionOut)
    // ---------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> ConsumptionOut(int warehouseId)
    {
        var vm = new StockTxnVm
        {
            WarehouseId = warehouseId,
            PageTitle = "Sarf / Tüketim Çıkışı",
            SubmitText = "Sarf Kaydet"
        };

        await FillProductsAsync();
        return View("/Views/Stocks/StockMove.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConsumptionOut(StockTxnVm vm)
    {
        if (!ModelState.IsValid)
        {
            await FillProductsAsync();
            return View("/Views/Stocks/StockMove.cshtml", vm);
        }

        var txn = new StockTxn
        {
            WarehouseId = vm.WarehouseId,
            ProductId = vm.ProductId,
            Qty = vm.Qty,
            Direction = StockDirection.Out,
            MovementType = StockMovementType.ConsumptionOut,
            TxnDate = vm.TxnDate
        };

        _context.StockTxns.Add(txn);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Warehouses", new { id = vm.WarehouseId });
    }

    // GET: /Stocks/Transfer
    [HttpGet]
    public async Task<IActionResult> Transfer(int? fromWarehouseId)
    {
        var vm = new StockTransferEditVm
        {
            FromWarehouseId = fromWarehouseId ?? 0,
            TransferDate = DateTime.Today,
            Lines = new List<StockTransferLineVm>
        {
            new StockTransferLineVm()
        }
        };

        await FillProductsAndWarehousesAsync(); // helper, action değil
        return View("/Views/Stocks/StockTransfer.cshtml", vm);
    }

    // POST: /Stocks/Transfer
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transfer(StockTransferEditVm vm)
    {
        if (vm.FromWarehouseId == vm.ToWarehouseId)
        {
            ModelState.AddModelError(string.Empty, "Kaynak ve hedef depo aynı olamaz.");
        }

        if (!ModelState.IsValid)
        {
            await FillProductsAndWarehousesAsync(); // helper, action değil
            return View("/Views/Stocks/StockTransfer.cshtml", vm);
        }

        // 1) Transfer fişini oluştur
        var transfer = new StockTransfer
        {
            FromWarehouseId = vm.FromWarehouseId,
            ToWarehouseId = vm.ToWarehouseId,
            TransferDate = vm.TransferDate,
            Notes = vm.Notes
        };

        _context.StockTransfers.Add(transfer);
        await _context.SaveChangesAsync(); // transfer.Id oluşsun diye

        // 2) Satırları ve stok hareketlerini oluştur
        foreach (var line in vm.Lines.Where(x => x.Qty > 0))
        {
            var transferLine = new StockTransferLine
            {
                TransferId = transfer.Id,
                ProductId = line.ProductId,
                Qty = line.Qty
            };
            _context.StockTransferLines.Add(transferLine);

            // Çıkış hareketi (kaynak depo)
            _context.StockTxns.Add(new StockTxn
            {
                ProductId = line.ProductId,
                WarehouseId = vm.FromWarehouseId,
                Qty = line.Qty,
                Direction = StockDirection.Out,
                MovementType = StockMovementType.TransferOut,
                TxnDate = vm.TransferDate,
                RefModule = "TRF",
                RefId = transfer.Id
            });

            // Giriş hareketi (hedef depo)
            _context.StockTxns.Add(new StockTxn
            {
                ProductId = line.ProductId,
                WarehouseId = vm.ToWarehouseId,
                Qty = line.Qty,
                Direction = StockDirection.In,
                MovementType = StockMovementType.TransferIn,
                TxnDate = vm.TransferDate,
                RefModule = "TRF",
                RefId = transfer.Id
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Warehouses", new { id = vm.FromWarehouseId });
    }



    // ---------------------------------------------------------
    //  DROPDOWN İÇİN ÜRÜN LİSTESİ DOLDURMA
    // ---------------------------------------------------------
    private async Task FillProductsAsync()
    {
        var products = await _context.Products
            .OrderBy(p => p.Code)
            .ToListAsync();

        ViewBag.Products = new SelectList(products, "Id", "Name");
    }

    private async Task FillProductsAndWarehousesAsync()
    {
        await FillProductsAsync();

        ViewBag.Warehouses = new SelectList(
            await _context.Warehouses.OrderBy(w => w.Name).ToListAsync(),
            "Id", "Name"
        );
    }
    private async Task FillWarehousesAsync()
    {
        var warehouses = await _context.Warehouses
            .OrderBy(w => w.Name)
            .ToListAsync();

        ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
    }

}
