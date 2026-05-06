set LOG=c:\wintool\timesync.log
rem ŠÇ—ŽÒŒ ŒÀ‚ÅŽÀs
net session >NUL 2>nul
if %errorlevel% neq 0 (
 @powershell start-process %~0 -verb runas
 exit
)
rem timesyncŽÀs
timesync -sync >>%LOG%
