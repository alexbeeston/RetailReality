- log scrap report to console, database, and/or file?
- load seeds from a database table?
- what should we do if the name of the product changes?
- on a worker, get metrics for how many offers it is parsing per second. (average them out; will hopefully help with optimization)
- Get a list of brands; pull in as a global variable, then do a string.contains until you find a match.
- switch to UTC time (store as UTC, then translate to local time for client)
- balance the search criteria configurations so that roughly, all search criteria yield the same number of search results
- log scrap reports?

# Items from testing
- can't count on Reg. to show unloaded products; "Reg." was a valid label on a pair of boots
- if no alternate (or primary) price, pass NULL to the individual price
- give search criteria an ID
- rename individual price
- employ logging