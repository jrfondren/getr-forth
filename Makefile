libgetrusage.so: getrusage.c
	$(CC) -fPIC -shared -o $@ $<

clean::
	rm -fv libgetrusage.so

test::
	./getr.fs 100 ./hello.fs >/dev/null
