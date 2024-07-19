using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using SQLite4Unity3d;
using System.Data;
using System.IO;

public class DBConnector : PersistentLazySingleton<DBConnector>
{
    private SQLiteConnection connection;
    public string databaseName = "fachkraft.db";


    #region [Databaseoperations]

    public void Startup()
    {
        if (this.connection == null)
        {
            string dbPath = Application.persistentDataPath + "/" + databaseName;
            ConnectToDatabase(dbPath);
            CreateTables();
            Debug.Log("DB Startup Completed. ");
        }
    }

    public SQLiteConnection GetConnection()
    {
        return this.connection;
    }

    public void TruncateTable<t>()
    {
        this.connection.DeleteAll<t>();
    }

    private void ConnectToDatabase(string dbPath)
    {
        Debug.Log("ConnectToDatabase :" + dbPath);
        this.connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
       
    }

    private void CreateTables()
    {
        this.connection.CreateTable<User>();
        this.connection.CreateTable<Pathpoint>();
        this.connection.CreateTable<AuthToken>();
        this.connection.CreateTable<Route>();
        this.connection.CreateTable<Way>();
        this.connection.CreateTable<Address>();
        this.connection.CreateTable<PathpointPhoto>();
        this.connection.CreateTable<PhotoData>();

        this.connection.CreateTable<RouteWalk>();
        this.connection.CreateTable<RouteWalkEventLog>();
        this.connection.CreateTable<PathpointLog>();
        this.connection.CreateTable<PathpointPIM>();
    }

    public void TruncateTables()
    {
        this.connection.DeleteAll<User>();
        this.connection.DeleteAll<AuthToken>();
        this.connection.DeleteAll<Pathpoint>();
        this.connection.DeleteAll<Route>();
        this.connection.DeleteAll<Way>();
        this.connection.DeleteAll<Address>();
        this.connection.DeleteAll<PathpointPhoto>();
        this.connection.DeleteAll<PhotoData>();

        this.connection.DeleteAll<RouteWalk>();
        this.connection.DeleteAll<RouteWalkEventLog>();
        this.connection.DeleteAll<PathpointLog>();
        this.connection.DeleteAll<PathpointPIM>();
    }

    public void DropTables()
    {
        try
        {
            this.connection.DropTable<User>();
        }
        catch { }
        try
        {
            this.connection.DropTable<AuthToken>();
        }
        catch { }
        try
        {
            this.connection.DropTable<Pathpoint>();
        }
        catch { }
        try
        {
            this.connection.DropTable<Route>();
        }
        catch { }
        try
        {
            this.connection.DropTable<Way>();
        }
        catch { }
        try
        {
            this.connection.DropTable<Address>();
        }
        catch { }
        try
        {
            this.connection.DropTable<PathpointPhoto>();
        }
        catch { }
        try
        {
            this.connection.DropTable<PhotoData>();
        }
        catch { }
        try
        {
            this.connection.DropTable<RouteWalk>();
        }
        catch { }
        try
        {
            this.connection.DropTable<RouteWalkEventLog>();
        }
        catch { }
        try
        {
            this.connection.DropTable<PathpointLog>();
        }
        catch { }
        try
        {
            this.connection.DropTable<PathpointPIM>();
        }
        catch { }
    }

    #endregion
}
