using DataAccessLayer.Data;
using DataAccessLayer.Interfaces.Models;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class StatistictsRepository : GenericRepository<UrlStatistics>, IUrlStastiticsRepository
    {
        public StatistictsRepository(ApplicationContext context, ILogger logger) : base(context, logger)
        {
        }
    }
}