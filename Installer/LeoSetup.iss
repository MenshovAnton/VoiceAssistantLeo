; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Голосовой ассистент Лео"
#define MyAppVersion "1.1.0"
#define MyAppPublisher "MenshovAnton"
#define MyAppURL "http://voiceassistantleo.tilda.ws/ru/home"
#define MyAppExeName "Ассистент Лео.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{FB28AC3D-C88A-48A3-B952-06A7F23238C6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userappdata}\WasyoonProgramms\AssistantLeo
; "ArchitecturesAllowed=x64compatible" specifies that Setup cannot run
; on anything but x64 and Windows 11 on Arm.
ArchitecturesAllowed=x64compatible
; "ArchitecturesInstallIn64BitMode=x64compatible" requests that the
; install be done in "64-bit mode" on x64 or Windows 11 on Arm,
; meaning it should use the native 64-bit Program Files directory and
; the 64-bit view of the registry.
ArchitecturesInstallIn64BitMode=x64compatible
DefaultGroupName=Ассистент Лео
LicenseFile=C:\Users\Waysoon\Documents\LICENSE.txt
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=C:\Users\Waysoon\Desktop
OutputBaseFilename=LeoSetup-1.0.0
SetupIconFile=E:\CSharpProjects\AssistantLeo\Assets\icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Registry]
Root: HKCU; Subkey: "Software\AssistantLeo"; ValueType: dword; ValueName: "Language"; ValueData: "0"

[Files]
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Ассистент Лео.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Ассистент Лео.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Ассистент Лео.dll.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Ассистент Лео.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Ассистент Лео.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Ассистент Лео.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Assets\*"; DestDir: "{app}\Assets"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Logs"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\VoskModel\*"; DestDir: "{app}\VoskModel"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\en\*"; DestDir: "{app}\VoskModel"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Caliburn.Micro.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Hardcodet.NotifyIcon.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\libgcc_s_seh-1.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\libstdc++-6.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\libvosk.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\libwinpthread-1.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Microsoft.Windows.SDK.NET.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\NAudio.Asio.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\NAudio.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\NAudio.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\NAudio.Midi.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\NAudio.Wasapi.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\NAudio.WinForms.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\NAudio.WinMM.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\Vosk.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\bin\Release\AssistantLeo\WinRT.Runtime.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\CSharpProjects\AssistantLeo\Assets\Fonts\MontserratAlternates-Regular.ttf"; DestDir: "{autofonts}"; FontInstall: "Montserrat Alternates"; Flags: onlyifdoesntexist uninsneveruninstall
Source: "E:\CSharpProjects\AssistantLeo\Assets\Fonts\MontserratAlternates-Light.ttf"; DestDir: "{autofonts}"; FontInstall: "Montserrat Alternates Light"; Flags: onlyifdoesntexist uninsneveruninstall
Source: "E:\CSharpProjects\AssistantLeo\Assets\Fonts\MontserratAlternates-SemiBold.ttf"; DestDir: "{autofonts}"; FontInstall: "Montserrat Alternates SemiBold"; Flags: onlyifdoesntexist uninsneveruninstall

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

