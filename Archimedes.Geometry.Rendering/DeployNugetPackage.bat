echo "Deploying nuget package"

del *.nupkg

nuget pack Archimedes.Geometry.Rendering.csproj -IncludeReferencedProjects -Prop Configuration=Release

echo "Pushing nuget package to nuget.org..."

nuget push *.nupkg

echo "Success!"