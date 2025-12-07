using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using testerkel.Data;
using testerkel.Models;
using testerkel.ViewModels.Warehouse;

namespace testerkel.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly ErkelErpDbContext _context;
        private readonly ILogger<WarehousesController> _logger;

        public WarehousesController(ErkelErpDbContext context, ILogger<WarehousesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Warehouses
        public async Task<IActionResult> Index()
        {
            var warehouses = await _context.Warehouses
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .ToListAsync();

            return View(warehouses);   // @model IEnumerable<Warehouse>
        }

        // GET: Warehouses/Create
        public IActionResult Create()
        {
            var vm = new WarehouseEditVm();
            return View(vm); // @model WarehouseEditVm
        }

        // POST: Warehouses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseEditVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var entity = new Warehouse
            {
                Code = vm.Code,
                Name = vm.Name,
                IsActive = vm.IsActive
            };

            _context.Warehouses.Add(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Warehouse created Id={Id}, Code={Code}", entity.Id, entity.Code);

            return RedirectToAction(nameof(Index));
        }

        // GET: Warehouses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Warehouses.FindAsync(id);
            if (entity == null) return NotFound();

            var vm = new WarehouseEditVm
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                IsActive = entity.IsActive
            };

            return View(vm);
        }

        // POST: Warehouses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WarehouseEditVm vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.Products = await LoadWarehouseProducts(vm.Id);
                return View("Details", vm);
            }

            var entity = await _context.Warehouses.FindAsync(id);
            if (entity == null) return NotFound();

            entity.Code = vm.Code;
            entity.Name = vm.Name;
            entity.IsActive = vm.IsActive;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Warehouse updated Id={Id}", entity.Id);

            return RedirectToAction(nameof(Index));
        }

        // POST: Warehouses/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Warehouses.FindAsync(id);
            if (entity == null)
                return Json(new { success = false });

            _context.Remove(entity);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        private Task<List<WarehouseProductRowVm>> GetWarehouseProductsAsync(int warehouseId)
        {
            var query =
                from st in _context.StockTxns.AsNoTracking()
                where st.WarehouseId == warehouseId && st.Product != null
                group st by st.Product into g
                select new WarehouseProductRowVm
                {
                    ProductId = g.Key!.Id,
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    Unit = g.Key.Unit.ToString(),
                    Price = g.Key.Price,
                    Quantity = g.Sum(x => x.Qty)
                };

            return query
                .Where(p => p.Quantity != 0)
                .OrderBy(p => p.Code)
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(int warehouseId)
        {
            var products = await GetWarehouseProductsAsync(warehouseId);

            // products zaten boş liste olabilir, sorun değil
            return Json(products);
        }


        private async Task<List<WarehouseProductRowVm>> LoadWarehouseProducts(int warehouseId)
        {
            var query =
                from st in _context.StockTxns
                    .Include(x => x.Product)
                where st.WarehouseId == warehouseId
                group st by st.Product into g
                select new WarehouseProductRowVm
                {
                    ProductId = g.Key.Id,
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    Unit = g.Key.Unit.ToString(),
                    Price = g.Key.Price,
                    Quantity = g.Sum(x => x.Qty)
                };

            return await query
                .Where(p => p.Quantity != 0)
                .OrderBy(p => p.Code)
                .ToListAsync();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var warehouse = await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouse == null) return NotFound();

            var products = await GetWarehouseProductsAsync(warehouse.Id);

            var vm = new WarehouseEditVm
            {
                Id = warehouse.Id,
                Code = warehouse.Code,
                Name = warehouse.Name,
                IsActive = warehouse.IsActive,
                Products = products
            };

            return View(vm); // @model WarehouseEditVm
        }
    }
}
