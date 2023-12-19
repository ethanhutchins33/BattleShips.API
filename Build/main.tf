resource "azurerm_resource_group" "rg" {
  name     = "battleships-api-rg"
  location = "westeurope"
}

resource "azurerm_service_plan" "appserviceplan" {
  name                = "battleships-service-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Windows"
  sku_name            = "B1"
}

resource "azurerm_windows_web_app" "webapp" {
  name                = "battleships-api"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.appserviceplan.id

  site_config {
    cors {
      allowed_origins     = ["https://bsstaticstorage.z6.web.core.windows.net"]
      support_credentials = true
    }

    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v8.0"
    }
  }

  connection_string {
    name  = "Db-ConnString"
    type  = "SQLAzure"
    value = "Server=tcp:${azurerm_mssql_server.sql-server.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.sql-db.name};Persist Security Info=False;User ID=${azurerm_mssql_server.sql-server.administrator_login};Password=${azurerm_mssql_server.sql-server.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}

resource "azurerm_mssql_server" "sql-server" {
  name                         = "bs-sql-server"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.SQL_SERVER_USERNAME
  administrator_login_password = var.SQL_SERVER_PASSWORD

  tags = {
    environment = "production"
  }
}

resource "azurerm_mssql_database" "sql-db" {
  name        = "bs-sql-db"
  server_id   = azurerm_mssql_server.sql-server.id
  max_size_gb = 10
  sku_name    = "S0"
  tags        = {
    environment = "production"
  }
}
