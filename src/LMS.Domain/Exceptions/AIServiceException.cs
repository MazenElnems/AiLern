using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Exceptions;

public class AIServiceException : Exception
{
    public AIServiceException(string message) : base(message)
    {
    }
}
