(rmdir bin); (rmdir obj); (dotnet build); (echo $(Get-Date -format "mm:ss.fffffff")); (dotnet run --no-build)
(rmdir bin); (rmdir obj); (dotnet publish -r win-x64 --sc -p:PublishSingleFile=true); (echo $(Get-Date -format "mm:ss.fffffff")); (.\bin\Debug\net6.0\win-x64\publish\publish-test.exe)