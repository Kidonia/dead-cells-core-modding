﻿
#pragma once

#include <stdlib.h>
#include <malloc.h>
#include <string.h>

#ifdef WIN32
#include <Windows.h>
#else
#endif

#include "hl.h"
#include "hlmodule.h"

typedef void(*real_hl_global_init)(void);

#ifdef __cplusplus
#define EXTERNC extern "C"
#else
#define EXTERNC
#endif

typedef unsigned long long int64;

typedef void(*hl2cs_callback)(void* table, void* retVal, int64* args, void** hl_error);

typedef struct
{
	int retType; /* 2: return int64 in x86, 1: return float/double, 0: return ptr/int32/int16/byte/others */
	int hasFloatArg; /* 1: Has float/double return value 0: hasn't */
	int argFloatMarks; /* bit marks : 1 means the parameter is float/double, otherwise it is int/ptr */
	void* origFunc;
	int argsCount;
	hl2cs_callback callback;
} hl2c_table;

typedef struct
{
	void* eip;
	void* esp;
} hlu_stack_frame;

EXTERNC void* get_ebp();
EXTERNC void* get_esp();

EXTERNC void debug_break();

EXTERNC void* asm_call_bridge_hl_to_cs();

EXTERNC extern void* call_jit_c2hl;
EXTERNC extern void* call_jit_hl2c;

HL_PRIM void* callback_c2hl(void* f, hl_type* t, void** args, vdynamic* ret);

EXTERNC void vsd_init(hl_module* module);
EXTERNC void init_trace();
