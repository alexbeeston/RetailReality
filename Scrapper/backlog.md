- what should we do if the name of the product changes?
- on a worker, get metrics for how many offers it is parsing per second. (average them out; will hopefully help with optimization)
- Get a list of brands; pull in as a global variable, then do a string.contains until you find a match.
- switch to UTC time (store as UTC, then translate to local time for client)
- balance the search criteria configurations so that roughly, all search criteria yield the same number of search results
- Is there a max number of records that be inserted on a single insert command?


# From testing
- log "just parsed page 1 of x" on logging
- wrap pressing next button in try/catch and if catch, the flush what you have, log the error, then move on.
- headless mode
