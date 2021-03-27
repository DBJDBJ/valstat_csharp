//
// (c) 2021 dbj@dbj.org -- free to test_simple_valstat at your own responsibility
// 
// compile in RELEASE mode with Microsoft compiler (cl.exe) :
// cl /LD /MT /std:c11 myfun.c
#include <string.h>
#define WIN32_LEAN_AND_MEAN
#include <windows.h>

// mmsystem requires windows be included 
// before it
#include <mmsystem.h>

/*
* C# equivalent:

	   [StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct WAVEFORMATEX
		{
			public ushort wFormatTag;
			public ushort nChannels;
			public uint nSamplesPerSec;
			public uint nAvgBytesPerSec;
			public ushort nBlockAlign;
			public ushort wBitsPerSample;
			public ushort cbSize;
		}

		[DllImport(@"myfun.dll", EntryPoint = "myFun")]
		static extern int myFun(out WAVEFORMATEX wfx);
*/
__declspec(dllexport)
int waveformat(void* const pWaveFormatex)
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
// to observe from debugger
	WAVEFORMATEX* wfxp_ = (WAVEFORMATEX*)pWaveFormatex;
	(void)wfxp_;
#endif
	return memcpy(pWaveFormatex, &wfx, sizeof(wfx));
}

///////////////////////////////////////////////////////////////////
/// https://stackoverflow.com/a/47648504/10870835
