using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Diagnostics;

namespace SymLink.Editor
{
    public class SymlinkWindow : EditorWindow
    {

        [MenuItem("Window/Symlink")]
        private static void ShowWindow()
        {
            var window = GetWindow<SymlinkWindow>();
            window.titleContent = new GUIContent("SymlinkWindow");
            window.Show();
        }

        private void CreateGUI()
        {
            DirectoryInfo symlinkDir = new DirectoryInfo(_originPath);
            
            titleContent = new GUIContent("SProject");
            rootVisualElement.style.flexShrink = 1;
            var label = new Label("Directories")
            {
                style =
            {
                marginLeft = 5,
                marginBottom = 10,
                fontSize = 18,
            }
            };

            var scrollView = new ScrollView()
            {
                style =
            {
                maxHeight = 500,
            }
            };

            rootVisualElement.Add(scrollView);
            scrollView.Add(label);
            _rootFolderGroup = new ToggleFolderGroup(symlinkDir);
            scrollView.Add(_rootFolderGroup);
            scrollView.Add(new ButtonGroup(OnConfirm));
            CheckFoxExistingSymlinks();
        }

        private void CheckFoxExistingSymlinks()
        {
            var toggleFolders = new List<ToggleFolder>();
            var folders = _rootFolderGroup.GetFolderAndSubFolders(toggleFolders);

            for (int i = 0; i < folders.Length; i++)
            {
                var folder = new DirectoryInfo($@"{_linkPath}/{folders[i].name}");
                var isValidSymlink = folder.Exists && IsDirectorySymLink(folder);
                if (!isValidSymlink) continue;
                folders[i].SetToggle(true);
            }
        }

        private void OnConfirm()
        {
            var toggleFolder = new List<ToggleFolder>();
            var folders = _rootFolderGroup.GetFolderAndSubFolders(toggleFolder);

            for (int i = 0; i < folders.Length; i++)
            {
                ToggleFolder folder = folders[i];
                var linkPath = $@"{_linkPath}/{folder.name}";
                var linkDirectory = new DirectoryInfo(linkPath);

                if (folder.isToggle)
                {
                    if (linkDirectory.Exists)
                    {
                        if (!IsDirectorySymLink(linkDirectory))
                        {
                            DeleteDirectory(linkDirectory.FullName);
                        }
                    }
                    CreateSymLinkDirectory(folder.name);
                    continue;
                }

                if (!linkDirectory.Exists) continue;
                if (!IsDirectorySymLink(linkDirectory)) continue;

                DeleteDirectory(linkPath);
            }
        }

        private void DeleteDirectory(string path)
        {
            string linkPath = $"{_linkPath}/{path}";
            var linkDirectory = new DirectoryInfo(linkPath);

            Directory.Delete(linkPath);
            File.Delete($"{linkPath}.meta");
            AssetDatabase.Refresh();
        }

        private void CreateSymLinkDirectory(string path)
        {
            string originPath = $"{_originPath}/{path}";
            var originInfo = new DirectoryInfo(originPath);

            string linkPath = $"{_linkPath}/{path}";
            var linkDirectory = new DirectoryInfo(linkPath);

            Directory.CreateDirectory(Directory.GetParent(linkDirectory.FullName).FullName);

            var partsPath = path.Split("/");
            var nestedPath = "";

            for (int i = 0; i < partsPath.Length - 1; i++)
            {
                nestedPath += $@"/{partsPath[i]}";
                var nestedDirectory = new DirectoryInfo($@"{_linkPath}{nestedPath}");

                if (nestedDirectory.Exists) continue;

                Directory.CreateDirectory($@"{_linkPath}{nestedPath}");
            }

            if (linkDirectory.Exists)
            {
                if (IsDirectorySymLink(linkDirectory))
                {
                    UnityEngine.Debug.LogWarning($"Symbolink dir exist{linkDirectory.FullName}");
                    return;
                }
                UnityEngine.Debug.LogError($"Readl Folder ssound in a Symbolik reserved folder {linkDirectory.FullName}");
                return;
            }

            if (!originInfo.Exists)
            {
                UnityEngine.Debug.LogError($"Could not found directory at path {originInfo.FullName}");
                return;
            }

            string args = $"/c mklink /J {linkPath} {originPath}";
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    Arguments = args
                }
            };
            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            if (string.IsNullOrWhiteSpace(stdout)) UnityEngine.Debug.LogError(stdout);
            if (string.IsNullOrWhiteSpace(stderr)) UnityEngine.Debug.LogError(stderr);

