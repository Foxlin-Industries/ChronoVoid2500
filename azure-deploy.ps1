# Azure CLI (manual login required)
# az login
# az group create --name ChronoVoidRG --location eastus
# az appservice plan create --name ChronoVoidPlan --resource-group ChronoVoidRG --sku B1 --is-linux
# az webapp create --name ChronoVoidAPIApp --plan ChronoVoidPlan --resource-group ChronoVoidRG --runtime "DOTNETCORE:10.0"