using System.IO;
using System.Linq;
using AddressableDefinition.AssetDatabase;
using Mono.Cecil;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AddressablesDefinition.Editor
{
    public class AddressableManager : MonoBehaviour
    {
        public static AddressableAssetGroup TryGetAddressableAssetGroup(string groupName, out AddressableAssetGroup group)
        {
            return group = AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName);
        }

        public static AddressableAssetGroup NewAddressableAssetGroup(string groupName)
        {

            if (TryGetAddressableAssetGroup(groupName, out var AddressableAssetGroup))
            {
                Debug.Log($"[Addressabel {groupName}] already exist!");
                return null;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings.CreateGroup(groupName: groupName,
                                        setAsDefaultGroup: false,
                                        readOnly: false,
                                        postEvent: true,
                                        schemasToCopy: null);
        }

        public static AssetReference SetAddressable(string guid, AddressableAssetGroup assetGroup = null, bool refreshData = false)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (assetGroup is null)
            {
                assetGroup = settings.DefaultGroup;
            }

            var entry = settings.CreateOrMoveEntry(guid, assetGroup);

            if (refreshData)
            {
                AssetDatabase.SaveAssetIfDirty(new GUID(entry.guid));
            }

            return new(entry.guid);
        }

        public static void ProcessFolderRecusrsively(string folderPath, AddressableAssetGroup group)
        {
            AddFilesToGroup(Directory.GetFiles(folderPath), group);
            var subFolders = Directory.GetDirectories(folderPath);

            for (int i = 0; i < subFolders.Length; i++)
            {
                string subFolder = subFolders[i];
                var subFolderFiles = Directory.GetFiles(subFolder);
                var hasDefinition = Directory.GetFiles(subFolder).Any(f => AssetDatabase.LoadAssetAtPath<AddressabeleDefinition>(f));

                if(hasDefinition) return;

                AddFilesToGroup(subFolderFiles, group);
                ProcessFolderRecusrsively(subFolder, group);
            }
        }
        public static void AddFilesToGroup(string[] filePaths, AddressableAssetGroup group)
        {
            var files = filePaths.Where(f => !f.EndsWith(".meta")).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                GUID guid = AssetDatabase.GUIDFromAssetPath(files[i]);
                
                if(AssetDatabase.GetLabels(guid).Contains(Labels.m_addressableIgnore))continue;

                var settings = AddressableAssetSettingsDefaultObject.Settings;
                settings.CreateOrMoveEntry($"{guid}", group);
            }
        }

        public static void ScanA()
        {
            var guids = AssetDatabase.FindAssets($"t: {nameof(AddressablesDefinition)}");

            for (int i = 0; i < guids.Length; i++)
            {
                var parentFolderPath = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(guids[i]));
                var parentFolderName = Path.GetFileName(parentFolderPath);
                var childGuids = AssetDatabase.FindAssets("", new string[] { parentFolderPath });
                var alreadyExist = TryGetAddressableAssetGroup(parentFolderName, out AddressableAssetGroup group);
                var newGroup = group ?? NewAddressableAssetGroup (parentFolderName);

                for (int j = 0; j < childGuids.Length; j++)
                {
                    if(AssetDatabase.GetLabels(new GUID(childGuids[j])).Contains(Labels.m_addressableIgnore))
                    {
                        continue;
                    }

                    SetAddressable(childGuids[i], newGroup, true);
                }
            }

            Clear();
        }

       public static void ScanB()
        {
            var guids = AssetDatabase.FindAssets($"t: {nameof(AddressablesDefinition)}");

            for (int i = 0; i < guids.Length; i++)
            {
                var definitionPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var parentFolderPath = Path.GetDirectoryName(definitionPath);
                var childGuids = AssetDatabase.FindAssets("", new string[] { parentFolderPath });
                
                var groupName = AssetDatabase.LoadAssetAtPath<AddressabeleDefinition>(definitionPath).name;

                var settings = AddressableAssetSettingsDefaultObject.Settings;
                var newGroup = settings.FindGroup(groupName) ?? null;
                settings.CreateGroup(groupName, false, false, true, null);
                ProcessFolderRecusrsively(parentFolderPath, newGroup);

                for (int j = 0; j < childGuids.Length; j++)
                {
                    if(AssetDatabase.GetLabels(new GUID(childGuids[j])).Contains(Labels.m_addressableIgnore))
                    {
                        continue;
                    }

                    SetAddressable(childGuids[i], newGroup, true);
                }
            }

            Clear();
        }

        public static void Clear()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group;
            
            for (int i = 0; i < settings.groups.Count; i++)
            {
                group = settings.groups[i];

                if(group.entries.Count == 0 && !group.IsDefaultGroup())
                {
                    settings.RemoveGroup(group);
                }
            }
        }
    }

    public class Labels
    {
        public const string m_addressableIgnore = "addressable_ignore";
    }
}