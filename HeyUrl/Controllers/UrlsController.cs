using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shyjus.BrowserDetection;
using UnitOfWork.Configuration;
using HeyUrl.Helpers;
using System.Linq;

namespace Entities.Controllers
{
    [ApiController]
    [Route("/")]
    public class UrlsController : Controller
    {
        private readonly ILogger<UrlsController> _logger;
        private static readonly Random getrandom = new Random();

        private readonly IBrowserDetector browserDetector;
        private readonly IUnitOfWork _unitOfWork;

        public UrlsController(IUnitOfWork unitOfWork, IBrowserDetector browserDetector)
        {
            this.browserDetector = browserDetector;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Url Url)
        {
            //Check if url is valid
            if (Uri.TryCreate(Url.LongUrl, UriKind.Absolute, out Uri validatedUri))
            {
                //CheckIF Url Exist in DataBase

                // Check if long URL already exists in the database
                Url existingURL = await _unitOfWork.Url.FindOne(u => u.ShortUrl.ToLower() == Url.LongUrl.ToLower());

                if (existingURL == null)
                {
                    TryValidateModel(Url);

                    if (ModelState.IsValid)
                    {
                        await _unitOfWork.Url.Insert(Url);
                        Url.ShortUrl = UrlShortener.Encode(Url.Id);
                        Url.Date = DateTime.Now;

                        await _unitOfWork.CompleteAsync();
                        return RedirectToAction(actionName: nameof(Show), routeValues: new { url = Url.ShortUrl });
                    }

                    return View(Url);
                }
                else
                {
                    return RedirectToAction(actionName: nameof(Show), routeValues: new { id = existingURL.ShortUrl });
                }
            }
            else
            {
                TempData["Error"] = "Invalid Url!";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Index()
        {
            var urlList = await _unitOfWork.Url.All();
            var model = new HomeViewModel();
            var Urls = new List<Url>();
            model.Urls = Urls;

            if (urlList.Count() > 0)
            {
                foreach (var item in urlList)
                {
                    Urls.Add(item);
                }
            }

            model.NewUrl = new();
            return View(model);
        }

        [Route("/{url}")]
        public async Task<IActionResult> Visit(string url)
        {
            //Check if exist
            var shortUrl = await _unitOfWork.Url.FindOne(u => u.ShortUrl.ToLower() == url.ToLower());

            if (shortUrl != null)
            {
                shortUrl.Count = shortUrl.Count + 1;

                UrlStatistics stats = new UrlStatistics();
                stats.UserAgent = this.browserDetector.Browser.OS;
                stats.Browser = this.browserDetector.Browser.Name;
                stats.Date = DateTime.Now;
                stats.Url = shortUrl;

                await _unitOfWork.Statisticts.Add(stats);
                await _unitOfWork.CompleteAsync();

                return new OkObjectResult($"{url}, {this.browserDetector.Browser.OS}, {this.browserDetector.Browser.Name}");
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel() { RequestId = "404" });
            }
        }

        [Route("urls/{url}")]
        public async Task<IActionResult> Show(string url)
        {
            var Url = await _unitOfWork.Url.FindOne(u => u.ShortUrl.ToLower() == url.ToLower());
            var statisticts = await _unitOfWork.Statisticts.Find(u => u.Url.ShortUrl.ToLower() == url.ToLower());

            if (statisticts.Count() > 0)
            {
                var dailyClicks = statisticts.GroupBy(x => new { Day = x.Date.ToShortDateString() }).ToDictionary(x => x.Key.Day, x => x.Count());
                var browseClicks = statisticts.GroupBy(x => x.Browser).ToDictionary(x => x.Key.ToString(), x => x.Count());
                var platformClicks = statisticts.GroupBy(x => x.UserAgent).ToDictionary(x => x.Key.ToString(), x => x.Count());

                return View(new ShowViewModel()
                {
                    Url = new Url { ShortUrl = url, Count = Url.Count },

                    DailyClicks = dailyClicks,
                    BrowseClicks = browseClicks,
                    PlatformClicks = platformClicks
                });
            }
            else
            {
                return View(new ShowViewModel()
                {
                    Url = new Url { ShortUrl = url, Count = 0 },
                    DailyClicks = new Dictionary<string, int> { { "0", 0 } },
                    BrowseClicks = new Dictionary<string, int> { },
                    PlatformClicks = new Dictionary<string, int> { }
                });
            }
        }
    }
}