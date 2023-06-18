using System;
using System.IO;
using UnityEngine;

namespace JsonSaveLoadSerializer.DataManagement
{
    public class FileDataHandler
    {
        private readonly bool _useEncryption;
        private const string EncryptionCodeWord = "Console";

        private readonly string _dataDirectionPath;
        private readonly string _dataFileName;

        public FileDataHandler(string dataDirectionPath, string dataFileName, bool useEncryption)
        {
            _dataDirectionPath = dataDirectionPath;
            _dataFileName = dataFileName;
            _useEncryption = useEncryption;
        }

        public GameData Load()
        {
            var fullPath = Path.Combine(_dataDirectionPath, _dataFileName);

            GameData loadedData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    var dataToLoad = "";

                    using var stream = new FileStream(fullPath, FileMode.Open);
                    using var reader = new StreamReader(stream);
                    
                    dataToLoad = reader.ReadToEnd();

                    if (_useEncryption)
                    {
                        dataToLoad = EncryptAndDecrypt(dataToLoad);
                    }

                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e)
                {
                    Debug.Log($"Unexpected error occured while trying to LOAD data! In File {fullPath} with Exception => {e} ");
                }
            }

            return loadedData;
        }

        public void Save(GameData data)
        {
            var fullPath = Path.Combine(_dataDirectionPath, _dataFileName);
            
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);

                var dataToSave = JsonUtility.ToJson(data, true);

                if (_useEncryption)
                {
                    dataToSave = EncryptAndDecrypt(dataToSave);
                }

                using var stream = new FileStream(fullPath, FileMode.Create);
                using var writer = new StreamWriter(stream);
                
                writer.Write(dataToSave);
            }
            catch (Exception e)
            {
                Debug.Log($"Unexpected error occured while trying to SAVE data! In File {fullPath} with Exception => {e} ");
            }
        }

        private static string EncryptAndDecrypt(string data)
        {
            var encryptAndDecrypt = "";

            for (var i = 0; i < data.Length; i++)
            {
                encryptAndDecrypt += (char) (data[i] ^ EncryptionCodeWord[i % EncryptionCodeWord.Length]);
            }

            return encryptAndDecrypt;
        }
    }
}
