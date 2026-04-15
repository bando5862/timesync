timesync
if %errorlevel% neq 0 (
  runas /user:Administrator "timesync -sync"
)
pause
