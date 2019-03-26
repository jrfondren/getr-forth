# getrusage() wrapper
- known to work on Linux
- created as my simple "time for x in {1..100}; ..." benchmarks were a lot less pleasant on OpenBSD.

## Forth notes
- this is a Forth translation of the C version at https://github.com/jrfondren/getr
- this works in gforth 0.7.3, and requires libffi and gcc for some C
  compilation that'll happen on the first run (in addition to compilation of
  the libgetrusage.so library)
- getr.fs expects to find libgetrusage.so in the current directory. For
  installation, only line 3 needs to be updated.

## build
```
make
```

## usage and examples
```
$ ./getr.fs 100 ./hello.fs >/dev/null
User time     : 0 s, 207005 us
System time   : 0 s, 105921 us
Time          : 312.926 ms (3.12926 ms/per)
Max RSS       : 4004 kB
Page reclaims : 39811 
Page faults   : 0 
Block inputs  : 0 
Block outputs : 0 
vol ctx switches   : 98 
invol ctx switches : 15 

$ ./getr.fs 100 $(which python3) -c 'print("Hello, world!")' >/dev/null
User time     : 1 s, 614826 us
System time   : 0 s, 297112 us
Time          : 1911.938 ms (19.11938 ms/per)
Max RSS       : 8608 kB
Page reclaims : 97849 
Page faults   : 0 
Block inputs  : 0 
Block outputs : 0 
vol ctx switches   : 100 
invol ctx switches : 32 

$ ./getr.fs 100 $(which python2) -c 'print "Hello, world!"' >/dev/null
User time     : 0 s, 582854 us
System time   : 0 s, 218034 us
Time          : 800.888 ms (8.00888 ms/per)
Max RSS       : 6872 kB
Page reclaims : 84054 
Page faults   : 0 
Block inputs  : 0 
Block outputs : 0 
vol ctx switches   : 80 
invol ctx switches : 82 
```

## defects and room for improvement
- no $PATH resolution occurs
- output is in an ad-hoc text format that machine consumers would need to parse manually
- only posix\_spawn is used, but fork&exec might be preferred for timings more like a fork&exec-using application
- 'getr' is probably a poor name
- kB and ms are always used even when number ranges might be easier to understand in MB or s, or GB or min:s
- RSS units are probably wrong on macOS
