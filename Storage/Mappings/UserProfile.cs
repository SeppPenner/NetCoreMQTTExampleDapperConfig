using AutoMapper;
using Storage.Database;
using Storage.Dto;

namespace Storage.Mappings
{
    /// <summary>
    ///     The user profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class UserProfile : Profile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserProfile" /> class.
        /// </summary>
        public UserProfile()
        {
            CreateMap<User, DtoReadUser>();
            CreateMap<DtoReadUser, User>();
            CreateMap<User, DtoCreateUpdateUser>();
            CreateMap<DtoCreateUpdateUser, User>();
            CreateMap<DtoReadUser, DtoCreateUpdateUser>();
            CreateMap<DtoCreateUpdateUser, DtoReadUser>();
        }
    }
}
