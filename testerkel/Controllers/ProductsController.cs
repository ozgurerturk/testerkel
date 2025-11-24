using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testerkel.Data;

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
            _logger.LogInformation("Fetched product list at {time}", DateTime.UtcNow);

            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            LoadUnitTypeSelectList();
            return View();
        }

        // POST: Products/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create()

        private void LoadUnitTypeSelectList()
        {
            throw new NotImplementedException();
        }
    }
}
