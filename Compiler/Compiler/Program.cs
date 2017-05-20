using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Compiler
{
    class Program
    {
        static bool GenerateMethod(ILGenerator gen, string code)
        {
            var memory = gen.DeclareLocal(typeof(byte[]));
            var n = gen.DeclareLocal(typeof(int));
            //create array
            gen.Emit(OpCodes.Ldc_I4, 30000);
            gen.Emit(OpCodes.Newarr, typeof(byte));
            gen.Emit(OpCodes.Stloc, memory);
            //create indexer
            gen.Emit(OpCodes.Ldc_I4, 0);
            gen.Emit(OpCodes.Stloc, n);
            int length = code.Length;

            Stack<Label> labelsStart = new Stack<Label>();
            Stack<Label> labelsEnd = new Stack<Label>();
            
            int lN = 0;
            for(int i = 0; i < length; i++) 
            {
                switch(code[i])
                {
                    case '[':
                    {
                        gen.Emit(OpCodes.Ldloc, memory);
                        gen.Emit(OpCodes.Ldloc, n);
                        gen.Emit(OpCodes.Ldelem, typeof(byte));
                        gen.Emit(OpCodes.Ldc_I4_0);
                        var stLabel = gen.DefineLabel();
                        labelsStart.Push(stLabel);
                        var endLabel = gen.DefineLabel();
                        labelsEnd.Push(endLabel);

                        gen.Emit(OpCodes.Beq, endLabel);
                        gen.MarkLabel(stLabel);
                    }
                    break;

                    case ']':
                    {
                        gen.Emit(OpCodes.Ldloc, memory);
                        gen.Emit(OpCodes.Ldloc, n);
                        gen.Emit(OpCodes.Ldelem, typeof(byte));
                        gen.Emit(OpCodes.Ldc_I4_1);
                        gen.Emit(OpCodes.Bge, labelsStart.Pop());
                        gen.MarkLabel(labelsEnd.Pop());
                    }
                    break;

                    case '+':
                    {
                        int count = 1;
                        while(i < length - 1 && code[++i] == '+') count++;
                        i--;

                        gen.Emit(OpCodes.Ldloc, memory);
                        gen.Emit(OpCodes.Ldloc, n);

                        gen.Emit(OpCodes.Ldloc, memory);
                        gen.Emit(OpCodes.Ldloc, n);
                        gen.Emit(OpCodes.Ldelem, typeof(byte));

                        gen.Emit(OpCodes.Ldc_I4, count);
                        gen.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToByte", new Type[] { typeof(int) }));
                        gen.Emit(OpCodes.Add);
                        gen.Emit(OpCodes.Stelem_I4);
                    }
                    break;

                    case '-':
                    {
                        int count = 1;
                        while(i < length - 1 && code[++i] == '-') count++;
                        i--;

                        gen.Emit(OpCodes.Ldloc, memory);
                        gen.Emit(OpCodes.Ldloc, n);

                        gen.Emit(OpCodes.Ldloc, memory);
                        gen.Emit(OpCodes.Ldloc, n);
                        gen.Emit(OpCodes.Ldelem, typeof(byte));

                        gen.Emit(OpCodes.Ldc_I4, count);
                        gen.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToByte", new Type[] { typeof(int) }));
                        gen.Emit(OpCodes.Sub);
                        gen.Emit(OpCodes.Stelem_I4);
                    }
                    break;

                    case '>':
                    {
                        gen.Emit(OpCodes.Ldloc, n);
                        gen.Emit(OpCodes.Ldc_I4, 1);
                        gen.Emit(OpCodes.Add);
                        gen.Emit(OpCodes.Stloc, n);
                    }
                    break;
                    case '<':
                    {
                        gen.Emit(OpCodes.Ldloc, n);
                        gen.Emit(OpCodes.Ldc_I4, 1);
                        gen.Emit(OpCodes.Sub);
                        gen.Emit(OpCodes.Stloc, n);
                    }
                    break;

                    case ',':
                    {
                        gen.Emit(OpCodes.Ldloc, memory);
                        gen.Emit(OpCodes.Ldloc, n);
                        gen.Emit(OpCodes.Ldloc, memory);
                        gen.Emit(OpCodes.Ldloc, n);
                        gen.Emit(OpCodes.Ldelem, typeof(byte));
                        gen.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine"));
                        gen.Emit(OpCodes.Call, typeof(byte).GetMethod("Parse", new Type[] { typeof(string) }));
                        gen.Emit(OpCodes.Add);
                        gen.Emit(OpCodes.Stelem, typeof(byte));
                    }break;

                    case '.':
                    {
                        gen.Emit(OpCodes.Ldloc, memory);
                        gen.Emit(OpCodes.Ldloc, n);
                        gen.Emit(OpCodes.Ldelem, typeof(byte));
                        gen.Emit(OpCodes.Call, typeof(Console).GetMethod("Write", new Type[] { typeof(byte) }));
                    }
                    break;
                }
            }
            gen.EmitWriteLine("\nProgram ended... press any key");
            gen.Emit(OpCodes.Call, typeof(Console).GetMethod("Read"));
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Ret);
            return true;
        }

        static bool SaveAssembly(AssemblyBuilder ass, string path)
        {
            ass.Save(path);
            return true;
        }

        static AssemblyBuilder CreateAssembly(string name)
        {
            var assBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndSave);
            var module = assBuilder.DefineDynamicModule("program", "bf.exe");
            var type = module.DefineType("BFProgram", TypeAttributes.Class | TypeAttributes.Public);
            var method = type.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig, typeof(void), new Type[] { typeof(string[])});
            var gen = method.GetILGenerator();
            //var genMethodRes = GenerateMethod(gen, Console.ReadLine());
            var genMethodRes = GenerateMethod(gen, "++++++[>++++++++++<-]>.");
            type.CreateType();
            assBuilder.SetEntryPoint(method, PEFileKinds.ConsoleApplication);
            if(genMethodRes)
            {
                return assBuilder;
            }
            else
                return null;
        }

        static void Main(string[] args)
        {
            var assembly = CreateAssembly("BF");
            SaveAssembly(assembly, "bf.exe");
        }
    }
}
