; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "FOP"
#define MyAppVersion GetFileVersion('D:\kevin\OneDrive\Documentos\GitHub\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\FOP.exe')
#define MyAppPublisher "Unidade de Tecnologia da Informação - CCSH - UFSM"
#define MyAppURL "https://www.ufsm.br/unidades-universitarias/ccsh/unidade-de-tecnologia-da-informacao/"
#define MyAppExeName "FOP.exe"
#define RegKey "Software\FOP"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{17E466E2-67ED-46AF-9BAE-C4C466E3744A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
CreateAppDir=yes
DefaultDirName={autopf}\{#MyAppName}
DisableDirPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
;PrivilegesRequiredOverridesAllowed=dialog
OutputBaseFilename=FOPsetupCCSH-{#MyAppVersion}-Win7
OutputDir=D:\kevin\OneDrive\Documentos\GitHub\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\Output
Compression=lzma
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#MyAppExeName}
VersionInfoVersion={#MyAppVersion}
MinVersion=6.1sp1
OnlyBelowVersion=6.2

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Dirs]
Name: "{app}\img"

[Files]
Source: "D:\kevin\OneDrive\Documentos\GitHub\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\FOP.pdb"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\FOP.exe.config"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\FOP.exe"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\img\*"; DestDir: "{commonpf32}\{#MyAppName}\img"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "D:\kevin\OneDrive\Documentos\GitHub\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\Rever tutorial de uso do computador.lnk"; DestDir: "{commondesktop}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\Rever tutorial de uso do computador.lnk"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\RunOnce"; ValueType: string; ValueName: FOP; ValueData: "{commonpf32}\{#MyAppName}\Rever tutorial de uso do computador.lnk"; Flags: uninsdeletekeyifempty

[Registry]
Root: HKCU; Subkey: "Software\FOP"; ValueType: DWORD; ValueName: DidItRunAlready; ValueData: "0"; Flags: uninsdeletekeyifempty

[UninstallDelete]
Type: filesandordirs; Name: "{commonpf32}\{#MyAppName}"

[Code]

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    if RegKeyExists(HKEY_CURRENT_USER, '{#RegKey}') then
        RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER, '{#RegKey}');
  end;
end;

{ ///////////////////////////////////////////////////////////////////// }
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;


{ ///////////////////////////////////////////////////////////////////// }
function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;


{ ///////////////////////////////////////////////////////////////////// }
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
{ Return Values: }
{ 1 - uninstall string is empty }
{ 2 - error executing the UnInstallString }
{ 3 - successfully executed the UnInstallString }

  { default return value }
  Result := 0;

  { get the uninstall string of the old app }
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

{ ///////////////////////////////////////////////////////////////////// }
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;

{ ///////////////////////////////////////////////////////////////////// }
function IsDotNetDetected(): boolean;
var
    key: string;
    install, release: cardinal;
    success: boolean;
begin
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full'
    
    // success: true if the registry has been read successfully
    success := RegQueryDWordValue(HKLM, key, 'Install', install);
    success := success and RegQueryDWordValue(HKLM, key, 'Release', release);

    // install = 1 if framework is installed
    // 461808 -> .NET 4.7.2 461814 before Win10 April 2018 Update
    // 528040 -> .NET 4.8 528040
    // see https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
    // for details
    result := success and (install = 1) and (release >= 528040);
end;

function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected() then begin
        MsgBox('Este programa requer o Microsoft .NET Framework 4.8 ou superior.'#13#13
            'Por favor, instale-o e execute este programa novamente.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;