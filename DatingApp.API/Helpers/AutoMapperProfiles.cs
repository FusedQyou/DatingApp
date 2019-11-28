using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // List user
            // Map main photo url from the photo that was chosen as main.
            // Also map age from the date of birth.
            CreateMap<User, ListUser>()
                .ForMember(dest => dest.MainPhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.AsMainPhoto).Url))
                .ForMember(dest => dest.Age,          opt => opt.MapFrom(src => src.DateOfBirth.GetAge()));

            // Detailed user
            // Map main photo url from the photo that was chosen as main.
            // Also map age from the date of birth.  
            CreateMap<User, DetailedUser>()
                .ForMember(dest => dest.MainPhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.AsMainPhoto).Url))
                .ForMember(dest => dest.Age,          opt => opt.MapFrom(src => src.DateOfBirth.GetAge()));

            // Updates user
            CreateMap<UpdateUser, User>();

            // Detailed photo
            CreateMap<Photo, DetailedPhoto>();

            // Single returned photo
            CreateMap<Photo, ReturnPhoto>();

            // Uploaded photo
            CreateMap<PhotoCreation, Photo>();
        }
    }
}