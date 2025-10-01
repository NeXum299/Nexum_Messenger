using System;

namespace Server.Application.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupMemberException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public GroupMemberException(string message) : base(message) { }
    }
}
