using Entities.Models;
using Shyjus.BrowserDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Configuration;
using UnitOfWork.Helpers;

namespace UnitOfWork.Services
{
    public class UrlService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UrlService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Url> CreateShortUrlAsync(Url Url)
        {
            //CheckIF Url Exist in DataBase
            if (Uri.TryCreate(Url.LongUrl, UriKind.Absolute, out Uri validatedUri))
            {
                // Check if long URL already exists in the database
                Url existingURL = await _unitOfWork.Url.FindOne(u => u.ShortUrl.ToLower() == Url.LongUrl.ToLower());

                if (existingURL == null)
                    await _unitOfWork.Url.Insert(Url);

                Url.ShortUrl = UrlShortener.Encode(Url.Id);
                Url.Date = DateTime.Now;

                await _unitOfWork.CompleteAsync();
            }
            else
            {
                Url.Successful = false;
                Url.Message = "Invalid Url!";
                return Url;
            }
            return Url;
        }

        public async Task<UrlStatistics> VisitUrl(string url, IBrowserDetector browserDetector)
        {
            var shortUrl = await _unitOfWork.Url.FindOne(u => u.ShortUrl.ToLower() == url.ToLower());

            //Check if exist
            if (shortUrl != null)
            {
                shortUrl.Count = shortUrl.Count + 1;

                UrlStatistics stats = new UrlStatistics();
                stats.UserAgent = browserDetector.Browser.OS;
                stats.Browser = browserDetector.Browser.Name;
                stats.Date = DateTime.Now;
                stats.Url = shortUrl;

                await _unitOfWork.Statisticts.Add(stats);
                await _unitOfWork.CompleteAsync();

                return stats;
            }
            else
            {
                return new UrlStatistics() { Successful = false, Message = "404" };
            }
        }

        public async Task<List<Url>> GetAllUrlAsync()
        {
            var urlList = await _unitOfWork.Url.All();
            return urlList.ToList();
        }

        public async Task<List<Url>> GetLast10UrlsAsync()
        {
            var urlList = await _unitOfWork.Url.All();
            return urlList.OrderByDescending(x=> x.Id).Take(10).ToList();
        }

        public async Task<Url> GetUrlsAsync(string url)
        {
            var Url = await _unitOfWork.Url.FindOne(u => u.ShortUrl.ToLower() == url.ToLower());

            //Check if exist
            if (Url != null)
            {
                return Url;
            }
            else
            {
                return new Url() { Successful = false, Message = "404" };
            }
        }

        public async Task<List<UrlStatistics>> GetUrlStatistictsAsync(Url url)
        {
            var statisticts = await _unitOfWork.Statisticts.Find(u => u.Url.ShortUrl.ToLower() == url.ShortUrl.ToLower());

            return statisticts.ToList();
        }
    }
}