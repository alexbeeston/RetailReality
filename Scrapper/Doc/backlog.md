﻿- could we prevent having to reload the page by passing the number of items to load? 
- globalize the list<int> in processPage so that it's cleaner (?)
- decide how many pings we should wait before we decide that the hub is not responding.
- figure out how to exit gracefully if the hub isn't responding (let the current tasks finish, then exit ???)
- clean up enums (are there unused values?)