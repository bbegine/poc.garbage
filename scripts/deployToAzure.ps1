dotnet build
dotnet publish .\src\Poc.Garbage.ServerApi\ -o publish --os linux
Compress-Archive -Path ./publish -DestinationPath ./publish.zip
# zip -j publish.zip .\publish\*


for ($i=1; $i -le 6; $i++) {
    $appService = "$ENV:poc_gc_app_prefix-$i"
    az webapp deployment source config-zip --src .\publish.zip -n $appService -g $ENV:poc_gc_resource_group_name
}
