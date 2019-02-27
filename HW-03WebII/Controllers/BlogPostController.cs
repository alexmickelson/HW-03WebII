using System;
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
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

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
        public async Task<IActionResult> Details(string id)
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
                GenerateSlug(blogPostModel);
                _context.Add(blogPostModel);

                if (!blogPostModel.Tags.Equals(null))
                {
                    blogPostModel.BlogTags = new List<BlogTags>();

                    var tags = blogPostModel.Tags.Split();
                    foreach(var tag in tags)
                    {
                        var dbTag = new TagModel() { Name = tag };
                        _context.Add(dbTag);
                        blogPostModel.BlogTags.Add(new BlogTags()
                        {
                            BlogId = blogPostModel.Id,
                            Tag=dbTag
                        });
                        await _context.SaveChangesAsync();
                    }
                }
                

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogPostModel);
        }

        public async Task<List<TagModel>> GetTagsAsync()
        {
            return await _context.Tags
                        .Include(t => t.BlogTags)
                        .ToListAsync();
        }

        

        // GET: BlogPost/Edit/5
        [Authorize(Policy = MyIdentityData.BlogPolicy_Edit)]
        public async Task<IActionResult> Edit(string id)
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Body,Summary,Posted")] BlogPostModel blogPostModel)
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
        public async Task<IActionResult> Delete(string id)
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
        public async Task<IActionResult> DeleteConfirmed(string id)
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



        private bool GenerateSlug(BlogPostModel blogPost)
        {
            string str = RemoveDiacritics(blogPost.Title).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens 

            blogPost.Id = str;  

            //check if exists
            int extraId = 0;
            while (BlogPostModelExists(blogPost.Id))
            {
                var numbers = Regex.Matches(blogPost.Id, @"\d+").ToList();
                if (numbers.Any())
                {
                    var a = numbers.Last().ToString();
                    int.TryParse(a, out extraId);
                    extraId++;
                }
                blogPost.Id = blogPost.Id.Remove(blogPost.Id.Length - extraId.ToString().Length);
                blogPost.Id = str + extraId;
            }
            return true;
        }

        private string RemoveDiacritics(string text)
        {
            var s = new string(text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
            return s.Normalize(NormalizationForm.FormC);
        }
    }
}
