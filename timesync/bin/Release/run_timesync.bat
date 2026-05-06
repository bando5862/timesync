set LOG=c:\wintool\timesync.log
eco "### #t#n" >>%LOG%
timesync >> %LOG%
if %errorlevel% neq 0 (
 rem バッチを管理者権限で実行
 powershell start-process  actual_timesync.bat -verb runas
)
