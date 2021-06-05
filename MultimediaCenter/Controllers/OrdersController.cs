using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                ApplicationUser = user,
                OrderDateTime = newOrderRequest.OrderedDateTime.GetValueOrDefault(),
                Movies = orderedMovies
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = _context.Orders.Where(o => o.ApplicationUser.Id == user.Id).Include(o => o.Movies).FirstOrDefault();
            var resultViewModel = _mapper.Map<OrdersForUserResponse>(result);

            return Ok(resultViewModel);
        }
    }
}
