--lua
local handle = findwindow("DeviceEmulator")[1][1]
workwindow (handle)
showwindow(handle, "TOP")
wait(120000)
sendex("f{Enter}")