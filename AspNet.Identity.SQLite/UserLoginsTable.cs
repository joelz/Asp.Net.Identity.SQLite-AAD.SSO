﻿using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace AspNet.Identity.SQLite
{
    /// <summary>
    /// Class that represents the AspNetUserLogins table in the SQLite Database
    /// </summary>
    public class UserLoginsTable
    {
        private SQLiteDatabase _database;

        /// <summary>
        /// Constructor that takes a SQLiteDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public UserLoginsTable(SQLiteDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Deletes a login from a user in the AspNetUserLogins table
        /// </summary>
        /// <param name="user">User to have login deleted</param>
        /// <param name="login">Login to be deleted from user</param>
        /// <returns></returns>
        public int Delete(IdentityUser user, UserLoginInfo login)
        {
            string commandText = "Delete from AspNetUserLogins where UserId = @userId and LoginProvider = @loginProvider and ProviderKey = @providerKey";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserId", user.Id);
            parameters.Add("loginProvider", login.LoginProvider);
            parameters.Add("providerKey", login.ProviderKey);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes all Logins from a user in the AspNetUserLogins table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "Delete from AspNetUserLogins where UserId = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new login in the AspNetUserLogins table
        /// </summary>
        /// <param name="user">User to have new login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public int Insert(IdentityUser user, UserLoginInfo login)
        {
            string commandText = "Insert into AspNetUserLogins (LoginProvider, ProviderKey, UserId) values (@loginProvider, @providerKey, @userId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("loginProvider", login.LoginProvider);
            parameters.Add("providerKey", login.ProviderKey);
            parameters.Add("userId", user.Id);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Return a userId given a user's login
        /// </summary>
        /// <param name="userLogin">The user's login info</param>
        /// <returns></returns>
        public string FindUserIdByLogin(UserLoginInfo userLogin)
        {
            string commandText = "Select UserId from AspNetUserLogins where LoginProvider = @loginProvider and ProviderKey = @providerKey";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("loginProvider", userLogin.LoginProvider);
            parameters.Add("providerKey", userLogin.ProviderKey);

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns a list of user's logins
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public List<UserLoginInfo> FindByUserId(string userId)
        {
            List<UserLoginInfo> logins = new List<UserLoginInfo>();
            string commandText = "Select * from AspNetUserLogins where UserId = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@userId", userId } };

            var rows = _database.Query(commandText, parameters);
            foreach (var row in rows)
            {
                var login = new UserLoginInfo(row["LoginProvider"], row["ProviderKey"]);
                logins.Add(login);
            }

            return logins;
        }
    }
}
