terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~3.62.1"
    }
  }
  
  backend "azurerm" {
    resource_group_name  = "Battleships_API_Group"
    storage_account_name = "battleshipstf"
    container_name       = "battleshipstfstate"
    key                  = "battleships.api.tfstate"
  }
}

provider "azurerm" {
  features {}
}

#Create Resource Group
resource "azurerm_resource_group" "tamops" {
  name     = "Ethan_Group_Test"
  location = "uksouth"
}