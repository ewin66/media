#ifndef __UTILITIES_H__
#define __UTILITIES_H__

#ifdef __cplusplus
extern "C" {
#endif

typedef void (*MngCbFunction)(void * arg);
#if defined(WIN32)
#include <windows.h>
#include <assert.h>
#include <conio.h>
typedef HANDLE MngSem;
typedef HANDLE MngThread;
typedef LPTHREAD_START_ROUTINE MngThreadFunction;
typedef struct MngPipe_s{
	HANDLE in, out, sem;
	int data_size;
} MngPipe;
typedef HANDLE MngMutex;
#elif defined(linux) //defined(WIN32)
#include <unistd.h>
#include <sys/time.h>
#include <sys/ioctl.h>
#include <poll.h>
#include <errno.h>
#include <semaphore.h>
#include <pthread.h>
#include <malloc.h>
typedef struct MngSem_s{
  pthread_mutex_t tmutex;
  pthread_cond_t tcond;
  sem_t sem;
} *MngSem;
typedef pthread_t MngThread;
typedef void *(*MngThreadFunction)(void*);
typedef struct MngPipe_s{
  int inout[2];
  int data_size;
} *MngPipe;
typedef struct MngMutex_s{
  pthread_mutex_t tmutex;
  pthread_cond_t tcond;
  pthread_mutex_t mutex;
} *MngMutex;
#elif defined(__vxworks) //defined(WIN32), elif defined(linux)
#include <semLib.h>
#include <taskLib.h>
#include <msgQLib.h>
typedef SEM_ID MngSem;
typedef int MngThread;
typedef FUNCPTR MngThreadFunction;
typedef struct MngPipe_s{
	MSG_Q_ID queue;
	int data_size;
} MngPipe;
typedef SEM_ID MngMutex;
#elif defined(__SVR4) //defined(WIN32), elif defined(linux), elif defined(__SVR4)
#include <unistd.h>
#include <sys/time.h>
#include <sys/ioctl.h>
#include <sys/filio.h>
#include <poll.h>
#include <errno.h>
#include <semaphore.h>
#include <pthread.h>
#include <malloc.h>
typedef struct MngSem_s{
  pthread_mutex_t tmutex;
  pthread_cond_t tcond;
  sem_t sem;
} *MngSem;
typedef pthread_t MngThread;
typedef void *(*MngThreadFunction)(void*);
typedef struct MngPipe_s{
  int inout[2];
  int data_size;
} *MngPipe;
typedef struct MngMutex_s{
  pthread_mutex_t tmutex;
  pthread_cond_t tcond;
  pthread_mutex_t mutex;
} *MngMutex;
#endif //defined(WIN32), elif defined(linux), elif defined(__vxworks), elif defined(__SVR4)

void MngSemCreate(MngSem * sem, int initial_value);
void MngSemDestroy(MngSem sem);
int MngSemTake(MngSem sem, int timeout);
void MngSemGive(MngSem sem);
int MngSemPoll(MngSem sem);
void MngMutexCreate(MngMutex * mutex, char * name);
void MngMutexDestroy(MngMutex mutex);
int MngMutexTake(MngMutex mutex, int timeout);
void MngMutexGive(MngMutex mutex);
int MngMutexPoll(MngMutex mutex);
void MngThreadCreate(MngThread * thread, MngThreadFunction function, void * param);
void MngThreadDestroy(MngThread thread);
void MngPipeCreate(MngPipe * pipe, int data_size);
void MngPipeDestroy(MngPipe pipe);
int MngPipeRead(MngPipe pipe, void * data, int timeout);
void MngPipeWrite(MngPipe pipe, void * data);
int MngPipePoll(MngPipe pipe);
void MngSleep(int milliseconds);
void MngWaitKeyPress();

#ifdef __cplusplus
}
#endif

#endif //__UTILITIES_H__
