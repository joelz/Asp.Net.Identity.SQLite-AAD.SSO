﻿using System;
using System.Collections.Generic;

namespace AspNet.Identity.SQLite
{
    /// <summary>
    /// Class that represents the Role table in the SQLite Database
    /// </summary>
    public class RoleTable<TRole> where TRole : IdentityRole
    {
        private SQLiteDatabase _database;

        /// <summary>
        /// Constructor that takes a SQLiteDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public RoleTable(SQLiteDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Deltes a role from the AspNetRoles table
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns></returns>
        public int Delete(string roleId)
        {
            string commandText = "Delete from AspNetRoles where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", roleId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new Role in the AspNetRoles table
        /// </summary>
        /// <param name="roleName">The role's name</param>
        /// <returns></returns>
        public int Insert(IdentityRole role)
        {
            string commandText = "Insert into AspNetRoles (Id, Name) values (@id, @name)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", role.Name);
            parameters.Add("@id", role.Id);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns a role name given the roleId
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns>Role name</returns>
        public string GetRoleName(string roleId)
        {
            string commandText = "Select Name from AspNetRoles where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", roleId);

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns the role Id given a role name
        /// </summary>
        /// <param name="roleName">Role's name</param>
        /// <returns>Role's Id</returns>
        public string GetRoleId(string roleName)
        {
            string roleId = null;
            string commandText = "Select Id from AspNetRoles where Name = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", roleName } };

            var result = _database.QueryValue(commandText, parameters);
            if (result != null)
            {
                return Convert.ToString(result);
            }

            return roleId;
        }

        /// <summary>
        /// Gets the IdentityRole given the role Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IdentityRole GetRoleById(string roleId)
        {
            var roleName = GetRoleName(roleId);
            IdentityRole role = null;

            if(roleName != null)
            {
                role = new IdentityRole(roleName, roleId);
            }

            return role;

        }

        public List<TRole> GetRoles()
        {
            List<TRole> rolelist = new List<TRole>();
            string commandText = "Select * from AspNetRoles";
            var rows = _database.Query(commandText, null);
            foreach (var row in rows)
            {
                TRole role = null;
                role = (TRole)Activator.CreateInstance(typeof(TRole));
                role.Id = row["Id"];
                role.Name = row["Name"];
                
                rolelist.Add(role);
            }
            return rolelist;
        }

        /// <summary>
        /// Gets the IdentityRole given the role name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IdentityRole GetRoleByName(string roleName)
        {
            var roleId = GetRoleId(roleName);
            IdentityRole role = null;

            if (roleId != null)
            {
                role = new IdentityRole(roleName, roleId);
            }

            return role;
        }

        public int Update(IdentityRole role)
        {
            string commandText = "Update AspNetRoles set Name = @name where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", role.Id);

            return _database.Execute(commandText, parameters);
        }
    }
}
