using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultimediaCenter.Data;
using MultimediaCenter.Models;
using MultimediaCenter.ViewModel;

namespace MultimediaCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MoviesController> _logger;
        private readonly IMapper _mapper;


        public MoviesController(ApplicationDbContext context, ILogger<MoviesController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MovieViewModel>> PostMovie(MovieViewModel movieRequest)
        {
            Movie movie = _mapper.Map<Movie>(movieRequest);
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        [HttpPost("{id}/user-reviews")]
        public IActionResult PostReviewForMovie(int id, UserReview userReview)
        {
            var movie = _context.Movies
                .Where(m => m.Id == id)
                .Include(m => m.UserReviews)
                .FirstOrDefault();

            if (movie == null)
            {
                return NotFound();
            }

            movie.UserReviews.Add(userReview);
            _context.Entry(movie).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieViewModel>>> GetMovies(int? minYear)
        {
            if (minYear == null)
            {
                var movies1 = await _context.Movies.ToListAsync();
                return _mapper.Map<List<Movie>, List<MovieViewModel>>(movies1);
            }

            var movies = await _context.Movies.Where(m => m.YearOfRelease >= minYear).ToListAsync();
            return _mapper.Map<List<Movie>,  List<MovieViewModel>>(movies);
        }
         
        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieViewModel>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieViewModel>(movie);
        }

        [HttpGet]
        [Route("filter/{from}_{to}")]
        public ActionResult<IEnumerable<Movie>> FilterMovies(int from, int to)
        {
            var movies = _context.Movies.Where(m => m.YearOfRelease >= from && m.YearOfRelease <= to).OrderByDescending(m => m.YearOfRelease).ToList();

            return movies;
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, MovieViewModel movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            _context.Entry(_mapper.Map<Movie>(movie)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
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

        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int idReview, ReviewViewModel review)
        {
            if (idReview != review.Id)
            {
                return BadRequest();
            }

            _context.Entry(_mapper.Map<UserReview>(review)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(idReview))
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

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}/Reviews/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _context.UserReviews.FindAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            _context.UserReviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/user-reviews")]
        public ActionResult<IEnumerable<MovieWithReviewsViewModel>> GetReviewForMovie(int id)
        {
            if (!MovieExists(id))
            {
                return NotFound();
            }

            return _context.Movies.Where(m => m.Id == id)
                .Include(m => m.UserReviews)
                .Select(m => _mapper.Map<MovieWithReviewsViewModel>(m))
                .ToList();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
