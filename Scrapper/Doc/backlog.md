﻿- could we prevent having to reload the page by passing the number of items to load? 
- decide how many pings we should wait before we decide that the hub is not responding.
- figure out how to exit gracefully if the hub isn't responding (let the current tasks finish, then exit ???)
- clean up enums (are there unused values?)
- what should we do if the name of the product changes?
- on a worker, get metrics for how many offers it is parsing per second. (average them out; will hopefully help with optimizing the grid)
- Get a list of brands; pull in as a global variable, then do a string.contains until you find a match.
- can we let MySQL manage the IDs of the derived prices?
- switch to UTC time (store as UTC, then translate to local time for client)