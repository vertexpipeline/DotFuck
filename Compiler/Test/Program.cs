// Decompiled with JetBrains decompiler
// Type: BFProgram
// Assembly: BF, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F217B4A-3C28-4953-9660-9C240E9557D3
// Assembly location: C:\Users\verte\Source\Repos\DotFuck\Compiler\Compiler\bin\Debug\bf.exe

using System;
using System.Runtime.InteropServices;

public class BFProgram
{
    public static void Main([In] string[] obj0)
    {
        byte[] numArray = new byte[30000];
        int index1 = 0;
        numArray[index1] = (byte)((int)numArray[index1] + (int)Convert.ToByte(6));
        if((int)numArray[index1] != 0)
        {
            do
            {
                int index2 = index1 + 1;
                numArray[index2] = (byte)((int)numArray[index2] + (int)Convert.ToByte(10));
                index1 = index2 - 1;
                numArray[index1] = (byte)((int)numArray[index1] - (int)Convert.ToByte(1));
            }
            while((int)numArray[index1] >= 1);
        }
        int index3 = index1 + 1;
        Console.Write((char)numArray[index3]);
        Console.WriteLine("\nProgram ended... press any key");
        Console.Read();
    }
}
