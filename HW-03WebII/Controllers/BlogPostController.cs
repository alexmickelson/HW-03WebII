﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HW_03WebII.Data;
using HW_03WebII.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HW_03WebII.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public BlogPostController(ApplicationDbContext context,  UserManager<IdentityUser> userManager)
        { 
            _context = context;
            _userManager = userManager;
        }

        // GET: BlogPost
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            //if (currentUser == null) return Challenge();
            return View(await _context.BlogPosts.ToListAsync());
        }

        // GET: BlogPost/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostModel = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (blogPostModel == null)
            {
                return NotFound();
            }

            return View(blogPostModel);
        }

        // GET: BlogPost/Create
        [Authorize (Policy =MyIdentityData.BlogPolicy_Add)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: BlogPost/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( BlogPostModel blogPostModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogPostModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogPostModel);
        }

        // GET: BlogPost/Edit/5
        [Authorize(Policy = MyIdentityData.BlogPolicy_Edit)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostModel = await _context.BlogPosts.FindAsync(id);
            if (blogPostModel == null)
            {
                return NotFound();
            }
            return View(blogPostModel);
        }

        // POST: BlogPost/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyIdentityData.BlogPolicy_Edit)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,Summary,Posted")] BlogPostModel blogPostModel)
        {
            if (!id.Equals(blogPostModel.Id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogPostModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostModelExists(blogPostModel.Id))
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
            return View(blogPostModel);
        }

        // GET: BlogPost/Delete/5
        [Authorize(Policy = MyIdentityData.BlogPolicy_Delete)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostModel = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (blogPostModel == null)
            {
                return NotFound();
            }

            return View(blogPostModel);
        }

        // POST: BlogPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyIdentityData.BlogPolicy_Delete)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPostModel = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPostModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostModelExists(string id)
        {
            return _context.BlogPosts.Any(e => e.Id.Equals(id));
        }
    }
}
