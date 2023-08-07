# Use the official .NET Core SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build-env

# Set the working directory inside the container
WORKDIR /app/apps/Api.ChatSv

# Copy the .NET Core API project file(s) to the container
COPY apps/Api.ChatSv/*.csproj ./
COPY ./libs /app/libs

# Restore NuGet packages
RUN dotnet restore

# Copy the rest of the application code
COPY ./apps/Api.ChatSv ./

# Build the application inside the container
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal

# Set the working directory inside the container
WORKDIR /app

# Copy the built application from the previous stage
COPY --from=build-env /app/apps/Api.ChatSv/out .

# Expose the port the API will listen on (change it if your API uses a different port)
EXPOSE 5000
EXPOSE 7000
ENV ASPNETCORE_URLS=http://+:5000;http://+:7000

# Define the command to run your .NET Core API (change it if your entry point is different)
ENTRYPOINT ["dotnet", "ApiChatSv.dll"]