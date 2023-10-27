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
        }
    #endregion
}
