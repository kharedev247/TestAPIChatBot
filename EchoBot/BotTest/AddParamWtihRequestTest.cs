using DocParser.Managers.RequestManager;
using DocParser.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BotTest
{
    class AddParamWithRequestTest
    {
        string path = string.Empty;
        RequestManager rm;

        [SetUp]
        public void Setup() {
            var mockFactory = new Mock<IHttpClientFactory>();
            rm = new RequestManager(mockFactory.Object);
        }

        //[Test]
        //public void AddParameterWithValidArguments()
        //{
        //    path = "http://localhost:44300/api/values/{param1}/{param2}";
        //    var dic = new Dictionary<string, string>();
        //    dic.Add("{param1}", "value1");
        //    dic.Add("{param2}", "value2");

        //    var res = rm.AddParamWithRequestPath(path, dic);

        //    Assert.AreEqual("http://localhost:44300/api/values/value1/value2", res.Result);
        //}
    }
}
