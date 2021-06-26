using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Adds a movie
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/Movies/
        /// </url>
        /// <body>
        /// {
        ///"title": "Friends",
        ///"description": "The one that is the best",
        ///"duration": 25,
        ///"yearOfRelease": 1994,
        ///"director": "?????",
        ///"addedDate": "2019-05-22T11:10:21",
        ///"rating": 5,
        ///"watched": true,
        ///"genre": "Comedy",
        ///"userReviews": []
        ///}
        /// </body>
        /// <param name="movieRequest"></param>
        /// <returns>
        /// 200 if successful
        /// 400 if error
        /// </returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
        public async Task<ActionResult<MovieViewModel>> PostMovie(MovieViewModel movieRequest)
        {
            Movie movie = _mapper.Map<Movie>(movieRequest);
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        /// <summary>
        /// Adds a review to a movie
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/Movies/1/user-reviews
        /// </url>
        /// <body>
        /// {
        ///"content": "destul de bun",
        ///"stars": 3,
        ///"dateTime": "2021-05-15T10:41:32",
        ///"movieId": 1
        ///}
        /// </body>
        /// <param name="id"></param>
        /// <param name="userReview"></param>
        /// <returns>
        /// 200 if successful
        /// 400 if error
        /// </returns>
        [HttpPost("{id}/user-reviews")]
        public IActionResult PostReviewForMovie(int id, ReviewViewModel userReview)
        {
            var movie = _context.Movies
                .Where(m => m.Id == id)
                .Include(m => m.UserReviews)
                .FirstOrDefault();

            if (movie == null)
            {
                return NotFound();
            }

            movie.UserReviews.Add(_mapper.Map<UserReview>(userReview));
            _context.Entry(movie).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Retrieves a list of movies filtered by the given year or all the movies if a year is not provided. The list includes the reviews 
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/movies
        /// </url>
        /// <param name="minYear"></param>
        /// <returns>A list of MovieWithReviewsViewModel</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieWithReviewsViewModel>>> GetMovies(int? minYear)
        {
            if (minYear == null)
            {
                var unfilteredMovies = await _context.Movies
                    .Include(m => m.UserReviews)
                    .Include(m => m.Favourites)
                    .ToListAsync();
                return _mapper.Map<List<MovieWithReviewsViewModel>>(unfilteredMovies);
            }

            var movies = await _context.Movies
                .Where(m => m.YearOfRelease >= minYear)
                .Include(m => m.UserReviews)
                .Include(m => m.Favourites)
                .ToListAsync();
            return _mapper.Map<List<MovieWithReviewsViewModel>>(movies);
        }

        /// <summary>
        /// Retrieve the movie with the given Id
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/movies/5
        /// </url>
        /// <param name="id"></param>
        /// <returns>Returns a MovieViewModel</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieWithReviewsViewModel>> GetMovie(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.UserReviews)
                .Include(m => m.Favourites)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieWithReviewsViewModel>(movie);
        }

        /// <summary>
        /// Retrieves a list of movies filtered by the interval ordered descendingly by the year of release
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/Movies/filter/2010_2022
        /// </url>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>A list of MovieViewModel</returns>
        [HttpGet]
        [Route("filter/{from}_{to}")]
        public ActionResult<IEnumerable<MovieViewModel>> FilterMovies(int from, int to)
        {
            var movies = _context.Movies.Where(m => m.YearOfRelease >= from && m.YearOfRelease <= to).OrderByDescending(m => m.YearOfRelease).ToList();

            return _mapper.Map<List<MovieViewModel>>(movies);
        }

        /// <summary>
        /// Get the list of reviews of the movie with specified id
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/Movies/1/user-reviews
        /// </url>
        /// <param name="id"></param>
        /// <returns>A list of MovieWithReviewsViewModel</returns>
        [HttpGet("{id}/user-reviews")]
        public async Task<ActionResult<MovieWithReviewsViewModel>> GetReviewsForMovie(int id)
        {
            if (!MovieExists(id))
            {
                return NotFound();
            }

            var movie = await _context.Movies.Where(m => m.Id == id).FirstOrDefaultAsync();
            var comments = await _context.UserReviews.Where(r => r.MovieId == id).ToListAsync();

            var result = _mapper.Map<MovieWithReviewsViewModel>(movie);
            result.UserReviews = _mapper.Map<List<UserReview>, List<ReviewViewModel>>(comments);

            return result;
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        /// <summary>
        /// Edit a movie
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/Movies/5
        /// </url>
        /// <body>
        /// {
        /// "id": 5,
        /// "title": "The best one",
        ///"description": "bbbbbbbbbb",
        ///"duration": 10,
        ///"yearOfRelease": 2019,
        ///"director": "ccccc",
        ///"addedDate": "2020-05-22T11:10:21",
        ///"rating": 3,
        ///"watched": true,
        ///"genre": "Horror"
        /// }
        /// </body>
        /// <param name="id"></param>
        /// <param name="movie"></param>
        /// <returns>
        /// 204 Response if successful
        /// 400 If the Id does not match
        /// 404 If the movie does not exist in the DB
        /// </returns>
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
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

        /// <summary>
        /// Edit a review
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/Movies/1/user-review/2
        /// </url>
        /// <body>
        ///{
        ///"id": 2,
        ///"content": "destul de bun",
        ///"stars": 5,
        ///"dateTime": "2021-05-15T11:20:20"
        ///}
        /// </body>
        /// <param name="idReview"></param>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPut("{id}/user-review/{idReview}")] // X
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
                if (!ReviewExists(idReview))
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

        private bool ReviewExists(int id)
        {
            return _context.UserReviews.Any(e => e.Id == id);
        }

        /// <summary>
        /// Deletes a movie
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/Movies/6
        /// </url>
        /// <param name="id"></param>
        /// <returns>
        /// 204 if successful
        /// 404 if the movie does not exist
        /// </returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
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

        /// <summary>
        /// Deletes a review of a movie
        /// </summary>
        /// <url>
        /// https://localhost:5001/api/Movies/1/user-review/5
        /// </url>
        /// <param name="reviewId"></param>
        /// <returns>
        /// 204 if successful
        /// 404 if the movie does not exist
        /// </returns>
        [HttpDelete("{id}/user-review/{reviewId}")]
        [Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
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
    }
}
