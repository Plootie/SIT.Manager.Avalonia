using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using ComponentAce.Compression.Libs.zlib;
using System.Threading;

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

        private async Task<Stream> Send(string url, HttpMethod? method = null, string? data = null, TarkovRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        {
            //TODO: Clean this, kinda icky
            method ??= HttpMethod.Get;
            //TODO: Look at making these default headers. Not sure if the added overhead of setting up each request is justifiable with keeping a single instance
            UriBuilder serverUriBuilder = new(requestOptions.Scheme, RemoteEndPoint.Host, RemoteEndPoint.Port, url);
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

            }

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter((int)Math.Round(requestOptions.Timeout.TotalMilliseconds));
                HttpResponseMessage response = await _httpClient.SendAsync(request, cts.Token);
                cts.TryReset(); //This is mostly to shut the compiler up about passing the token, idk
                return await response.Content.ReadAsStreamAsync(cancellationToken);
            }
            catch
            {
                //TODO: Implement the retry with best compression on http. I hate how the OG did this
                return null;
            }
        }
    }

    public struct TarkovRequestOptions()
    {
        public int CompressionProfile { get; init; } = zlibConst.Z_BEST_SPEED;
        public string? Scheme { get; init; } = "https://";
        public string[] AcceptEncoding { get; init; } = ["deflate", "gzip"];
        public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(1);
    }
}
