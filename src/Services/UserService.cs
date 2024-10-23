using AutoMapper;
using PetProject.Repositories;
using System;
using System.Threading.Tasks;

namespace PetProject.Services
{
    public class UserService : BaseService<User, UserModel, UserViewModel, UserCreateDto, UserUpdateDto>, IUserService
    {
        protected readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository, IMapper mapper) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
        }

        public override Task ValidateForInserting(UserCreateDto entityCreateDto)
        {
            throw new NotImplementedException();
        }

        public override Task ValidateForUpdating(Guid id, UserUpdateDto entityUpdateDto)
        {
            throw new NotImplementedException();
        }
    }
}
