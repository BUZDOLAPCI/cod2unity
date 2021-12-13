using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Cod2Unity
{
    public static class Utils
    {
        public static Dictionary<string, byte[]> maps = new Dictionary<string, byte[]>();
        public static Dictionary<string, byte[]> materials = new Dictionary<string, byte[]>();
        public static Dictionary<string, byte[]> images = new Dictionary<string, byte[]>();
        public static Dictionary<string, byte[]> xmodel = new Dictionary<string, byte[]>();
        public static Dictionary<string, byte[]> xmodelparts = new Dictionary<string, byte[]>();
        public static Dictionary<string, byte[]> xmodelsurfs = new Dictionary<string, byte[]>();

        // so we can populate UI list
        public static List<string> mpMapNames = new List<string>();
        public static List<string> otherMapNames = new List<string>();

        public static string ReadStringTerminated(this BinaryReader br, byte terminatingChar = 0x00)
        {
            char[] rawName = new char[64];

            int i = 0;
            for (i = 0; i < 64; i++)
            {
                if (br.PeekChar() == terminatingChar)
                    break;

                rawName[i] = br.ReadChar();
            }

            return new string(rawName).Replace("\0", string.Empty).Trim();
        }

        public static string ReadStringLength(this BinaryReader br, uint length)
        {
            char[] rawName = new char[length];

            rawName = br.ReadChars((int)length);

            return new string(rawName).Replace("\0", string.Empty).Trim();
        }

        public static bool ReadZipContentsToDictionaries(string gamePath)
        {
            try
            {
                string[] iwdNames = Directory.GetFiles(gamePath + "main\\", "*.iwd", SearchOption.TopDirectoryOnly);

                // Bug: if reads the users mod first which should modify the contents of the original iwds 
                // than it might be overwrittem with the original ones, because the order is not the same 
                // every time
                Parallel.ForEach(iwdNames, (iwdName) =>
                {
                    //Debug.Log(iwdName);
                    using (ZipArchive archive = ZipFile.OpenRead(iwdName))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (!string.IsNullOrEmpty(entry.Name))
                            {
                                AddEntryToDictionaryByType(entry);
                            }
                        }
                    }
                });
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private static string GetFolderName(string entryFullName)
        {
            int found = entryFullName.IndexOf("/");
            return (found > 0) ? entryFullName.Substring(0, found) : "";
        }

        private static void AddEntryToDictionaryByType(ZipArchiveEntry entry)
        {
            string folderName = GetFolderName(entry.FullName);

            switch (folderName)
            {
                case "maps":
                    CreateNewOrUpdateExisting(maps, entry.Name, ReadStream(entry.Open()));
                    if (entry.Name.EndsWith(".d3dbsp"))
                    {
                        if (entry.Name.StartsWith("mp_"))
                        {
                            mpMapNames.Add(entry.Name);
                        }
                        else if (true) //check size maybe?
                        {
                            otherMapNames.Add(entry.Name);
                        }
                        else
                        {

                        }
                    }
                    break;
                case "materials":
                    CreateNewOrUpdateExisting(materials, entry.Name, ReadStream(entry.Open()));
                    break;
                case "images":
                    CreateNewOrUpdateExisting(images, entry.Name, ReadStream(entry.Open()));
                    break;
                case "xmodel":
                    CreateNewOrUpdateExisting(xmodel, entry.Name, ReadStream(entry.Open()));
                    break;
                case "xmodelparts":
                    CreateNewOrUpdateExisting(xmodelparts, entry.Name, ReadStream(entry.Open()));
                    break;
                case "xmodelsurfs":
                    CreateNewOrUpdateExisting(xmodelsurfs, entry.Name, ReadStream(entry.Open()));
                    break;
            }
        }

        public static void CreateNewOrUpdateExisting<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            if (map.ContainsKey(key))
            {
                map[key] = value;
            }
            else
            {
                map.Add(key, value);
            }
        }

        private static byte[] ReadStream(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static Dictionary<string, string> consoleNameToIngameName = new Dictionary<string, string>()
        {
            { "mp_silotown", "Beaumont-Hague" },
            { "mp_farmhouse", "Beltot, France" },
            { "mp_brecourt", "Brecourt, France" },
            { "mp_burgundy", "Burgundy, France" },
            { "mp_matmata", "Matmata, Tunisia" },
            { "mp_downtown", "Moscow, Russia" },
            { "mp_newvillers", "Newvillers, France" },
            { "mp_normandy", "Normandy, France" },
            { "mp_trainstation", "Caen" },
            { "mp_carentan", "Carentan, France" },
            { "mp_crossroads", "Crossroads, France" },
            { "mp_railyard", "Railyard, Poland" },
            { "mp_decoytown", "Decoytown, Egypt" },
            { "mp_dawnville", "Sainte-Mère-Église, France" },
            { "mp_toujane", "Toujane, Tunisia" },
            { "mp_decoy", "El Alamein, Egypt" },
            { "mp_harbor", "Harbor, Russia" },
            { "mp_breakout", "Villers-Bocage" },
            { "mp_bunker", "Vossenack, Germany" },
            { "mp_rhine", "Wallendar" },
            { "mp_tankhunt", "Kalach" },
            { "mp_leningrad", "Leningrad, Russia" }
        };
    }
}