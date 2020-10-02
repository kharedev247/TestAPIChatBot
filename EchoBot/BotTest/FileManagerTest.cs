using DocParser.Managers.FileManager;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;

namespace Tests
{
    public class Tests
    {
        FileManager FileManager;
        [SetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .Build();

            FileManager = new FileManager(config);
        }

        [Test]
        public void apiFileNameCannotBeNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await FileManager.GetAPIObject(""));
        }
    }
}