#!/bin/bash
mkdir -p bin/Debug
#mono-mcs is required for building
mcs -sdk:4.6 --mcs-debug 1 -debug -define:DEBUG -r:System.Drawing.dll -r:System.Windows.Forms.dll -r:System.Xml.Linq.dll PolarisBiosEditor.cs PolarisBiosEditor.Designer.cs -out:bin/Debug/PolarisBiosEditor.exe
