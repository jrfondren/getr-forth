#include <sys/time.h>
#include <sys/resource.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <errno.h>
#include <stddef.h>
#include <spawn.h>
#include <unistd.h>

extern char **environ;

int getrusage_children (long *vals) {
	struct rusage usage;

	if (-1 == getrusage(RUSAGE_CHILDREN, &usage)) {
		return -errno;
	} else {
		vals[0] = usage.ru_utime.tv_sec;
		vals[1] = usage.ru_utime.tv_usec;
		vals[2] = usage.ru_stime.tv_sec;
		vals[3] = usage.ru_stime.tv_usec;
		vals[4] = usage.ru_maxrss;
		vals[5] = usage.ru_minflt;
		vals[6] = usage.ru_majflt;
		vals[7] = usage.ru_inblock;
		vals[8] = usage.ru_oublock;
		vals[9] = usage.ru_nvcsw;
		vals[10] = usage.ru_nivcsw;
	}
	return 0;
}

int getr_spawn (char **argv) {
	int ret;
	int pid;

	if (0 != (ret = posix_spawn(&pid, argv[0], NULL, NULL, argv, environ)))
		return ret;
	if (-1 == (ret = waitpid(pid, NULL, 0)))
		return -errno;
	return 0;
}

void output_to_stderr (void) {
	dup2(2, 1);
}
