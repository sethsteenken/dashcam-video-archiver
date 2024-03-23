| Command    | Code | Param | Description |
| ---------- | ---- | ----- |------------ |
| Get Version  | 3012 | NA | Get project version. |
| Query Status | 3014 | NA | Get status/value for all available commands |
| File List | 3015 | NA | List all files sorting by file creation time. |
| Heartbeat | 3016 | NA | Check if exist. Returns result if alive, otherwise no return. |
| Get Disk Free Space | 3017 | NA | Get free space of file storage on SD in bytes. |
| Reconnect WiFi | 3018 | NA | Disconnect and reconnect WiFi |
| Get SD Card Status | 3024 | NA | Get SD card status. |
| Delete File | 4003 | str=<filepath> | Delete file from SD card. |
| Get File Info | 4005 | Use url of file path | Get file information including width, height, and length. |
| Force Shutdown | 8251 | NA | Forces dashcam to shutdown |
| Auto Power Off | 3007 | 0 = On, 1,2,3,4 (5 mins),5 (10 mins), 6 (off setting max) | Set system auto power off time |
