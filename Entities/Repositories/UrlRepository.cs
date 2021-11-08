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
    public class UrlRepository : GenericRepository<Url>, IUrlRepository
    {
        public UrlRepository(ApplicationContext context, ILogger logger) : base(context, logger)
        {
        }

        public override async Task<IEnumerable<Url>> All()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(UrlRepository));
                return new List<Url>();
            }
        }
        public override async Task<bool> Insert(Url entity)
        {
            try
            {
                var existingUrl = await dbSet.Where(x => x.Id == entity.Id)
                                                    .FirstOrDefaultAsync();

                if (existingUrl == null)
                    return await Add(entity);

                existingUrl.ShortUrl = entity.ShortUrl;
                existingUrl.Count = entity.Count;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Insert function error", typeof(UrlRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(int id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.Id == id)
                                        .FirstOrDefaultAsync();

                if (exist == null) return false;

                dbSet.Remove(exist);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete function error", typeof(UrlRepository));
                return false;
            }
        }
    }
}