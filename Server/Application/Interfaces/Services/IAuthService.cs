using System.Threading.Tasks;
using Server.Application.DTO;

namespace Server.Application.Interface.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task RegisterUserAsync(RegisterModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task LoginUserAsync(LoginModel model);
    }
}
