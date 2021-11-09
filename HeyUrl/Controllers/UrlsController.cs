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
using UnitOfWork.Services;

namespace Entities.Controllers
{
    [ApiController]
    [Route("/")]
    public class UrlsController : Controller
    {
        private readonly IBrowserDetector browserDetector;
        private readonly UrlService _UrlService;

        public UrlsController(IUnitOfWork unitOfWork, IBrowserDetector browserDetector)
        {
            this.browserDetector = browserDetector;
            _UrlService = new UrlService(unitOfWork);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Url Url)
        {
            if (ModelState.IsValid)
            {
                var result = await _UrlService.CreateShortUrlAsync(Url);

                if (result.Successful)
                {
                    return RedirectToAction(actionName: nameof(Show), routeValues: new { url = result.ShortUrl });
                }
                else
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Index");
                }
            }
            return View(Url);
        }

        //Contains the form and a list of the last 10 URL created with their click count
        public async Task<IActionResult> Index()
        {
            var urlList = await _UrlService.GetLast10UrlsAsync();
            var model = new HomeViewModel();
            model.Urls = urlList;

            model.NewUrl = new();
            return View(model);
        }

        [Route("/{url}")]
        //Redirects from a short URL to the original URL and should also track the click event
        public async Task<IActionResult> Visit(string url)
        {
            var result = await _UrlService.VisitUrl(url, this.browserDetector);

            if (result.Successful)
            {
                return Redirect(result.Url.LongUrl);
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel() { RequestId = result.Message });

            }
        }

        [Route("urls/{url}")]
        //Shows the metrics associated to the short URL
        public async Task<IActionResult> Show(string url)
        {
            var _oUrl = await _UrlService.GetUrlsAsync(url);

            if (_oUrl.Successful)
            {
                var statisticts = await _UrlService.GetUrlStatistictsAsync(_oUrl);

                if (statisticts.Count() > 0)
                {
                    var dailyClicks = statisticts.GroupBy(x => new { Day = x.Date.ToShortDateString() }).ToDictionary(x => x.Key.Day, x => x.Count());
                    var browseClicks = statisticts.GroupBy(x => x.Browser).ToDictionary(x => x.Key.ToString(), x => x.Count());
                    var platformClicks = statisticts.GroupBy(x => x.UserAgent).ToDictionary(x => x.Key.ToString(), x => x.Count());

                    return View(new ShowViewModel()
                    {
                        Url = _oUrl,
                        DailyClicks = dailyClicks,
                        BrowseClicks = browseClicks,
                        PlatformClicks = platformClicks
                    });
                }
                else
                {
                    return View(new ShowViewModel()
                    {
                        Url = _oUrl,
                        DailyClicks = new Dictionary<string, int> { { "0", 0 } },
                        BrowseClicks = new Dictionary<string, int> { },
                        PlatformClicks = new Dictionary<string, int> { }
                    });

                }
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel() { RequestId = "404" });
            }
        }

        // GET: api/<UrlController>
        [HttpGet("/api/last10")]
        public async Task<JsonResult> Get()
        {
            var urlList = await _UrlService.GetLast10UrlsAsync();
            return Json(urlList);
        }
    }
}