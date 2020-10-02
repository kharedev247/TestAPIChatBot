using EchoBot;
using EchoBot.Handlers;
using Microsoft.OpenApi.Readers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BotTest.GetApiPhysicalPathTest
{
    class GetApiPhysicalPathTest
    {
        UserProfile dummyUserProfile;

        public static async Task<Stream> GetFile()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://chatbotfood.blob.core.windows.net/")
            };

            return await httpClient.GetStreamAsync("apidocs/dummyapi.json");
        }

        [SetUp]
        public void Setup()
        {
            var stream = GetFile();
            var openApiDocument = new OpenApiStreamReader().Read(stream.Result, out var diagnostic);

            dummyUserProfile = new UserProfile()
            {
                Name = "Ketan",
                ApiObject = openApiDocument
            };
        }

        [Test]
        public void GetPhysicalPathTest()
        {
            var response = GetApiPathHandler.AvailablePhysicalPath(dummyUserProfile.ApiObject);
            Assert.AreEqual(5, response.Count);
        }
    }
}
