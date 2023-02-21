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

    private string dataModelXml = FileManagement.persistentDataPath + "/dataModelVersion1.0.xml";
    private List<InternalDataModel> _idm;
    // Note: constructor is 'private'
    private InternalDataModelController()
    {
        DeserializeDataModel();
    }

    public static InternalDataModelController GetInternalDataModelController()
    {
        return instance;
    }

    public InternalDataModel idm
    {
        get { return _idm.First(); }
    }

    public void CheckDirtyFlagsAndSave()
    {
        SerializeDataModel();
    }

    private void SerializeDataModel()
    {

        Debug.Log("SerializeDataModel:" + dataModelXml);


        var objType = _idm.GetType();

        try
        {
            using (var xmlWriter = new XmlTextWriter(dataModelXml, Encoding.Default))
            {
                xmlWriter.Indentation = 2;
                xmlWriter.IndentChar = ' ';
                xmlWriter.Formatting = Formatting.Indented;
                var xmlSerializer = new XmlSerializer(objType);
                xmlSerializer.Serialize(xmlWriter, _idm);
            }
        }
        catch (IOException)
        {
            // todo: call error handler here
        }
    }

    private void DeserializeDataModel()
    {
        try
        {
            Debug.Log("DeserializeDataModel:" + dataModelXml);

            _idm = new List<InternalDataModel>();

            if (File.Exists(dataModelXml))
            {
                using (var xmlReader = new XmlTextReader(dataModelXml))
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<InternalDataModel>));
                    _idm = (List<InternalDataModel>)xmlSerializer.Deserialize(xmlReader);
                }
            }
            else
            {
                // fill with empty placeholder
                _idm.Add(new InternalDataModel());
                idm.exploritoryRouteWalks = new List<InternalDataModel.DataOfImportedERW>();
            }
        }
        catch (IOException)
        {
            // todo: call error handler here

            // fill with empty placeholder
            _idm.Add(new InternalDataModel());
            idm.exploritoryRouteWalks = new List<InternalDataModel.DataOfImportedERW>();
        }
    }
}

