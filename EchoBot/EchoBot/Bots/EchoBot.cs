// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using EchoBot.Handlers;
using EchoBot.Handlers.FileHandler;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly BotState _conversationState;
        private readonly BotState _userState;
        private readonly IFileHandler _fileHandler;
        private readonly IConfiguration _configuration;
        private readonly IApiParameterHandler _apiParameterHandler;
        private readonly IApiRequestHandler _apiRequestHandler;

        private Dictionary<string, string> messageList;
        public UserProfile userProfile;
        public ConversationData userConversation;

        public EchoBot(ConversationState conversationState, UserState userState, IFileHandler fileHandler, IConfiguration configuration, IApiParameterHandler apiParameterHandler, IApiRequestHandler apiRequestHandler)
        {
            _conversationState = conversationState;
            _userState = userState;
            _fileHandler = fileHandler;
            _configuration = configuration;
            messageList = new Dictionary<string, string>();
            _apiParameterHandler = apiParameterHandler;
            _apiRequestHandler = apiRequestHandler;

        }

        // when someone join the conversation then it is hited always
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await SendWelcomeMessageAsync(turnContext, cancellationToken);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }


        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            IMessageActivity response;
            try
            {
                //UserData
                var userStateAccessors = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
                userProfile = await userStateAccessors.GetAsync(turnContext, () => new UserProfile());
                //Conversation Data
                var conversationAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
                userConversation = await conversationAccessors.GetAsync(turnContext, () => new ConversationData());


                if (userConversation.LastMessage == null)
                {
                    if (ProcessInput(turnContext, userProfile))
                    {
                        response = MessageFactory.Text("File uploaded successfully");
                        await turnContext.SendActivityAsync(response);
                        await GetAPIPathsAsync(turnContext, cancellationToken, userProfile.ApiObject);
                        userConversation.LastMessage = "Paths";
                    }
                    else
                    {
                        response = MessageFactory.Text("File can not be uploaded");
                        await turnContext.SendActivityAsync(response);
                    }
                }
                else if (userConversation.LastMessage == "Paths")
                {
                    if (HandleIncomingChoice(turnContext.Activity).Result)
                    {
                        if (userConversation.RequestParams.Count > 0)
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text("Its time to add Request parameter"));
                            await RequestParamFromUser(turnContext);
                            userConversation.LastMessage = "Parameter";
                        }
                        else if (userConversation.OperationType == OperationType.Post && userConversation.RequestBody.Count > 0)
                        {
                            // send reuqest body to user
                            await turnContext.SendActivityAsync(MessageFactory.Text("Its time to add Request body"));
                            await turnContext.SendActivityAsync(userConversation.RequestBody["body"]);
                            userConversation.LastMessage = "body";
                        }
                        else
                        {
                            await ShowServerResponse(turnContext, cancellationToken);
                            ClearUserConversationData(userConversation);
                        }
                    }
                }
                else if (userConversation.LastMessage == "Parameter")
                {
                    if (userConversation.RequestParams.Count > 0)
                    {
                        TakeUserInput(turnContext);
                        if (userConversation.RequestParams.Count > 0)
                        {
                            await RequestParamFromUser(turnContext);
                        }
                        else if (userConversation.OperationType == OperationType.Post && userConversation.RequestBody.Count > 0)
                        {
                            // send reuqest body to user
                            await turnContext.SendActivityAsync(MessageFactory.Text("Its time to add Request body"));
                            await turnContext.SendActivityAsync(userConversation.RequestBody["body"]);
                            userConversation.LastMessage = "body";
                        }
                        else
                        {
                            await ShowServerResponse(turnContext, cancellationToken);
                            ClearUserConversationData(userConversation);
                        }
                    }
                }
                else if (userConversation.LastMessage == "body")
                {
                    userConversation.RequestBody.Add("userBody", turnContext.Activity.Text);
                    await ShowServerResponse(turnContext, cancellationToken);
                    ClearUserConversationData(userConversation);
                }
            }

            catch (Exception e)
            {
                response = MessageFactory.Text(e.Message);
                await turnContext.SendActivityAsync(response);
            }
        }

        private async Task RequestParamFromUser(ITurnContext<IMessageActivity> turnContext)
        {
            ParamInfo paramInfo = userConversation.RequestParams[0];
            await turnContext.SendActivityAsync(MessageFactory.Text($"Please Enter {paramInfo.ParamType} for {paramInfo.ParamDescription}"));
        }

        private void TakeUserInput(ITurnContext<IMessageActivity> turnContext)
        {
            try
            {
                ParamInfo paramInfo = userConversation.RequestParams[0];
                paramInfo.ParamValue = turnContext.Activity.Text;
                userConversation.ParamsInfo[paramInfo.ParamName] = paramInfo;
                userConversation.RequestParams.RemoveAt(0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task ShowServerResponse(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text("Hang On while we take your request to server \n This may take a while...!!!"));
            var serverResponse = await _apiRequestHandler.SendApiRequest(userProfile.ApiObject, userConversation);//call ketan method
            await turnContext.SendActivityAsync("Response code  :" + serverResponse.StatusCode);
            await turnContext.SendActivityAsync(serverResponse.ResponseBody);
            userConversation.LastMessage = "Paths";
            await GetAPIPathsAsync(turnContext, cancellationToken, userProfile.ApiObject);
        }

        // Given the input from the message, create the response.
        private bool ProcessInput(ITurnContext turnContext, UserProfile userProfile)
        {
            var activity = turnContext.Activity;
            if (activity.Attachments != null && activity.Attachments.Any())
            {
                return HandleIncomingAttachment(activity, turnContext, userProfile).Result;
            }

            throw new Exception("Invalid input. Please upload api document(JSON/YAML)");
        }
        // TODO: Handling of return type
        private async Task<bool> HandleIncomingChoice(IMessageActivity activity)
        {
            userConversation.ApiPath = await _apiParameterHandler.GetApiPath(activity.Text);
            userConversation.OperationType = await _apiParameterHandler.GetOperationType(activity.Text);
            userConversation.ParamsInfo = await _apiParameterHandler.GetParams(userConversation.ApiPath, userConversation.OperationType, userProfile.ApiObject);
            if (userConversation.OperationType == OperationType.Post)
            {
                userConversation.RequestBody = new Dictionary<string, string>();
                using (var stringWriter = new StringWriter())
                {
                    var document = userProfile.ApiObject;
                    var writer = new OpenApiJsonWriter(stringWriter);
                    var reference = document.Paths[userConversation.ApiPath].Operations[OperationType.Post].RequestBody.Content["application/json"].Schema.Reference;
                    var schema = document.Components.Schemas[reference.ReferenceV3.Substring(reference.ReferenceV3.LastIndexOf('/') + 1)];
                    schema.SerializeAsV3WithoutReference(writer);
                    var requestBody = JValue.Parse(stringWriter.ToString()).ToString(Formatting.Indented);
                    userConversation.RequestBody.Add("body", requestBody);
                }
            }
            userConversation.RequestParams = userConversation.ParamsInfo.Select(param => param.Value).ToList();
            return true;
        }

        private async Task<bool> HandleIncomingAttachment(IMessageActivity activity, ITurnContext turnContext, UserProfile userProfile)
        {
            OpenApiDocument response = null;
            foreach (var file in activity.Attachments)
            {
                // check for the extension
                var extension = file.Name.Substring(file.Name.LastIndexOf('.'));

                if (extension == ".json" || extension == ".yaml")
                {
                    // Determine where the file is hosted.
                    var remoteFileUrl = file.ContentUrl;

                    // Save the attachment to the system temp directory.
                    var localFileName = Path.Combine(_configuration.GetSection("PhysicalStoragePath").Value, file.Name);

                    // Download the actual attachment
                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadFile(remoteFileUrl, localFileName);
                        // stopping the call to azure for local testing
                        //response = await _fileHandler.UploadFileToAzure(localFileName, file.Name.Substring(0, file.Name.LastIndexOf('.')));
                        response = await _fileHandler.SaveFileToLocal(localFileName, file.Name.Substring(0, file.Name.LastIndexOf('.')));
                    }
                }
                else
                {
                    throw new Exception("Unsupported File Extension.");
                }
            }
            if (response != null)
            {
                userProfile.ApiObject = response;
                return true;
            }
            else
            {
                return false;
            }
        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync($"Welcome to AttachmentsBot {member.Name}.", cancellationToken: cancellationToken);
                }
            }
        }

        private static async Task GetAPIPathsAsync(ITurnContext turnContext, CancellationToken cancellationToken, OpenApiDocument openApiDocument)
        {
            var paths = GetApiPathHandler.AvailablePhysicalPath(openApiDocument);
            var card = new HeroCard
            {
                Title = "Below are the avialable paths:",
                Buttons = new List<CardAction>(),
            };
            foreach (var path in paths)
            {
                card.Buttons.Add(new CardAction(ActionTypes.PostBack, title: path, value: path));
            }

            var reply = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(reply, cancellationToken);

        }
        
        private static void ClearUserConversationData(ConversationData conversationData)
        {
            conversationData.OperationType = OperationType.Get;
            conversationData.ParamsInfo.Clear();
            conversationData.RequestBody.Clear();
            conversationData.RequestParams.Clear();
        }
    }
}
