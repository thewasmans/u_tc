using System;
using Level.Data;
using Misc.Runtime;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

using AddressableSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject;

namespace Level.Editor
{
    [CustomEditor(typeof(LevelData))]
    public class LevelDataInspector : UnityEditor.Editor
    {
        public const int BUTTON_MIN_WIDTH = 100;

        // public object RemoveSceneWindow {get; private set;}

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            VisualElement buttonContainer = new VisualElement(); // new VisualElement(root);
            buttonContainer.style.flexDirection = FlexDirection.Row;
            buttonContainer.style.justifyContent = Justify.Center;
            root.Add(buttonContainer);

            Button addSceneButton = new Button(OnAddSceneClicked) { text = "Add Scene" };

            addSceneButton.style.minWidth = BUTTON_MIN_WIDTH;
            addSceneButton.style.flexGrow = 1;

            buttonContainer.Add(addSceneButton);

            Button removeSceneButton = new Button(OnRemoveSceneClicked) { text = "Remove Scene" };

            removeSceneButton.style.minWidth = BUTTON_MIN_WIDTH;
            removeSceneButton.style.flexGrow = 1;

            buttonContainer.Add(removeSceneButton);
            buttonContainer.style.marginTop = 12;
            
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            
            return root;
        }

        public void OnAddSceneClicked()
        {
            AddSceneWindow.ShowWindow(target as LevelData);
        }

