; --------------------------------
; Include Modern UI

    !include "MUI2.nsh"

; --------------------------------
; General
    ; Name and file
    Name "RocketcadManager"
    OutFile "InstallRCM.exe"
    Unicode True
    ; Default installation folder
    InstallDir "$PROGRAMFILES64\RocketcadManager"
    ; Get installation folder from registry if available
    InstallDirRegKey HKCU "Software\RocketcadManager" ""
    RequestExecutionLevel admin

; --------------------------------
; Variables
    Var StartMenuFolder

;--------------------------------
; Create desktop icon
    Function createDesktopShortcut
    CreateShortcut "$DESKTOP\RocketcadManager.lnk" "$INSTDIR\RocketcadManager.exe"
    FunctionEnd

;--------------------------------
; Launch app
    Function launchApp
    ExecShell "" "$INSTDIR\RocketcadManager.exe"
    FunctionEnd

; --------------------------------
; Interface Settings
    !define MUI_ABORTWARNING

; --------------------------------
; Pages
    !insertmacro MUI_PAGE_COMPONENTS
    !insertmacro MUI_PAGE_DIRECTORY

    ; Start menu folder page config
    !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
    !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\RocketcadManager" 
    !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
    
    ; Desktop icon creation
    !define MUI_FINISHPAGE_SHOWREADME ""
    !define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
    !define MUI_FINISHPAGE_SHOWREADME_TEXT "Create Desktop Shortcut"
    !define MUI_FINISHPAGE_SHOWREADME_FUNCTION createDesktopShortcut

    ; Launch after install option
    !define MUI_FINISHPAGE_RUN
    !define MUI_FINISHPAGE_RUN_TEXT "Launch Application"
    !define MUI_FINISHPAGE_RUN_FUNCTION launchApp

    !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder
    !insertmacro MUI_PAGE_INSTFILES
    !insertmacro MUI_PAGE_FINISH
    !insertmacro MUI_UNPAGE_CONFIRM
    !insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
; Languages
    !insertmacro MUI_LANGUAGE "English"

;--------------------------------
; Installer Sections

Section "RocketcadManager" SecRocketcadManager
    SetOutPath "$INSTDIR"

    ; Check RegAsm
	IfFileExists "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe" yes_dot_net
    Abort "Aborted: .Net framework not found."

yes_dot_net:
    ; Add files
    File "..\RocketcadManagerLib\bin\Release\Newtonsoft.Json.dll"
    File "..\RocketcadManagerLib\bin\Release\RocketcadManagerLib.dll"
    File "..\RocketcadManagerLib\bin\Release\RocketcadManagerLib.dll"
    File "..\RocketcadManagerLib\bin\Release\System.IO.Compression.dll"

    File "..\RocketcadManager\bin\Release\RocketcadManager.exe"
    File "..\RocketcadManager\bin\Release\RocketcadManager.exe.config"
    File "..\RocketcadManager\bin\Release\RocketcadManager.pdb"
    
    File "..\RocketcadManagerPlugin\bin\Release\RocketcadManagerPlugin.dll"
    File "..\RocketcadManagerPlugin\bin\Release\RocketcadManagerPlugin.pdb"
    
    ; Store installation folder and write uninstaller
    WriteRegStr HKCU "Software\RocketcadManager" "" $INSTDIR
    WriteUninstaller "$INSTDIR\UninstallRCM.exe"

    ; Create start menu shortcuts
    !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
    CreateShortcut "$SMPROGRAMS\$StartMenuFolder\UninstallRCM.lnk" "$INSTDIR\UninstallRCM.exe"
    CreateShortcut "$SMPROGRAMS\$StartMenuFolder\RocketcadManager.lnk" "$INSTDIR\RocketcadManager.exe"
    !insertmacro MUI_STARTMENU_WRITE_END

	; Install
	SetOutPath $INSTDIR
    ; Register plugin with RegAsm
	ExecWait '"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe" /codebase "$INSTDIR\RocketcadManagerPlugin.dll"' $0
    IntCmp $0 0 register_success
    DetailPrint "Plugin registration failed"

register_success:
    DetailPrint "Plugin registered successfully"

SectionEnd

;--------------------------------
; Descriptions

    ; Language strings
    LangString DESC_SecRocketcadManager ${LANG_ENGLISH} "Test description."

    ; Assign language strings to sections
    !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
        !insertmacro MUI_DESCRIPTION_TEXT ${SecRocketcadManager} $(DESC_SecRocketcadManager)
    !insertmacro MUI_FUNCTION_DESCRIPTION_END

; --------------------------------
; Uninstaller Section

Section "Uninstall"

    ; Un-register plugin with RegAsm
    ExecWait '"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe" /u "$INSTDIR\RocketcadManagerPlugin.dll"'

    ; Delete saved settings and crash logs
    RMDir "$LocalAppdata\RocketcadManager"

    ; Delete program files
    Delete "$INSTDIR\Newtonsoft.Json.dll"
    Delete "$INSTDIR\RocketcadManagerLib.dll"
    Delete "$INSTDIR\RocketcadManagerLib.dll"
    Delete "$INSTDIR\System.IO.Compression.dll"

    Delete "$INSTDIR\RocketcadManager.exe"
    Delete "$INSTDIR\RocketcadManager.exe.config"
    Delete "$INSTDIR\RocketcadManager.pdb"
    
    Delete "$INSTDIR\RocketcadManagerPlugin.dll"
    Delete "$INSTDIR\RocketcadManagerPlugin.pdb"

    Delete "$INSTDIR\UninstallRCM.exe"
    RMDir "$INSTDIR"

    ; Delete start menu shortcuts
    !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
    Delete "$SMPROGRAMS\$StartMenuFolder\UninstallRCM.lnk"
    Delete "$SMPROGRAMS\$StartMenuFolder\RocketcadManager.lnk"
    RMDir "$SMPROGRAMS\$StartMenuFolder"
    
    DeleteRegKey HKCU "Software\RocketcadManager"

SectionEnd
