﻿using Hashlink;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModCore.Hashlink
{
    public static unsafe class HashlinkUtils
    {
        private readonly static Dictionary<nint, string> stringsLookup = [];
        private readonly static Dictionary<string, nint> name2hltype = [];

        private readonly static Dictionary<nint, nint> funcNativePtr = [];
        private readonly static Dictionary<nint, Dictionary<string, nint>> name2func = [];
        
        public static IReadOnlyDictionary<string, nint> GetHashlinkTypes()
        {
            return name2hltype;
        }


        internal static void Initialize(HL_code* code, HL_module* m)
        {
            for (int i = 0; i < code->ntypes; i++)
            {
                var g = code->types + i;
                string name;

                
                if(g->data.obj == null)
                {
                    continue;
                }

                if(g->kind == HL_type.TypeKind.HOBJ)
                {
                    var n = g->data.obj->name;
                    name = GetString(n);

                }
                else if(g->kind == HL_type.TypeKind.HENUM)
                {
                    name = GetString(g->data.tenum->name);
                }
                else
                {
                    continue;
                }
                
                if(string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                Log.Logger.Verbose("Type: {name}", name);
                name2hltype[name] = (nint)g;
            }
            for(int i = 0; i < code->nfunctions; i++)
            {
                var f = code->functions + i;
                var fp = m->functions_ptrs[f->findex];
                

                if(!name2func.TryGetValue((nint)f->type->data.obj, out var funcTable))
                {
                    funcTable = [];
                    name2func.Add((nint)f->type->data.obj, funcTable);
                }

                var name = GetString(f->field);
                Log.Logger.Verbose("Func: {name} {ptr:x} {index}", name, (nint)fp, f->findex);

                funcTable[name] = (nint)f;
                funcNativePtr[(nint)f] = (nint) fp;
            }
        }

        public static string GetString(char* ch)
        {
            //It is too bad, but I dont know how to do
            if ((nint)ch < 0x1ffff)
            {
                return "(null)";
            }
            if (stringsLookup.TryGetValue((nint)ch, out var s))
            {
                return s;
            }
            return stringsLookup[(nint)ch] = new string(ch);
        }
        public static string GetString(byte* ch, int num, Encoding encoding)
        {
            if (stringsLookup.TryGetValue((nint)ch, out var s))
            {
                return s;
            }
            return stringsLookup[(nint)ch] = encoding.GetString(ch, num);
        }
        public static HL_type* FindTypeFromName(string name)
        {
            return (HL_type*) name2hltype[name];
        }
        
        public static HL_function* FindFunction(HL_type* type, string name)
        {
            if(!name2func.TryGetValue((nint)type->data.obj, out var table) ||
                !table.TryGetValue(name, out var result))
            {
                return null;
            }
            return (HL_function*)result;
        }
        public static void* GetFunctionNativePtr(HL_function* func)
        {
            if(!funcNativePtr.TryGetValue((nint)func, out var result))
            {
                return null;
            }
            return (void*)result;
        }

        public static int HLHash(string str)
        {
            int h = 0;
            fixed (char* pname = str)
            {
                char* name = pname;
                while (*name != 0)
                {
                    h = 223 * h + (int) name;
                    name++;
                }
            }
            return h;
        }
    }
}
