nuget pack Data_Eval.nuspec -Symbols -SymbolPackageFormat snupkg
nuget push Data.Eval.2.6.1.nupkg -Source https://api.nuget.org/v3/index.json -apikey %NUGET_KEY%
pause