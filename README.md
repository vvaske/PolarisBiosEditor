## 🧾PolarisBiosEditor 1.7xml🧾
VBIOS editor fork with detailed dumping data as XML in console.<br>
The intended usage is **viewing** and/or help with **editing by external HEX editor**.<br>
Editing functionality is 99% identical to the upstream, no major new GUI features.

To view XML just open vbios file via file dialog and look in console/stdout.
To save XML to file:
* open cmd in folder containing PolarisBiosEditor.exe
* start program with stdout redirection, like<br>
`PolarisBiosEditor.exe path\to\input-file.rom > outfile.xml`
* close gui window to finish application and finalize/flush file writing

[Example XML output (large, ~1k lines)](./output-example.xml)

### 📉Changelog📉
v1.7xml-2021-08
 * Try partially parsing without crashing even unsupported VBIOSes

v1.7xml-2021-07
 * Fix exceptions while runing linux-built binary on windows
 * Dump all parsed data as valid xml
 * add dumping for generic option rom & EFI headers
 * add dumping of VDCCI-related tables, sligtly extend VDCCI viewing GUI

v1.7xml-2021-01
 * Introduce xml dump
 * Add parsing i2c and video-output realted tables

### 📎Download and run with [prebuilt cross-platform .Net exe file](./bin/Debug/PolarisBiosEditor.exe)📎
Please see ⛏️Build⛏️ section below and compile yourself if you don't trust prebuilt binary.
| Run on Windows | Run on Linux |
|:--------|:------|
|Requires only .Net 4, so just run exe file. |Contribution from Sebohe:<br><br>**Install mono** (and maybe libcanberra-gtk3-module, not sure):<br><ul><li> _Ubuntu/Debian_ `sudo apt-get install mono-complete`<br><li>_Arch Linux_ `yaourt -Sy mono48`</ul>With mono installed just change directory to the PolarisBiosEditor and<br>**execute `./run.sh`**<br><blockquote>see inside run.sh file for a bit more detailed dependencies info.<br>If mono is registered as .Net exe handler, just directly execute:`bin/Debug/PolarisBiosEditor.exe`</blockquote>|

### ⛏️Build⛏️
| Build on Windows | Build on Linux |
|:--------|:------|
| Install Visual Studio, open .sln file, build Debug configuration.      | Install mono and mono-mcs (to get `mcs` executable) and run<br>`sh build.sh` |


### ⏫Upstream (GUI-only version without XML dumping support)⏫
> <details>
>    <summary>Upstream info</summary>
> https://github.com/vvaske/PolarisBiosEditor
> 
> BTC donation address: 181dtEjhFWWxvHDmx2R3N41rnRPedSEUf5
> 
> one click timing feature should be used with care, it maybe not stable for you
> </details>
