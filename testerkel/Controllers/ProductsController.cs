using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using testerkel.Data;
using testerkel.Models;
using testerkel.ViewModels.Product;
using testerkel.ViewModels.Stock;

namespace testerkel.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ErkelErpDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ErkelErpDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Data()
        {
            var draw = Request.Form["draw"].FirstOrDefault();

            var start = int.TryParse(Request.Form["start"].FirstOrDefault(), out var s) ? s : 0;
            var length = int.TryParse(Request.Form["length"].FirstOrDefault(), out var l) ? l : 25;

            var searchValue = Request.Form["search[value]"].FirstOrDefault()?.Trim();

            var sortColIndex = int.TryParse(Request.Form["order[0][column]"].FirstOrDefault(), out var ci) ? ci : 0;
            var sortDir = Request.Form["order[0][dir]"].FirstOrDefault() ?? "asc";

            var sortCol = Request.Form[$"columns[{sortColIndex}][data]"].FirstOrDefault() ?? "code";

            var query = _context.Products.AsNoTracking();

            var recordsTotal = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                var trCollation = "Turkish_CI_AS";

                query = query.Where(x =>
                    EF.Functions.Like(
                        EF.Functions.Collate(x.Code, trCollation),
                        $"%{searchValue}%"
                    )
                    ||
                    (x.Name != null && EF.Functions.Like(
                        EF.Functions.Collate(x.Name, trCollation),
                        $"%{searchValue}%"
                    ))
                );
            }

            var recordsFiltered = await query.CountAsync();

            // Sorting
            if (sortCol == "code")
                query = sortDir == "asc" ? query.OrderBy(x => x.Code) : query.OrderByDescending(x => x.Code);
            else if (sortCol == "name")
                query = sortDir == "asc" ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
            else if (sortCol == "unit")
                query = sortDir == "asc" ? query.OrderBy(x => x.Unit) : query.OrderByDescending(x => x.Unit);
            else
                query = query.OrderBy(x => x.Code);

            var data = await query
                .Skip(start)
                .Take(length)
                .Select(x => new
                {
                    id = x.Id,
                    code = x.Code,
                    name = string.IsNullOrWhiteSpace(x.Name) ? "-" : x.Name,
                    unit = x.Unit.ToString(),
                    actions =
                        $"<a class='btn btn-sm btn-outline-secondary me-1' href='/Products/Details/{x.Id}'>Detay/Düzenle</a>" +
                        $"<a class='btn btn-sm btn-outline-info me-1' href='/Products/PriceInfo/{x.Id}'>Fiyat Bilgisi</a>" +
                        $"<button type='button' class='btn btn-sm btn-outline-danger js-product-delete' data-id='{x.Id}'>Sil</button>"
                })
                .ToListAsync();

            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data
            });
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var vm = new ProductEditVm
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Unit = product.Unit
            };

            PopulateUnitTypes(vm); // Edit’te kullandığın helper

            return View(vm); // @model ProductEditVm
        }


        // GET: Products/Create
        public IActionResult Create()
        {
            var vm = new ProductCreateVm
            {
                Code = string.Empty,
                Unit = UnitType.Adet
            };
            PopulateUnitTypes(vm);
            return View(vm); // @model ProductCreateVm
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateUnitTypes(vm);
                return View(vm);
            }

            var product = new Product
            {
                Code = vm.Code,
                Name = vm.Name,
                Unit = vm.Unit
            };

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created. Id={Id}, Code={Code}", product.Id, product.Code);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Örneğin Code unique ise ve çakışma olduysa
                _logger.LogError(ex, "Error while creating product. Code={Code}", product.Code);
                ModelState.AddModelError(string.Empty, "An error occurred while saving product.");
                PopulateUnitTypes(vm);
                return View(vm);
            }
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditVm vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateUnitTypes(vm);
                return View("Details", vm);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.Code = vm.Code;
            product.Name = vm.Name;
            product.Unit = vm.Unit;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = product.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult CreateExcelTemplate()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Malzemeler");

            ws.Cell(1, 1).Value = "Malzeme Kodu";
            ws.Cell(1, 2).Value = "Malzeme İsmi";
            ws.Cell(1, 3).Value = "Birim";

            ws.Row(1).Style.Font.Bold = true;

            ws.Cell(2, 1).Value = "MALZ-001";
            ws.Cell(2, 2).Value = "Örnek Malzeme";
            ws.Cell(2, 3).Value = "Adet";

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            byte[] content = stream.ToArray();

            var fileName = "MalzemeExcelKalıbı.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["Error"] = "Lütfen bir excel dosyası seçin.";
                    return RedirectToAction(nameof(Index));
                }

                if (!Path.HasExtension(file.FileName) ||
                    !Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "Sadece .xlsx uzantılı dosyalar yüklenebilir.";
                    return RedirectToAction(nameof(Index));
                }

                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);

                var worksheet = workbook.Worksheet(1);
                if (worksheet.IsEmpty())
                {
                    TempData["Error"] = "Excel dosyası boş veya ilk sayfa boş.";
                    return RedirectToAction(nameof(Index));
                }

                var firstRowUsed = worksheet.FirstRowUsed();
                var lastRowUsed = worksheet.LastRowUsed();
                if (firstRowUsed == null || lastRowUsed == null)
                {
                    TempData["Error"] = "Excel dosyası boş veya ilk sayfa boş.";
                    return RedirectToAction(nameof(Index));
                }

                var headerRowNumber = firstRowUsed.RowNumber();

                var codeHeader = worksheet.Cell(headerRowNumber, 1).GetString().Trim();
                var nameHeader = worksheet.Cell(headerRowNumber, 2).GetString().Trim();
                var unitHeader = worksheet.Cell(headerRowNumber, 3).GetString().Trim();

                if ((codeHeader != "Malzeme Kodu" && codeHeader != "Kod") ||
                    (nameHeader != "Malzeme İsmi" && nameHeader != "İsim" && nameHeader != "Ad" && nameHeader != "Malzeme Adı") ||
                    unitHeader != "Birim")
                {
                    TempData["Error"] = "Excel başlıkları beklenen formatta değil. Lütfen şablonu kullanın.";
                    return RedirectToAction(nameof(Index));
                }

                // Rapor kolonlarını ekle (Durum + Hata)
                var lastCol = worksheet.LastColumnUsed()?.ColumnNumber() ?? 3;
                var statusCol = lastCol + 1;
                var errorCol = lastCol + 2;

                worksheet.Cell(headerRowNumber, statusCol).Value = "Durum";
                worksheet.Cell(headerRowNumber, errorCol).Value = "Hata Notu";
                worksheet.Row(headerRowNumber).Style.Font.Bold = true;

                // DB'de olan kodları tek seferde çek (unique hatasını burada yakalarız)
                var incomingCodes = new List<string>();

                var firstDataRow = headerRowNumber + 1;
                var lastDataRow = lastRowUsed.RowNumber();

                for (var row = firstDataRow; row <= lastDataRow; row++)
                {
                    var code = worksheet.Cell(row, 1).GetString().Trim();
                    if (!string.IsNullOrWhiteSpace(code))
                        incomingCodes.Add(code);
                }

                var distinctIncoming = incomingCodes
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var existingCodes = await _context.Products.AsNoTracking()
                    .Where(p => distinctIncoming.Contains(p.Code))
                    .Select(p => p.Code)
                    .ToListAsync();

                var existingSet = new HashSet<string>(existingCodes, StringComparer.OrdinalIgnoreCase);

                // Excel içi duplicate kontrolü için
                var seenCodesInExcel = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var productsToAdd = new List<Product>();
                var hasAnyError = false;

                for (var row = firstDataRow; row <= lastDataRow; row++)
                {
                    // tamamen boş satır skip
                    if (worksheet.Row(row).CellsUsed().Count() == 0)
                        continue;

                    var codeCell = worksheet.Cell(row, 1);
                    var nameCell = worksheet.Cell(row, 2);
                    var unitCell = worksheet.Cell(row, 3);

                    var code = codeCell.GetString().Trim();
                    var name = nameCell.GetString().Trim();
                    var unitStr = unitCell.GetString().Trim();

                    var errors = new List<string>();

                    // Kod zorunlu
                    if (string.IsNullOrWhiteSpace(code))
                    {
                        errors.Add("Kod boş olamaz.");
                        // Kod hücresini kırmızı yap
                        codeCell.Style.Fill.BackgroundColor = XLColor.LightPink;
                    }
                    else
                    {
                        // Excel içi duplicate
                        if (!seenCodesInExcel.Add(code))
                        {
                            errors.Add($"Excel içinde tekrar eden kod: {code}");
                            codeCell.Style.Fill.BackgroundColor = XLColor.LightPink;
                        }

                        // DB duplicate
                        if (existingSet.Contains(code))
                        {
                            errors.Add($"Bu kod zaten sistemde var: {code}");
                            codeCell.Style.Fill.BackgroundColor = XLColor.LightPink;
                        }
                    }

                    // Unit alias çözümle
                    UnitType? resolvedUnit = null;
                    if (!string.IsNullOrWhiteSpace(unitStr))
                    {
                        resolvedUnit = await ResolveUnitTypeAsync(unitStr);
                        if (resolvedUnit == null)
                        {
                            errors.Add($"Birim çözümlenemedi: '{unitStr}'");
                            unitCell.Style.Fill.BackgroundColor = XLColor.LightPink;
                        }
                    }
                    else
                    {
                        errors.Add("Birim boş olamaz.");
                        unitCell.Style.Fill.BackgroundColor = XLColor.LightPink;
                    }

                    if (errors.Count > 0)
                    {
                        hasAnyError = true;

                        // Satırı kırmızıya boya (tam satır)
                        worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#ffe5e5");

                        worksheet.Cell(row, statusCol).Value = "HATALI";
                        worksheet.Cell(row, statusCol).Style.Font.FontColor = XLColor.DarkRed;
                        worksheet.Cell(row, statusCol).Style.Font.Bold = true;

                        worksheet.Cell(row, errorCol).Value = string.Join(" | ", errors);
                        continue;
                    }

                    // Doğru satır
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#eaffea");
                    worksheet.Cell(row, statusCol).Value = "OK";
                    worksheet.Cell(row, statusCol).Style.Font.FontColor = XLColor.DarkGreen;
                    worksheet.Cell(row, statusCol).Style.Font.Bold = true;

                    worksheet.Cell(row, errorCol).Value = "";

                    productsToAdd.Add(new Product
                    {
                        Code = code,
                        Name = string.IsNullOrWhiteSpace(name) ? null : name,
                        Unit = resolvedUnit!.Value
                    });
                }

                worksheet.Columns().AdjustToContents();

                // Hata varsa: raporlu Excel indir
                if (hasAnyError)
                {
                    using var outStream = new MemoryStream();
                    workbook.SaveAs(outStream);
                    var content = outStream.ToArray();

                    var fileName = $"MalzemeImportRaporu_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }

                // Hata yoksa: DB kaydet
                if (productsToAdd.Count == 0)
                {
                    TempData["Error"] = "Excel dosyasında eklenebilecek geçerli ürün bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                await _context.Products.AddRangeAsync(productsToAdd);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{productsToAdd.Count} ürün başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while importing products from excel.");
                TempData["Error"] = "Excel içe aktarma sırasında bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CreateProductsExcel()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Malzemeler");

            ws.Cell(1, 1).Value = "Malzeme Kodu";
            ws.Cell(1, 2).Value = "Malzeme İsmi";
            ws.Cell(1, 3).Value = "Birim";

            ws.Row(1).Style.Font.Bold = true;

            var products = _context.Products.AsNoTracking().OrderBy(p => p.Code).ToList();

            for (int i = 0; i < products.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = products[i].Code;
                ws.Cell(i + 2, 2).Value = products[i].Name;
                ws.Cell(i + 2, 3).Value = products[i].Unit.ToString();
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            byte[] content = stream.ToArray();

            var fileName = "GüncelMalzemeExcelListesi.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CreateProductsPdf()
        {
            var products = _context.Products.AsNoTracking().OrderBy(p => p.Code).ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Text("Malzeme Listesi")
                        .FontSize(18)
                        .Bold()
                        .AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().PaddingVertical(6).Text("Kod").Bold();
                            header.Cell().PaddingVertical(6).Text("Ürün Adı").Bold();
                            header.Cell().PaddingVertical(6).Text("Birim").Bold();
                        });

                        foreach (var p in products)
                        {
                            table.Cell().PaddingVertical(6).Text(p.Code);
                            table.Cell().PaddingVertical(6).Text(p.Name ?? "");
                            table.Cell().PaddingVertical(6).Text(p.Unit.ToString());
                        }
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", "GüncelMalzemeListesi.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> PriceInfo(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var averagePrice = await _context.StockTxns
                .Where(st =>
                st.ProductId == product.Id &&
                st.Direction == StockDirection.In &&
                st.UnitPrice != null)
                .GroupBy(_ => 1)
                .Select(g => g.Sum(x => x.Qty * x.UnitPrice) / g.Sum(x => x.Qty))
                .FirstOrDefaultAsync();

            var lastPrice = await _context.StockTxns
                .Where(st =>
                    st.ProductId == product.Id &&
                    st.Direction == StockDirection.In &&
                    st.UnitPrice != null)
                .OrderByDescending(x => x.TxnDate)
                .Select(x => x.UnitPrice)
                .FirstOrDefaultAsync();

            var stockMovements = await _context.StockTxns
                .Where(x => x.ProductId == product.Id)
                .Include(x => x.Warehouse)
                .OrderByDescending(x => x.TxnDate)
                .Select(x => new StockTxnRowVm
                {
                    TxnDate = x.TxnDate,
                    Direction = x.Direction,
                    MovementType = x.MovementType,
                    Qty = x.Qty,
                    UnitPrice = x.UnitPrice,
                    WarehouseId = x.WarehouseId,
                    WarehouseName = x.Warehouse != null ? x.Warehouse.Name! : "",
                    RefModule = x.RefModule,
                    RefId = x.RefId,
                    Note = x.RefModule,
                    ProductId = x.ProductId
                })
                .ToListAsync();


            var vm = new ProductPriceInfoVm
            {
                Code = product.Code,
                Id = product.Id,
                Name = product.Name,
                Unit = product.Unit,
                PriceAverage = averagePrice,
                LastPrice = lastPrice,
                MovementHistory = stockMovements
            };
            return View(vm); // @model ProductPriceInfoVm
        }

        // --- Helper ---

        private static void PopulateUnitTypes(ProductCreateVm vm)
        {
            vm.UnitTypes = [.. Enum.GetValues(typeof(UnitType))
                .Cast<UnitType>()
                .Select(u => new SelectListItem
                {
                    Value = ((byte)u).ToString(),
                    Text = u.ToString(),
                    Selected = (vm.Unit == u)
                })];
        }

        private static void PopulateUnitTypes(ProductEditVm vm)
        {
            vm.UnitTypes = [.. Enum.GetValues(typeof(UnitType))
                .Cast<UnitType>()
                .Select(u => new SelectListItem
                {
                    Value = ((byte)u).ToString(),
                    Text = u.ToString(),
                    Selected = (vm.Unit == u)
                })];
        }

        private async Task<UnitType?> ResolveUnitTypeAsync(string unitStr)
        {
            if (string.IsNullOrWhiteSpace(unitStr))
                return null;

            var raw = unitStr.Trim();

            // Normalize: boşluk, nokta, büyük/küçük farkı kaldır
            var normalized = raw
                .Replace(" ", "")
                .Replace(".", "")
                .ToLowerInvariant();

            // 1) Alias tablosundan ara
            var unitFromAlias = await _context.UnitAliases
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Alias
                     .Replace(" ", "")
                     .Replace(".", "")
                     .ToLower() == normalized);

            if (unitFromAlias != null)
                return unitFromAlias.UnitType;

            // 2) Fallback: enum adı yazılmış olabilir (Kilogram, Metre vb.)
            if (Enum.TryParse<UnitType>(raw, true, out var enumUnit))
                return enumUnit;

            return null;
        }
    }
}
