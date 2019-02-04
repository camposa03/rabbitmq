Testing...

####
Gotchas
Subscibers
######

In order to create async subscribers, make sure you pass in the DispatchAsync flag when constructing
the connection factory


#To build the project

$>dotnet publish

##Install windows service


Run the "sc" command

$> sc query <name_of_service>


Delete the service

$> sc delete <name_of_service>

install the service

$> sc create <name_of_service> binPath= <path_to_exe_file>


$> sc create rabbitSubscriber binPath= "C:\Users\Armando\Projects\dotnet\Epic.Messaging.Consumer\Epic.Messaging.Consumer\bin\Debug\netcoreapp2.1\win7-x64\publish\apphost.exe"


sc create rabbitSubscriber binPath= "C:\Users\Armando\Projects\dotnet\Epic.Messaging.Consumer\Epic.Messaging.Consumer\bin\Debug\netcoreapp2.1\win7-x64\publish\Epic.Messaging.Consumer.exe"