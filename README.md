# Universe Technocite


## How to clone
 1. mkdir ``c:\_\tc``
 2. cd ``c:\_\tc``
 3. git clone https://github.com/thewasmans/u_tc.git
 4. mkdir symlink

Inside symlink, you can place ``Audio``, ``Graphics``, ``Levels`` ... folders

https://miro.com/app/board/uXjVLKW_SnU=/

## FEATURES
![image](https://github.com/user-attachments/assets/8c889704-d4b8-43e8-a3a5-a0f054add44a)

1. `Hello Level`
Double click on scriptableobject asset on print a message in console log
2. `Load Scene Async`
3. `Close Scene Async` Add close/unload Scene Editor and Runtime. Close Feature not called for now, just exposed
4. `Addressable Async Loading` Change Scene path to AssetReference Scene with Addressable
Create new group addressable
Add all scenes to the new group addressable (Default group)
You can reference scene like a AssetReference in LevelData
Change foreach to basic for to prevente create garbage with foreach
5. `Editor Window, create a feature via a click` Create the new feature directly into Assets/_/Features with name New Feature
6. `Unity Toolbar + Level Selection`
   - In the Unity Toolbar, asside of the button Play, Pause, Step in the top of Editor UI, added a level selection dropdown to open a Specific LevelData. The dropdown will be automaticly refresh if a LevelData.asset is added in the project.
   - Fix the AddressableGroup references scene lost.
   - Fix wrong function to open level in editor with EditorSceneManager.OpenScene
7. `Override Play Mode`
   - Ajouter un boolean dans la toolbar pour activer ou non notre override du playmode.
   - The override should be play the current level opened when the user click on button play. Currently the button doesn't working juste change the state when click on the button
9. `Button Create Level`
   - Ajouter un bouton pour créer un level (meme idée que la création de feature)
   - Add button `New Level` in the toolbar to create a new level. Will open a window to choice the name of the level. Will create a folder in Assets/_/Levels/<<name_choiced>> with a
   - LevelData and Scenes folder
9. `Dropdown + Button Create Feature`
   - A droite du playmode button, ajouter un dropdown qui pour l'instant n'a que "Features" et un bouton Add Feature
     - Add Dropdown in toolbar to select Feature
     - Add Button in toolbar to create new Feature
     - Add asmdef
     - Update FeatureCreator to show the path where the feature will be create
10. `Add to dropdown "Build Addressable"` Ajouter au dropdown "Addressable" avec un bouton "build addressable" qui target le new build de l'addressable window
11. `Lighting Fix`
    - Add RP package
    - Add RP Settings in Config/Render
    - Add lightScene ref in LevelData
    - Load and Unload light scene
12. `Custom Inspector For Level add and remove`
13. `Symlinks`
14. `Addressable Definition`
15. `Edit "New Level Logic" to create addressable definition`
16. `"Scan the project" to get all the file that need to be addressablified`
17. `Diff with existing groups`
18. `Apply only outdated groups`
19. `Ignore Addressable Tag`
20. `Changer le LevelLoader pour qu'il utilise les addressables`


## LEAD DEV ARCHITECTURE 

## FOLDER ARCHITECTURE
/_

/_/tc

/_/tc/dlnet

/_/tc/symlink

/_/tc/u_tc                                                                                  # Content Unity project

/_/tc/u_tc/_                                                                                # Root work folder in unity project to avoid mix your content and all shit from the external extensions or else

/_/tc/u_tc/_/Audio
/_/tc/u_tc/_/Config
/_/tc/u_tc/_/Features

/_/tc/u_tc/_/Features/Symlink
/_/tc/u_tc/_/Features/Symlink/Data
/_/tc/u_tc/_/Features/Symlink/Data/Symlink.Data.asmdef
/_/tc/u_tc/_/Features/Symlink/Editor
/_/tc/u_tc/_/Features/Symlink/Editor/Symlink.Editor.asmdef
/_/tc/u_tc/_/Features/Symlink/Runtime
/_/tc/u_tc/_/Features/Symlink/Runtime/Symlink.Runtime.asmdef

/_/tc/u_tc/_/Graphist       

/_/tc/u_tc/_/Levels
          
# LINKS
https://docs.unity3d.com/Manual/UnityAccelerator.html
