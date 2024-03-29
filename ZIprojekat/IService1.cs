﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ZIprojekat
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IService1
    {
        [OperationContract]
        string GetData(string value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        bool EncryptBitmap(string inputPath, string outputPath, string alghorithm, bool hash, string key, string nonce);
        [OperationContract]
        bool DecryptBitmap(string inputPath, string outputPath, string alghorithm, bool hash, string key, string nonce);
        [OperationContract]
        string EncryptRC6(string source, string key);
        [OperationContract]
        string EncryptRC6_CTRmode(string source, string key, string nonce);
        [OperationContract]
        string DecryptRC6(string source, string key);
        [OperationContract]
        string DecryptRC6_CTRmode(string source, string key, string nonce);
        [OperationContract]
        List<string> GenerateRandomKeyBifid();
        [OperationContract]
        void LoadKeyBifid(string key);
        [OperationContract]
        List<string> EncryptBifid(List<string> source);
        [OperationContract]
        List<string> DecryptBifid(List<string> source);

        [OperationContract]
        byte[] GenerateTigerHash(string source);

        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "ZIprojekat.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
