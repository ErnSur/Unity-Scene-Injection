# Scene Injection

## Findings
- In batch mode modifying the `EditorBuildSettings.scenes` from `InitializeOnLoad` method doesn't modify the _ProjectSettings/EditorBuildSettings.asset_
- More details on loading assets from `InitializeOnLoad` method:
  - Callback is invoked prior to AssetDataBase importing
  - It is possible to load assets from this callback if they were previously imported (asset is present in the Library folder). 

## Goal
- update build scenes on every unity project launch

Already achieved
- updates build scenes on every clean project launch (because it needs to import all assets, triggering database update)

- is it possible to break it?
