using DataAccessLayer.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork.Configuration
{
    public interface IUnitOfWork
    {
        IUrlRepository Url { get; }
        IUrlStastiticsRepository Statisticts { get; }
        Task CompleteAsync();
        void Dispose();
    }
}
