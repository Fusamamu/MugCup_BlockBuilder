# MUGCUP Block Builder for Unity

#### Created by Sukum Duangpattra [MUGCUP], using Unity [version 2020.3.22.f1]

## Bugs
- [ ] Hit normal not detect properly against FBX objects. Need to find other ways to deal with face checking.

## Introduction

<p>
Block Builder project is an attempt to create a flexible tile block editor tool in Unity.
</p>

## Features
<ul>
    <li>Generate grid blocks and edit blocks.</li>
    <li>Able to edit blocks during both "Gameplay" and "Unity Editor"</li>
</ul>


## Documentation

```mermaid
classDiagram
BlockBuilderManager *-- BlockManager

<<Singleton>> BlockBuilderManager
BlockBuilderManager : Init()

BlockManager o-- GridBlockDataManager : Blockmanager manage GridBlockData

BlockBuilderManager *-- BlockEditorManager
BlockBuilderManager *-- BlockHandleManager
BlockBuilderManager *-- BlockSelectionManager

BlockEditorManager    <|-- BaseBuilderManager
BlockHandleManager    <|-- BaseBuilderManager
BlockSelectionManager <|-- BaseBuilderManager

BaseBuilderManager *-- BlockManager

class GridBlockDataManager{
<<Singleton>>
}

class BaseBuilderManager{
<<BaseClass>>
}


```

### BlockBuilderManager.cs
BlockBuilderManager manage all grid block data initializations and dependencies. GridDataManger
is initialized through this. BlockBuilderManager can choose to swap GridDataManagers when needed.

### GridDataManager.cs
GridDataManager holds all grid block data. The manager resides inside BlockBuilderManager.cs.

## Dependencies

