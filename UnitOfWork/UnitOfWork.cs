using DataAccessLayer.Data;
using DataAccessLayer.Interfaces.Models;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UnitOfWork.Configuration;

namespace UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationContext _context;
        private readonly ILogger _logger;

        public IUrlRepository Url { get; private set; }
        public IUrlStastiticsRepository Statisticts { get; private set; }

        public UnitOfWork(ApplicationContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("logs");

            Url = new UrlRepository(context, _logger);
            Statisticts = new StatistictsRepository(context, _logger);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}