using System;

namespace Server.Application.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class UserException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public UserException(string message) : base(message) { }
    }
}
