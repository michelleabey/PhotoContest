using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoContest.Data;
using PhotoContest.DTOs;
using PhotoContest.Models;
using System.Security.Claims;

namespace PhotoContest.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PhotoController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost("test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok("Authorization successful");
        }

        [HttpPost("upload")]
        //[AllowAnonymous]
        public async Task<IActionResult> UploadPhoto([FromForm] PhotoUploadDto photoDto)
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
            {
                return Unauthorized("User claim is missing.");
            }

            var userId = int.Parse(userClaim.Value);

            if (photoDto.Image == null || photoDto.Image.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + photoDto.Title;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photoDto.Image.CopyToAsync(fileStream);
            }

            var photo = new Photo
            {
                Title = photoDto.Title,
                ImageUrl = uniqueFileName,
                DateUploaded = DateTime.UtcNow,
                UserId = userId,
                VoteCount = 0
            };

            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Photo uploaded successfully", photoId = photo.Id });
        }

        [HttpPost("{photoId}/vote")]
        public async Task<IActionResult> VotePhoto(int photoId)
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
            {
                return Unauthorized("User claim is missing.");
            }

            var userId = int.Parse(userClaim.Value);

            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(v => v.PhotoId == photoId && v.UserId == userId);

            if (existingVote != null)
                return BadRequest("You have already voted for this photo");

            var photo = await _context.Photos.FindAsync(photoId);
            if (photo == null)
                return NotFound();

            var vote = new Vote
            {
                PhotoId = photoId,
                UserId = userId,
                VoteDate = DateTime.UtcNow
            };

            photo.VoteCount++;

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Vote recorded successfully" });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPhotos()
        {

            var photos = await _context.Photos
                .Include(p => p.User)
                .OrderByDescending(p => p.VoteCount)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.ImageUrl,
                    p.VoteCount,
                    p.DateUploaded,
                    UserName = p.User != null ? p.User.Username :"Unknown"
                })
                .ToListAsync();

            return Ok(photos);
        }
    }
}