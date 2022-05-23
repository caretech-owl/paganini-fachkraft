using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class InternalApiServer
{
    private static readonly InternalApiServer instance = new InternalApiServer();

    public static InternalApiServer GetInternalApiServer()
    {
        return instance;
    }

  

    private InternalApiServer()
    {
        //myServer = new SimpleHTTPServer({ your path }, { your port}, { your controller}, { your bufferSize});
    }
}
