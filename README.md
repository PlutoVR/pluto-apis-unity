# Pluto APIs Unity Package

## Description

A library for interacting with Pluto communication service using [these](https://app.swaggerhub.com/apis/Pluto-VR/pluto_apis) well documented APIs.

## Installation

In Unity, navigate to `Window` > `Package Manager` and click the `(+)` button to add a package from git url. Paste this url: `https://github.com/PlutoVR/pluto-apis-unity.git`

## Usage

1. Add the `Pluto APIs` prefab to your scene
1. Create a new script that references the PlutoAPIs script on the prefab
1. Use the `Client` propery on the PlutoAPIs script to make API calls

## Building Pluto.APIs.dll

1. [Build the client SDK](https://support.smartbear.com/swaggerhub/docs/apis/generating-code/client-sdk.html) from the [Pluto APIs OpenAPI spec](https://app.swaggerhub.com/apis/Pluto-VR/pluto_apis)
1. [Install ILMerge](https://github.com/dotnet/ILMerge#installation)
1. Merge the output from step 1 into a single dll using ILMerge.

```shell
$ SET OUTPUT_DIR=\path\to\client\sdk\bin
$ "%USERPROFILE%\.nuget\packages\ilmerge\%ILMERGE_VERSION%\tools\net452\ILMerge.exe" /out:Plugins\Pluto.APIs.dll %OUTPUT_DIR%\Pluto.APIs.dll %OUTPUT_DIR%\Newtonsoft.Json.dll %OUTPUT_DIR%\JsonSubTypes.dll %OUTPUT_DIR%\RestSharp.dll /internalize /t:library
```