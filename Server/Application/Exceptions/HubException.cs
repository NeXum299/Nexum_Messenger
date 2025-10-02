using System;

namespace Server.Application.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class HubException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public HubException(string message) : base(message) { }
    }
}
