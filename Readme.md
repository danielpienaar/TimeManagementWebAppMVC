## Time Management ASP MVC Web Application

<br>

***

_Author: Daniel Pienaar_

***

<br>

### How to run the program:

<br>

NOTE: All data is stored in an azure sql database, ensuring data persists after the program closes. The database connection is already set up, so you will just need an internet connection to run the application. The program is an asp.net core mvc web-app that will run in your browser.
If the azure db is down, you may create a local sql database, update the connection string in appsettings.json, then run "update-database" in the nuget package manager console.

<br>

* Please make sure you have .NET version 6 installed as this program uses that version. In the base folder, there are 2 project folders, one containing the application, and the other for the class library. Open the file named "TimeManagementWebApp.sln" in the TimeManagementWebApp folder to see the application in Visual Studio. 
* Click the "Start Without Debugging" button at the top of the editor, or alternatively press ctrl+f5. The web app should now appear in a browser tab.
* To access the app, login with an existing account or register a new one. There should be a test account with some data in it already. The Student Id is "ST100" and the password is "test" (This only applies if connected to the azure database).
* For more information about the usage of the app, please see the user manual.

<br>

***
