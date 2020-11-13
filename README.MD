# Onshape Windows Sample

Onshape Windows Sample is a collection of sample projects which demonstrate Onshape OAuth authentication and Onshape REST API workflow on Windows platform. 

## Prerequisites

- [Visual Studio IDE](https://www.visualstudio.com/) (version 2019 has been tested and works)

- [Registered Onshape App](https://dev-portal.onshape.com/oauthApps).

Sample Onshape App registration parameters:

![AppRegistration](AppRegistration.PNG)

Make sure to save the client secret (visible in a toast after app registration) and the client id.

To specify a tab based extension click on the “Extensions” tab and “Add extension” in the [Developer Portal](https://dev-portal.onshape.com/)

	- Name: `<Enter extension name>`
	- Description(Optional): `<Enter extension description>`
	- Location: `Element Tab`
	- Action URL: `https://<url-from-heroku-create.herokuapp.com>
	- Icon(Optional): `<Drop an image to upload>`

You'll need to set some environment variables to connect your desktop app to the app created via the dev portal. 

Environment variables can be set in developer command prompt before launching Visual Studio:

```
>SET ONSHAPE_BASE_URI=https://cad.onshape.com 
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

- [ Onshape developper portal](https://dev-portal.onshape.com)
- [ Visual Studio Community Edition](https://www.visualstudio.com/en-us/news/vs2013-community-vs.aspx)
