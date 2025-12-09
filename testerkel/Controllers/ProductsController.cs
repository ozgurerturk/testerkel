using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2019.Excel.RichData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using testerkel.Data;
using testerkel.Models;
using testerkel.ViewModels;
using testerkel.ViewModels.Product;

namespace testerkel.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            // View: @model IEnumerable<testerkel.Models.Product>
            var products = await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Code)
                .ToListAsync();

            return View(products);
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
                Price = product.Price,
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
                Name = vm.Name,   // null olabilir
                Price = vm.Price,
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
            product.Price = vm.Price;
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
            ws.Cell(1, 4).Value = "Fiyat";

            ws.Row(1).Style.Font.Bold = true;

            ws.Cell(2, 1).Value = "MALZ-001";
            ws.Cell(2, 2).Value = "Örnek Malzeme";
            ws.Cell(2, 3).Value = "Adet";
            ws.Cell(2, 4).Value = 5.00m;

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
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Lütfen bir excel dosyası seçin.";
                return RedirectToAction(nameof(Index));
            }

            if (!Path.HasExtension(file.FileName))
            {
                TempData["Error"] = "Geçersiz dosya uzantısı.";
                return RedirectToAction(nameof(Index));
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Sadece .xlsx uzantılı dosyalar yüklenebilir.";
                return RedirectToAction(nameof(Index));
            }

            var productsToAdd = new List<Product>();

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            var worksheet = workbook.Worksheet(1); // İlk sayfa

            if (worksheet.IsEmpty())
            {
                TempData["Error"] = "Excel dosyası boş veya ilk sayfa boş," +
                    " lütfen dosyanın boş olmadığından veya ilk excel sayfasını doldurduğunuzdan emin olun.";

                return RedirectToAction(nameof(Index));
            }

            var firstRowUsed = worksheet.FirstRowUsed();
            var lastRowUsed = worksheet.LastRowUsed();

            if (firstRowUsed == null || lastRowUsed == null)
            {
                TempData["Error"] = "Excel dosyası boş veya ilk sayfa boş," +
                    " lütfen dosyanın boş olmadığından veya ilk excel sayfasını doldurduğunuzdan emin olun.";
                return RedirectToAction(nameof(Index));
            }

            var headerRowNumber = firstRowUsed.RowNumber();

            var codeHeader = worksheet.Cell(headerRowNumber, 1).GetString().Trim();
            var nameHeader = worksheet.Cell(headerRowNumber, 2).GetString().Trim();
            var unitHeader = worksheet.Cell(headerRowNumber, 3).GetString().Trim();
            var priceHeader = worksheet.Cell(headerRowNumber, 4).GetString().Trim();

            if (codeHeader != "Malzeme Kodu" || codeHeader != "Kod" ||
                nameHeader != "Malzeme İsmi" || nameHeader != "İsim" || nameHeader != "Ad" || nameHeader != "Malzeme Adı" ||
                unitHeader != "Birim" ||
                priceHeader != "Fiyat")
            {
                TempData["Error"] = "Excel başlıkları beklenen formatta değil." +
                    " Lütfen şablonu(|Malzeme Kodu|Malzeme İsmi|Birim|Fiyat|) kullanın.";
                return RedirectToAction(nameof(Index));
            }

            var firstDataRow = headerRowNumber + 1;
            var lastDataRow = lastRowUsed.RowNumber();

            for (var row = firstDataRow; row <= lastDataRow; row++)
            {
                // If the entire row is empty, skip it
                var usedCells = worksheet.Row(row).CellsUsed().Count();
                if (usedCells == 0)
                {
                    continue;
                }

                var codeCell = worksheet.Cell(row, 1);
                if (codeCell.IsEmpty() || string.IsNullOrWhiteSpace(codeCell.GetString()))
                {
                    // If code is empty, skip this row
                    continue;
                }

                var code = codeCell.GetString().Trim();
                var name = worksheet.Cell(row, 2).GetString().Trim();
                var unitStr = worksheet.Cell(row, 3).GetString().Trim();
                var priceCell = worksheet.Cell(row, 4);

                if (!Enum.TryParse(unitStr, out UnitType unit))
                {
                    TempData["Error"] = $"Geçersiz birim türü '{unitStr}' satır {row}." +
                        " Lütfen şablondaki birim türlerini kullanın.";
                    continue;
                }

                if (priceCell.IsEmpty())
                {
                    TempData["Error"] = $"Fiyat boş olamaz satır {row}.";
                    continue;
                }

                if (!priceCell.TryGetValue(out decimal priceValue))
                {
                    TempData["Error"] = $"Geçersiz fiyat değeri '{priceCell.GetString()}' satır {row}.";
                    continue;
                }

                var product = new Product
                {
                    Code = code,
                    Name = string.IsNullOrEmpty(name) ? null : name,
                    Unit = unit,
                    Price = priceValue
                };

                productsToAdd.Add(product);
            }

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
    }
}
