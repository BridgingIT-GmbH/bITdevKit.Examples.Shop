workspace {
    # http://localhost:4044/workspace/diagrams
    # https://github.com/structurizr/dsl/blob/master/docs/language-reference.md
    # https://github.com/structurizr/dsl/tree/master/docs/cookbook
    !identifiers hierarchical

    model {
        properties {
            # needed for nested froups
            "structurizr.groupSeparator" "/"
        }

        group "Users" {
            customer = person "Shop Customer" "A customer of the shop"
            admin = person "Shop Admin" "An administrator of the shop" "External Person"
        }
        pim = softwaresystem "PIM" "The centralized platform that manages all product-related information, such as descriptions, images, pricing, and other data. It is designed to help businesses manage their product information more efficiently and effectively" "Existing System"
        seq = softwaresystem "SEQ" "The search, analysis, and alerting system for structured log data" "Existing System"

        shopSystem = softwareSystem "Shop System" "The software system designed to manage a retail shop's operations. It includes a client-facing SPA for customers to browse and purchase products, as well as a server-side web application handling the requests" {
            group "Frontend" {
                clientapp = container "Client SPA" "Lorem...." "HTML/JavaScript" "Web Browser"
            }

            group "Backend" {
                webapp = container "Server Web Application" "Lorem...." "ASP.NET" {
                    component "Authentication"
                    component "Middleware"
                    component "Logging"
                    component "Tracing"
                }

                !include shared-resources.dsl

                # modules
                coreModule = container "Core Module" "Lorem...." "Module" {
                    !include core-module.dsl
                }

                identityModule = container "Identity Module" "Lorem...." "Module" {
                !include identity-module.dsl
                }
            }

            # relationships
            customer -> clientapp "Visits"
            admin -> clientapp "Visits"
            admin -> seq "Investigate log data from"
            clientapp -> webapp "Uses"
            webapp -> coreModule "Consists of"
            webapp -> identityModule "Consists of"
            webapp -> seq  "Sends log data to"
        }
    }

    views {
        systemlandscape "System_Landscape" {
            include *
            autoLayout lr
        }

        container shopSystem "Shop_System" {
            include *
            autolayout lr
        }

        component shopSystem.webapp {
            include *
            autolayout lr
        }

        component shopSystem.coreModule "Core_Module" {
            include *
            autolayout lr
        }

        component shopSystem.identityModule "Identity_Module" {
            include *
            autolayout lr
        }

        component shopSystem.sharedDatabase "Database_Schemas" {
            include *
            autolayout lr
        }

        styles {
            element "Person" {
                background #08427b
                color #ffffff
                fontSize 22
                shape Person
            }
            element "External Person" {
                background #999999
                color #ffffff
                fontSize 22
                shape Person
            }
            element "Software System" {
                background #1168bd
                color #ffffff
            }
            element "Existing System" {
                background #999999
                color #ffffff
            }
            element "Container" {
                background #438dd5
                color #ffffff
            }
            element "Component" {
                background #85bbf0
                color #000000
            }
            element "Module" {
                background #85bbf0
                color #000000
            }
            element "Web Browser" {
                shape WebBrowser
            }
            element "Database" {
                shape Cylinder
            }
            element "Messaging" {
                shape Pipe
            }
            element "Group" {
                color #08427b
            }
        }
    }
}