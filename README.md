# RocketcadManager

## Setup

1.	Download and run the installer `InstallRCM.exe` for the [latest release](https://github.com/betato/RocketcadManager/releases).
2.	Run the program and open the settings menu (click the gear icon in the toolbar)
3.	Add your CAD directory eg. `C:\Some\Other\Folders\GrabCAD\2020_2021 uORocketry` and click OK

SOLIDWORKS will now create additional `.status` files when parts and assemblies are saved. These files should be uploaded along with their associated assemblies and parts. For example, when you upload `21-02-16.SLDASM`, you should also upload `21-02-16.SLDASM.status`

## Interface

![RocketcadManager GUI](https://raw.githubusercontent.com/betato/RocketcadManager/master/Docs/ManagerInterface.png)

### 1 - Toolbar Buttons
- Open file in SOLIDWORKS (not implemented yet)
- Open containing folder
- Refresh file view
- View file [warnings and errors](#warnings)
- Settings (add/remove CAD directories)

### 2 - File View
- Browse for SOLIDWORKS parts and assemblies
- Icons indicate the [warnings and errors](#warnings) for each file/folder

### 3 - Part Dependencies
- The top list show the assemblies that the selected part/assembly is used in
- The bottom list shows the parts and assemblies that are used in the selected assembly

### 4 - Part Information
#### 4.1 - General Information
- **Description** - A short description of the part/assembly. This is set by the custom SOLIDWORKS file property `Description`
- **Thumbnail** - Thumbnail image of the part/assembly
- **Notes** - Any extra information about the part/assembly

#### 4.2 - Inventory Information
- **In Stock** - The number of parts/assemblies that we currently have. This value should be manually updated to match the number of parts/assemblies currently in stock.
- **Required - Assemblies** - The number of parts/assemblies that are required by other assemblies, for example if four M2 screws are used in only one assembly, and three of these assemblies are required, the number displayed here will be twelve. This number is automatically updated when assemblies are modified or additional part requirements are changed.
- **Required - Additional** - The number of parts/assemblies that are required in addition to those used by other assemblies. For example, we want one 'additional' rocket since it is not required for any other assemblies. We might also want 'additional' parts to have as spares, for example additional M2 screws.
- **Required - Total** - The sum of the assembly and additional required parts. This is the total number of parts that need to be built or purchased.

## Warnings

Warnings are shown to indicate incorrectly named files and folders or file errors. Warning and error icons are shown in the [file view](#file-view) and details can be read in the warnings and errors window.

|Icon                                                                                                           | Warning                           |
|---------------------------------------------------------------------------------------------------------------|-----------------------------------|
|![WarningFile](https://raw.githubusercontent.com/betato/RocketcadManager/master/Docs/Icons/WarningFile.png)    | Naming violation                  |
|![WarningFile](https://raw.githubusercontent.com/betato/RocketcadManager/master/Docs/Icons/WarningFile.png)    | Name does not match parent folder	|
|![QuestionFile](https://raw.githubusercontent.com/betato/RocketcadManager/master/Docs/Icons/QuestionFile.png)  | Missing info file                 |
|![ErrorFile](https://raw.githubusercontent.com/betato/RocketcadManager/master/Docs/Icons/ErrorFile.png)        | Referenced components not found   |
|![ErrorFile](https://raw.githubusercontent.com/betato/RocketcadManager/master/Docs/Icons/ErrorFile.png)        | Error loading info file           |


## Naming

### Naming Convention

File and folder names that break the naming convention still work with RocketcadManager but are displayed with warnings. If these warnings appear on files that you have saved, they should remind you to stop being a lazy person and to fix your file names. Basic file and folder naming rules are as follows:

Parts, assemblies and subfolders must continue the numbering of their parent folder, eg. the assembly `21-02-10-00.SLDASM`, the part `21-02-10-02.SLDPRT`, and the folder `21-02-10-01` must be contained in the folder `21-02-10`.

Assembly names must end with `00`, eg. `21-02-00.SLDASM`
Part names must **not** end with `00`, eg. `21-02-01.SLDPRT`

Part, assembly and, folder names may have an additional short descriptions that are separated from the numbered portion of the name by a space, eg. `21-02-00 Some Other Text.SLDPRT`


### Naming Examples

`21-01-11` is a valid folder name. For files and folders contained inside:

The following are **correctly** named:
`21-01-11-01` is a valid subfolder name 
`21-01-11-02 Folder Description` is a valid subfolder name 
`21-01-11-01.SLDPRT` is a valid part name 
`21-01-11-01 2701T22.SLDPRT` is a valid part name 

The following are **incorrectly** named:
`21-01-04-11` is a valid folder name but is in the **wrong folder**
`21-01-21-01.SLDPRT` is a valid part name but is in the **wrong folder**
`21-01-11-00.SLDPRT` is an **invalid** part name (only assemblies may end with `00`)
`21-01-11-01-2701T22.SLDPRT` is an **invalid** part name (Additional text in file and folder names must be separated with a space)


### Name Verification

To check if a part, assembly, or folder has a valid name, the following regexes are applied. A match must be found for the name to be valid.

|Applied to     |Regex                                                  |
|---------------|-------------------------------------------------------|
|Part           |`^([0-9]{2}-)+(0[1-9]|[1-9][0-9])(\s.*)?\.(?i)SLDPRT$` |
|Assembly       |`^([0-9]{2}-)+00(\s.*)?\.(?i)SLDASM$`                  |
|Folder         |`^([0-9]{2}-)+[0-9]{2}($|\s)`                          |

To check if a part, assembly, or child folder in a correctly named parent folder, the following regexes are applied. The first capturing group result of the parent folder and the Child Folder/Assembly/Part regex must be equal.

|Applied to     |Regex                                                               |
|---------------|--------------------------------------------------------------------|
|Parent Folder  |`^(([0-9]{2}-)+[0-9]{2}`                                            |
|Child Folder   |`^([0-9]{2}(-[0-9]{2})*)-[0-9]{2}($|\s)`                            |
|Assembly/Part  |`^([0-9]{2}(-[0-9]{2})*)-[0-9]{2}(\s.*)?\.(?i)(SLDASM|SLDPRT)$`     |

## Other Information

Error logs and settings are stored in `%localappdata%\RocketcadManager`

Information files for parts and assemblies, `ASSEMBLYNAME.SLDPRT.status` and `PARTNAME.SLDASM.status` respectively, are stored in a `.partstatus` folder. This folder is hidden by default and is located in the same directory as the source part and assembly files. These `.status` files should be uploaded along with any modified `.SLDASM` and `.SLDPRT` files.

Each `.status` file is a zip archive containing the files `status.json` and `thumbnail.jpg`. The `status.json` file stores part and assembly descriptions, stock numbers, and notes. For assemblies, this file also stores the file location and count of every component referenced in the assembly.
