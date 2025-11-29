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


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product); // @model Product
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product deleted. Id={Id}, Code={Code}", product.Id, product.Code);
            }

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
