# Pluto APIs Unity Package

## Description

A library for interacting with Pluto communication service using [these](https://app.swaggerhub.com/apis/Pluto-VR/pluto_apis) well documented APIs.

## Installation

In Unity, navigate to `Window` > `Package Manager` and click the `(+)` button to add a package from git url. Paste this url: `https://github.com/PlutoVR/pluto-apis-unity.git`

## Usage

1. Add the `Pluto APIs` prefab and the `PlutoCoordinateSpace` prefab to your scene
1. Make sure the `Pluto APIs` field in the `PlutoCoordinateSpace` instance has a reference to the `Pluto APIs` instance
1. Create a script with a reference to the `PlutoConnectionStatusChecker` in the `Pluto APIs` instance
   - This script should use the connection status checker to connect to a room at a sensible time e.g.:
     ```
     if (PlutoStatusChecker.IsLoggedIn) PlutoAPIs.Client.RoomsRoomIdJoin(RoomName);
     ```
1. Note the hierarchy of the `PlutoCoordinateSystem` instance, which looks like:
   ```
   PlutoCoordinateSystem
     RotationPivot
       PositionPivot
   ```
   **All content that you want to exist in the same space for all chat users should be children of the PositionPivot!**
1. Use the networking library of your choice to make sure all instances of this shared content have the same **local** transform data i.e. position, rotation, scale

### Making sure that you're receiving positional data
As of the current version of XRChat (2020.20.0), the following must be true for anchor transform data to be available from the API:
- the user must be in a room
- there must be at least one other user in the room
- both users must be in the session with headsets on or otherwise providing spatial data through their XR runtime (e.g. with a dummy driver that provides spatial transform data)

Given that last point especially, Pluto has some internal tooling that can be used to spoof a second user, especially for development purposes. Please reach out to us to learn more about this.

## Troubleshooting
If you're not receiving spatial data back from the APIs, check for the following:
- Does the XRChat process have a Node subprocess running? If not, try restarting the client, or manually run the apis executable at
  `<userdirectory>/AppData/Local/Pluto/app-<version>/resources/Pluto-APIs/plutoApis.exe`
- If the process is running, but attempts to hit the endpoint (see below) are timing out, try restarting the client, or kill the Node subprocess and manually run the apis executable
- If the process is running and attempts to hit the endpoint return a 404, see the previous section about making sure that you're receiving positional data
- If the process is running and attempts to hit the endpoint return data with a position and rotation, the APIs are functioning as expected

To see if you can get a response from the API, you can curl (or Postman, Insomnia, etc.) `http://localhost:12000/v2/conversation`

## Testing positional and rotational changes for users
If you'd like to force your users' spaces to move around so that you can make sure that data appears correctly from all perspectives, you can used the `pluto://rich-presence-update` URL that the chat client registers with your system.

This can be done by typing `start <url-with-urlencoded-params>` at a prompt. The following examples are from Powershell for the sake of easy URL encoding.

To set the user's position to e.g. (0, 0, 0):
`start pluto://rich-presence-update/$([system.uri]::EscapeDataString('{ "position" : {"x" : 0, "y" : 0, "z" : 0}}'))`

To set the user's rotation to e.g. 15 degrees (rotation is currently only around the y-axis):
`start pluto://rich-presence-update/$([system.uri]::EscapeDataString('{ "rotation" : 15 }'))`

NB: this method of setting rotation currently doesn't support a zero rotation. We just use 1 degree to get close enough. (todo: double-check this)
NB: this method of setting position only supports whole, positive coordinates. (todo: double-check this)

## Building Pluto.APIs.dll

1. [Download the C# Client SDK](https://support.smartbear.com/swaggerhub/docs/apis/generating-code/client-sdk.html) from the [Pluto APIs OpenAPI spec](https://app.swaggerhub.com/apis/Pluto-VR/pluto_apis)
2. Build the Client dll using the Visual Studio Solution in the just-downloaded ZIP
   * If the solution doesn't seem to find the NuGet packages when it's building - i.e. an error like `type or namespace newtonsoft could not be found` - uninstall and reinstall the same versions of the packages that can't be found. If Visual Studio won't let you uninstall those packages, you can [force Visual Studio to let you uninstall the package](https://stackoverflow.com/a/62719476).
1. [Install ILMerge](https://github.com/dotnet/ILMerge#installation)
   * You can also install from PowerShell using `Install-Package -Name ILMerge`, though you'll then have to use whatever path it's been installed to, e.g. `C:\Program Files\PackageManagement\NuGet\Packages\ILMerge.3.0.41\tools\net452\ILMerge.exe` instead of the path in the shell example below
1. Merge the output from step 1 into a single dll using ILMerge.

```shell
$ SET OUTPUT_DIR=\path\to\client\sdk\bin
$ "%USERPROFILE%\.nuget\packages\ilmerge\%ILMERGE_VERSION%\tools\net452\ILMerge.exe" /out:Plugins\Pluto.APIs.dll %OUTPUT_DIR%\Pluto.APIs.dll %OUTPUT_DIR%\Newtonsoft.Json.dll %OUTPUT_DIR%\JsonSubTypes.dll %OUTPUT_DIR%\RestSharp.dll /internalize /t:library
```
