using System;
using System.Threading.Tasks;
using Server.Core.Entities;

namespace Server.Application.Interface.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<UserEntity?> GetUserByUserNameAsync(string userName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserEntity?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<UserEntity?> GetUserByPhoneNumberAsync(string phoneNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task UpdateUserAsync(UserEntity user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="avatarPath"></param>
        /// <returns></returns>
        Task UpdateUserAvatarAsync(Guid userId, string avatarPath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteUserAsync(Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refreshToken"></param>
        /// <param name="expiryTime"></param>
        /// <returns></returns>
        Task UpdateUserRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime);
    }
}
