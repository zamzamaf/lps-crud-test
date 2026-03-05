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
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: Books
        public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
        {
            const int pageSize = 10;
            ViewData["CurrentFilter"] = searchString;
            var paged = await _bookService.GetPagedAsync(searchString, pageNumber, pageSize);
            return View(paged);
        }

        // GET: Books/ExportToCsv
        public async Task<IActionResult> ExportToCsv(string? searchString)
        {
            var list = await _bookService.GetAsync(searchString);
            var builder = new StringBuilder();
            builder.AppendLine("Id,BookTitle,Author,BookTypeId,ReleaseDate,NumberOfPages,IsActive");
            foreach (var b in list)
            {
                var line = $"{b.Id},\"{b.BookTitle}\",\"{b.Author}\",{b.BookTypeId},{b.ReleaseDate},{b.NumberOfPages},{b.IsActiveBool}";
                builder.AppendLine(line);
            }
            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            return File(bytes, "text/csv", "books.csv");
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _bookService.GetByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            var selectListItems = (await _bookService.GetBooksWithTypesAsync()).Select(bt => new SelectListItem
            {
                Value = bt.Id.ToString(),
                Text = bt.BookTypeName
            }).ToList();
            ViewData["BookTypeId"] = new SelectList(selectListItems, "Value", "Text");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookTitle,Author,BookTypeId,ReleaseDate,NumberOfPages,IsActiveBool,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy")] Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookService.CreateAsync(book);
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookTypeId"] = new SelectList((await _bookService.GetBooksWithTypesAsync()).Select(bt => new SelectListItem
            {
                Value = bt.Id.ToString(),
                Text = bt.BookTypeName
            }), "Value", "Text", book.BookTypeId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var book = await _bookService.GetByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["BookTypeId"] = new SelectList((await _bookService.GetBooksWithTypesAsync()).Select(bt => new SelectListItem
            {
                Value = bt.Id.ToString(),
                Text = bt.BookTypeName
            }), "Value", "Text", book.BookTypeId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookTitle,Author,BookTypeId,ReleaseDate,NumberOfPages,IsActiveBool,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bookService.EditAsync(book);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _bookService.BookExists(book.Id))
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
            ViewData["BookTypeId"] = new SelectList((await _bookService.GetBooksWithTypesAsync()).Select(bt => new SelectListItem
            {
                Value = bt.Id.ToString(),
                Text = bt.BookTypeName
            }), "Value", "Text", book.BookTypeId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _bookService.GetByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book != null)
            {
                await _bookService.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
