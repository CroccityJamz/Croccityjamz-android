using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AutoMapper;
using DeepSound.Helpers.Model;
using DeepSound.Helpers.Utils;
using DeepSoundClient;
using DeepSoundClient.Classes.Comments;
using DeepSoundClient.Classes.Common;
using DeepSoundClient.Classes.Global;
using DeepSoundClient.Classes.User;
using Newtonsoft.Json;
using SQLite;
 
//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) DeepSound 25/04/2019 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================
namespace DeepSound.SQLite
{
    public class SqLiteDatabase : IDisposable
    {
        //############# DON'T MODIFY HERE #############
        private static readonly string Folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public static readonly string PathCombine = Path.Combine(Folder, "DeepSoundMusic.db");
        private SQLiteConnection Connection;

        //Open Connection in Database
        //*********************************************************

        #region Connection

        private SQLiteConnection OpenConnection()
        {
            try
            {
                Connection = new SQLiteConnection(PathCombine);
                return Connection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void CheckTablesStatus()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    Connection.CreateTable<DataTables.LoginTb>();
                    Connection.CreateTable<DataTables.SettingsTb>();
                    Connection.CreateTable<DataTables.LibraryItemTb>();
                    Connection.CreateTable<DataTables.InfoUsersTb>();
                    Connection.CreateTable<DataTables.GenresTb>();
                    Connection.CreateTable<DataTables.PriceTb>();
                    Connection.CreateTable<DataTables.SharedTb>();
                    Connection.CreateTable<DataTables.LatestDownloadsTb>();
                    Connection.CreateTable<DataTables.PublicRecentlyPlayedTb>();
                    Connection.Dispose();
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Close Connection in Database
        public void Dispose()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    Connection.Dispose();
                    Connection.Close();
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ClearAll()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    Connection.DeleteAll<DataTables.LoginTb>();
                    Connection.DeleteAll<DataTables.SettingsTb>(); 
                    Connection.DeleteAll<DataTables.LibraryItemTb>(); 
                    Connection.DeleteAll<DataTables.InfoUsersTb>(); 
                    Connection.DeleteAll<DataTables.GenresTb>(); 
                    Connection.DeleteAll<DataTables.PriceTb>(); 
                    Connection.DeleteAll<DataTables.SharedTb>(); 
                    Connection.DeleteAll<DataTables.LatestDownloadsTb>(); 
                    Connection.DeleteAll<DataTables.PublicRecentlyPlayedTb>(); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Delete table
        public void DropAll()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    Connection.DropTable<DataTables.LoginTb>();
                    Connection.DropTable<DataTables.SettingsTb>();
                    Connection.DropTable<DataTables.LibraryItemTb>();
                    Connection.DropTable<DataTables.InfoUsersTb>();
                    Connection.DropTable<DataTables.GenresTb>();
                    Connection.DropTable<DataTables.PriceTb>();
                    Connection.DropTable<DataTables.SharedTb>();
                    Connection.DropTable<DataTables.LatestDownloadsTb>(); 
                    Connection.DropTable<DataTables.PublicRecentlyPlayedTb>(); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion Connection
         
        //############# CONNECTION #############

        //########################## End SQLite_Entity ##########################

        //Start SQL_Commander >>  General
        //*********************************************************

        #region General

        public void InsertRow(object row)
        {
            try
            {
                using (OpenConnection())
                {
                    Connection?.Insert(row);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void UpdateRow(object row)
        {
            try
            {
                using (OpenConnection())
                {
                    Connection?.Update(row);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DeleteRow(object row)
        {
            try
            {
                using (OpenConnection())
                {
                    Connection?.Delete(row);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InsertListOfRows(List<object> row)
        {
            try
            {
                using (OpenConnection())
                {
                    Connection?.InsertAll(row);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion General

        //Start SQL_Commander >>  Custom
        //*********************************************************

        #region Login

        //Insert Or Update data Login
        public void InsertOrUpdateLogin_Credentials(DataTables.LoginTb db)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    var dataUser = Connection.Table<DataTables.LoginTb>().FirstOrDefault();
                    if (dataUser != null)
                    {
                        dataUser.UserId = UserDetails.UserId.ToString();
                        dataUser.AccessToken = UserDetails.AccessToken;
                        dataUser.Cookie = UserDetails.Cookie;
                        dataUser.Username = UserDetails.Username;
                        dataUser.Password = UserDetails.Password;
                        dataUser.Status = UserDetails.Status;
                        dataUser.Lang = AppSettings.Lang;
                        dataUser.DeviceId = UserDetails.DeviceId;
                        dataUser.Email = UserDetails.Email;
                         
                        Connection.Update(dataUser);
                    }
                    else
                    {
                        Connection.Insert(db);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get data Login
        public DataTables.LoginTb Get_data_Login_Credentials()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return null;

                    var dataUser = Connection.Table<DataTables.LoginTb>().FirstOrDefault();
                    if (dataUser != null)
                    {
                        UserDetails.Username = dataUser.Username;
                        UserDetails.FullName = dataUser.Username;
                        UserDetails.Password = dataUser.Password;
                        UserDetails.AccessToken = dataUser.AccessToken;
                        UserDetails.UserId = Convert.ToInt32(dataUser.UserId);
                        UserDetails.Status = dataUser.Status;
                        UserDetails.Cookie = dataUser.Cookie;
                        UserDetails.Email = dataUser.Email;
                        UserDetails.DeviceId = dataUser.DeviceId;
                        AppSettings.Lang = dataUser.Lang;

                        Current.AccessToken = dataUser.AccessToken;

                        ListUtils.DataUserLoginList.Clear();
                        ListUtils.DataUserLoginList.Add(dataUser);

                        return dataUser;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
          
        #endregion
         
        #region Settings

        public void InsertOrUpdateSettings(OptionsObject.Data settingsData)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    if (settingsData != null)
                    {
                        var select = Connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                        if (select == null)
                        {
                            var db = Mapper.Map<DataTables.SettingsTb>(settingsData);
                            Connection.Insert(db);
                        }
                        else
                        {
                            var db = Mapper.Map<DataTables.SettingsTb>(settingsData);
                            Connection.Update(db);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get Settings
        public void GetSettings()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    var select = Connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                    if (select != null)
                    {
                        var db = Mapper.Map<OptionsObject.Data>(select);
                        if (db != null)
                        {
                            ListUtils.SettingsSiteList.Clear();
                            ListUtils.SettingsSiteList.Add(db);
                        } 
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        #region Library Item

        //Insert data LibraryItem
        public void InsertLibraryItem(Classes.LibraryItem libraryItem)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    if (libraryItem == null) return;

                    var select = Connection.Table<DataTables.LibraryItemTb>().FirstOrDefault(a => a.SectionId == libraryItem.SectionId);
                    if (select != null)
                    {
                        select.SongsCount = libraryItem.SongsCount;
                        select.BackgroundImage = libraryItem.BackgroundImage;
                        Connection.Update(select);
                    }
                    else
                    {
                        DataTables.LibraryItemTb item = new DataTables.LibraryItemTb
                        {
                            SectionId = libraryItem.SectionId,
                            SectionText = libraryItem.SectionText,
                            SongsCount = libraryItem.SongsCount,
                            BackgroundImage = libraryItem.BackgroundImage
                        };
                        Connection.Insert(item);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Insert data LibraryItem
        public void InsertLibraryItem(ObservableCollection<Classes.LibraryItem> libraryList)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    if (libraryList?.Count == 0)
                        return;

                    if (libraryList != null)
                    {
                        foreach (var libraryItem in libraryList)
                        {
                            var select = Connection.Table<DataTables.LibraryItemTb>().FirstOrDefault(a => a.SectionId == libraryItem.SectionId);
                            if (select != null)
                            {
                                select.SectionId = libraryItem.SectionId;
                                select.SectionText = libraryItem.SectionText;
                                select.SongsCount = libraryItem.SongsCount;
                                select.BackgroundImage = libraryItem.BackgroundImage;

                                Connection.Update(select);
                            }
                            else
                            {
                                DataTables.LibraryItemTb item = new DataTables.LibraryItemTb
                                {
                                    SectionId = libraryItem.SectionId,
                                    SectionText = libraryItem.SectionText,
                                    SongsCount = libraryItem.SongsCount,
                                    BackgroundImage = libraryItem.BackgroundImage
                                };
                                Connection.Insert(item);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get data LibraryItem
        public ObservableCollection<DataTables.LibraryItemTb> Get_LibraryItem()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return new ObservableCollection<DataTables.LibraryItemTb>();

                    var select = Connection.Table<DataTables.LibraryItemTb>().OrderBy(a => a.SectionId).ToList();
                    if (select.Count > 0)
                    {
                        return new ObservableCollection<DataTables.LibraryItemTb>(select);
                    }
                    else
                    {
                        return new ObservableCollection<DataTables.LibraryItemTb>();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<DataTables.LibraryItemTb>();
            }
        }

        #endregion
         
        #region Genres

        //Insert data Genres
        public void InsertOrUpdate_Genres(ObservableCollection<GenresObject.DataGenres> listData)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;
                     
                    var result = Connection.Table<DataTables.GenresTb>().ToList();
                    var list = listData.Select(cat => new DataTables.GenresTb()
                    {
                        Id = cat.Id,
                        CateogryName = cat.CateogryName,
                        Tracks = Convert.ToInt32(cat.Tracks),
                        Color = cat.Color,
                        BackgroundThumb = cat.BackgroundThumb,
                        Time = Convert.ToInt32(cat.Time),
                    }).ToList();
                      
                    if (list.Count <= 0) return;
                      
                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (newItemList.Count > 0)
                        Connection.InsertAll(newItemList);

                    Connection.UpdateAll(list);
                     
                    result = Connection.Table<DataTables.GenresTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            Connection.Delete(delete);

                    Connection.Commit(); 
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get List Genres
        public ObservableCollection<GenresObject.DataGenres> Get_GenresList()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return new ObservableCollection<GenresObject.DataGenres>();

                    var result = Connection.Table<DataTables.GenresTb>().ToList();
                    if (result?.Count > 0)
                    {
                        var list = result.Select(cat => new GenresObject.DataGenres()
                        {
                            Id = cat.Id,
                            CateogryName = cat.CateogryName,
                            Tracks = cat.Tracks,
                            Color = cat.Color,
                            BackgroundThumb = cat.BackgroundThumb,
                            Time = cat.Time,
                        }).ToList();
                          
                        return new ObservableCollection<GenresObject.DataGenres>(list);
                    }
                    else
                    {
                        return new ObservableCollection<GenresObject.DataGenres>();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<GenresObject.DataGenres>();
            }
        }

        #endregion
         
        #region My Profile

        //Insert Or Update data My Profile Table
        public void InsertOrUpdate_DataMyInfo(UserDataObject info)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    var resultInfoTb = Connection.Table<DataTables.InfoUsersTb>().FirstOrDefault(a => a.Id == info.Id);
                    if (resultInfoTb != null)
                    {
                        var db = Mapper.Map<DataTables.InfoUsersTb>(info); 
                        Connection.Update(db);
                    }
                    else
                    {
                        var db = Mapper.Map<DataTables.InfoUsersTb>(info); 
                        Connection.Insert(db);
                    }

                    UserDetails.Avatar = info.Avatar;
                    UserDetails.Cover = info.Cover;
                    UserDetails.Username = info.Username;
                    UserDetails.FullName = info.Name;
                    UserDetails.Email = info.Email;

                    ListUtils.MyUserInfoList = new ObservableCollection<UserDataObject>();
                    ListUtils.MyUserInfoList.Clear();
                    ListUtils.MyUserInfoList.Add(info);
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Get data To My Profile Table
        public void GetDataMyInfo()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    var listData = new ObservableCollection<UserDataObject>();
                    var user = Connection.Table<DataTables.InfoUsersTb>().FirstOrDefault();
                    if (user != null)
                    {
                        UserDataObject data = new UserDataObject()
                        {
                            Id = user.Id,
                            Username = user.Username,
                            Email = user.Email,
                            IpAddress = user.IpAddress,
                            Name = user.Name,
                            Gender = user.Gender,
                            EmailCode = user.EmailCode,
                            Language = user.Language,
                            Avatar = user.Avatar,
                            Cover = user.Cover,
                            Src = user.Src,
                            CountryId = user.CountryId,
                            Age = user.Age,
                            About = user.About,
                            Google = user.Google,
                            Facebook = user.Facebook,
                            Twitter = user.Twitter,
                            Instagram = user.Instagram,
                            Website = user.Website,
                            Active = user.Active,
                            Admin = user.Admin,
                            Verified = user.Verified,
                            LastActive = user.LastActive,
                            Registered = user.Registered,
                            Uploads = user.Uploads,
                            Wallet = user.Wallet,
                            Balance = user.Balance,
                            Artist = user.Artist,
                            IsPro = user.IsPro,
                            ProTime = user.ProTime,
                            LastFollowId = user.LastFollowId,
                            OrAvatar = user.OrAvatar,
                            OrCover = user.OrCover,
                            Url = user.Url,
                            AboutDecoded = user.AboutDecoded,
                            NameV = user.NameV,
                            CountryName = user.CountryName,
                            GenderText = user.GenderText,  
                        };
                         
                        listData.Add(data);

                        UserDetails.Avatar = user.Avatar;
                        UserDetails.Cover = user.Cover;
                        UserDetails.Username = user.Username;
                        UserDetails.FullName = user.Name;
                        UserDetails.Email = user.Email;

                        ListUtils.MyUserInfoList = new ObservableCollection<UserDataObject>();
                        ListUtils.MyUserInfoList.Clear();
                        ListUtils.MyUserInfoList = listData;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Price

        //Insert data Price
        public void InsertOrUpdate_Price(ObservableCollection<PricesObject.DataPrice> listData)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    var result = Connection.Table<DataTables.PriceTb>().ToList();
                    var list = listData.Select(item => new DataTables.PriceTb()
                    {
                        Id = item.Id,
                        Price = item.Price
                    }).ToList();

                    if (list.Count <= 0) return;

                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (newItemList.Count > 0)
                        Connection.InsertAll(newItemList);

                    Connection.UpdateAll(list);

                    result = Connection.Table<DataTables.PriceTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            Connection.Delete(delete);

                    Connection.Commit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get List Price
        public ObservableCollection<PricesObject.DataPrice> Get_PriceList()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return new ObservableCollection<PricesObject.DataPrice> ();

                    var result = Connection.Table<DataTables.PriceTb>().ToList();
                    if (result?.Count > 0)
                    {
                        var list = result.Select(item => new PricesObject.DataPrice()
                        {
                            Id = item.Id,
                            Price = item.Price,
                        }).ToList();

                        return new ObservableCollection<PricesObject.DataPrice>(list);
                    }
                    else
                    {
                        return new ObservableCollection<PricesObject.DataPrice>();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<PricesObject.DataPrice>();
            }
        }

        #endregion
         
        #region Shared Sound

        //Insert Or Update Shared Sound
        public void InsertOrUpdate_SharedSound(SoundDataObject info)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    if (info != null)
                    {
                        var select = Connection.Table<DataTables.SharedTb>().FirstOrDefault(a => a.Id == info.Id);
                        if (select != null)
                        {
                            var db = Mapper.Map<DataTables.SharedTb>(info);
                            db.Publisher = JsonConvert.SerializeObject(info.Publisher);
                            db.TagsArray = JsonConvert.SerializeObject(info.TagsArray);
                            db.TagsFiltered = JsonConvert.SerializeObject(info.TagsFiltered);
                            db.SongArray = JsonConvert.SerializeObject(info.SongArray);
                            db.Comments = JsonConvert.SerializeObject(info.Comments);
                            Connection.Update(db);
                        }
                        else
                        {
                            var db = Mapper.Map<DataTables.SharedTb>(info);
                            db.Publisher = JsonConvert.SerializeObject(info.Publisher);
                            db.TagsArray = JsonConvert.SerializeObject(info.TagsArray);
                            db.TagsFiltered = JsonConvert.SerializeObject(info.TagsFiltered);
                            db.SongArray = JsonConvert.SerializeObject(info.SongArray);
                            db.Comments = JsonConvert.SerializeObject(info.Comments);
                            Connection.Insert(db);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get Shared Sound
        public ObservableCollection<SoundDataObject> Get_SharedSound()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return new ObservableCollection<SoundDataObject>();

                    var select = Connection.Table<DataTables.SharedTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<SoundDataObject>();
                        foreach (var item in select)
                        {
                            var db = Mapper.Map<SoundDataObject>(item);

                            if (!string.IsNullOrEmpty(item.Publisher))
                                db.Publisher = JsonConvert.DeserializeObject<UserDataObject>(item.Publisher);

                            if (!string.IsNullOrEmpty(item.TagsArray)) 
                                db.TagsArray = JsonConvert.DeserializeObject<List<string>>(item.TagsArray);

                            if (!string.IsNullOrEmpty(item.TagsFiltered)) 
                                db.TagsFiltered = JsonConvert.DeserializeObject<List<string>>(item.TagsFiltered);

                            if (!string.IsNullOrEmpty(item.SongArray)) 
                                db.SongArray = JsonConvert.DeserializeObject<SongArray>(item.SongArray);

                            if (!string.IsNullOrEmpty(item.TagsFiltered))
                                db.Comments = JsonConvert.DeserializeObject<List<CommentsDataObject>>(item.Comments);
                            
                            list.Add(db);
                        }

                        return list;
                    }
                    else
                    {
                        return new ObservableCollection<SoundDataObject>();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<SoundDataObject>();
            }
        }

        #endregion

        #region LatestDownloads Sound

        //Insert Or Update Latest Downloads Sound
        public void InsertOrUpdate_LatestDownloadsSound(SoundDataObject info)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    if (info != null)
                    {
                        var select = Connection.Table<DataTables.LatestDownloadsTb>().FirstOrDefault(a => a.Id == info.Id);
                        if (select != null)
                        {
                            var db = Mapper.Map<DataTables.LatestDownloadsTb>(info);
                            db.Publisher = JsonConvert.SerializeObject(info.Publisher);
                            db.TagsArray = JsonConvert.SerializeObject(info.TagsArray);
                            db.TagsFiltered = JsonConvert.SerializeObject(info.TagsFiltered);
                            db.SongArray = JsonConvert.SerializeObject(info.SongArray);
                            db.Comments = JsonConvert.SerializeObject(info.Comments);
                            Connection.Update(db);
                        }
                        else
                        {
                            var db = Mapper.Map<DataTables.LatestDownloadsTb>(info);
                            db.Publisher = JsonConvert.SerializeObject(info.Publisher);
                            db.TagsArray = JsonConvert.SerializeObject(info.TagsArray);
                            db.TagsFiltered = JsonConvert.SerializeObject(info.TagsFiltered);
                            db.SongArray = JsonConvert.SerializeObject(info.SongArray);
                            db.Comments = JsonConvert.SerializeObject(info.Comments);
                            Connection.Insert(db);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        public void InsertOrUpdate_LatestDownloadsSound(int soundId, string soundPath)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    var select = Connection.Table<DataTables.LatestDownloadsTb>().FirstOrDefault(a => a.Id == soundId);
                    if (select != null)
                    {
                        //select.SoundName = soundId + ".mp3";
                        select.AudioLocation = soundPath;
                        Connection.Update(select);
                    } 
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception); 
            }
        }
         
        //Get LatestDownloads Sound
        public ObservableCollection<SoundDataObject> Get_LatestDownloadsSound()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return new ObservableCollection<SoundDataObject>(); 

                    var select = Connection.Table<DataTables.LatestDownloadsTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<SoundDataObject>();
                        foreach (var item in select)
                        {
                            var db = Mapper.Map<SoundDataObject>(item);

                            if (!string.IsNullOrEmpty(item.Publisher))
                                db.Publisher = JsonConvert.DeserializeObject<UserDataObject>(item.Publisher);

                            if (!string.IsNullOrEmpty(item.TagsArray))
                                db.TagsArray = JsonConvert.DeserializeObject<List<string>>(item.TagsArray);

                            if (!string.IsNullOrEmpty(item.TagsFiltered))
                                db.TagsFiltered = JsonConvert.DeserializeObject<List<string>>(item.TagsFiltered);

                            if (!string.IsNullOrEmpty(item.SongArray))
                                db.SongArray = JsonConvert.DeserializeObject<SongArray>(item.SongArray);

                            if (!string.IsNullOrEmpty(item.TagsFiltered))
                                db.Comments = JsonConvert.DeserializeObject<List<CommentsDataObject>>(item.Comments);

                            list.Add(db);
                        }

                        return list;
                    }
                    else
                    {
                        return new ObservableCollection<SoundDataObject>();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<SoundDataObject>();
            }
        }

        //Remove LatestDownloads Sound
        public void Remove_LatestDownloadsSound(int soundId)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    var select = Connection.Table<DataTables.LatestDownloadsTb>().FirstOrDefault(a => a.Id == soundId);
                    if (select != null)
                    {
                        Connection.Delete(select);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public SoundDataObject Get_LatestDownloadsSound(int soundId)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return null;

                    var select = Connection.Table<DataTables.LatestDownloadsTb>().FirstOrDefault(a => a.Id == soundId);
                    if (select != null)
                    {
                        var db = Mapper.Map<SoundDataObject>(select);

                        if (!string.IsNullOrEmpty(select.Publisher))
                            db.Publisher = JsonConvert.DeserializeObject<UserDataObject>(select.Publisher);

                        if (!string.IsNullOrEmpty(select.TagsArray))
                            db.TagsArray = JsonConvert.DeserializeObject<List<string>>(select.TagsArray);

                        if (!string.IsNullOrEmpty(select.TagsFiltered))
                            db.TagsFiltered = JsonConvert.DeserializeObject<List<string>>(select.TagsFiltered);

                        if (!string.IsNullOrEmpty(select.SongArray))
                            db.SongArray = JsonConvert.DeserializeObject<SongArray>(select.SongArray);

                        if (!string.IsNullOrEmpty(select.TagsFiltered))
                            db.Comments = JsonConvert.DeserializeObject<List<CommentsDataObject>>(select.Comments);

                        return db;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        #endregion

        #region Public Recently Played Sound

        //Insert Or Update Shared Sound
        public void InsertOrUpdate_PublicRecentlyPlayedSound(SoundDataObject info)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    if (info != null)
                    {
                        var select = Connection.Table<DataTables.PublicRecentlyPlayedTb>().FirstOrDefault(a => a.Id == info.Id);
                        if (select != null)
                        {
                            var db = Mapper.Map<DataTables.PublicRecentlyPlayedTb>(info);
                            db.Publisher = JsonConvert.SerializeObject(info.Publisher);
                            db.TagsArray = JsonConvert.SerializeObject(info.TagsArray);
                            db.TagsFiltered = JsonConvert.SerializeObject(info.TagsFiltered);
                            db.SongArray = JsonConvert.SerializeObject(info.SongArray);
                            db.Comments = JsonConvert.SerializeObject(info.Comments);
                            Connection.Update(db);
                        }
                        else
                        {
                            var db = Mapper.Map<DataTables.PublicRecentlyPlayedTb>(info);
                            db.Publisher = JsonConvert.SerializeObject(info.Publisher);
                            db.TagsArray = JsonConvert.SerializeObject(info.TagsArray);
                            db.TagsFiltered = JsonConvert.SerializeObject(info.TagsFiltered);
                            db.SongArray = JsonConvert.SerializeObject(info.SongArray);
                            db.Comments = JsonConvert.SerializeObject(info.Comments);
                            Connection.Insert(db);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Insert PublicRecentlyPlayed Sound List
        public void InsertOrReplace_PublicRecentlyPlayedSoundList(ObservableCollection<SoundDataObject> soundList)
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return;

                    var result = Connection.Table<DataTables.PublicRecentlyPlayedTb>().ToList();
                    List<DataTables.PublicRecentlyPlayedTb> list = new List<DataTables.PublicRecentlyPlayedTb>();
                    foreach (var info in soundList)
                    {
                        var db = Mapper.Map<DataTables.PublicRecentlyPlayedTb>(info);
                        db.Publisher = JsonConvert.SerializeObject(info.Publisher);
                        db.TagsArray = JsonConvert.SerializeObject(info.TagsArray);
                        db.TagsFiltered = JsonConvert.SerializeObject(info.TagsFiltered);
                        db.SongArray = JsonConvert.SerializeObject(info.SongArray);
                        db.Comments = JsonConvert.SerializeObject(info.Comments);
                        list.Add(db);
                    }

                    if (list.Count <= 0) return;

                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (newItemList.Count > 0)
                        Connection.InsertAll(newItemList);

                    Connection.UpdateAll(list);

                    result = Connection.Table<DataTables.PublicRecentlyPlayedTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            Connection.Delete(delete);

                    Connection.Commit(); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        //Get PublicRecentlyPlayed Sound
        public ObservableCollection<SoundDataObject> Get_PublicRecentlyPlayedSound()
        {
            try
            {
                using (OpenConnection())
                {
                    if (Connection == null) return new ObservableCollection<SoundDataObject>();

                    var select = Connection.Table<DataTables.PublicRecentlyPlayedTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<SoundDataObject>();
                        foreach (var item in select)
                        {
                            var db = Mapper.Map<SoundDataObject>(item);

                            if (!string.IsNullOrEmpty(item.Publisher))
                                db.Publisher = JsonConvert.DeserializeObject<UserDataObject>(item.Publisher);

                            if (!string.IsNullOrEmpty(item.TagsArray))
                                db.TagsArray = JsonConvert.DeserializeObject<List<string>>(item.TagsArray);

                            if (!string.IsNullOrEmpty(item.TagsFiltered))
                                db.TagsFiltered = JsonConvert.DeserializeObject<List<string>>(item.TagsFiltered);

                            if (!string.IsNullOrEmpty(item.SongArray))
                                db.SongArray = JsonConvert.DeserializeObject<SongArray>(item.SongArray);

                            if (!string.IsNullOrEmpty(item.TagsFiltered))
                                db.Comments = JsonConvert.DeserializeObject<List<CommentsDataObject>>(item.Comments);

                            list.Add(db);
                        }

                        return list;
                    }
                    else
                    {
                        return new ObservableCollection<SoundDataObject>();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<SoundDataObject>();
            }
        }

        #endregion
         
    }
}