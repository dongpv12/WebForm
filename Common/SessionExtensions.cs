using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using WebForm.Models;

namespace WebForm
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            try
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
            catch (Exception ex)
            {
                WebForm.Common.Logger.Log.Error(ex.ToString());
            }
        }

        public static void Remove_Session_ByKey(this ISession session, string key)
        {
            try
            {
                session.Remove(key);
            }
            catch (Exception ex)
            {
                WebForm.Common.Logger.Log.Error(ex.ToString());
            }
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            try
            {
                var value = session.GetString(key);
                return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                WebForm.Common.Logger.Log.Error(ex.ToString());
            }
            return default(T);
        }

        public static User CurrentUser(this HttpContext context)
        {
            User user = new User();
            try
            {
                user = context.Session.GetObjectFromJson<User>("user");

                if (user != null)
                {
                    //if (MemoryData.c_dic_Function_ByUser.ContainsKey(user.User_Id))
                    //{
                    //    user.List_User_Functions = MemoryData.c_dic_Function_ByUser[user.User_Id];
                    //}
                    //else
                    //{
                    //    user.List_User_Functions = new List<Cb_Function_Info>();
                    //}
                }

            }
            catch (Exception ex)
            {
                WebForm.Common.Logger.Log.Error(ex.ToString());
                user = new User();
            }
            return user;
        }
    }

    public class ClientInformation
    {
        public string Family { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public string Patch { get; set; }
        public string Ip { get; set; }
    }
}
