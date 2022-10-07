# ReferenceManager 🛠️
A F# console app to replace PackageReferences with ProjectReferences in an entire solution.

![image](https://user-images.githubusercontent.com/52143624/194674263-811512d7-00f3-4417-a701-7eb632d81cba.png)

# Why?
Imagine the following situation, you have several NuGet libraries and projects that depend on them. When you need to debug the NuGet library in multiple projects of a solution, you will need to manually remove and add the reference for each project. The purpose of this application is to automate this process.
# Limitations
- Only solutions with projects with the new SDK style are supported. (Sad for me too, because I created this project thinking about non-SDK projects 😢)
- For now, only SDK-style .csprojs are supported for the ProjectReference input. If you need .vbproj and .fsproj support, you can ask me for this improvement or make a pull request
# Installation

## Building from source
> .NET 6 required.
```
git clone https://github.com/gumbarros/ReferenceManager .
dotnet build
dotnet run
```
## Downloading and running the cross-platform binary
Download from the [releases](https://github.com/gumbarros/ReferenceManager/releases/tag/v1.0.0) page.
Run from your OS the console
```
./ReferenceManager
```

If you want a single file binary, build from the source for your platform.
