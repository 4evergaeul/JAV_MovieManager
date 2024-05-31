using MovieManager.ClassLibrary;
using MovieManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Serilog;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MovieManager.BusinessLogic
{
    public class ScrapeService
    {
        public void GetActorInformation()
        {
            try
            {
                using (var context = new DatabaseContext())
                {
                    var sqlString = $"select * from Actor where LastUpdated IS NULL";
                    var actorNames = context.Database.SqlQuery<Actor>(sqlString).Select(x => x.Name).ToList();
                    //var actorNames = new List<string>() { "徳永しおり" };
                    var index = 0;

                    foreach (var name in actorNames)
                    {
                        var actor = context.Actors.Where(x => x.Name == name).FirstOrDefault();
                        Log.Debug($"Thread {Thread.CurrentThread.ManagedThreadId}: Start to process {index} : {actor.Name}");
                        var cleanActorName = string.Empty;
                        if (actor.Name.Contains('（'))
                        {
                            cleanActorName = actor.Name.Substring(0, actor.Name.IndexOf('（'));
                        }
                        else
                        {
                            cleanActorName = actor.Name;
                        }
                        
                        try
                        {
                            var queryPage = $@"https://www.minnano-av.com/search_result.php?search_scope=actress&search_word={cleanActorName}&search=+Go+";
                            HtmlWeb web = new HtmlWeb();
                            var htmlDoc = web.Load(queryPage);
                            var actorScorePage = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='act-area']");
                            var actorInfoPage = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='act-profile']");

                            // Actor Info
                            if (actorInfoPage != null && !string.IsNullOrEmpty(actorInfoPage?.InnerText))
                            {
                                var dobInfo = actorInfoPage.SelectSingleNode("//span[text()='生年月日']/following-sibling::p");
                                var bodyInfo = actorInfoPage.SelectSingleNode("//span[text()='サイズ']/following-sibling::p");
                                if (dobInfo == null)
                                {
                                    Log.Error($"Can't find {actor.Name} date of birth information!");
                                }
                                else
                                {
                                    DateTime temp = new DateTime();
                                    if (DateTime.TryParse(dobInfo.InnerText.Trim().Substring(0, 11), out temp))
                                    {
                                        actor.DateofBirth = temp.ToString("yyyy-MM-dd");
                                    }
                                }
                                if (bodyInfo == null)
                                {
                                    Log.Error($"Can't find {actor.Name} body information!");
                                }
                                else
                                {
                                    var bodyInfoList = bodyInfo.InnerText.Trim().Split("/");
                                    actor.Height = String.IsNullOrEmpty(bodyInfoList[0].Replace("T", "").Trim().ToString()) ? "" : $"{bodyInfoList[0].Replace("T", "").Trim()}cm";
                                    actor.Cup = bodyInfoList[1].Trim().IndexOf("(") == -1 ? "" : $"{bodyInfoList[1].Trim().Substring(bodyInfoList[1].Trim().IndexOf("(") + 1, 1)} Cup";
                                    actor.Bust = bodyInfoList[1].Trim().IndexOf("(") == -1 ? "" : $"{bodyInfoList[1].Replace("B", "").Trim().Substring(0, bodyInfoList[1].Trim().IndexOf("(") - 1)}";
                                    actor.Waist = $"{bodyInfoList[2].Replace("W", "").Trim()}";
                                    actor.Hips = $"{bodyInfoList[3].Replace("H", "").Trim()}";
                                }
                            }
                            else
                            {
                                Log.Error($"Can't find {actor.Name} information!");
                            }
                            // Actor scores
                            if (actorScorePage != null && !string.IsNullOrEmpty(actorInfoPage?.InnerText))
                            {
                                var scoreNodes = actorScorePage.SelectNodes(".//td[@class='t9']");
                                if (scoreNodes != null && scoreNodes?.Count >= 9)
                                {
                                    actor.Looks = scoreNodes[1]?.InnerText?.Trim();
                                    actor.Body = scoreNodes[3]?.InnerText?.Trim();
                                    actor.SexAppeal = scoreNodes[7]?.InnerText?.Trim();
                                    actor.Overall = scoreNodes[9]?.InnerText?.Trim();
                                }
                                else
                                {
                                    Log.Error($"Can't find {actor.Name} score!");                                
                                }
                            }
                            else
                            {
                                Log.Error($"Can't find {actor.Name} score!");
                            }
                            actor.LastUpdated = DateTime.Now.ToString("yyyy-MM-dd");
                            context.SaveChanges();
                            Thread.Sleep(1000);
                            Log.Debug($"Thread {Thread.CurrentThread.ManagedThreadId}: complete to process {index} : {actor.Name}");
                            index++;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"An error occurs when processing actor{cleanActorName} information! \n\r");
                            Log.Error(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private string GetActorInfoHtml_Deprecated(string actorName)
        {
            var actorInfoPage = string.Empty;

            var queryPage = $@"https://xslist.org/search?query={actorName}";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(queryPage);
            var nodes = htmlDoc.DocumentNode.SelectNodes("//a");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var title = node.GetAttributes("title").FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(title))
                    {
                        if (title.EndsWith(actorName))
                        {
                            actorInfoPage = node.GetAttributes("href").FirstOrDefault()?.Value;
                            break;
                        }
                    }
                }
            }
            return actorInfoPage;
        }
    }
}
