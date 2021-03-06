using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using MatrixCDN.Engine;
using MatrixCDN.Models;
using System.Collections.Generic;
using System.Linq;
using IO = System.IO;

namespace MatrixCDN.Controllers
{
    public class TorApiController : Controller
    {
        #region TorApiController
        IMemoryCache memoryCache;

        static Random random = new Random();

        public static Dictionary<string, List<TorInfo>> torDb = System.IO.File.Exists("torDb.json") ? JsonConvert.DeserializeObject<Dictionary<string, List<TorInfo>>>(System.IO.File.ReadAllText("torDb.json")) : new Dictionary<string, List<TorInfo>>();

        public TorApiController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        string getUserId()
        {
            var userData = HttpContext?.Features?.Get<UserData>();
            return userData?.login ?? "public";
        }

        string getHash(string link)
        {
            string hash = Regex.Match(link, "magnet:\\?xt=urn:btih:([a-zA-Z0-9]+)").Groups[1].Value.ToLower();
            if (string.IsNullOrWhiteSpace(hash))
                return link;

            return hash;
        }

        string getHost(string hash)
        {
            string memKey = $"tplay:torrents:{HttpContext.Connection.RemoteIpAddress}:{hash}";
            if (!memoryCache.TryGetValue(memKey, out string thost))
            {
                var ports = (int[])CronController.currenthostAMS.ports.Clone();
                thost = $"{HttpContext.Request.Host.Value}:{ports[random.Next(0, ports.Length)]}";

                memoryCache.Set(memKey, thost, DateTime.Now.AddHours(4));
            }

            return thost;
        }
        #endregion


        #region Index
        [Route("/")]
        public ActionResult Index()
        {
            if (IO.File.Exists("index.html"))
                return Content(IO.File.ReadAllText("index.html").Replace("{online}", CronController.currenthostAMS.online.ToString()), "text/html");

            return Content($"online: {CronController.currenthostAMS.online}\n\nhttps://github.com/cores-system/MatrixCDN/blob/master/README.md");
        }
        #endregion

        #region Echo
        [Route("/echo")]
        public ActionResult Echo()
        {
            return Content("MatriX.CDN");
        }
        #endregion

        #region settings
        [Route("/settings")]
        async public Task<ActionResult> Settings()
        {
            string memKey = $"tplay:settings";
            if (!memoryCache.TryGetValue(memKey, out string settings))
            {
                settings = await HttpClient.Post($"http://127.0.0.1:1010/settings", "{\"action\":\"get\"}");
                if (string.IsNullOrWhiteSpace(settings))
                    return Content("settings == null");

                memoryCache.Set(memKey, settings, DateTime.Now.AddMinutes(20));
            }

            return Content(settings, "application/json");
        }
        #endregion

        #region Viewed
        [Route("/viewed")]
        public ActionResult Viewed()
        {
            return Content("[]", "application/json");
        }
        #endregion


        #region Torrents
        [Route("/torrents")]
        async public ValueTask<ActionResult> Torrents()
        {
            // Копируем поток
            MemoryStream mem = new MemoryStream();
            await HttpContext.Request.Body.CopyToAsync(mem);

            // Получаем данные запроса
            string json = Encoding.UTF8.GetString(mem.ToArray());
            if (string.IsNullOrWhiteSpace(json))
                return Json(new { code = 1 });

            // Парсим перменные
            var tinfo = JsonConvert.DeserializeObject<TorInfo>(json);

            string userid = getUserId();

            #region action list
            if (tinfo.action == "list")
            {
                if (!torDb.TryGetValue(userid, out List<TorInfo> infos))
                    return Json(new { code = 2 });

                return Json(infos.Select(i => new
                {
                    i.title,
                    i.poster,
                    i.hash,
                    stat = 3,
                    stat_string = "Torrent working"
                }));
            }
            #endregion

            if (tinfo.action != "add" && tinfo.action != "rem" && tinfo.action != "get")
                return Json(new { code = 2 });

            if (string.IsNullOrWhiteSpace(tinfo.hash))
                tinfo.hash = getHash(tinfo.link);

            if (string.IsNullOrWhiteSpace(tinfo.hash))
                return Json(new { code = 3 });

            string thost = getHost(tinfo.hash);

            switch (tinfo.action)
            {
                case "add":
                    {
                        #region Сохраняем в базу
                        if (tinfo.save_to_db)
                        {
                            if (!torDb.TryGetValue(userid, out List<TorInfo> infos))
                                torDb[userid] = new List<TorInfo>();

                            infos = torDb[userid];
                            var item = infos.FirstOrDefault(i => i.hash == tinfo.hash);

                            if (item != null)
                            {
                                item.title = tinfo.title;
                                item.link = tinfo.link;
                                item.poster = tinfo.poster;
                            }
                            else
                            {
                                infos.Insert(0, tinfo);
                            }

                            torDb[userid] = infos;
                        }
                        #endregion

                        string data = "{\"action\":\"add\",\"link\":\"" + tinfo.link + "\",\"save_to_db\":false}";
                        return Content(await HttpClient.Post($"http://{thost}/torrents", data), "application/json");
                    }

                case "get":
                    {
                        string data = "{\"action\":\"get\",\"hash\":\"" + tinfo.hash + "\"}";
                        string result = await HttpClient.Post($"http://{thost}/torrents", data);

                        if (result == null)
                        {
                            if (await HttpClient.Post($"http://{thost}/torrents", "{\"action\":\"add\",\"link\":\"" + tinfo.hash + "\",\"save_to_db\":false}") != null)
                                result = await HttpClient.Post($"http://{thost}/torrents", data);
                        }

                        return Content(result, "application/json");
                    }

                case "rem":
                    {
                        #region Удаляем с базы
                        if (torDb.TryGetValue(userid, out List<TorInfo> infos))
                        {
                            var item = infos.FirstOrDefault(i => i.hash == tinfo.hash);

                            if (item != null)
                            {
                                infos.Remove(item);
                                torDb[userid] = infos;
                            }
                        }
                        #endregion

                        string data = "{\"action\":\"rem\",\"hash\":\"" + tinfo.hash + "\"}";
                        return Content(await HttpClient.Post($"http://{thost}/torrents", data), "application/json");
                    }

                default:
                    return Json(new { code = 4 });
            }
        }
        #endregion

