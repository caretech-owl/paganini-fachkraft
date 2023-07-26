using System;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using WebSocketSharp;


[System.Serializable]
public class PathpointTraceEvent : UnityEvent<PathpointTraceMessage>
{
}


public class SocketsAPI : MonoBehaviour
{
    private WebSocket ws;
    private Route CurrentRoute;

    [SerializeField] private string ServerURL = "ws://localhost:8080";

    public PathpointTraceEvent OnIncomingPathpointTrace;

    const string CMD_CONNECT = "connect";
    const string CMD_TRACE   = "pathpoint";

    public enum POIState { None, OnPOI, LeftPOI, OffTrack, OnTrack, Arrived, Invalid }

    private void Start()
    {

        CurrentRoute = AppState.CurrentRoute;
        ConnectToServer();

    }

    public void ConnectToServer()
    {
        try
        {
            ws = new WebSocket(ServerURL);

            ws.OnMessage += (sender, e) =>
            {
                HandleMessage(e.Data);
            };

            ws.OnOpen += (sender, e) =>
            {
                Debug.Log("Connected to server.");
                SendMessage(new
                {
                    type = CMD_CONNECT,
                    userId = -1,
                    sessionId = CurrentRoute.Id
                });
            };

            ws.ConnectAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void SendPathpointTrace(PathpointTraceMessage message)
    {
        message.type = CMD_TRACE;
        SendMessage(message);
    }

    private void SendMessage(object message)
    {
        if (ws == null || !ws.IsAlive)
        {
            Debug.Log("Not connected to server.");
            return;
        }

        try
        {
            string jsonMessage = JsonConvert.SerializeObject(message);
            ws.Send(jsonMessage);
            Debug.Log("Client sent message: " + jsonMessage);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void HandleMessage(string message)
    {
        Debug.Log("Client recieved message");

        MainThreadDispatcher.Instance.QueueOnMainThread(() =>
        {
            try
            {
                PathpointTraceMessage data = JsonConvert.DeserializeObject<PathpointTraceMessage>(message);

                if (data.type == CMD_TRACE)
                {
                    Debug.Log("Client processed message: " + data.pathpoint.ppoint_timestamp);
                    OnIncomingPathpointTrace?.Invoke(data);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }

}
