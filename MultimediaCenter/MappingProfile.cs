using AutoMapper;
using MultimediaCenter.Models;
using MultimediaCenter.ViewModel;
using MultimediaCenter.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieViewModel>().ReverseMap();
            CreateMap<Movie, MovieWithReviewsViewModel>().ReverseMap();

            CreateMap<UserReview, ReviewViewModel>().ReverseMap();

            CreateMap<Order, OrdersForUserResponse>().ReverseMap();

            CreateMap<ApplicationUser, ApplicationUserViewModel>().ReverseMap();

            CreateMap<Favourites, FavouritesForUserResponse>().ReverseMap();
        }
    }
}
