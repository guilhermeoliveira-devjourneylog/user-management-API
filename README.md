# user-management-API

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio Code](https://code.visualstudio.com/) or any other IDE of your choice

### Running the Project

1. Clone the repository:
    ```sh
    git clone https://github.com/your-username/user-management-API.git
    cd user-management-API/UserManagementAPI
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

3. Run the project:
    ```sh
    dotnet run
    ```

4. Open your browser and navigate to `http://localhost:5011/swagger` to see the Swagger UI.

### Building the Project

1. Build the project:
    ```sh
    dotnet build
    ```

2. Publish the project:
    ```sh
    dotnet publish -c Release -o ./publish
    ```

The published files will be available in the `./publish` directory.

### Running with Visual Studio Code

1. Open the project in Visual Studio Code:
    ```sh
    code .
    ```

2. Open the terminal in Visual Studio Code and run the project:
    ```sh
    dotnet run
    ```

3. Alternatively, you can use the built-in debugger by pressing `F5`.

### Environment Variables

Make sure to set the following environment variables for JWT configuration:

- `JWT_SECRET_KEY`
- `JWT_ISSUER`
- `JWT_AUDIENCE`

You can set these variables in the [launchSettings.json](http://_vscodecontentref_/1) file or in your system environment variables.

### Testing the API

You can use the provided [UserManagementAPI.http](http://_vscodecontentref_/2) file to test the API endpoints using [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension in Visual Studio Code.

### Additional Information

For more information on ASP.NET Core, visit the [official documentation](https://docs.microsoft.com/en-us/aspnet/core/).