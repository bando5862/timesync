set LOG=c:\wintool\timesync.log
eco "### #t#n" >>%LOG%
timesync >> %LOG%
if %errorlevel% neq 0 (
  call actual_timesync
)
