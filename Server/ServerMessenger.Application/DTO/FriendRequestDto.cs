using System;

namespace ServerMessenger.Application.DTO
{
    public class FriendRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarPath { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}