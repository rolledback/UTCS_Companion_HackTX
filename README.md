UTCS Windows 8 Companion App
============================
Version 1.0

By:  
[Chris Sheahan](https://github.com/csheahan)  
[Matthew Rayermann](https://github.com/rolledback)  
[Jonathan Lee](https://github.com/chenclee)

This app was made in a 24 hour hackathon known as [HackTX](https://www.hackerleague.org/hackathons/hacktx/).  

Inspired from an idea of an android app of the old CS labs (before the new Gates-Dell Complex at UT), we set out to make a companion application for Windows 8 to interactively help users optimize finding computers in the lab, both remotely and physically. Having no prior experience of C#, VisualStudios, and Windows 8 development, it was an interesting challenge both front end and back.

What Is It?
-----------
Our companion app runs on Windows 8, and has 2 main features.  

1. The ability to look at all the machines, including the basement lab, 3rd floor lab, and headless machines. The information gathered for these machines are the machine name, floor it is on if it is physically occupied, the total amount of users logged in (physical + ssh), and the load averages (1 min, 5 min, and 15 min).
2. The second feature is to look at all the printers and see their queue's, including how many documents are in the queue and the total amount of bytes left to be printed. This information is useful because there may only be one job on a floor but it could be many documents and bytes, making it more efficient to use another printer.

Challenges Faced
----------------
On the front end: The obvious challenge here was our overall lack of C# knowledge, coupled with no knowledge of Windows 8. This led to some tasks which would have been trivial in other cases being 10-40 minute ordeals just to write around 5 lines of code. Thanks to the drag and drop nature of the layouts for Windows 8 and Microsoft evangelists around we were able to make it work.  
On the back end: On the back end we faced 2 main challenges: the fact that we needed to run Unix commands within the UTCS network to get machine information and the solution to that problem, which was being able to get information from our Perl scripts. The solution was a simple fix which involved making the Perl file a .scgi file.

Version History
---------------
1.0 - HackTX presentation ready. Features lab availability and printer status.
