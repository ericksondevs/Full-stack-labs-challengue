using Autofac;
using DataAccessLayer.Interfaces.Models;
using Entities.Controllers;
using Entities.Models;
using Entities.ViewModels;
using HeyUrl.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using NUnit.Framework;
using Shyjus.BrowserDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork.Configuration;
using UnitOfWork.Services;

namespace HeyUrlTests
{
    public class UrlControllerTest
    {
        private readonly Mock<IBrowserDetector> _browserDetector;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UrlsController controller;
        private Mock<IUrlRepository> _urlRepository;
        private Mock<IUrlStastiticsRepository> _stastiticsRepository;

        private readonly UrlService _Urlservice;


        [SetUp]
        public void Setup()
        {

        }

        public UrlControllerTest()
        {
            //Config Browser
            _browserDetector = new Mock<IBrowserDetector>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _urlRepository = new Mock<IUrlRepository>();
            _stastiticsRepository = new Mock<IUrlStastiticsRepository>();

            _unitOfWork.Setup(c => c.Url).Returns(_urlRepository.Object);
            _unitOfWork.Setup(c => c.Statisticts).Returns(_stastiticsRepository.Object);

            _Urlservice = new UrlService(_unitOfWork.Object);
            controller = new UrlsController(_unitOfWork.Object, _browserDetector.Object);
        }

        [Test]
        public async Task Index()
        {
            var urlList = new List<Url>() {
           new Url() { Id = 1,  LongUrl = "longUrl.com", ShortUrl="ABCDE", Count = 1 },
           new Url() { Id = 2,  LongUrl = "longUrl.com", ShortUrl="JKLOP", Count = 2 },
           new Url() { Id = 3,  LongUrl = "longUrl.com", ShortUrl="UIOPY", Count = 3 }
          };

            //Arrange
            _urlRepository.Setup(x => x.All()).ReturnsAsync(urlList);

            //Act
            var result = await ((controller.Index() as Task<IActionResult>)) as ViewResult;

            HomeViewModel Model = result.Model as HomeViewModel;

            //Assert
            Assert.AreEqual(Model.Urls.Count(), 3);

            Assert.IsTrue(Model.Urls.Where(x => x.ShortUrl == "ABCDE").Any());
            Assert.IsTrue(Model.Urls.Where(x => x.ShortUrl == "JKLOP").Any());
            Assert.IsTrue(Model.Urls.Where(x => x.ShortUrl == "UIOPY").Any());
        }
        [Test]
        public async Task Create()
        {
            string Url = "https://stackoverflow.com/";
            Url url = new Url() { LongUrl = Url, Id = 1 };

            //Arrange
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            controller.ObjectValidator = objectValidator.Object;

            //Act
            url.ShortUrl = UrlShortener.Encode(url.Id);
            var result = await ((controller.Create(url) as Task<IActionResult>)) as RedirectToActionResult;

            //Assert
            _unitOfWork.Verify(m => m.Url.Insert(It.IsAny<Url>()), Times.Once());
            _unitOfWork.Verify(m => m.CompleteAsync(), Times.Once());

            Assert.IsTrue(result.RouteValues.Where(x => x.Value.ToString() == url.ShortUrl).Any());
        }

        [Test]
        public async Task Visit()
        {
            string Url = "ABCDE";
            Url url = new Url() { ShortUrl = Url, Id = 1 };

            //Arrange
            var urlList = new List<Url>() {
           new Url() { Id = 1,  LongUrl = "longUrl.com", ShortUrl= Url, Count = 1 }
            };

            _urlRepository.Setup(x => x.All()).ReturnsAsync(urlList);

            _unitOfWork.Setup(x => x.Url).Returns(_urlRepository.Object);

            Browser browser = new Browser() { OS = "Windows", Name = "Chrome" };

            _browserDetector.Setup(x => x.Browser).Returns(browser);

            //Act
            var result = await _Urlservice.VisitUrl(url.ShortUrl, _browserDetector.Object);

            //Assert
            _unitOfWork.Verify(m => m.Statisticts.Add(It.IsAny<UrlStatistics>()), Times.Once());
            _unitOfWork.Verify(m => m.CompleteAsync(), Times.Once());
        }

        [Test]
        public async Task VisitInvalidUrl()
        {
            string Url = "ABCDE";
            Url url = new Url() { ShortUrl = Url, Id = 1 };

            //Arrange
            var urlList = new List<Url>() {
           new Url() { Id = 1,  LongUrl = "longUrl.com", ShortUrl= "WRONGURL", Count = 1 }
            };

            _urlRepository.Setup(x => x.All()).ReturnsAsync(urlList);

            _unitOfWork.Setup(x => x.Url).Returns(_urlRepository.Object);

            Browser browser = new Browser() { OS = "Windows", Name = "Chrome" };

            _browserDetector.Setup(x => x.Browser).Returns(browser);

            //Act
            var result = await _Urlservice.VisitUrl(url.ShortUrl, _browserDetector.Object);

            //Assert
            Assert.AreEqual("404", result.Message);
        }
    }
}