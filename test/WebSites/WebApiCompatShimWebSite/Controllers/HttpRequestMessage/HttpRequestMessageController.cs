﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using System.Net;

namespace WebApiCompatShimWebSite
{
    public class HttpRequestMessageController : ApiController
    {
        public async Task<IActionResult> EchoProperty()
        {
            await Echo(Request);
            return new EmptyResult(); 
        }

        public async Task<IActionResult> EchoParameter(HttpRequestMessage request)
        {
            if (!object.ReferenceEquals(request, Request))
            {
                throw new InvalidOperationException();
            }

            await Echo(request);
            return new EmptyResult();
        }

        public async Task<HttpResponseMessage> EchoWithResponseMessage(HttpRequestMessage request)
        {
            var message = string.Format(
                "{0} {1}", 
                request.Method.ToString(), 
                await request.Content.ReadAsStringAsync());

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(message);
            response.Headers.TryAddWithoutValidation("X-Test", "Hello!");
            return response;
        }

        public async Task<HttpResponseMessage> EchoWithResponseMessageChunked(HttpRequestMessage request)
        {
            var message = string.Format(
                "{0} {1}",
                request.Method.ToString(),
                await request.Content.ReadAsStringAsync());

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(message);
            response.Headers.TransferEncodingChunked = true;
            response.Headers.TryAddWithoutValidation("X-Test", "Hello!");
            return response;
        }

        private async Task Echo(HttpRequestMessage request)
        {
            var message = string.Format(
                "{0} {1} {2} {3} {4}",
                request.Method,
                request.RequestUri.AbsoluteUri,
                request.Headers.Host,
                request.Content.Headers.ContentLength,
                await request.Content.ReadAsStringAsync());

            await Context.Response.WriteAsync(message);
        }
    }
}