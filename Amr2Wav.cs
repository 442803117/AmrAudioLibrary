using System;
using System.IO;
using System.Text;

namespace Doit.Media.Audio.Amr {
public class Amr2Wav {

    /* From WmfDecBytesPerFrame in dec_input_format_tab.cpp */
    private int[] sizes = { 12, 13, 15, 17, 19, 20, 26, 31, 5, 6, 5, 5, 0, 0, 0, 0 };
    private static IntPtr mNativeAmrDecoder = IntPtr.Zero; // the pointer to the native
    // amr-nb decoder

    // 读取amr文件头的6个字节


    public string Converter(string amrPath, string wavPath) {
        FileInfo inFileInfo = new FileInfo(amrPath);
        if (!inFileInfo.Exists) {
            return "";
        }

        byte[] header = new byte[6];
        int inFileSize = (int)inFileInfo.Length;

        /* amr文件输入信息，这里我们测试的是单声道的文件，文件头开始是“#AMR!/n” */

        BinaryReader binaryReader = null;

        try {
            binaryReader = new BinaryReader(inFileInfo.Open(FileMode.Open));
            if (binaryReader.BaseStream.Length < 6) {
                return "BAD FILE";
            }
            header = binaryReader.ReadBytes(6);
        } catch (Exception ex) {
            Console.Out.WriteLine(ex.StackTrace);
            //System.out.println("读入文件错误！");
            return ex.Message;
        }
        string strHeader = Encoding.UTF8.GetString(header);
        if (!"#!AMR\n".Equals(strHeader)) {
            Console.Out.WriteLine("BAD HEADER"); // 检查文件头是否是由#！AMR/n开始的
        }

        Console.Out.WriteLine("开始创建文件1！");
        try {
            // 创建WaveWriter对象
            WaveWriter wav = new WaveWriter(wavPath, 8000, 16, 1);
            bool flag = wav.CreateFile();
            if (!flag) {
                binaryReader.Close();
                return "Failed to createWaveFile.";
            }
            mNativeAmrDecoder = Amrnb.Decoder.Decoder_Interface_init();
            try {
                while (true) {

                    byte[] buffer = new byte[500];
                    // 读入模式字节
                    /* Read the mode byte */
                    buffer[0] = binaryReader.ReadByte();
                    // 按照模式字节显示的数据包的大小来读数据
                    int size = sizes[(buffer[0] >> 3) & 0x0f];
                    if (size <= 0)
                        break;

                    byte[] temBuffer = binaryReader.ReadBytes(size);
                    Array.Copy(temBuffer, 0, buffer, 1, size);

                    short[] outbuffer = new short[160];

                    // System.out.println("开始写入wav文件！");
                    Amrnb.Decoder.Decoder_Interface_Decode(mNativeAmrDecoder, buffer, outbuffer, 0);

                    byte[] littleendian = new byte[320];
                    int j = 0;
                    for (int i = 0; i < 160; i++) {
                        littleendian[j] = (byte)(outbuffer[i] >> 0 & 0xff);
                        littleendian[j + 1] = (byte)(outbuffer[i] >> 8 & 0xff);
                        j = j + 2;
                    }

                    wav.Write(littleendian, 320);
                }
            } catch (EndOfStreamException ex) {
                Console.Out.WriteLine("read end");
            }
            binaryReader.Close();
            Amrnb.Decoder.Decoder_Interface_exit(mNativeAmrDecoder);
            wav.CloseFile();

        } catch (IOException ex) {

        }

        return "success";
    }

}
}
