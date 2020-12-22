dotnet tool install --global dotnet-sonarscanner

dotnet sonarscanner begin /k:"data-eval" /o:"bruce-dunwiddie-github" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.login=%SONARQUBE_TOKEN% /d:sonar.cs.opencover.reportsPaths="bin\Debug\CodeCoverageResult.xml" /d:sonar.verbose=true /d:sonar.log.level=TRACE

dotnet build ..\Data_Eval.sln

%userprofile%\.nuget\packages\opencover\4.7.922\tools\OpenCover.Console.exe -target:"%ProgramFiles%\dotnet\dotnet.exe" -targetargs:"test Tests.csproj"  -filter:"+[*Eval*]*" -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" -register:user -output:"bin\Debug\CodeCoverageResult.xml" -oldStyle

dotnet sonarscanner end /d:sonar.login=%SONARQUBE_TOKEN%

pause
