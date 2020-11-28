using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NETCOREM4DatabaseFirst.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace NETCOREM4DatabaseFirst.Controllers
{
    public class CustomerController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private readonly SalesContext _context;

        public CustomerController(SalesContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> ReporteXLS()
        {
            string WebRootPah = _hostingEnvironment.WebRootPath;
            string fileName = @"ReporteClientes.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(WebRootPah, fileName));
            var memoryStream = new MemoryStream();

            using (var fs = new FileStream(Path.Combine(WebRootPah, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workBook = new XSSFWorkbook();
                ISheet excelSheet = workBook.CreateSheet("Reportes");

                IRow row = excelSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("CustomerId");
                row.CreateCell(1).SetCellValue("FirstName");
                row.CreateCell(2).SetCellValue("LastName");
                row.CreateCell(3).SetCellValue("City");

                List<Customer> listado = await _context.Customers.ToListAsync();
                int contador = 1;
                string firstName = string.Empty;
                foreach (var item in listado)
                {
                    if (item.FirstName.Length > 100)
                        firstName = item.FirstName.Substring(0, 100);
                    else
                        firstName = item.FirstName;

                    row = excelSheet.CreateRow(contador);
                    row.CreateCell(0).SetCellValue(item.Id);
                    row.CreateCell(1).SetCellValue(firstName);
                    row.CreateCell(2).SetCellValue(item.LastName);
                    row.CreateCell(3).SetCellValue(item.City);
                    contador++;
                }

                workBook.Write(fs);
            }

            using (var fs = new FileStream(Path.Combine(WebRootPah, fileName), FileMode.Open))
            {
                fs.CopyTo(memoryStream);
            }
            memoryStream.Position = 0;
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> ReporteCSV() 
        {
            var builder = new StringBuilder();
            builder.AppendLine("Id,FirstName,LastName");
            List<Customer> customers = await _context.Customers.ToListAsync();
            foreach (var item in customers)
            {
                builder.AppendLine($"{item.Id},{item.FirstName},{item.LastName}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Clientes.csv");

        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,City,Country,Phone")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,City,Country,Phone")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
