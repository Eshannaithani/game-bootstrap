// SaveSystem.cs
// Local JSON save with the safeguards from SOP sections 5 and 13:
// schema stamp set in the constructor only, atomic writes, quarantine of corrupt files, a migration hook.
// Why the constructor: deserialisers skip constructors, so a loaded save whose stamp is still 0 was
// written before versions existed, and "false" can be told apart from "never written".
using System;
using System.IO;
using UnityEngine;

namespace __NAMESPACE__
{
    [Serializable]
    public class SaveData
    {
        public int schemaVersion;
        public long coins;
        public int level;
        public string lastSessionUtc = "";

        public SaveData() { schemaVersion = GameConstants.SaveSchemaVersion; }
    }

    public static class SaveSystem
    {
        static string Dir => Application.persistentDataPath;
        static string MainPath => Path.Combine(Dir, GameConstants.SaveFileName);

        public static SaveData Current { get; private set; }

        public static SaveData Load()
        {
            if (!File.Exists(MainPath)) { Current = new SaveData(); return Current; }
            try
            {
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(MainPath));
                if (data == null) throw new Exception("empty save");
                if (data.schemaVersion < GameConstants.SaveSchemaVersion)
                {
                    Migrate(data);
                    Save(data);   // persist immediately: a crash before the next save cadence would re-run the migration
                }
                Current = data;
            }
            catch (Exception e)
            {
                // Quarantine rather than throw: a corrupt save must never brick startup.
                string quarantine = MainPath + ".corrupt-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
                try { File.Move(MainPath, quarantine); } catch { }
                Debug.LogWarning("[Save] corrupt save moved to " + quarantine + ": " + e.Message);
                Current = new SaveData();
            }
            return Current;
        }

        // Atomic: write a temp file, then replace. Returns false on failure so callers can refuse destructive follow-ups.
        public static bool Save(SaveData data)
        {
            try
            {
                Directory.CreateDirectory(Dir);
                string tmp = MainPath + ".tmp";
                File.WriteAllText(tmp, JsonUtility.ToJson(data, true));
                if (File.Exists(MainPath)) File.Replace(tmp, MainPath, MainPath + ".bak");
                else File.Move(tmp, MainPath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("[Save] write failed: " + e.Message);
                return false;
            }
        }

        static void Migrate(SaveData data)
        {
            // if (data.schemaVersion < 2) { /* one-time fixup for fields added in version 2 */ }
            data.schemaVersion = GameConstants.SaveSchemaVersion;
        }

        // Destructive reset. Preserves: nothing inside the save file. Consent and the install stamp live in
        // PlayerPrefs on purpose (Consent.cs) so a reset player is not counted as a new install.
        public static SaveData Reset()
        {
            Current = new SaveData();
            Save(Current);
            return Current;
        }
    }
}
