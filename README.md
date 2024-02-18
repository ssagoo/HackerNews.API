# HackerNews.API

A RESTFul API to retrieve best 'n' stories from the 'hacker-news.firebaseio.com' API

## Contains the following projects

1. HackerNews.API.Application - Contains all of the underlying business logic, DTO's and controllers to query the Hacker News API
2. HackerNews.API.Host - A light-weight hosting REST API project containing a WebHost context and some init code
3. HackerNews.API.Client.Tester - A console application to test the Rest API by sending a large volume of requests
4. HackerNews.API.Application.IntegrationTests - A set of integration tests to call the underlying firebaseio HackerNew API
5. HackerNews.API.Application.Tests - A set of unit-tests to test core functionality only

While all projects are being hosted by the "HackerNews.API.sln" solution file and can be built as follows:
```
dotnet build
```
NOTE: Run the above command from powershell or a command prompt and from the solution folder

## HackerNews.API.Host

The hosting console application can be launched to start the IIS web service with the required port number.

### Launcher information

Use the default profile "http" to launch using the url 'http://localhost:5270' and swagger page.

You can also launch the web api directly from the console:
```
dotnet run --project .\HackerNews.API.Host
```

NOTE: You can also first launch from Visual Studio the HackerNews.API.Host project and use the 'HackerNews.API.Host.http' file to call the API with the given parameters

### Available app settings:

HackerNewApiUrl - The base url used for the underlying firebaseio hacker news api.

## HackerNews.API.Client.Tester

The console application which will send requests to the hosted HackerNew API instance.

### Available Parameters

The following command line arguments can be passed to the console application:

-u or --baseUri	- The Base Url for the Hacker News API Host, Default = "http://localhost:5270/api/"
-c or --count	- The Total number of concurrent requests to send, Default = 10
-s or --stories	- Per batch number of stories to retrieve, Default = 5

### Example run parameters

```
cd .\HackerNews.API.Client.Tester\bin\Debug\net8.0
.\HackerNews.API.Client.Tester.exe -c 10 -s 5 -u "http://localhost:5270/api/"
```

## Assumptions

- Requires Visual Studio 2022 and .NET 8.0/SDK
- Defaults to port 5270 and http target when launching from VS.2022
- Assuming anonymous login to the api only, i.e. no authentication
- Rate limiter strategy is total allowable concurrent requests - defaults to 5
- Basic exception handling added to catch HTTP errors when calling firebaseio API

## Enhancements

- Better exception handling and reporting to the client
- Using the .net 8.0 rate limiter instead to protect the Hacker New API from large number of requests
- More unit test coverage
- Add better logger support such as log4net or serilog