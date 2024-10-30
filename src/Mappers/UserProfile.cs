using AutoMapper;

namespace PetProject
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<UserModel, User>();
            CreateMap<UserModel, UserViewModel>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<FilterResult<UserModel>, FilterResult<UserViewModel>>();
        }
    }
}
