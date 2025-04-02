using LiteDB;
using Masuit.Tools.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace MyTools.Helpers
{
    public static class LiteDBHelper
    {
        private static string litedbName = "LiteDB.db";
        private static LiteDatabase _db;

        public static void SetUpDatabase(string dbName = null)
        {
            if (dbName != null)
            {
                litedbName = dbName;
            }
        }
        /// <summary>
        /// 单例模式
        /// </summary>
        /// <returns></returns>
        public static LiteDatabase GetDataBase()
        {
            if (_db == null)
            {
                _db = new LiteDatabase(litedbName);
            }
            return _db;
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="t"></param>
        public static void Init<T>(Expression<Func<T, bool>> exp, T t)
        {
            GetDataBase();
            var data = _db.GetCollection<T>().Query().Where(exp).SingleOrDefault();
            if (data == null)
            {
                DBInsert(t);
            }
        }
        #region 增删改查
        public static void DBInsert<T>(this T t)
        {
            GetDataBase();
            _db.GetCollection<T>().Insert(t);
        }
        public static void DBInsert<T>(this ICollection<T> t)
        {
            GetDataBase();
            _db.GetCollection<T>().Insert(t);
        }
        public static void DBDelete<T>(this T t)
        {
            var propertyInfo = t.GetProperties().FirstOrDefault(x => x.Name == "Id" && x.PropertyType == typeof(ObjectId));
            if (propertyInfo == null)
            {
                throw new ArgumentException("主键Id错误或不存在ObjectId类型的Id字段");
            }
            var Id = propertyInfo.GetValue(t) as ObjectId;
            GetDataBase();
            _db.GetCollection<T>().Delete(Id);
        }
        public static void DBDelete<T>(BsonExpression predicate)
        {
            GetDataBase();
            _db.GetCollection<T>().DeleteMany(predicate);
        }
        public static void DBUpdate<T>(this T t)
        {
            GetDataBase();
            _db.GetCollection<T>().Update(t);
        }
        public static void DBUpdate<T>(this ICollection<T> t)
        {
            GetDataBase();
            _db.GetCollection<T>().Update(t);
        }
        public static T DBFindById<T>(ObjectId id)
        {
            GetDataBase();
            return _db.GetCollection<T>().FindById(id);
        }
        public static IEnumerable<T> DBFind<T>(Expression<Func<T, bool>> predicate = null)
        {
            GetDataBase();
            if (predicate == null)
            {
                return _db.GetCollection<T>().FindAll();
            }
            return _db.GetCollection<T>().Find(predicate);
        }
        #endregion
    }
    public class BaseEntity
    {
        public ObjectId Id { get; set; }
    }
}
