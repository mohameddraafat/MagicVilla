using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs;

namespace MagicVilla_VillaAPI.Repositories.IRepositories
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO/*, string password*/);
        Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