        #region TorrentUpload
        [Route("/torrent/upload")]
        async public ValueTask<ActionResult> TorrentUpload()
        {
            // Копируем torrent
            MemoryStream mem = new MemoryStream();
            await HttpContext.Request.Form.Files[0].CopyToAsync(mem);
            var torrent = mem.ToArray();

            if (torrent == null || torrent.Length == 0)
                return Json(new { code = 1 });

            // Получаем хост
            var ports = CronController.currenthostAMS.ports;
            string thost = $"{HttpContext.Request.Host.Value}:{ports[random.Next(0, ports.Length)]}";

            string resupload = null;

            // Отправляем торрент на сервер
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var form = new System.Net.Http.MultipartFormDataContent();

                form.Add(new System.Net.Http.ByteArrayContent(torrent, 0, torrent.Length), "file1", "filename");
                form.Add(new System.Net.Http.StringContent(""), "title");
                form.Add(new System.Net.Http.StringContent(""), "poster");
                form.Add(new System.Net.Http.StringContent(""), "data");

                var response = await httpClient.PostAsync($"http://{thost}/torrent/upload", form);
                resupload = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(resupload))
                    return Json(new { code = 2 });

                string hash = Regex.Match(resupload, "\"hash\":\"([^\"]+)\"").Groups[1].Value;
                if (string.IsNullOrWhiteSpace(hash))
                    return Json(new { code = 3 });

                #region Сохраняем в базу
                if (HttpContext.Request.Form.TryGetValue("save", out var _save) && _save.ToString() == "true")
                {
                    string userid = getUserId();
                    if (!torDb.TryGetValue(userid, out List<TorInfo> infos))
                        torDb[userid] = new List<TorInfo>();

                    infos = torDb[userid];
                    var item = infos.FirstOrDefault(i => i.hash == hash);

                    var tinfo = new TorInfo()
                    {
                        title = Regex.Match(resupload, "\"name\":\"([^\"]+)\"").Groups[1].Value.Split(".")[0],
                        poster = Regex.Match(resupload, "\"poster\":\"([^\"]+)\"").Groups[1].Value,
                        hash = hash
                    };

                    if (item != null)
                    {
                        item.title = tinfo.title;
                        item.poster = tinfo.poster;
                    }
                    else
                    {
                        infos.Insert(0, tinfo);
                    }

                    torDb[userid] = infos;
                }
                #endregion

                // Сохраняем кеш хоста
                memoryCache.Set($"tplay:torrents:{HttpContext.Connection.RemoteIpAddress}:{hash}", thost, DateTime.Today.AddHours(4));
            }

            // Отдаем json
            return Content(Regex.Replace(resupload, "(^\\[|\\]$)", ""), "application/json");
        }
        #endregion

        #region Stream
        [Route("/stream")]
        [Route("/stream/{filename}")]
        async public ValueTask<ActionResult> Stream(string filename, string link, int index)
        {
            string hash = getHash(link);
            if (hash == null)
                return Json(new { code = 1 });

            string thost = getHost(hash);
            string queryString = HttpContext.Request.Path.Value + HttpContext.Request.QueryString.Value;

            if (queryString.Contains("&preload"))
                return Content(await HttpClient.Get($"http://{thost}{queryString}"), "application/json");

            string host = thost.Split(":")[0];
            string port = thost.Split(":")[1];

            return Redirect($"http://{host}:8090/{port}:{index}/{hash}");
        }
        #endregion

        #region Playlists
        [Route("/stream/playlists.m3u")]
        async public ValueTask<ActionResult> Playlists(string link)
        {
            string hash = getHash(link);
            if (hash == null)
                return Content("null", "audio/x-mpegurl");

            string thost = getHost(hash);
            string queryString = HttpContext.Request.Path.Value + HttpContext.Request.QueryString.Value;

            string m3u = await HttpClient.Get($"http://{thost}{queryString}&m3u");
            if (m3u == null)
                return Content(string.Empty, "audio/x-mpegurl");

            StringBuilder newm3u = new StringBuilder();

            foreach (string line in m3u.Split("\n"))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.Contains("http://"))
                {
                    string filehash = Regex.Match(line, "link=([^&]+)").Groups[1].Value;
                    string index = Regex.Match(line, "&index=([0-9]+)").Groups[1].Value;

                    string host = thost.Split(":")[0];
                    string port = thost.Split(":")[1];

                    newm3u.Append($"http://{host}:8090/{port}:{index}/{filehash}" + "\n");
                }
                else
                {
                    newm3u.Append(line + "\n");
                }
            }

            return Content(newm3u.ToString(), "audio/x-mpegurl");
        }
        #endregion
    }
}
