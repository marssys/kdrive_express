
Copy the kdriveExpress_dotnet.dll and optional the kdriveExpress_dotnet.dll.xml (for intellisense) into the bin/{Platform e.g. Visual Studio 12 2013} directory from 
cmake build directory kdrive/bin/Release

kdrive_express_vs120.sln
	Solution file for Visual Studio 2013
	.NET Framework 4.5.1
	Platform Target x86
	
If you use a other .NET version or for other platform (e.g. 64bit) you'll have to change the C# project properties.

To create the intellisense file kdriveExpress_dotnet.xml you need to enable the xml documenation generation.
See http://msdn.microsoft.com/en-us/library/ms173501%28v=vs.90%29.aspx