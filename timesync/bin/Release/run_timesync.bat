set LOG=c:\wintool\timesync.log
eco "### #t#n" >>%LOG%
timesync >> %LOG%
if %errorlevel% neq 0 (
 rem “¯Šú‚ğŠÇ—ÒŒ ŒÀ‚ÅÀs
 powershell Start-Process  actual_timesync.bat -Verb runas
)
