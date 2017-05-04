using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Doit.Media.Audio.Amr.Amrnb {
public class Decoder {
    [DllImport("libopencore-amrnb-0.dll", EntryPoint = "Decoder_Interface_init", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr Decoder_Interface_init();

    [DllImport("libopencore-amrnb-0.dll", EntryPoint = "Decoder_Interface_Decode", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Decoder_Interface_Decode(IntPtr state, byte[] inBuffer, short[] outBuffer, int bfi);

    [DllImport("libopencore-amrnb-0.dll", EntryPoint = "Decoder_Interface_exit", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Decoder_Interface_exit(IntPtr state);

}
}
