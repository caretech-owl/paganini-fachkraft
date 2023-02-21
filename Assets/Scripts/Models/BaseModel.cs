using System;
using System.Collections.Generic;
using System.Linq;

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
        IsDirty = false;

        var conn = DBConnector.Instance.GetConnection();
        conn.InsertOrReplace(this);        
    }

    public static IEnumerator<T> GetAll<T>() where T : BaseModel, new()
    {
        var conn = DBConnector.Instance.GetConnection();
        return conn.Table<T>().GetEnumerator();
    }
    
    public static T Get<T>(object pk) where T : BaseModel, new()
    {
        var conn = DBConnector.Instance.GetConnection();
        return conn.Get<T>(pk); 
    }

    public static void Delete<T>(T obj) 
    {
        var conn = DBConnector.Instance.GetConnection();
        conn.Delete<T>(obj);
    }

    public static void DeleteAll<T>() where T : BaseModel, new()
    {
        var conn = DBConnector.Instance.GetConnection();
        conn.DeleteAll<T>();
    }

    public static void DeleteNonDirtyCopies<T>() where T : BaseModel, new()
    {
        var conn = DBConnector.Instance.GetConnection();

        string query = String.Format("DELETE from {0} where IsDirty = false",
                                      conn.Table<T>().Table.TableName);      

        conn.Execute(query);
    }


    public static List<TModel> ToModelList<TAPI, TModel>(IEnumerable<TAPI> collection, Func<TAPI, TModel> ctor)
    {
        return collection.Select(api => ctor(api)).ToList();
    }

}
