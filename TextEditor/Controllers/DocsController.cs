using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TextEditor.Data;
using TextEditor.Models;

namespace TextEditor.Controllers
{
    [Authorize] 
	public class DocsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Docs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = from c in _context.docs select c;

            applicationDbContext = applicationDbContext
                .Where(a => a.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View(await applicationDbContext.Include(u=>u.user).ToListAsync());
        }

        // GET: Docs/Create
        public IActionResult Create()
        {
            return View(new Docs());
        }

        // POST: Docs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,UserId")] Docs docs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(docs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(docs);
        }

        // GET: Docs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docs = await _context.docs.FindAsync(id);
            if (docs == null)
            {
                return NotFound();
            }
            if (docs.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier)) 
            { // if id not match the id of user that currently logged in .
				return NotFound();
			}
            return View(docs);
        }

        // POST: Docs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,UserId")] Docs docs)
        {
            if (id != docs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(docs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocsExists(docs.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", docs.UserId);
            return View(docs);
        }

        // GET: Docs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docs = await _context.docs
                .Include(d => d.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docs == null)
            {
                return NotFound();
            }
			if (docs.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
			{ // if id not match the id of user that currently logged in .
				return NotFound();
			}
			return View(docs);
        }

        // POST: Docs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var docs = await _context.docs.FindAsync(id);
            if (docs != null)
            {
                _context.docs.Remove(docs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocsExists(int id)
        {
            return _context.docs.Any(e => e.Id == id);
        }
    }
}
