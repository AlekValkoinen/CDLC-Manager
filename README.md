The controls should be pretty simple. So this is mostly a beta version at this time.

The backup function does nothing at current. These feature is working in the previous version, however this modern refactor it is not completed yet.

The transfer is currently not multi-threaded, I plan to make the file handling into an iDisposable pass in the list, handle it on another thread, rather than static like the previous versions.

There are still some situations where it may throw exceptions. However I'm currently going back through, the switch to a new framework was quick and dirty.
