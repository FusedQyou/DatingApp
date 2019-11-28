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
            CreateMap<User, ListUser>()
                .ForMember(dest => dest.MainPhotoUrl,
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.AsMainPhoto).Url))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.GetAge()));

            // Detailed user      
            CreateMap<User, DetailedUser>()
                .ForMember(dest => dest.MainPhotoUrl,
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.AsMainPhoto).Url))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.GetAge()));

            // Detailed photo
            CreateMap<Photo, DetailedPhoto>();
        }
    }
}