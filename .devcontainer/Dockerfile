# [Choice] .NET version: 6.0, 5.0, 3.1
ARG VARIANT="6.0"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}-focal

# [Choice] Node.js version: none, lts/*, 16, 14, 12, 10
ARG NODE_VERSION="none"
RUN if [ "${NODE_VERSION}" != "none" ]; then su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && nvm install ${NODE_VERSION} 2>&1"; fi

# Install SQL Tools: SQLPackage and sqlcmd
# COPY installSQLtools.sh ./library-scripts/installSQLtools.sh
# RUN bash ./installSQLtools.sh \
     # && apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts

# Install dotnet tools
RUN dotnet tool install -g dotnet-ef
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH $PATH:/root/.dotnet/tools