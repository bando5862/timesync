set LOG=c:\wintool\timesync.log
eco "### #t#n" >>%LOG%
timesync >> %LOG%
if %errorlevel% neq 0 (
  eco "rc=1 (to be adjusted)#n" >>%LOG%
  start "" cscript c:\wintool\SendKey.js timesync as1113{ENTER}
  start "timesync" runas /user:Administrator "timesync -sync"
)
