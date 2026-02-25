# Build Error Example for issue 56434
https://github.com/Azure/azure-sdk-for-net/issues/56434

## Solution Includes 
- NugetPackage
	- Class Library project mulit-targeting .NET 8.0, 9.0 *and potentially 10.0 (not included)*
	- Package Reference: Azure.Core 1.51.1 -> System.ClientModel 1.9.0 -> Microsoft.Extensions.Logging.Abstractions 10.0.2
- Net8Web
	- Simple ASP.NET Core Web project targeting .NET 8.0
	- Includes custom logger (program.cs) *- imagine this comes from nuget package...*
	- Project Reference to NugetPackage *- imagine this is an actual nuget package...*
		- *remove project reference to see project build/work*

## Build Error
```
1>------ Build started: Project: NugetPackage, Configuration: Debug Any CPU ------
1>NugetPackage -> C:\PATH\BuildError56434\NugetPackage\bin\Debug\net8.0\NugetPackage.dll
1>NugetPackage -> C:\PATH\BuildError56434\NugetPackage\bin\Debug\net9.0\NugetPackage.dll
2>------ Build started: Project: Net8Web, Configuration: Debug Any CPU ------
2>C:\PATH\BuildError56434\Net8Web\Program.cs(20,2,20,15): error CS0433: The type 'ProviderAliasAttribute' exists in both 'Microsoft.Extensions.Logging.Abstractions, Version=10.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60' and 'Microsoft.Extensions.Logging, Version=8.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60'
2>Done building project "Net8Web.csproj" -- FAILED.
========== Build: 1 succeeded, 1 failed, 0 up-to-date, 0 skipped ==========
========== Build completed at 9:12 AM and took 06.032 seconds ==========

```

## Expected Behavior
Existing projects should build successfully without any type conflicts. 
The presence of the same type 'ProviderAliasAttribute' in both 'Microsoft.Extensions.Logging.Abstractions' (v10) and 'Microsoft.Extensions.Logging' (v8/v9) causes compile error.