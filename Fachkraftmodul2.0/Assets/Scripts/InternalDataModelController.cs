using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

class InternalDataModelController
{
    private static readonly InternalDataModelController instance = new InternalDataModelController();

    private string dataModelXml = Application.streamingAssetsPath + "/dataModel.xml";
    private InternalDataModel _idm;
    // Note: constructor is 'private'
    private InternalDataModelController()
    {
        _idm = new InternalDataModel();
        _idm.isLoggedIn = true;

        SerializeDataModel();
    }

    public static InternalDataModelController GetInternalDataModelController()
    {
        return instance;
    }

    public InternalDataModel idm
    {
        get { return _idm; }
        set { _idm = value; }
    }

    public void SerializeDataModel()
    {
        var objType = idm.GetType();

        try
        {
            using (var xmlWriter = new XmlTextWriter(dataModelXml, Encoding.Default))
            {
                xmlWriter.Indentation = 2;
                xmlWriter.IndentChar = ' ';
                xmlWriter.Formatting = Formatting.Indented;
                var xmlSerializer = new XmlSerializer(objType);
                xmlSerializer.Serialize(xmlWriter, idm);
            }
        }
        catch (IOException)
        {

        }
    }

    public void DeserializeDataModel()
    {
        if (File.Exists(dataModelXml))
        {
            using (var xmlReader = new XmlTextReader(dataModelXml))
            {
                var xmlSerializer = new XmlSerializer(typeof(InternalDataModel));
                idm = (InternalDataModel)xmlSerializer.Deserialize(xmlReader);
            }
        }
    }
}

