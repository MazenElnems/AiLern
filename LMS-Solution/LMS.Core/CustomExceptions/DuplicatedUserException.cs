using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.CustomExceptions
{
    public class DuplicatedUserException : Exception
    {
        public DuplicatedUserException(string message = "this user already exists")
            : base(message)
        {
        }
    }
}
