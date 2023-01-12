terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.38.0"
    }
  }

  backend "azurerm" {
    storage_account_name = "ehbsstorage"
    container_name       = "terraform-blob"
    key                  = "terraform.api.tfstate"
  }
}

provider "azurerm" {
  features {}
}

