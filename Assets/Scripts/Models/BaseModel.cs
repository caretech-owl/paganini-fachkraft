using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class BaseModel
{

    public bool IsDirty { set; get; } = false;
    public bool FromAPI { set; get; } = false;

    public void MarkAsClean()
    {
        IsDirty = false;
    }

    public void Insert()
    {
        var conn = DBConnector.Instance.GetConnection();
        conn.InsertOrReplace(this);        
    }

    public static int InsertAll<T>(List<T> list) where T : BaseModel, new()
    {
        var conn = DBConnector.Instance.GetConnection();
        return conn.InsertAll(list.AsEnumerable());
    }

    public static List<T> GetAll<T>(Expression<Func<T, bool>> predicate = null) where T : BaseModel, new()
    {
        var conn = DBConnector.Instance.GetConnection();
        var query = conn.Table<T>();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query.ToList();
    }

    public static T Get<T>(object pk) where T : BaseModel, new()
    {
        var conn = DBConnector.Instance.GetConnection();
        return conn.Get<T>(pk); 
    }

    public static void Delete<T>(object objId) 
    {
        var conn = DBConnector.Instance.GetConnection();
        conn.Delete<T>(objId);
    }

    public static void DeleteAll<T>() where T : BaseModel, new()
    {
        var conn = DBConnector.Instance.GetConnection();
        conn.DeleteAll<T>();
    }

    public static void DeleteNonDirtyCopies<T>() where T : BaseModel, new()
    {
        var conn = DBConnector.Instance.GetConnection();

        string query = String.Format("DELETE from {0} where IsDirty = 0",
                                      conn.Table<T>().Table.TableName);      

        conn.Execute(query);
    }


    public static List<TModel> ToModelList<TAPI, TModel>(IEnumerable<TAPI> collection, Func<TAPI, TModel> ctor)
    {
        return collection.Select(api => ctor(api)).ToList();
    }

}
