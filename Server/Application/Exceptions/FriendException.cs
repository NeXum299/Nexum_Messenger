using System;

namespace Server.Application.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class FriendException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public FriendException(string message) : base(message) { }
    }
}
