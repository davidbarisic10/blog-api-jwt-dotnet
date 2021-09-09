using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Authentication;
using BlogAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //GET: api/Post
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPost()
        {
            
            return await _context.Post.ToListAsync();
        }

        // GET: api/Post/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(string id)
        {
            

            var post = await _context.Post.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Post/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost([FromRoute] string id, [FromBody] Post post)
        {
            var postid = this.User.FindFirst(ClaimTypes.Sid).ToString().Split(' ')[1];
            
            var updated = await _context.Post.Where(pid => pid.Id == id && pid.UID == postid).ToListAsync();
            
            if (updated.Count() > 0)
            {
                updated[0].Title = post.Title;
                updated[0].Description = post.Description;
                _context.Entry(updated[0]).State = EntityState.Modified;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Post
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost([FromBody] Post post)
        {

            post.UID = this.User.FindFirst(ClaimTypes.Sid).ToString().Split(' ')[1];
            
            _context.Post.Add(post);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PostExists(post.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Post/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var postid = this.User.FindFirst(ClaimTypes.Sid).ToString().Split(' ')[1];
            var post = await _context.Post.Where(pid => pid.Id == id && pid.UID == postid).ToListAsync();
            if (post.Count() < 1)
            {
                return NotFound();
            }
            else
            {
                _context.Post.Remove(post[0]);
                await _context.SaveChangesAsync();

                return NoContent();
            }

        }

        private bool PostExists(string id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
