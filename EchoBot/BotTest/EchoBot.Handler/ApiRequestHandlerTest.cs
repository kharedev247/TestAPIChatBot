using DocParser.Managers.RequestManager;
using EchoBot;
using EchoBot.Handlers;
using Microsoft.OpenApi.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotTest.EchoBot.Handler
{
    class ApiRequestHandlerTest
    {
        ApiRequestHandler ApiRequestHandler;
        OpenApiDocument OpenApiDocument;
        ConversationData UserConversation;

        [SetUp]
        public void Setup()
        {
            ApiRequestHandler = new ApiRequestHandler(new Mock<IRequestManager>().Object);
            OpenApiDocument = new OpenApiDocument();
            UserConversation = new ConversationData();
        }

        [Test]
        public void apiFileNameCannotBeNull()
        {
            UserConversation.ParamsInfo = new Dictionary<string, ParamInfo>() {
                {"{dataset}",new ParamInfo("{dataset}","","Get","path",true) },
                {"{version}",new ParamInfo("{version}","","Get","query",true) },
                {"{field}",new ParamInfo("{field}","","Get","query",true) }
            };
            UserConversation.ApiPath = "/{dataset}/{version}/{field}";
            UserConversation.OperationType = OperationType.Get;

            OpenApiDocument.Servers.Add(new OpenApiServer() {
                Url = "{scheme}://developer.uspto.gov/ds-api"
            });

            var response = ApiRequestHandler.SendApiRequest(OpenApiDocument, UserConversation);
            Assert.AreEqual(" ", response);
        }
    }
}
