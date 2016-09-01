cd build/output/
if test "$OS" = "Windows_NT"
then
    ./Gateway.exe
else
    mono Gateway.exe
fi
