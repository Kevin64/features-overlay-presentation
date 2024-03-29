# Features Overlay Presentation (FOP)

<img align="right" width="150" height="150" src=https://github.com/Kevin64/features-overlay-presentation/assets/1903028/883287c7-5ecf-4ac7-8081-b08914e40aa0 />

FOP is a program designed to present to the user some tutorial or step-by-step instructions of any subject, in order to educate and teach the workforce about any procedure. The software allows to be executed after user logon if desired, enforcing them to watch all steps and enabling a timer between each step. Afterwards, the user can watch again, but this time without the timer restraints.

For IT admins: FOP allows to register in [APCS](https://github.com/Kevin64/asset-and-personnel-control-system) the service delivery made, triggering an logon execution. That way, each slide will have a one-time 4 second timer, to make sure that it is fully read by the user.

## Screens

### Relaunch window:

![fop-relaunch](https://github.com/Kevin64/features-overlay-presentation/assets/1903028/4a970955-7194-4b57-850f-02d42dd8f838)

### Main window:

![fop-main-window](https://github.com/Kevin64/features-overlay-presentation/assets/1903028/acd347eb-5cc4-4991-ae87-94005ff49a78)

### Output in APCS

![Firefox_Screenshot_2023-12-07T18-32-59 888Z](https://github.com/Kevin64/features-overlay-presentation/assets/1903028/6af9df12-fbf9-464f-93da-bbc681afc0f1)

## Creating a custom installer

To customize a FOP installation, you have to:
1. Install [Inno Setup](https://jrsoftware.org/download.php/is.exe).
2. Download FOP's [installer script file](https://github.com/Kevin64/features-overlay-presentation/blob/master/FeaturesOverlayPresentation/custom-installer/installerInno.iss).
3. Download the [zip file](https://github.com/Kevin64/features-overlay-presentation/releases/latest) containing the compiled FOP files.
4. Create a folder somewhere and copy the downloaded installer script (`installerInno.iss`) file to it.
5. Extract the zip file into the recently created directory (folder structure must be `Newly-created-folder\Release\...`) and then put your custom PNG slides and assets inside `Release\resources`, replacing the existing ones (make sure you keep the same file names). PNG images inside `Release\resources\img\` must have a prefix number before their file names (e.g. 00 - filename0, 01 - filename1, etc), which will serve as a parameter to sort the picture presentation inside FOP. The latter string (e.g. filename0, filename1, etc) will show up as the slide title, helping users to navigate and view a specific image.
6. Modify the desired details inside the `config.json` file present in `Release\`.
7. Finally, open `installerInno.iss`, customize the branding and other settings if necessary, and then select Build -> Compile. The final installation executable should show up inside your newly created folder.
8. Deploy FOP in your machines.

## Build from scratch

If you want to build FOP from source code, you'll also need to clone its [Dependencies](https://github.com/Kevin64/Dependencies).
