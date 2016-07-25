#r "packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

let buildDir = "./build/output"

Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "BuildApp" (fun _ ->
    !! "Gateway/**/*.fsproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "
)

"Clean"
  ==> "BuildApp"

RunTargetOrDefault "BuildApp"
