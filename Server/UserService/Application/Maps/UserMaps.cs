using UserSerivce.Application.DTO.Created;
using UserSerivce.Application.DTO.Updated;
using UserSerivce.Application.DTO.Received;
using UserSerivce.Core.Entities;

namespace UserSerivce.Application.Maps;

public static class UserMaps
{
    public static UserEntity MapToUser(UserCreatedRequest request)
    {
        return new UserEntity
        {
            Id = request.Id,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            PhoneNumber = request.PhoneNumber,
            AvatarPath = request.AvatarPath,
            BirthDate = request.BirthDate,
            CreatedAt = request.CreatedAt,
            PasswordHash = string.Empty
        };
    }

    public static UserEntity MapToUser(UserUpdatedRequest request)
    {
        return new UserEntity
        {
            Id = request.Id,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            PhoneNumber = request.PhoneNumber,
            AvatarPath = request.AvatarPath,
            BirthDate = request.BirthDate,
            CreatedAt = request.CreatedAt,
            PasswordHash = request.PasswordHash
        };
    }

    public static UserEntity MapToUser(UserReceivedRequest request)
    {
        return new UserEntity
        {
            Id = request.Id,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            PhoneNumber = request.PhoneNumber,
            AvatarPath = request.AvatarPath,
            BirthDate = request.BirthDate,
            CreatedAt = request.CreatedAt,
            PasswordHash = request.PasswordHash
        };
    }

    public static UserCreatedResponse MapToCreatedResponse(UserEntity user)
    {
        return new UserCreatedResponse
        (
            Id: user.Id,
            UserName: user.UserName,
            FirstName: user.FirstName,
            LastName: user.LastName,
            MiddleName: user.MiddleName,
            PhoneNumber: user.PhoneNumber,
            AvatarPath: user.AvatarPath,
            BirthDate: user.BirthDate,
            CreatedAt: user.CreatedAt
        );
    }

    public static UserUpdatedResponse MapToUpdatedResponse(UserEntity user)
    {
        return new UserUpdatedResponse
        (
            Id: user.Id,
            UserName: user.UserName,
            FirstName: user.FirstName,
            LastName: user.LastName,
            MiddleName: user.MiddleName,
            PhoneNumber: user.PhoneNumber,
            AvatarPath: user.AvatarPath,
            BirthDate: user.BirthDate,
            CreatedAt: user.CreatedAt
        );
    }

    public static UserReceivedResponse MapToReceivedResponse(UserEntity user)
    {
        return new UserReceivedResponse
        (
            Id: user.Id,
            UserName: user.UserName,
            FirstName: user.FirstName,
            LastName: user.LastName,
            MiddleName: user.MiddleName,
            PhoneNumber: user.PhoneNumber,
            AvatarPath: user.AvatarPath,
            BirthDate: user.BirthDate,
            CreatedAt: user.CreatedAt
        );
    }
}
