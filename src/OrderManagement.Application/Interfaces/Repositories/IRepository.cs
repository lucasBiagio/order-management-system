using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.Interfaces.Repositories;

public interface IRepository
{
    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
