tags "Module"

# jbs = component "Job Scheduling" {
# }
# msg = component "Messaging" {
# }

group "Presentation Layer" {
    coreController = component "Core Controller" "Allows the client to interact with the backend" "REST Controller"
}

group "Application Layer" {
    cqs = component "Commands/Queries" "All Commands and Queries for the module" "CQS"
    jobs = component "Jobs" "All Jobs for the module" "Jobs"
    group "Jobs"{
        job1 = component "Job 1" "Job 1" "Job"
    }
}

group "Domain Layer" {
    dmm = component "Domain Model" "The Domain Model for the module" "Domain Model"
    repo = component "Repository" "The repositories for this domain" "Repository"
}

group "Infrastructure Layer" {
    datacontext = component "Data Context" "Allows interactions with the data store" "Entity Framework"
    dataadapter = component "Data Adapter" "Allows the PIM data to be used by the domain" "Adapter"
}

coreController -> cqs "Sends"
jobs -> cqs "Sends"
cqs -> dataadapter "Uses"
cqs -> dmm "Uses"
cqs -> repo "Uses"
repo -> datacontext "Uses"

dataadapter -> pim "Import product catalog data from"
datacontext -> sharedDatabase.s1 "Reads from and writes to"
cqs -> sharedMessaging "Publishes messages to"