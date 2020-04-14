Author:     Gabriel Job && CS3500 staff
Partner:    N/A
Date:       3/13/20
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  gabejob
Commit ID: 59ee2fc2dffdc276d7f971a4a38b53f9703b8fea
Repo:       https://github.com/uofu-cs3500-spring20/agar.io
Assignment: Assignment # - A8 Agario
Copyright:  CS 3500 and Gabe- This work may not be copied for use in Academic Coursework.

1. Comments to Evaluators:

**KNOWN BUGS** =>Did not receive reply on, bugs a direct result of using new server/networking dll's
-Player jittering
-Freeze upon holding space bar to split.

**OTHER INFO**
I've commented out my eat sound, as I didn't have time to implement botht the background music and another sound playing at once.
If the client for some reason doesn't load any picture but is still running updates on the status board, the only thing I've found fixes it is to restart
everything...


2. Assignment Specific Topics
Any additional feedback or write up required by the assignment.

Bonus features:
-grid lines
-background music
-death sound

Design choices:

I decided to keep the area where the actual gameplay happens seperate from the rest of my GUI, this allowed for easier coordinate transfer 
and a bit more abstraction, I used multiple different events to trace communication between that and my GUI class.

I felt I followed DRY really well in this assignment, making quite a few helper methods that resulted in cleaner bug free code. I do feel that
I could've seperated the projects into even smaller compenents, but alas, time constraints.

Testing:

For testing I felt it increasingly effective to just simply run manual UI tests, including having my little brother attempt to play (and break) the game.
I think unit tests *could* be used to better ensure the quality of the code, but I felt if everything was displaying correctly; 
there probably wasn't any critical bugs.


2.5. Time keeping

I estimated that this assignment would take roughly 20 hours to implement AND bugtest (NICE TRY HAHA).

Session 1: 7 hours

Session 2: 3 hours

Session 3: 4 hour

Session 4: 55 mins

Session 5: 4 hours

Session 6: 4 hours

Session 7: 6 hours

+ a few misc sessions under an hour

Total time ~ 28 Hours


My time estimate was quite a bit off, as I led myself down a wrong path early with transcribing coordinates...

3. Consulted Peers:
N/A

4. References:

https://stackoverflow.com/questions/31840902/my-fps-counter-is-in-accurate-any-ideas-why
https://www.dotnetperls.com/timespan
https://www.codeproject.com/Questions/541560/PanelplusBackgroundplusimageplusmoving
https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
https://stackoverflow.com/questions/6240002/play-two-sounds-simultaneusly
https://www.online-convert.com/result/1d29b20b-4dcb-4b06-9f19-eb0040c87f4a
https://www.fesliyanstudios.com/royalty-free-music/downloads-c/8-bit-music/6
https://stackoverflow.com/questions/58766399/cannot-access-disposed-object-on-socket-close
https://dotnetcoretutorials.com/2018/04/12/using-the-ilogger-beginscope-in-asp-net-core/
Various MSdocs
https://stackoverflow.com/questions/39174989/how-to-register-multiple-implementations-of-the-same-interface-in-asp-net-core
https://www.tutorialsteacher.com/core/fundamentals-of-logging-in-dotnet-core
