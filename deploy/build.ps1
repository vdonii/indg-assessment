aws configure set region eu-west-1
dotnet run --project ../src/INDG.Image.Service.Deploy/INDG.Image.Service.Deploy.csproj -- $args
exit $LASTEXITCODE;