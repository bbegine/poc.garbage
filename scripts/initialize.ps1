# Create resource group
az group create --name $ENV:poc_gc_resource_group_name --location $ENV:poc_gc_location

# Create App service Plan
az appservice plan create `
    --name $ENV:poc_gc_asp_name `
    --resource-group $ENV:poc_gc_resource_group_name `
    --is-linux `
    --sku P1V3

# Create App Services
for ($i=1; $i -le 6; $i++) {
    $appService = "$ENV:poc_gc_app_prefix-$i"

    az webapp create `
        --name $appService `
        --resource-group $ENV:poc_gc_resource_group_name `
        --plan $ENV:poc_gc_asp_name `
        --runtime "DOTNETCORE:8.0"
}