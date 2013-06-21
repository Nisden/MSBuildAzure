MSBuildAzure
============

Simple build task for using Azure during a MSBuild

Usage
-------------------------

```xml
<UsingTask TaskName="BlobUpload" AssemblyFile="MSBuildAzure.dll" />
<BlobUpload Container="$root" ConnectionString="" Files="TestFile.txt" />
```

Files can also be a ItemGroup