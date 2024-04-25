# group "Shared Resources"{
    sharedDatabase = container "Shared Database" "Lorem...." "SQL Server" {
        tags "Database"
        s1 = component "Core Schema"
        s2 = component "Identity Schema"
    }

    sharedMessaging = container "Shared Messaging" "Lorem...." "RabbitMQ" {
        tags "Messaging"
    }
# }