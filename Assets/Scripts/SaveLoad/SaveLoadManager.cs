namespace SaveLoad
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    namespace survivalPrototype
    {
        public static class SavingManager
        {
            public static string savingPath;

            public static void SaveData<T>(T dataToSave, int Save) where T : SaveableObject
            {
                if (!Directory.Exists(savingPath))
                    Directory.CreateDirectory(savingPath);

                string path = savingPath + "Save" + Save.ToString() + "/" + dataToSave.GetKey() + ".bin";

                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(path, FileMode.Create);

                bf.Serialize(fs, dataToSave);
                fs.Close();
            }

            public static T LoadData<T>(string key, int Save) where T : SaveableObject
            {
                string path = savingPath + "Save" + Save.ToString() + "/" + key + ".bin";

                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();

                    T objectToReturn = (T)bf.Deserialize(fs);
                    fs.Close();

                    return objectToReturn;
                }

                return default(T);
            }

            public static bool HasSaveFile(string key, int Save)
            {
                return System.IO.File.Exists(savingPath + "Save" + Save.ToString() + "/" + key + ".bin");
            }

            public static bool HasAnySaveFile(int save)
            {
                if (Directory.GetFiles(savingPath + "Save" + save.ToString()).Length > 0)
                {
                    return true;
                }

                return false;
            }

            public static string[] GetObjectsToInstantiate(int save)
            {
                string[] dirs = Directory.GetFiles(savingPath + "Save" + save.ToString());

                List<string> objectsToInstantiatePath = new List<string>();

                for (int i = 0; i < dirs.Length; i++)
                {
                    FileStream fs = new FileStream(dirs[i], FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();

                    SaveableObject saveObject = (SaveableObject)bf.Deserialize(fs);
                    fs.Close();
                    if (saveObject.isInstantiatable())
                    {
                        string pathToInstantiateObject = saveObject.GetPrefabPath();
                        objectsToInstantiatePath.Add(pathToInstantiateObject);
                    }
                }

                return objectsToInstantiatePath.ToArray();
            }
        }
    }
}