using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MatrixCDN.Engine;
using Newtonsoft.Json;
using System.Text;

namespace MatrixCDN.Controllers
{
    [Route("cron/[action]")]
    public class CronController : Controller
    {
        #region CronController
        public static (int[] ports, int online) currenthostAMS = (new int[1] { 1010 }, 0);

        IMemoryCache memory;

        public CronController(IMemoryCache memory)
        {
            this.memory = memory;
        }
        #endregion

        #region UpdateNodes
        static bool workUpdateNodes = false;

        async public Task<string> UpdateNodes()
        {
            if (workUpdateNodes)
                return "work";

            workUpdateNodes = true;

            try
            {
                while (true)
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(5);

                        int online = 0;
                        var ports = new List<(int port, int torrenst)>();

                        for (int i = 10; i <= 40; i++)
                        {
                            int port = i + 1000;
                            string tid = (i + 100).ToString();

                            string json = null;

                            try
                            {
                                json = await (await client.PostAsync($"http://127.0.0.1:{port}/torrents", new StringContent("{\"action\":\"list\"}")))?.Content?.ReadAsStringAsync();
                            }
                            catch { }

                            if (json != null)
                            {
                                int count = json.Split("\"hash\"").Length;
                                if (count > 1)
                                    online += (count - 1);

                                #region Перезапускаем каждые 20 минут
                                string memKey = $"Tplay:Nodes:{tid}";
                                if (!memory.TryGetValue(memKey, out DateTime nextReload))
                                {
                                    nextReload = DateTime.Now.AddMinutes(20);
                                    memory.Set(memKey, nextReload, DateTime.Now.AddDays(1));
                                }
                                else if (count == 1 && DateTime.Now > nextReload)
                                {
                                    Bash.Run($"service tor{tid} restart");
                                    nextReload = DateTime.Now.AddMinutes(20);
                                    memory.Set(memKey, nextReload, DateTime.Now.AddDays(1));
                                    continue;
                                }
                                #endregion

                                ports.Add((port, count));
                            }
                            else
                            {
                                Bash.Run($"service tor{tid} restart");
                            }
                        }

                        #region Обновляем currenthost
                        var _pors = ports.Where(i => i.torrenst <= 2).Select(i => i.port).ToArray();
                        if (_pors.Length > 3)
                        {
                            currenthostAMS.ports = _pors;
                        }
                        else
                        {
                            currenthostAMS.ports = ports.Select(i => i.port).ToArray();
                        }

                        currenthostAMS.online = online;
                        #endregion
                    }

                    await Task.Delay(2_000);
                }
            }
            catch { }

            workUpdateNodes = false;
            return "ok";
        }
        #endregion

        #region UpdateSettings
        async public Task<string> UpdateSettings()
        {
            string data = System.IO.File.ReadAllText("settings.json");

            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);

                for (int i = 10; i <= 40; i++)
                    _ = await client.PostAsync($"http://127.0.0.1:{i + 1000}/settings", new StringContent("{\"action\":\"set\",\"sets\":" + data + "}", Encoding.UTF8, "application/json"));
            }

            return "ok";
        }
        #endregion

        #region UpdateAccs
        public string UpdateAccs()
        {
            if (System.IO.File.Exists("accs.db"))
                Startup.accs = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText("accs.db"));
            else
                Startup.accs = null;

            return "ok";
        }
        #endregion

        #region saveTorDb
        public string saveTorDb()
        {
            System.IO.File.WriteAllText("torDb.json", JsonConvert.SerializeObject(TorApiController.torDb, Formatting.Indented));

            return "ok";
        }
        #endregion
    }
}
