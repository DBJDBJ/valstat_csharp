// (c) 2021 dbj@dbj.org -- free to test_simple_valstat at your own responsibility
// compile in RELEASE mode with Microsoft compiler (cl.exe) :
// cl /LD /MT /std:c11 myfun.c
#include <string.h>
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <mmsystem.h>

__declspec(dllexport)
int myFun(void* const pWaveFormatex)
{
	WAVEFORMATEX wfx = {
	.cbSize = 1,
	.nAvgBytesPerSec = 2,
	.nBlockAlign = 3,
	.nChannels = 4,
	.nSamplesPerSec = 5,
	.wBitsPerSample = 6,
	.wFormatTag = 7
	};

#ifdef _DEBUG
	WAVEFORMATEX* wfxp_ = (WAVEFORMATEX*)pWaveFormatex;
	(void)wfxp_;
#endif
	return memcpy(pWaveFormatex, &wfx, sizeof(wfx));
}
