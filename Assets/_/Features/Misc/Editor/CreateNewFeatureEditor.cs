using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

namespace Misc.Editor
{
    [Serializable]
    public class AssemblyDefinition
    {
        public string name;
        public string rootNamespace;
        public string[] references = new string[] { };
        public string[] includePlatforms;
        public string[] excludePlatforms = new string[] { };
        public bool allowUnsafeCode = false;
        public bool overrideReferences = false;
        public string[] precompiledReferences = new string[] { };
        public bool autoReferenced = true;
        public string[] defineConstraints = new string[] { };
        public string[] versionDefines = new string[] { };
        public bool noEngineReferences = false;

        public AssemblyDefinition(string name, string rootNamespace, string[] includePlatforms)
        {
            this.name = name;
            this.rootNamespace = rootNamespace;
            this.includePlatforms = includePlatforms;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    public struct FeaturePart
    {
        public string Name;
        public bool Editor;
    }

    public class CreateNewFeatureEditor : MonoBehaviour
    {
        private static string rootFolder = "_/Features";
        private static FeaturePart[] featureParts = new FeaturePart[]
        {
            new FeaturePart(){Name = "Data", Editor = false},
            new FeaturePart(){Name = "Editor", Editor = true},
            new FeaturePart(){Name = "Runtime", Editor = false},
        };

        [MenuItem("Assets/Create/New Feature %#f", priority = 11)] //Right Click Project > Create > New Feature
        public static void CreateFeature()
        {
            string featureName = "NewFeature";
            string rootFolders = $"Assets/{rootFolder}";

            //Return ASSET ID, if the folder already exist, will create a new folder with a number
            featureName = AssetDatabase.CreateFolder(rootFolders, featureName);
            //Get the folder name created
            featureName = AssetDatabase.GUIDToAssetPath(featureName).Split("/").Last();
            foreach (var part in featureParts)
            {
                var path = $"{rootFolders}/{featureName}";
                AssetDatabase.CreateFolder(path, part.Name);
                CreateModuleAsmdef($"{path}/{part.Name}", featureName, part.Name, part.Editor);
            }
        }

        public static void CreateModuleAsmdef(string path, string feature, string name, bool includeEditor = false, bool rootNamespace = false)
        {
            var fullname = $"{feature}.{name}";

            string asmdefContent = new AssemblyDefinition(
                fullname, "",
                includeEditor ? new string[] { "Editor" } : new string[] { }).ToString();

            string filePath = Path.Combine(path, $"{fullname}.asmdef");

            File.WriteAllText(filePath, asmdefContent);

            AssetDatabase.Refresh();
        }
    }
}