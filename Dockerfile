FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# Copy everything and restore
COPY . ./
RUN dotnet publish ./src/IssueOpenerLabeler/IssueOpenerLabeler.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="Eilon Lipton <elipton@microsoft.com>"
LABEL repository="https://github.com/eilon/IssueOpenerLabeler"
LABEL homepage="https://github.com/eilon/IssueOpenerLabeler"

LABEL com.github.actions.name="IssueOpenerLabeler"
LABEL com.github.actions.description="A Github action that sets issue/PR labels based on who created it"
LABEL com.github.actions.icon="alert-circle"
LABEL com.github.actions.color="yellow"

# Build the runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "/IssueOpenerLabeler.dll" ]
