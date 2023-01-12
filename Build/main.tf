resource "azurerm_resource_group" "rg" {
  name     = "battleships-api-rg"
  location = "westeurope"
}

resource "azurerm_app_service_plan" "appserviceplan" {
  name                = "battleships-service-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku {
    tier = "Basic"
    size = "B1"
  }
}

resource "azurerm_app_service" "webapp" {
  name                = "battleships-api"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.appserviceplan.id

  site_config {
    cors {
      allowed_origins     = ["https://bsstaticstorage.z6.web.core.windows.net"]
      support_credentials = true
    }
  }

  connection_string {
    name  = "Db-ConnString"
    type  = "SQLAzure"
    value = "Server=tcp:${azurerm_mssql_server.sql-server.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.sql-db.name};Persist Security Info=False;User ID=${azurerm_mssql_server.sql-server.administrator_login};Password=${azurerm_mssql_server.sql-server.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }

  logs {
    application_logs {
      file_system_level = "Error"
    }
    http_logs {
      file_system {
        retention_in_days = "5"
        retention_in_mb   = "35"
      }
    }
    detailed_error_messages_enabled = true
    failed_request_tracing_enabled  = true

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
  name                        = "bs-sql-db"
  server_id                   = azurerm_mssql_server.sql-server.id
  max_size_gb                 = 8
  min_capacity                = 0.5
  sku_name                    = "GP_S_Gen5_1" //sku name specifies the db is 'serverless'
  auto_pause_delay_in_minutes = 60
  tags = {
    environment = "production"
  }
}
