# PWP SPRING 2019
# Limping Data of Patients
## Requirements
* .NET Core 2.1.3 or Higher (https://dotnet.microsoft.com/download/dotnet-core/2.1)
* Visual Studio 2017 (https://visualstudio.microsoft.com/vs/older-downloads/)
* Docker (https://www.docker.com/get-started)

## How to run
* Run in a Command Prompt "docker-compose up" to create the postgresql database.
* Restoring dependencies should happen automatically when you run the application
* Run by pressing F5

## How to run alternatively
* Run in Command Prompt "docker-compose -f docker-compose-deploy.yml up". (It will be exposed in http://localhost:8090)
* This can fail the first time since it will build the database the first time. First time you need to run it and then stop it (CTRL + C) then run it again.

## Dependencies
* Docker, Docker-compose (https://docs.docker.com/docker-for-windows/install/)
* Swachbuckle (https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio) 
* HALcyon (https://github.com/visualeyes/halcyon)
* .NET CORE Framework libraries
* .NET Unit Test Runner and Code Coverage Tool https://www.jetbrains.com/dotcover/

# Group information
* Jonada Ferracaku jonada.ferracaku@student.oulu.fi
* Parsa Sharmila parsa.sharmila@student.oulu.fi
* Ciprian Florea ciprian.florea@oulu.fi


