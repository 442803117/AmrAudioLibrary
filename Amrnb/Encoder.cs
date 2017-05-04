using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Doit.Media.Audio.Amr;

namespace Doit.Media.Audio.Amr.Amrnb {
public class Encoder {
    [DllImport("libopencore-amrnb-0.dll")]
    public static extern IntPtr Encoder_Interface_init(int dtx);
    [DllImport("libopencore-amrnb-0.dll")]
    public static extern void Encoder_Interface_exit(IntPtr state);
    [DllImport("libopencore-amrnb-0.dll")]
    public static extern int Encoder_Interface_Encode(IntPtr state, Mode mode, short[] speech, byte[] outBuffer, int forceSpeech);

}
}
