using System;

namespace Server.Application.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupMessageException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public GroupMessageException(string message) : base(message) { }
    }
}
