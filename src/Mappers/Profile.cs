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

    public class PrivateMessageProfile : AutoMapper.Profile
    {
        public PrivateMessageProfile()
        {
            CreateMap<PrivateMessage, PrivateMessageViewModel>();
            CreateMap<PrivateMessageCreateDto, PrivateMessage>();
        }
    }

    public class PrivateChatProfile : AutoMapper.Profile
    {
        public PrivateChatProfile()
        {
            CreateMap<PrivateChat, PrivateChatViewModel>();
        }
    }
}
