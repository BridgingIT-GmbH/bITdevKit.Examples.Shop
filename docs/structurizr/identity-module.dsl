tags "Module"

# str = component "Document Storage" {
# }
# msg = component "Messaging" {
# }

group "Presentation Layer" {
    tokenController = component "Token Controller" "Lorem..." "REST Controller"
    accountController = component "Account Controller" "Lorem..." "REST Controller"
}
group "Application Layer" {
    cqs = component "Commands/Queries" "Lorem..." "CQS"
}

group "Domain Layer" {
}

group "Infrastructure Layer" {
    datacontext = component "Data Context" "Allows interactions with the data store" "Entity Framework"
}

tokenController -> cqs "Sends"
accountController -> cqs "Sends"
cqs -> datacontext "Uses"
datacontext -> sharedDatabase.s2 "Reads from and writes to"