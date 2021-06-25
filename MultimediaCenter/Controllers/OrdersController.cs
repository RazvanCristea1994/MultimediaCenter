using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultimediaCenter.Data;
using MultimediaCenter.Models;
using MultimediaCenter.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MultimediaCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;

        public OrdersController(
            ApplicationDbContext context, 
            ILogger<OrdersController> logger, 
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> PlaceOrder(NewOrderRequest newOrderRequest)
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            List<Movie> orderedMovies = new List<Movie>();
            newOrderRequest.OrderedMoviesIds.ForEach(mId =>
            {
                var movieWithId = _context.Movies.Find(mId);
                if (movieWithId != null)
                {
                    orderedMovies.Add(movieWithId);
                }
            });

            if (orderedMovies.Count == 0)
            {
                return BadRequest();
            }

            var order = new Order
            {
                User = user,
                OrderDateTime = newOrderRequest.OrderedDateTime.GetValueOrDefault(),
                Movies = orderedMovies
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersForUserResponse>>> GetAll()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (user == null)
            {
                return NotFound();
            }

            var result = _context.Orders
                .Where(o => o.User.Id == user.Id)
                .Include(o => o.Movies)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDateTime)
                .ToList();

            return _mapper.Map<List<OrdersForUserResponse>>(result);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateOrder(UpdateOrderForUserViewModel updateOrderRequest)
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Order order = _context.Orders
                .Where(o => o.Id == updateOrderRequest.Id && o.User.Id == user.Id)
                .Include(f => f.Movies)
                .FirstOrDefault();

            if (order == null)
            {
                return BadRequest("There is no favourites list with this ID.");
            }

            order.Movies = _context.Movies
                .Where(m => updateOrderRequest.OrderIds.Contains(m.Id))
                .ToList();

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var order = _context.Orders
                .Where(o => o.User.Id == user.Id && o.Id == id)
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
