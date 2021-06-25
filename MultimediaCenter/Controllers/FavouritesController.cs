using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultimediaCenter.Data;
using MultimediaCenter.Models;
using MultimediaCenter.ViewModel;
using MultimediaCenter.ViewModel.Favourites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MultimediaCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
    public class FavouritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FavouritesController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavouritesController(ApplicationDbContext context, ILogger<FavouritesController> logger, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavouritesForUserResponse>>> GetAll()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (user == null)
            {
                return NotFound();
            }

            var result = _context.Favourites
                .Where(f => f.User.Id == user.Id)
                .Include(f => f.Movies)
                .ToList();
            return _mapper.Map<List<FavouritesForUserResponse>>(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateFavourites(NewFavouritesRequest newFavouriteRequest)
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            List<Movie> movies = new    List<Movie>();
            newFavouriteRequest.MovieIds.ForEach(mId =>
            {
                var movie = _context.Movies.Find(mId);
                if (movie != null)
                {
                    movies.Add(movie);
                }
            });

            if (movies.Count == 0)
            {
                return BadRequest("The movies you provided cannot be found.");
            }

            var favourites = new Favourites
            {
                User = user,
                Movies = movies,
            };

            _context.Favourites.Add(favourites);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateFavourites(UpdateFavouritesForUserViewModel updateFavouritesRequest)
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Favourites favourites = _context.Favourites
                .Where(f => f.Id == updateFavouritesRequest.Id && f.User.Id == user.Id)
                .Include(f => f.Movies)
                .FirstOrDefault();

            if (favourites == null)
            {
                return BadRequest("There is no favourites list with this ID.");
            }

            favourites.Movies = _context.Movies
                .Where(m => updateFavouritesRequest.MovieIds.Contains(m.Id))
                .ToList();

            _context.Entry(favourites).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavourites(int id)
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var favourites = _context.Favourites
                .Where(f => f.User.Id == user.Id && f.Id == id)
                .FirstOrDefault();

            if (favourites == null)
            {
                return NotFound();
            }

            _context.Favourites.Remove(favourites);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
