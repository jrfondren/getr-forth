#! /usr/bin/env gforth
require lib.fs
library libgetr ./libgetrusage.so
libgetr getrusage ptr (int) getrusage_children
libgetr getr-spawn ptr (int) getr_spawn
libgetr output-to-stderr (void) output_to_stderr

struct
   cell% field utime.sec
   cell% field utime.usec
   cell% field stime.sec
   cell% field stime.usec
   cell% field maxrss
   cell% field minflt
   cell% field majflt
   cell% field inblock
   cell% field oublock
   cell% field nvcsw
   cell% field nivcsw
end-struct rusage%

: rusage.ms ( rusage -- r-ms )
   dup  utime.sec  @ over stime.sec @ + s>d d>f 1000e f*
   over utime.usec @ swap stime.usec @ + s>d d>f 1000e f/ f+ ;

: report-usage ( rusage n-count -- )
   s>d d>f
   ." User time     : " dup utime.sec ? ." s, " dup utime.usec ? ." us" cr
   ." System time   : " dup stime.sec ? ." s, " dup stime.usec ? ." us" cr
   ." Time          : " dup rusage.ms fdup f. ." ms (" fswap f/ f. ." ms/per)" cr
   ." Max RSS       : " dup maxrss ? ." kB" cr
   ." Page reclaims : " dup minflt ? cr
   ." Page faults   : " dup majflt ? cr
   ." Block inputs  : " dup inblock ? cr
   ." Block outputs : " dup oublock ? cr
   ." vol ctx switches   : " dup nvcsw ? cr
   ." invol ctx switches : "     nivcsw ? cr ;

: .getrusage ( n-count -- )
   align here dup getrusage abort" getrusage failed" swap report-usage ;

: etype ( c-addr u -- )
   stderr write-file throw ;

: usage ( -- )
   s" usage: " etype
   0 arg etype
   s\"  <n> <command> [<args> ...]\n" etype
   1 (bye) ;

: alldigits ( c-addr u -- f )
   dup if
      bounds do
         i c@ [char] 0 [char] 9 1+ within 0=
         if false unloop exit then
      loop true
   else
      2drop false
   then ;

: #runs ( -- n )
   1 arg alldigits if
      1 arg evaluate
   else
      usage
   then ;

: go ( -- )
   argc @ case
      0 of usage endof
      1 of usage endof
      dup of
         #runs dup 0 ?do
            argv @ 2 cells + getr-spawn
            if s\" Error: posix_spawn() failed\n" etype 2 (bye) then
         loop
         output-to-stderr
         ( #runs ) .getrusage bye
      endof
   endcase ;

go
