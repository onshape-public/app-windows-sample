# Onshape Windows Sample

Onshape Windows Sample is a collection of sample projects which demonstrate Onshape OAuth authentication and Onshape REST API workflow on Windows platform. 

## Prerequisites

- [Visual Studio 2013](https://www.visualstudio.com/en-us/news/vs2013-community-vs.aspx) 

- [Registered Onshape App](https://dev-portal.dev.onshape.com/oauthApps).

Sample Onshape App registration parameters:

![AppRegistration](AppRegistration.PNG)

Base URL, Client Id, and Client Secret used by the sample are extracted from the environment variables. 

Environment variables can be set in developer command prompt before launching Visual Studio:

```
>SET ONSHAPE_BASE_URI=https://partner.dev.onshape.com 
>SET ONSHAPE_CLIENT_SECRET=TLLFGK56YJSL2... 
>SET ONSHAPE_CLIENT_ID=K5SUPTJ3NX3LUA2...  
```

or set globally in the configuration panel:

![AppRegistration](EnvironmentVariables.PNG)

## How to run

Open OnshapeWindowsSample.sln solution in Windows UI, or in developper command prompt:

```
>cd c:\dev\OnshapeWindowsSample
>devenv OnshapeWindowsSample.sln
```

Select "Debug > Start Debugging" in Visual Studio menue to start the sample.

Onshape authentication page in a browser should appear:

![Start](Start.PNG)

List of the user's document should be printed in JSON format, and interactive prompt should appear:

![Console](Console.PNG)

## Projects

- OnshapeClient - a .NET library, skeleton project which encapsulates Onshape REST API interface;

- OnshapeConsoleApp - .NET executable, a skeleton console application whch wraps some of the REST API entry points into commands exposed through the command line interface.

## References

- [ Onshape Partner Development](https://partner.dev.onshape.com/documents/8b803ff47462494dafecc822/w/3f7fd009244d434e87397169/e/a059a060a80a420ba7a4b534)
- [ Using OAuth 2.0 with Onshape](https://partner.dev.onshape.com/documents/8b803ff47462494dafecc822/w/3f7fd009244d434e87397169/e/b945dd68003a4ac692508ceb)
- [ Onshape partner development stack](https://partner.dev.onshape.com)
- [ Onshape developper portal](https://dev-portal.dev.onshape.com)
- [ Visual Studio Community Edition](https://www.visualstudio.com/en-us/news/vs2013-community-vs.aspx)
