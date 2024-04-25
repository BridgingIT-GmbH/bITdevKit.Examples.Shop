run the following command with PowerShell from the project folder to start the Structurizr docker container:

`docker run -it --rm -p 8080:8080 -v $PWD/docs/structurizr:/usr/local/structurizr structurizr/lite`

more information about Structurizr (DSL):
- https://www.youtube.com/watch?v=4HEd1EEQLR0
- https://dev.to/simonbrown/getting-started-with-structurizr-lite-27d0