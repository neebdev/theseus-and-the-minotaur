using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SerializationManager
{
    public static string extension = ".dyjjjttyd";
    public static bool Save(string saveName, object saveData)
    {
        try
        {
            //BinaryFormatter formatter = GetBinaryFormatter();
            if (!Directory.Exists(Application.persistentDataPath + "/saves"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/saves");
            }
            string path = Application.persistentDataPath + "/saves/" + saveName + extension;

            //using (FileStream file = File.Create(path))
            //{
            //    formatter.Serialize(file, saveData);
            //}
            using (StreamWriter sw = new StreamWriter(path))
            {
                //Debug.Log(saveData.ToString());
                sw.Write(saveData.ToString());
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string Load(string saveName)
    {
        try
        {
            string path = Application.persistentDataPath + "/saves/" + saveName + extension;

            //Debug.Log(path);
            if (!File.Exists(path))
            {
                //Debug.Log("no file");
                return null;
            }

            //BinaryFormatter formatter = GetBinaryFormatter();
            string save = "";
            //using (FileStream file = File.Open(path, FileMode.Open))
            //{
            //    save = formatter.Deserialize(file);
            //}
            using (StreamReader sr = new StreamReader(path))
            {
                save = sr.ReadToEnd();
            }
            return save;
        }
        catch
        {
            return null;
        }
    }
    public static void Log(string saveName)
    {
        /*string path = Application.persistentDataPath + "/saves/" + saveName + extension;
        if (!File.Exists(path))
        {
            return;
        }

        BinaryFormatter formatter = GetBinaryFormatter();
        object save = null;
        using (FileStream file = File.Open(path, FileMode.Open))
        {
            save = formatter.Deserialize(file);
            Debug.Log(save);
        }*/
    }

    /*public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }*/
}
