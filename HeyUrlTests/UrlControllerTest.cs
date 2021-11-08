using Autofac;
using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using Entities.Controllers;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Shyjus.BrowserDetection;
using System.Threading.Tasks;
using UnitOfWork.Configuration;
using DataAccessLayer.Interfaces.Models;
using System.Collections.Generic;
using System.Linq;
using Entities.ViewModels;
using HeyUrl.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;

namespace HeyUrlTests
{
    public class UrlControllerTest
    {
        private readonly Mock<IBrowserDetector> browserDetector;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UrlsController controller;
        private Mock<IUrlRepository> _urlRepository;

        [SetUp]
        public void Setup()
        {

        }

        public UrlControllerTest()
        {
            this.browserDetector = new Mock<IBrowserDetector>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _urlRepository = new Mock<IUrlRepository>();
            _unitOfWork.Setup(c => c.Url).Returns(_urlRepository.Object);

            controller = new UrlsController(_unitOfWork.Object, browserDetector.Object);
        }

        [Test]
        public async Task Index()
        {
            var productRepositoryMock = new Mock<IUrlRepository>();

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
            Assert.IsTrue(result.RouteValues.Where(x=>x.Value.ToString() == url.ShortUrl).Any());
        }
    }
}