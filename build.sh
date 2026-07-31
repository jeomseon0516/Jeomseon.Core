#!/usr/bin/env bash
set -euo pipefail

repository_root="$(cd "$(dirname "$0")" && pwd)"
configuration="${CONFIGURATION:-Release}"
version="${VERSION:-0.1.0}"
project="$repository_root/Source~/Jeomseon.Core/Jeomseon.Core.csproj"
source_directory="$repository_root/Source~/Jeomseon.Core"
tests="$repository_root/Tests~/Jeomseon.Core.Tests/Jeomseon.Core.Tests.csproj"
boundary_validation="$repository_root/Tools~/BoundaryValidation/BoundaryValidation.csproj"
artifacts="$repository_root/artifacts~/nuget"
plugins="$repository_root/Runtime/Plugins"

dotnet test "$tests" --configuration "$configuration"
dotnet pack "$project" --configuration "$configuration" --output "$artifacts" \
  -p:Version="$version"

mkdir -p "$plugins"
assembly="$repository_root/Source~/Jeomseon.Core/bin/$configuration/netstandard2.1/Jeomseon.Core.dll"
documentation="$repository_root/Source~/Jeomseon.Core/bin/$configuration/netstandard2.1/Jeomseon.Core.xml"
cp "$assembly" "$plugins/"
cp "$documentation" "$plugins/"

dotnet run --project "$boundary_validation" --configuration Release -- \
  "$project" "$source_directory" "$assembly"
