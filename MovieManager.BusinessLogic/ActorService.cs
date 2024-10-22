﻿using Microsoft.Extensions.Options;
using MovieManager.ClassLibrary;
using MovieManager.ClassLibrary.RequestBody;
using MovieManager.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MovieManager.BusinessLogic
{
    public class ActorService
    {
        private UserSettingsService _config;

        public ActorService(UserSettingsService config)
        {
            CultureInfo PronoCi = new CultureInfo(2052);
            _config = config;
        }

        #region Get Full Actor Info
        public List<ActorViewModel> GetAll()
        {
            var results = new List<ActorViewModel>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var actors = dbContext.Actors.ToList();
                    actors.Sort(delegate (Actor x, Actor y)
                    {
                        return x.Name.CompareTo(y.Name);
                    });
                    results = BuildActorViewModels(actors);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting actors. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }

        public List<ActorViewModel> GetByName(string searchString)
        {
            var results = new List<ActorViewModel>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var sqlString = @$"select * from Actor where Name like '%{searchString}%'";
                    var actors = dbContext.Database.SqlQuery<Actor>(sqlString).ToList();
                    actors.Sort(delegate (Actor x, Actor y)
                    {
                        return x.Name.CompareTo(y.Name);
                    });
                    results = BuildActorViewModels(actors);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting actor. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }

        public List<ActorViewModel> GetByNames(List<string> name)
        {
            var results = new List<ActorViewModel>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var actors = dbContext.Actors.Where(x => name.Contains(x.Name)).ToList();
                    results = BuildActorViewModels(actors);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting actors by names. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }
        #endregion

        #region Get Names Only Methods
        public List<string> GetAllNames()
        {
            var results = new List<string>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    results = dbContext.Actors.Select(x => x.Name).ToList();
                    results.Sort();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting actor names. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }

        public List<string> GetNamesByRange(int heightLower, int heightUpper, string cupLower, string cupUpper, int age)
        {
            var results = new List<string>();
            var sqlString = "";
            if ((heightLower == 140 && heightUpper == 190) && age == 50 && (cupLower == "A" && cupUpper == "Z"))
            {
                sqlString = "select * from Actor";
            }
            else
            {
                var sb = new StringBuilder();
                var counter = 0;
                sb.Append("select * from Actor where ");
                if (heightLower != 140 || heightUpper != 190)
                {
                    sb.Append($"Height between '{heightLower}' and '{heightUpper}'");
                    counter++;
                }
                if (age != 50)
                {
                    sb.Append(counter == 0 ? $"date(DateOfBirth, '+{age} years') >= date('now')" : $" and date(DateOfBirth, '+{age} years') >= date('now')");
                    counter++;
                }
                if (cupLower != "A" || cupUpper != "Z")
                {
                    sb.Append(counter == 0 ? $"Cup between '{cupLower} Cup' and '{cupUpper} Cup'" : $" and Cup between '{cupLower} Cup' and '{cupUpper} Cup'");
                    counter++;
                }
                sqlString = sb.ToString();
            }

            try
            {
                using (var context = new DatabaseContext())
                {
                    var actors = context.Database.SqlQuery<Actor>(sqlString).ToList();
                    actors.Sort(delegate (Actor x, Actor y)
                    {
                        return x.Name.CompareTo(y.Name);
                    });
                    results = actors.Select(x => x.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting actor names by range. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }

        public List<string> GetLikedActorNames()
        {
            var results = new List<string>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    results = dbContext.Actors.Where(x => x.Liked).Select(x => x.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting liked actor. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }
        #endregion

        public bool LikeActor(string actorName)
        {
            try
            {
                using (var context = new DatabaseContext())
                {
                    var actor = context.Actors.Where(x => x.Name == actorName).FirstOrDefault();
                    actor.Liked = !actor.Liked;
                    context.SaveChanges();
                    return actor.Liked;
                }

            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when setting actor's like flag. \n\r");
                Log.Error(ex.ToString());
            }
            return false;
        }

        public string GetImagePath(ImageRequest imageRequest)
        {
            var result = "";
            var figureSmallPath = "";
            var figureLargePath = "";
            try
            {
                if (!string.IsNullOrEmpty(_config.GetUserSettings().ActorFiguresDMMDirectory))
                {
                    figureSmallPath = Directory.EnumerateFiles(_config.GetUserSettings().ActorFiguresDMMDirectory, $"AI-Fix-{imageRequest.Id}.jpg", SearchOption.AllDirectories).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(_config.GetUserSettings().ActorFiguresAllDirectory))
                {
                    if (string.IsNullOrEmpty(figureSmallPath))
                    {
                        figureSmallPath = Directory.EnumerateFiles(_config.GetUserSettings().ActorFiguresAllDirectory, $"{imageRequest.Id}.jpg", SearchOption.AllDirectories).FirstOrDefault();
                    }
                    figureLargePath = Directory.EnumerateFiles(_config.GetUserSettings().ActorFiguresAllDirectory, $"{imageRequest.Id}.jpg", SearchOption.AllDirectories).OrderByDescending(f => new FileInfo(f).Length).FirstOrDefault();
                }
                using (var context = new DatabaseContext())
                {
                    switch (imageRequest.ImageType)
                    {
                        case 10:
                            result = figureSmallPath;
                            break;
                        case 11:
                            result = figureLargePath;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting image path. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        private List<ActorViewModel> BuildActorViewModels(List<Actor> actors)
        {
            var result = new List<ActorViewModel>();
            var lockObject = new object();
            var keyValuePairs = new List<KeyValuePair<Actor, bool>>();
            foreach (var a in actors)
            {
                keyValuePairs.Add(new KeyValuePair<Actor, bool>(a, false));
            }
            var taskArray = new Task[8];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < keyValuePairs.Count; j++)
                    {
                        lock (lockObject)
                        {
                            if (!keyValuePairs[j].Value)
                            {
                                var newKvp = new KeyValuePair<Actor, bool>(keyValuePairs[j].Key, true);
                                keyValuePairs[j] = newKvp;
                                var actor = keyValuePairs[j].Key;
                                result.Add(new ActorViewModel()
                                {
                                    Cup = actor.Cup,
                                    DateofBirth = actor.DateofBirth,
                                    Height = actor.Height,
                                    LastUpdated = actor.LastUpdated,
                                    Liked = actor.Liked,
                                    Name = actor.Name,
                                    Bust = actor.Bust,
                                    Waist = actor.Waist,
                                    Hips = actor.Hips,
                                    Looks = actor.Looks,
                                    Body = actor.Body,
                                    SexAppeal = actor.SexAppeal,
                                });
                            }
                        }
                    }
                });
            }
            Task.WaitAll(taskArray);
            return result;
        }
    }
}
