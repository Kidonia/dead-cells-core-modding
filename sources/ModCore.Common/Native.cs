﻿using System.Runtime.InteropServices;

namespace ModCore
{
    internal static unsafe partial class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct HLU_stack_frame
        {
            public nint eip;
            public nint esp;
        }
        private const string MODCORE_NATIVE_NAME = "modcorenative";
        [LibraryImport(MODCORE_NATIVE_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static partial bool mcn_memory_readable( void* ptr );
        #region Stack Trace
        [LibraryImport(MODCORE_NATIVE_NAME)]
        public static partial int mcn_load_stacktrace( HLU_stack_frame* buf, int maxCount, void* stackBottom );
        [LibraryImport(MODCORE_NATIVE_NAME)]
        public static partial void* mcn_get_ebp();
        [LibraryImport(MODCORE_NATIVE_NAME)]
        public static partial void* mcn_get_esp();
        [LibraryImport(MODCORE_NATIVE_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static partial bool mcn_get_sym( void* ptr, char* symNameBuf, out int symNameLen,
            char* moduleNameBuf, ref int moduleNameBufLen,
            out byte* fileName, out int line );

        #endregion
        #region HL Utils

        [LibraryImport(MODCORE_NATIVE_NAME)]
        public static partial void* hlu_get_hl_bytecode_from_exe(
            [MarshalAs(UnmanagedType.LPWStr)] string path, int* outSize );
        [LibraryImport(MODCORE_NATIVE_NAME)]
        public static partial int hlu_start_game( void* code );

        #endregion
        #region HL CS Interop
        [LibraryImport(MODCORE_NATIVE_NAME)]
        public static partial void* get_asm_call_bridge_hl_to_cs();
        [LibraryImport(MODCORE_NATIVE_NAME)]
        public static partial void* hlu_call_c2hl( void* f, void* t, void** args, void* ret );
        #endregion
    }
}