            process.WaitForExit();
            
            UnityEngine.Debug.Log(process.ExitCode == 0 ? "Symlink Generation Succes" : $"Symlink Generation Failed {process.ExitCode}");

            UnityEngine.Debug.unityLogger.logEnabled = false;
            AssetDatabase.Refresh();
            UnityEngine.Debug.unityLogger.logEnabled = true;
            process.Dispose();
        }

        public bool IsDirectorySymLink(DirectoryInfo dirInfo) => dirInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        private ToggleFolderGroup _rootFolderGroup;
        private static string _linkPath => $@"{Directory.GetCurrentDirectory()}/U_TC/Assets/_";
        private static string _originPath => $@"{Directory.GetParent(Directory.GetCurrentDirectory())}/symlink";
    }

    public class ToggleFolderGroup : VisualElement
    {
        public ToggleFolderGroup(DirectoryInfo parent, string parentPath = null)
        {
            style.marginLeft = 15;
            foreach (var folder in parent.GetDirectories())
            {
                var subfolder = parentPath != null ? $@"{parentPath}/{folder.Name}" : folder.Name;
                var match = Regex.Match(folder.Name, _pattern);

                if (match.Success) continue;

                var toggleFolder = new ToggleFolder(folder.Name, "\u2514", parentPath);
                Add(toggleFolder);
                _folders.Add(toggleFolder);

                var subdirectoryInfo = new DirectoryInfo(Path.Combine(parent.FullName, folder.Name));
                if(subdirectoryInfo.GetDirectories().Length <= 0) continue;
                var subFolderGroup = new ToggleFolderGroup(subdirectoryInfo, subfolder);
                Add(subFolderGroup);

                _subFolderGroups.Add(subFolderGroup);
                toggleFolder.SetSubFolder(subFolderGroup);
            }
        }

        public ToggleFolder[] GetFolderAndSubFolders(List<ToggleFolder> allFolders)
        {
            allFolders.AddRange(_folders);
            
            for (int i = 0; i < _subFolderGroups.Count; i++)
            {
                _subFolderGroups[i].GetFolderAndSubFolders(allFolders);
            }

            return allFolders.ToArray();
        }

        private readonly List<ToggleFolder> _folders = new();
        private readonly List<ToggleFolderGroup> _subFolderGroups = new();
        private const string _pattern = @"^[.]\w+";
    }

    public class ToggleFolder : VisualElement
    {
        public bool isToggle => _toggle.value;

        public ToggleFolder(string directoryName = null, string unicodeSymbol = null, string parentPath = null)
        {
            name = parentPath != null ? $@"{parentPath}/{directoryName}/" : directoryName;

            if (!string.IsNullOrWhiteSpace(unicodeSymbol))
            {
                Add(new Label(unicodeSymbol));
            }

            _toggle = new Toggle() { style = { marginRight = 20 } };

            style.flexDirection = FlexDirection.Row;
            Add(_toggle);
            Add(new Label(directoryName));

            _toggle.RegisterValueChangedCallback(OnToggle);
        }

        private void OnToggle(ChangeEvent<bool> evt)
        {
            if(_subFolders is null) return;

            var value = evt.newValue;

            var folderHierarchy = new List<ToggleFolder>();
            var folders = _subFolders.GetFolderAndSubFolders(folderHierarchy);
            if(!value)
            {
                for (int i = 0; i < folders.Length; i++)
                {
                    var folder = folders[i];
                    folder.ShowToggle();
                }
                return;
            }

            for (int i = 0; i < folders.Length; i++)
            {
                var folder = folders[i];
                folder.HideToggle();
            }
        }

        public void SetSubFolder(ToggleFolderGroup sub) => _subFolders = sub;

        public void HideToggle() => _toggle.SetEnabled(false);

        public void ShowToggle() => _toggle.SetEnabled(true);

        public void SetToggle(bool value) => _toggle.value = value;

        private readonly Toggle _toggle;
        private ToggleFolderGroup _subFolders;
    }

    public class ButtonGroup : VisualElement
    {
        public ButtonGroup(Action confirmAction)
        {
            style.flexDirection = FlexDirection.Row;
            style.marginLeft = 10;
            style.marginTop = 20;
            Add(new Button(confirmAction) { text = "Confirm" });
        }
    }

}
