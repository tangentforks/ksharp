---
name: remove-dead-code
description: Identify and removedead code
---


Edit the *.csproj file for the target project, e.g., K3CSharp/K3CSharp.csproj
[ ] Add `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` under `<PropertyGroup>` 
[ ] Add `IDE0051` In `<WarningsAsErrors>` (e.g., if it is empty, `<WarningsAsErrors />` change it to `<WarningsAsErrors>IDE0051</WarningsAsErrors>`)
[ ] run `dotnet clean` to clean the project
[ ] run `dotnet build` to build the project
[ ] resolve any `IDE0051` warnings:
    [ ] run a search to confirm the unused code is not used elsewhere
    [ ] remove the unused code, or the entire file if it is unused
[ ] repeat cycles of `dotnet build` and resolve any `IDE0051` warnings until you reach a clean build with no more `IDE0051` warnings
[ ] remove `IDE0051` from `<WarningsAsErrors>`
[ ] remove `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` under `<PropertyGroup>`