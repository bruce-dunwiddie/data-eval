%userprofile%\.nuget\packages\opencover\4.7.922\tools\OpenCover.Console.exe -target:"%ProgramFiles%\dotnet\dotnet.exe" -targetargs:"test Tests.csproj"  -filter:"+[*Eval*]*" -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" -register:user -output:"bin\Debug\CodeCoverageResult.xml" -oldStyle
%userprofile%\.nuget\packages\reportgenerator\4.2.17\tools\net47\ReportGenerator.exe "-reports:bin\Debug\CodeCoverageResult.xml" "-targetdir:bin\Debug\CodeCoverageReport"
pause
