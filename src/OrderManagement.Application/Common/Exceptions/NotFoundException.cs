using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.Common.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
