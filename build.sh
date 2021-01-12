#!/bin/bash
mkdir -p bin/Debug
#mono-mcs is required for building
mcs -debug -define:DEBUG -unsafe -lib:/usr/lib/mono/4.0 -r:System.Drawing.dll -r:System.Windows.Forms.dll -r:System.Xml.Linq.dll PolarisBiosEditor.cs PolarisBiosEditor.Designer.cs -out:bin/Debug/PolarisBiosEditor.exe
