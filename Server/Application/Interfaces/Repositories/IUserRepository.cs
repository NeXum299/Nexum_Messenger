using System;
using System.Threading.Tasks;
using Server.Core.Entities;

namespace Server.Application.Interface.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserEntity?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<UserEntity?> GetUserByUserNameAsync(string userName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<UserEntity?> GetUserByPhoneNumberAsync(string? phoneNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task CreateUserAsync(UserEntity user, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task UpdateUserAsync(UserEntity user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteUserAsync(Guid id);
    }
}
