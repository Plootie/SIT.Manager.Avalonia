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
        public string Session;
        public Uri RemoteEndPoint;
        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpClientHandler;
        
        public TarkovRequesting(string session, Uri remoteEndPont, HttpClient httpClient, HttpClientHandler httpClientHandler)
        { 
            Session = session;
            RemoteEndPoint = remoteEndPont;
            _httpClient = httpClient;
            _httpClientHandler = httpClientHandler;
        }

        private async Task<Stream> Send(string url, HttpMethod? method = null, string? data = null, bool useCompression = true, CancellationToken cancellationToken = default)
        {
            //Why make simple things enums when we can make them non static types to make life hard :)
            method ??= HttpMethod.Get;
            //TODO: Look at making these default headers. Not sure if the added overhead of setting up each request is justifiable with keeping a single instance
            HttpRequestMessage request = new(method, new Uri(RemoteEndPoint, url));
            request.Headers.ExpectContinue = true;
            
            if(!string.IsNullOrEmpty(Session))
            {
                request.Headers.Add("Cookie", $"PHPSESSID={Session}");
                request.Headers.Add("SessionId", Session);
            }

            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            if(method != HttpMethod.Get && !string.IsNullOrEmpty(data))
            {
                byte[] bytes = useCompression ? SimpleZlib.CompressToBytes(data, zlibConst.Z_BEST_SPEED) : Encoding.UTF8.GetBytes(data);
                request.Content = new ByteArrayContent(bytes);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                if (useCompression)
                    request.Content.Headers.ContentEncoding.Add("deflate");
            }

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter((int)Math.Round(TimeSpan.FromMinutes(1).TotalMilliseconds));
                HttpResponseMessage response = await _httpClient.SendAsync(request, cts.Token);
#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods
                return await response.Content.ReadAsStreamAsync();
#pragma warning restore CA2016 // Forward the 'CancellationToken' parameter to methods
            }
            catch
            {
                //TODO: Implement the retry with best compression on http. I hate how the OG did this
                return null;
            }
        }
    }
}
