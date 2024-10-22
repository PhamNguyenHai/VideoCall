using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetProject
{
    public class UnauthorizeException : Exception
    {
        public int ErrorCode { get; set; }

        public UnauthorizeException() { }

        public UnauthorizeException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public UnauthorizeException(string message) : base(message) { }

        public UnauthorizeException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
