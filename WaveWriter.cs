using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmrAudioLibrary {
    public class WaveWriter {

        private FileInfo wavFile;
        private BinaryWriter binaryWriter;

        private string filePath;
        private int sampleRate;
        private int channels;
        private int bitsPerSample;
        private int dataLength;


        /**
         * Constructor; initializes WaveWriter with file name and path
         *
         * @param path  output file path
         * @param name  output file name
         * @param sampleRate  output sample rate
         * @param channels  number of channels
         * @param sampleBits  number of bits per sample (S8LE, S16LE)
         */
        public WaveWriter(String path, int sampleRate,
            int bitsPerSample, int channels) {

            this.filePath = path;
            this.sampleRate = sampleRate;
            this.channels = channels;
            this.bitsPerSample = bitsPerSample;
            this.dataLength = 0;
        }

        /**
         * Create output WAV file
         *
         * @return whether file creation succeeded
         *
         * @throws IOException if file I/O error occurs allocating header
         */
        public bool CreateFile() {
            try {
                wavFile = new FileInfo(filePath);
                if (wavFile.Exists) {
                    wavFile.Delete();
                }
                UTF8Encoding utf8 = new UTF8Encoding(false);
                binaryWriter = new BinaryWriter(wavFile.Create(), utf8);
                binaryWriter.Seek(0, SeekOrigin.Begin);
                this.WriteHeader();
            } catch (Exception ex) {
                return false;
            }

            return true;
        }

        /**
         * Write audio data to output file (mono). Does
         * nothing if output file is not mono channel.
         *
         * @param littleendian  mono audio data input buffer
         * @param offset offset into src buffer
         * @param length  buffer size in number of samples
         *
         * @throws IOException if file I/O error occurs
         */
        public void Write(byte[] littleendian, int length) {

            if (this.binaryWriter == null) {
                return;
            }
            this.dataLength += length;
            this.binaryWriter.Write(littleendian);
        }




        /**
         * Close output WAV file and write WAV header. WaveWriter
         * cannot be used again following this call.
         *
         * @throws IOException if file I/O error occurs writing WAV header
         */
        public void CloseFile() {
            if (this.binaryWriter == null) {
                return;
            }

            this.binaryWriter.Seek(0, SeekOrigin.Begin);
            WriteHeader();
            this.binaryWriter.Close();

        }

        private void WriteHeader() {
            // rewind to beginning of the file
            if (this.binaryWriter == null) {
                return;
            }

            int bytesPerFrame;
            int bytesPerSec;
            
            binaryWriter.Write(UTF8Encoding.UTF8.GetBytes("RIFF")); // WAV chunk header
            binaryWriter.Write(4 + 8 + 16 + 8 + this.dataLength); // WAV chunk size
            binaryWriter.Write(UTF8Encoding.UTF8.GetBytes("WAVE")); // WAV format

            binaryWriter.Write(UTF8Encoding.UTF8.GetBytes("fmt ")); // format subchunk header
            binaryWriter.Write(16); // format subchunk size

            bytesPerFrame = this.bitsPerSample / 8 * this.channels;
            bytesPerSec = bytesPerFrame * this.sampleRate;

            binaryWriter.Write((short)1); // audio format
            binaryWriter.Write((short)this.channels); // number of channels
            binaryWriter.Write(this.sampleRate); // sample rate
            binaryWriter.Write(bytesPerSec); // byte rate
            binaryWriter.Write((short)bytesPerFrame); // block align
            binaryWriter.Write((short)this.bitsPerSample); // bits per sample

            binaryWriter.Write(UTF8Encoding.UTF8.GetBytes("data")); // data subchunk header
            binaryWriter.Write(this.dataLength); // data subchunk size
                                             
            binaryWriter.Flush();

        }

    }
}
