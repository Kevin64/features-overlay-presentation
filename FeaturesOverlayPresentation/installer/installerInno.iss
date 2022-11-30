; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "FOP"
#define MyAppVersion GetFileVersion('D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\TutorialDeUsoDaEstaçãoDeTrabalho.exe')
#define MyAppPublisher "Subdivisão de Tecnologia da Informação - CCSH - UFSM"
#define MyAppURL "https://www.ufsm.br/unidades-universitarias/ccsh/unidade-de-tecnologia-da-informacao/"
#define MyAppExeName "TutorialDeUsoDaEstaçãoDeTrabalho.exe"
#define RegKey "Software\FOP"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{774C846A-339E-4668-9AB4-78D769F29CAB}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName}-{#MyAppVersion}
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
OutputBaseFilename=FOPsetupCCSH-{#MyAppVersion}
OutputDir=D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\Output
Compression=lzma
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#MyAppExeName}
VersionInfoVersion={#MyAppVersion}
MinVersion=6.1sp1

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Dirs]
Name: "{app}\img-windows11"
Name: "{app}\img-windows10"
Name: "{app}\img-windows7"

[Files]
Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\TutorialDeUsoDaEstaçãoDeTrabalho.exe.config"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\TutorialDeUsoDaEstaçãoDeTrabalho.exe"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\Newtonsoft.Json.dll"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\Microsoft.Xaml.Behaviors.dll"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\ConstantsDLL.dll"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\LogGeneratorDLL.dll"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\INIFileParser.dll"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\JsonFileReaderDLL.dll"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\definitions.ini"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\img-windows11\*"; DestDir: "{commonpf32}\{#MyAppName}\img-windows11"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\img-windows10\*"; DestDir: "{commonpf32}\{#MyAppName}\img-windows10"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\img-windows7\*"; DestDir: "{commonpf32}\{#MyAppName}\img-windows7"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\Rever tutorial de uso do computador.lnk"; DestDir: "{commondesktop}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\Rever tutorial de uso do computador.lnk"; DestDir: "{commonstartmenu}"; Flags: ignoreversion

Source: "D:\kevin\OneDrive\Documentos\GitHub\C#\FeaturesOverlayPresentation\FeaturesOverlayPresentation\bin\Release\Rever tutorial de uso do computador.lnk"; DestDir: "{commonpf32}\{#MyAppName}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

;[Registry]
;Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\RunOnce"; ValueType: string; ValueName: FOP; ValueData: "{commonpf32}\{#MyAppName}\Rever tutorial de uso do computador.lnk"; Flags: uninsdeletekeyifempty

[Registry]
Root: HKCU; Subkey: "Software\FOP"; ValueType: DWORD; ValueName: DidItRunAlready; ValueData: "1"; Flags: uninsdeletekeyifempty

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
var
    Version: TWindowsVersion;
    S: String;

begin
    GetWindowsVersionEx(Version);

    // Disallow installation on Home edition of Windows
    if Version.SuiteMask and VER_SUITE_PERSONAL <> 0 then
    begin
      SuppressibleMsgBox('Este programa não pode ser instalado na versão Home do Windows',
        mbCriticalError, MB_OK, IDOK);
      result := False;
      Exit;
    end;

    // Disallow installation on Windows 8.x
    if Version.Major = 6 then
    begin
      if Version.Minor = 3 then
      begin
        SuppressibleMsgBox('Este programa não pode ser instalado no Windows 8.1',
          mbCriticalError, MB_OK, IDOK);
        result := False;
        Exit;
      end;
      if Version.Minor = 2 then
      begin
        SuppressibleMsgBox('Este programa não pode ser instalado no Windows 8',
          mbCriticalError, MB_OK, IDOK);
        result := False;
        Exit;
      end
    end;

    if not IsDotNetDetected() then begin
        MsgBox('Este programa requer o Microsoft .NET Framework 4.8 ou superior.'#13#13
            'Por favor, instale-o e execute este programa novamente.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;