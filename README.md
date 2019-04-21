# Spotlight-Images
A simple Windows Service to save new Spotlight images from Microsoft.


## Requirements
* Windows 10 1809 (Might work on older versions)
* The latest Visual Studio with the `.NET Desktop Development` 
workload installed. 


## Install
1. Modify the output directory location (in the `SpotlightImages.cs` 
file), else the service will fail to start.
2. Build the project with `Ctrl + Shift + B` or use the `Build` menu
3. From the Start menu, select the Visual Studio <version> directory, 
then select `Developer Command Prompt for VS <version>`.
4. Use the installutil tool to install the service
5. The binary is located in `bin/Release` directory 
6. Run `InstallUtil /u "Spotlight Images"` from the command 
prompt with the project's executable as a parameter:

<pre>
<b>C:\...\Community></b> where installutil
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe

We modify the above path to use the 64-bit version of the installutil (Framework -> Framework64)

<b>C:\...\Community></b> cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319\
<b>C:\...\v4.0.30319></b> installutil "full/path/to/project/bin/Release/Spotlight Images.exe"
</pre>


After install, the service runs in the background and auto-starts on 
system start-up `Automatic (Delayed Start)`, to prevent resource 
usage at logon). 

## Uninstall
<pre>
Still using the 64-bit installutil, do

<b>C:\...\v4.0.30319></b> installutil \u "full/path/to/project/bin/Release/Spotlight Images.exe"
</pre>

Follow instructions in the above link to 
remove _this and only this_ service.
More details on service installation/uninstallation can be found 
[here](https://docs.microsoft.com/en-us/dotnet/framework/windows-services/how-to-install-and-uninstall-services).  

## Additional Info
This process runs under the `LocalSystem` privileged user which
(as of now) seems to be the only way give the service the right
permissions to access the directory where the images are stored.

##### TODO:
* Add an installer to allow easy install/uninstall
* Allow user to specify save directory during install
* Find a way to run the service as a less privileged user

## License
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

Copyright © 2019  Mythreya K
