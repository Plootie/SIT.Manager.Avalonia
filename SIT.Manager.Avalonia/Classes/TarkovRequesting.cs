﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using ComponentAce.Compression.Libs.zlib;
using System.Threading;
using System.Diagnostics;

namespace SIT.Manager.Avalonia.Classes
{
    public class TarkovRequesting
    {
        public Uri RemoteEndPoint;
        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpClientHandler;
        
        public TarkovRequesting(Uri remoteEndPont, HttpClient httpClient, HttpClientHandler httpClientHandler)
        { 
            RemoteEndPoint = remoteEndPont;
            _httpClient = httpClient;
            _httpClientHandler = httpClientHandler;
        }

        private async Task<Stream> Send(string url, HttpMethod? method = null, string? data = null, TarkovRequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
        {
            //TODO: Clean this, kinda icky
            method ??= HttpMethod.Get;
            requestOptions ??= new TarkovRequestOptions();
            //TODO: Look at making these default headers. Not sure if the added overhead of setting up each request is justifiable with keeping a single instance
            UriBuilder serverUriBuilder = new(requestOptions.SchemeOverride ?? RemoteEndPoint.Scheme, RemoteEndPoint.Host, RemoteEndPoint.Port, url);
            HttpRequestMessage request = new(method, serverUriBuilder.Uri);
            request.Headers.ExpectContinue = true;


            //Typically deflate, gzip
            foreach(string encoding in requestOptions.AcceptEncoding)
            {
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(encoding));
            }

            if(method != HttpMethod.Get && !string.IsNullOrEmpty(data))
            {
                byte[] contentBytes = SimpleZlib.CompressToBytes(data, requestOptions.CompressionProfile);
                request.Content = new ByteArrayContent(contentBytes);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content.Headers.ContentEncoding.Add("deflate");
                request.Content.Headers.Add("Content-Length", contentBytes.Length.ToString());
            }

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter((int)Math.Round(requestOptions.Timeout.TotalMilliseconds));
                HttpResponseMessage response = await _httpClient.SendAsync(request, cts.Token);
                cts.TryReset(); //This is mostly to shut the compiler up about passing the token, idk
                return await response.Content.ReadAsStreamAsync();
            }
            catch(HttpRequestException ex)
            {
                if (requestOptions.TryAgain)
                {
                    TarkovRequestOptions options = new()
                    {
                        Timeout = TimeSpan.FromSeconds(5),
                        CompressionProfile = zlibConst.Z_BEST_COMPRESSION,
                        SchemeOverride = "http://",
                        AcceptEncoding = ["deflate"],
                        TryAgain = false
                    };
                    return await Send(url, method, data, options, cancellationToken);
                }
                else
                {
                    //TODO: I dislike rethrowing exceptions, the architecture of these net requests are flawed and need redesigned 
                    throw;
                }
            }
        }

        public async Task<string> PostJson(string url, string data)
        {
            using Stream postStream = await Send(url, HttpMethod.Post, data);
            if (postStream == null)
                return string.Empty;
            using MemoryStream ms = new();
            await postStream.CopyToAsync(ms);
            return SimpleZlib.Decompress(ms.ToArray());
        }


    }

    public class TarkovRequestOptions()
    {
        public int CompressionProfile { get; init; } = zlibConst.Z_BEST_SPEED;
        public string? SchemeOverride { get; init; }
        public string[] AcceptEncoding { get; init; } = ["deflate", "gzip"];
        public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(1);
        public bool TryAgain { get; init; } = true;
    }
}
