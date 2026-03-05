using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lps_crud_test.Models.LpsDb;
using lps_crud_test.Services.Interfaces;

namespace lps_crud_test.Controllers
{
    public class BookTypesController : Controller
    {
        private readonly IBookTypeService _bookTypeService;

        public BookTypesController(IBookTypeService bookTypeService)
        {
            _bookTypeService = bookTypeService;
        }

        // GET: BookTypes
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var list = await _bookTypeService.GetAsync(searchString);
            return View(list);
        }

        // GET: BookTypes/ExportToExcel
        public async Task<IActionResult> ExportToExcel(string? searchString)
        {
            var list = await _bookTypeService.GetAsync(searchString);
            var builder = new StringBuilder();
            // header
            builder.AppendLine("Id,BookTypeName,IsActive");
            foreach (var bt in list)
            {
                var line = $"{bt.Id},\"{bt.BookTypeName}\",{bt.IsActiveBool}";
                builder.AppendLine(line);
            }
            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            return File(bytes, "text/csv", "booktypes.csv");
        }

        // GET: BookTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
           if (id == null)
           {
               return NotFound();
           }

           var bookType = await _bookTypeService.GetByIdAsync(id.Value);
           if (bookType == null)
           {
               return NotFound();
           }

           return View(bookType);
        }

        // GET: BookTypes/Create
        public IActionResult Create()
        {
           return View();
        }

        // POST: BookTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookTypeName,IsActiveBool")] BookType bookType)
        {
           if (ModelState.IsValid)
           {
                await _bookTypeService.CreateAsync(bookType);
               return RedirectToAction(nameof(Index));
           }
           return View(bookType);
        }

        // GET: BookTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
           if (id == null)
           {
               return NotFound();
           }

           var bookType = await _bookTypeService.GetByIdAsync(id.Value);
           if (bookType == null)
           {
               return NotFound();
           }
           return View(bookType);
        }

        // POST: BookTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookTypeName,IsActiveBool")] BookType bookType)
        {
           if (id != bookType.Id)
           {
               return NotFound();
           }

           if (ModelState.IsValid)
           {
               try
               {
                    await _bookTypeService.EditAsync(bookType);
               }
               catch (DbUpdateConcurrencyException)
               {
                   if (!await _bookTypeService.BookTypeExists(bookType.Id))
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
           return View(bookType);
        }

        // GET: BookTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
           if (id == null)
           {
               return NotFound();
           }

           var bookType = await _bookTypeService.GetByIdAsync(id.Value);
           if (bookType == null)
           {
               return NotFound();
           }

           return View(bookType);
        }

        // POST: BookTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookType = await _bookTypeService.GetByIdAsync(id);
           if (bookType != null)
           {
               await _bookTypeService.DeleteAsync(id);
           }

           return RedirectToAction(nameof(Index));
        }
    }
}
