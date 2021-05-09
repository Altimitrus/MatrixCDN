using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MatrixCDN.Engine
{
    public static class HttpClient
    {
        #region Get
        async public static ValueTask<string> Get(string url, Encoding encoding = default, string cookie = null, string referer = null, int timeoutSeconds = 15, List<(string name, string val)> addHeaders = null, long MaxResponseContentBufferSize = 0)
        {
            return (await BaseGetAsync(url, encoding, cookie: cookie, referer: referer, timeoutSeconds: timeoutSeconds, addHeaders: addHeaders, MaxResponseContentBufferSize: MaxResponseContentBufferSize)).content;
        }
        #endregion

        #region BaseGetAsync
        async public static ValueTask<(string content, HttpResponseMessage response)> BaseGetAsync(string url, Encoding encoding = default, string cookie = null, string referer = null, int timeoutSeconds = 15, long MaxResponseContentBufferSize = 0, List<(string name, string val)> addHeaders = null)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.Brotli | DecompressionMethods.GZip | DecompressionMethods.Deflate
                };

                handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                using (var client = new System.Net.Http.HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                    client.MaxResponseContentBufferSize = MaxResponseContentBufferSize == 0 ? 10_000_000 : MaxResponseContentBufferSize; // 10MB
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");

                    if (cookie != null)
                        client.DefaultRequestHeaders.Add("cookie", cookie);

                    if (referer != null)
                        client.DefaultRequestHeaders.Add("referer", referer);

                    if (addHeaders != null)
                    {
                        foreach (var item in addHeaders)
                            client.DefaultRequestHeaders.Add(item.name, item.val);
                    }

                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            return (null, response);

                        using (HttpContent content = response.Content)
                        {
                            if (encoding != default)
                            {
                                string res = encoding.GetString(await content.ReadAsByteArrayAsync());
                                if (string.IsNullOrWhiteSpace(res))
                                    return (null, response);

                                return (res, response);
                            }
                            else
                            {
                                string res = await content.ReadAsStringAsync();
                                if (string.IsNullOrWhiteSpace(res))
                                    return (null, response);

                                return (res, response);
                            }
                        }
                    }
                }
            }
            catch
            {
                return (null, new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    RequestMessage = new HttpRequestMessage()
                });
            }
        }
        #endregion

        #region Post
        public static ValueTask<string> Post(string url, string data, string cookie = null, int MaxResponseContentBufferSize = 0, int timeoutSeconds = 15, List<(string name, string val)> addHeaders = null)
        {
            return Post(url, new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded"), cookie: cookie, MaxResponseContentBufferSize: MaxResponseContentBufferSize, timeoutSeconds: timeoutSeconds, addHeaders: addHeaders);
        }

        async public static ValueTask<string> Post(string url, HttpContent data, Encoding encoding = default, string cookie = null, int MaxResponseContentBufferSize = 0, int timeoutSeconds = 15, List<(string name, string val)> addHeaders = null)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.Brotli | DecompressionMethods.GZip | DecompressionMethods.Deflate
                };

                handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                using (var client = new System.Net.Http.HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                    client.MaxResponseContentBufferSize = MaxResponseContentBufferSize != 0 ? MaxResponseContentBufferSize : 10_000_000; // 10MB

                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                    if (cookie != null)
                        client.DefaultRequestHeaders.Add("cookie", cookie);

                    if (addHeaders != null)
                    {
                        foreach (var item in addHeaders)
                            client.DefaultRequestHeaders.Add(item.name, item.val);
                    }

                    using (HttpResponseMessage response = await client.PostAsync(url, data))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            return null;

                        using (HttpContent content = response.Content)
                        {
                            if (encoding != default)
                            {
                                string res = encoding.GetString(await content.ReadAsByteArrayAsync());
                                if (string.IsNullOrWhiteSpace(res))
                                    return null;

                                return res;
                            }
                            else
                            {
                                string res = await content.ReadAsStringAsync();
                                if (string.IsNullOrWhiteSpace(res))
                                    return null;

                                return res;
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
