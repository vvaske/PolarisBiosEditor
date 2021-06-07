#!/bin/bash
SELF=`realpath "$0"`
SELF_DIR=`dirname "$SELF"`
cd "$SELF_DIR/bin/Debug"

# Required mono libs (at least):
#  libmono-system-windows-forms4.0-cil
#  libmono-system-xml-linq4.0-cil

mono PolarisBiosEditor.exe
