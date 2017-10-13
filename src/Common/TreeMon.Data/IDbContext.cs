// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;

/// <summary>
/// A DbContext instance represents a combination of the Unit Of Work and Repository patterns such that
/// it can be used to query from a database and group together changes that will then be written
/// back to the store as a unit.
/// DbContext is conceptually similar to ObjectContext.
/// </summary>
namespace TreeMon.Data
{
    public interface IDbContext
    {
        string ConnectionKey { get; set; }

        string Message { get; set; }

        Database Database { get; }

        #region CRUD
        IEnumerable<T> GetAll<T>() where T : class;

        string GetTableName<T>() where T : class;

        IEnumerable<T> Select<T>(string sql, object parameters) where T : class;
      
        T Get<T>(int id) where T : class;

        int SaveChanges();

   
        int Delete<T>(T entity) where T : class;

        int Delete<T>(string whereStatement, object parameters) where T : class;

        bool Insert<T>(T entity) where T : class;

        int Update<T>(T entity) where T : class;

        int ExecuteNonQuery(string sql, object parameters);

        object ExecuteScalar(string sql, object parameters);

        #endregion

        #region CRUD Async 
        IDataReader Execute(string sql, object parameters);

        Task<T> GetAsync<T>(int id) where T : class;

        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;

        Task<IEnumerable<T>> SelectAsync<T>(string sql, object parameters) where T : class;

       Task<int> DeleteAsync<T>(T entity) where T : class;

        Task<int> ExecuteNonQueryAsync(string sql, object parameters);

        Task<int> InsertAsync<T>(T entity) where T : class;

        Task<int> UpdateAsync<T>(T entity) where T : class;

        Task<int> SaveAsync();

        Task<object> ExecuteScalarAsync(string sql, object parameters);

        #endregion

        List<string> GetTableNames();

        string GetTableName(string typeName);

        object GetTableObject(string tableName);

        void LoadTableNames();

    }
}