        public void OnRemoveSceneClicked()
        {
            RemoveSceneWindow.ShowWindow(target as LevelData);
        }
    }

    public class AddSceneWindow : EditorWindow
    {
        public static int WINWO_MIN_WIDTH = 250;
        public static int WINWO_MIN_HEIGHT = 150;
        public static AddSceneWindow window;
        private static LevelData _levelData;
        
        public static void ShowWindow(LevelData levelData)
        {
            window?.Close();

            _levelData = levelData;
            window = GetWindow<AddSceneWindow>("Add Scene");

            Vector2 screenPoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            screenPoint.x -= -WINWO_MIN_WIDTH;
            window.position = new Rect(screenPoint.x, screenPoint.y, WINWO_MIN_WIDTH, WINWO_MIN_HEIGHT);
        }

        private void CreateGUI()
        {
            rootVisualElement.RegisterCallback<KeyUpEvent>(OnKeyUp);
            _sceneNameField = new TextField("Scene name");
            _sceneNameField.RegisterValueChangedCallback(OnSceneCHange);
            _sceneNameField.RegisterCallback<KeyUpEvent>(e => _sceneNameField.Focus());
            rootVisualElement.Add(_sceneNameField);
            _sceneNameField.Focus();

            var addSceneBUtton = new Button()
            {
                text = "Craate Scene",
                focusable = false,
            };
            
            addSceneBUtton.clicked +=  () => CreateScene();

            rootVisualElement.Add(addSceneBUtton);
        }

        private void CreateScene()
        {
            if(string.IsNullOrWhiteSpace(_sceneName))
            {
                Debug.LogError($"{_sceneName} is null a valid scene name!!");
                return;
            }
            SceneAssetManager.CreateSceneAsset(_levelData, _sceneName);

            _sceneNameField.value = "";
            // _sceneNameField.Focus = false;
        }

        private void OnKeyUp(KeyUpEvent evt)
        {
            if(evt.keyCode is not KeyCode.Return) return;
            CreateScene();
        }

        private void OnSceneCHange(ChangeEvent<string> evt) => _sceneName = evt.newValue;

    private TextField _sceneNameField;
    private string _sceneName;
    
    }
    public class RemoveSceneWindow : EditorWindow
    {
        public static void ShowWindow(LevelData levelData)
        {
            window?.Close();

            window = GetWindow<RemoveSceneWindow>("Remove Scene");
            window._levelData = levelData;

            Vector2 screenPoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            screenPoint.x -= -WINWO_MIN_WIDTH;
            window.position = new Rect(screenPoint.x, screenPoint.y, WINWO_MIN_WIDTH, WINWO_MIN_HEIGHT);
            window.Show();
        }

        public void CreateGUI()
        {
            SerializedObject serializedObject = new SerializedObject(_levelData);
            SerializedProperty sceneRefProperty = serializedObject.FindProperty("_scenesPath");
            ScrollView scrollView = new ScrollView();

            _deleteToggles = new Toggle[sceneRefProperty.arraySize];

            for (int i = 0; i < _deleteToggles.Length; i++)
            {
                SerializedProperty serializedProperty = sceneRefProperty.GetArrayElementAtIndex(i);
                AssetReference reference = serializedProperty.boxedValue as AssetReference;
                DeleteSceneToggle d = new DeleteSceneToggle(reference);
                _deleteToggles[i] = d.Toggle;
                scrollView.Add(d);
            }

            rootVisualElement.Add(scrollView);

            Button deleteButton = new Button(OnDeleteButtonClick){text = "Delete"};
            deleteButton.clicked += OnDeleteButtonClick;
            rootVisualElement.Add(deleteButton);
        }

        private void OnDeleteButtonClick()
        {
            DeletePopup.ShowDeletePopup().Confirm += OnDeletionCOnfirm;
        }

        private void OnDeletionCOnfirm(bool isConfirm)
        {
            if(!isConfirm) return;

            for (int i = 0; i < _deleteToggles.Length; i++)
            {
                if(! _deleteToggles[i].value) continue;
                SceneAssetManager.RemoveSceneAsset(_levelData, i);
            }
            Close();
        }

        public static int WINWO_MIN_WIDTH = 250;
        public static int WINWO_MIN_HEIGHT = 150;
        public static RemoveSceneWindow window;
        public LevelData _levelData;
        public Toggle[] _deleteToggles;
    }

    public class DeletePopup : EditorWindow
    {
        public static DeletePopup ShowDeletePopup()
        {
            window = CreateInstance<DeletePopup>();
            
            Vector2 screenPoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            screenPoint.x -= -MIN_WIDTH;
            window.position = new Rect(screenPoint.x, screenPoint.y, MIN_WIDTH, MIN_HEIGHT);
            window.ShowPopup();
            return window;
        }

        private void CreateGUI()
        {
            int CANCEL_BUTTON_MAX_HEIGHT = 20;
            int CANCEL_BUTTON_MAX_WIDTH = 50;

            rootVisualElement.style.flexDirection = FlexDirection.Row;
            rootVisualElement.style.justifyContent = Justify.SpaceAround;
            rootVisualElement.style.alignItems = Align.Center;

            Button buttonCancel = new Button(OnCancel){text = "Cancel"};
            buttonCancel.style.maxHeight = CANCEL_BUTTON_MAX_HEIGHT;
            buttonCancel.style.maxWidth = CANCEL_BUTTON_MAX_WIDTH;

            rootVisualElement.Add(buttonCancel);

            Button buttonConfirm = new Button(OnConfirm){text = "Confirm"};
            buttonConfirm.style.maxHeight = CANCEL_BUTTON_MAX_HEIGHT;
            buttonConfirm.style.maxWidth = CANCEL_BUTTON_MAX_WIDTH;

            rootVisualElement.Add(buttonConfirm);
        }

        private void OnLostFocus() => OnCancel();

        private void OnCancel()
        {
            Confirm?.Invoke(false);
            Close();
        }

        private void OnConfirm()
        {
            Confirm?.Invoke(true);
            Close();
        }

        public Action<bool> Confirm;
        public static DeletePopup window;
        public static int MIN_WIDTH = 140;
        public static int MIN_HEIGHT = 80;
    }

    public class DeleteSceneToggle : VisualElement
    {

        public DeleteSceneToggle(AssetReference assetReference)
        {
            style.flexDirection = FlexDirection.Row;
            style.height = style.width = _DIMENSIONS;

            Add(Toggle);
            string path = AssetDatabase.GetAssetPath(assetReference.editorAsset);
            Texture icon = AssetDatabase.GetCachedIcon(path);
            Image image = new Image(){ image = icon}; 
            image.style.minWidth = _DIMENSIONS;
            image.style.minHeight = _DIMENSIONS;
            Add(image);
            Label label = new Label(assetReference.editorAsset.name);
            label.style.color = UnityEngine.Color.white;
            Add(label);
            // Toggle.RegisterValueChangedCallback(e => e.newValue ? UnityEngine.Color.green : UnityEngine.Color.red);
        }

        private static int _DIMENSIONS = 20;
        public Toggle Toggle {get;} = new Toggle();
    }

    public static class SceneAssetManager
    {
        public static void CreateSceneAsset(LevelData levelData, string sceneName)
        {
            string assetPath = SaveSceneAsset(levelData, sceneName);

            AssetReference assetReference = SaveAddressable(assetPath);
            AddSceneInInspector(levelData, assetReference);
        }

        public static string SaveSceneAsset(LevelData levelData, string sceneName)
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            string levelSceneName = $"{levelData.name}-{sceneName}".ToLower();
            string sceneAssetPath = $"{GlobalPaths.LEVEL_FOLDER}/{levelData.name}/Scenes/{levelSceneName}.unity";
            
            EditorSceneManager.SaveScene(scene, sceneAssetPath);
            EditorSceneManager.CloseScene(scene, true);
            AssetDatabase.Refresh();
            return sceneAssetPath;
        }

        public static AssetReference SaveAddressable(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            var settings = AddressableSettings.Settings;
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
            return new AssetReference(entry.guid);
        }

        public static void AddSceneInInspector(LevelData levelData, AssetReference assetReference)
        {
            SerializedObject serializeObject = new SerializedObject(levelData);
            
            SerializedProperty property = serializeObject.FindProperty("_sceneReferences"); 
            
            int lastIndex = property.arraySize++;

            SerializedProperty lastElement = property.GetArrayElementAtIndex(lastIndex);

            lastElement.boxedValue = assetReference;
            serializeObject.ApplyModifiedProperties();
        }

        internal static void RemoveSceneAsset(LevelData levelData, int indexScene)
        {
            SerializedObject serializeObject = new SerializedObject(levelData);
            SerializedProperty propertyScenes = serializeObject.FindProperty("_scenePaths");

            SerializedProperty propertyScene = propertyScenes.GetArrayElementAtIndex(indexScene);

            AssetReference assetReference = propertyScene.boxedValue as AssetReference;

            AddressableSettings.Settings.RemoveAssetEntry(assetReference.AssetGUID);
            string path = AssetDatabase.GUIDToAssetPath(assetReference.AssetGUID);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();

            propertyScenes.DeleteArrayElementAtIndex(indexScene);
            serializeObject.ApplyModifiedProperties();
        }
    }
}
