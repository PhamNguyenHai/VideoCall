using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetProject
{
    public class ValidateException : Exception
    {
        public int ErrorCode { get; set; }

        public ValidateException() { }

        public ValidateException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public ValidateException(string message) : base(message) { }

        public ValidateException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
