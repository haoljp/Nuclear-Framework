﻿using Newtonsoft.Json;
using Nuclear.ExportLocator.Decorators;
using Nuclear.ExportLocator.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Nuclear.Channels.Remoting
{
    public class ChannelRemotingClient : IChannelRemotingClient
    {
        private string _http;

        private class ChannelMessage
        {
            public bool Success { get; set; }
            public object Output { get; set; }
            public string Message { get; set; }
        }

        public ChannelRemotingClient()
        {
            _http = string.Empty;
        }

        public object GetResponse(ChannelRequest request)
        {
            return GetResponseObject(request);
        }

        public TEntity GetResponse<TEntity>(ChannelRequest request)
        {
            return (TEntity)GetResponseObject(request);
        }

        public void Send(ChannelRequest request)
        {
            ChannelMessage channelMessage = GetChannelMessage(request);
            if (!channelMessage.Success)
                throw new ChannelRequestException(channelMessage.Message);
        }

        private string GetHttpMethod(Type requestType)
        {
            if (requestType == typeof(ChannelMethodGetRequest))
                return "GET";
            else
                return "POST";
        }

        private object GetResponseObject(ChannelRequest request)
        {
            ChannelMessage channelMessage = GetChannelMessage(request);
            return channelMessage.Output;
        }

        private ChannelMessage GetChannelMessage(ChannelRequest request)
        {
            HttpWebRequest httpRequest = CreateWebRequest(request);
            string response = GetWebResponse(httpRequest);
            ChannelMessage channelMessage = JsonConvert.DeserializeObject<ChannelMessage>(response);
            if (channelMessage.Success)
                return channelMessage;
            else
                throw new ChannelRequestException(channelMessage.Message);
        }

        private HttpWebRequest CreateWebRequest(ChannelRequest request)
        {
            _http = GetHttpMethod(request.GetType());
            string requestBody = string.Empty;
            byte[] bodyBuffer = null;

            if (_http == "GET" && request.Parameters != null && request.Parameters.Count() > 0)
            {
                StringBuilder queryUrl = new StringBuilder(request.Url);
                ChannelMethodParameter first = request.Parameters.GetFirstParam();
                queryUrl.Append($"?{first.Name}={first.Value}");
                request.Parameters.RemoveParameter(first);
                foreach (ChannelMethodParameter cmparam in request.Parameters.AllParameters())
                {
                    queryUrl.Append($"&{cmparam.Name}={cmparam.Value}");
                }

                request.Url = queryUrl.ToString();
            }
            else if (_http == "POST" && request.Parameters != null && request.Parameters.Count() > 0)
            {
                if (request.Parameters.ContentType == RequestContentType.JSON)
                {
                    Dictionary<string, object> jsonRequsetBuilder = new Dictionary<string, object>();
                    foreach (ChannelMethodParameter cmparam in request.Parameters.AllParameters())
                    {
                        jsonRequsetBuilder.Add(cmparam.Name, cmparam.Value);
                    }

                    requestBody = JsonConvert.SerializeObject(jsonRequsetBuilder);
                }
                else
                {
                    string xmlHeader = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>";
                    StringBuilder xmlRequestBuilder = new StringBuilder(xmlHeader);
                    xmlRequestBuilder.Append(Environment.NewLine);
                    xmlRequestBuilder.Append("<channels>");
                    foreach (ChannelMethodParameter cmparam in request.Parameters.AllParameters())
                    {
                        xmlRequestBuilder.Append(Environment.NewLine);
                        xmlRequestBuilder.Append($"<{cmparam.Name}>{cmparam.Value}</{cmparam.Name}>");
                    }
                    xmlRequestBuilder.Append("</channels>");

                    requestBody = xmlRequestBuilder.ToString();
                }
            }


            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(request.Url);
            httpRequest.Method = _http;
            if (request.Credentials != null)
            {
                httpRequest.PreAuthenticate = true;
                string authorize = string.Empty;
                if (request.Credentials.GetType() == typeof(ChannelBasicCredentials))
                {
                    ChannelBasicCredentials basic = (ChannelBasicCredentials)request.Credentials;
                    authorize = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{basic.Username}:{basic.Password}"));
                }
                else
                {
                    ChannelTokenCredentials token = (ChannelTokenCredentials)request.Credentials;
                    authorize = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{token.Token}"));
                }
                httpRequest.Headers["Authorization"] = authorize;
            }


            if (!String.IsNullOrEmpty(requestBody))
            {
                bodyBuffer = Encoding.ASCII.GetBytes(requestBody);
                Stream requestStream = httpRequest.GetRequestStream();
                requestStream.Write(bodyBuffer, 0, bodyBuffer.Length);
                requestStream.Close();

                if (request.Parameters.ContentType == RequestContentType.JSON)
                    httpRequest.ContentType = "application/json";
                else
                    httpRequest.ContentType = "application/xml";
            }

            return httpRequest;
        }

        private string GetWebResponse(HttpWebRequest request)
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
