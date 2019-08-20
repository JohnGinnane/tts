using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tts
{
    class Program
    {
        static void Main(string[] args)
        {
            wave test = new wave();
            test.data = test.square();
            test.saveToFile("test.wav");
        }
    }

    class wave
    {
        string chunkID = "RIFF";
        private uint chunkSize => 4 + (8 + subChunk1Size) + (8 + subChunk2Size);
        string format = "WAVE";
        string subChunk1ID = "fmt ";
        uint subChunk1Size = 16; // for PCM
        ushort audioFormat = 1; // PCM = 1
        ushort numChannels;
        uint sampleRate;
        private uint byteRate => sampleRate * numChannels * bitsPerSample / 8; 
        private uint blockAlign => (uint)(numChannels * bitsPerSample / 8);
        ushort bitsPerSample = 8;
        string subChunk2ID = "data";
        private uint subChunk2Size => (uint)(data.Length * numChannels * bitsPerSample / 8);
        public byte[] data;

        public wave(ushort numChannels = 1, uint sampleRate = 11025)
        {
            this.numChannels = numChannels;
            this.sampleRate = sampleRate;
        }

        public void saveToFile(string path)
        {
            // http://soundfile.sapp.org/doc/WaveFormat/
            // https://stackoverflow.com/questions/14659684/creating-a-wav-file-in-c-sharp
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.CreateNew);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);

            bw.Write(System.Text.Encoding.ASCII.GetBytes(chunkID));
            bw.Write(chunkSize);
            bw.Write(System.Text.Encoding.ASCII.GetBytes(format));
            bw.Write(System.Text.Encoding.ASCII.GetBytes(subChunk1ID));
            bw.Write(subChunk1Size);
            bw.Write(audioFormat);
            bw.Write(numChannels);
            bw.Write(sampleRate);
            bw.Write(byteRate);
            bw.Write(blockAlign);
            bw.Write(bitsPerSample);
            bw.Write(System.Text.Encoding.ASCII.GetBytes(subChunk2ID));
            bw.Write(subChunk2Size);
            bw.Write(data);
        }

        public byte[] square(uint frequency = 440, float volume = 0.5f, uint length = 1000)
        {
            volume = Math.Abs(volume);
            double i = 0;
            byte[] data;
            uint samples = (uint)(sampleRate * (double)(length / 1000));
            data = new byte[samples];
            int min = (int)((bitsPerSample == 16 ? (int)short.MinValue : (int)byte.MinValue) * volume);
            int max = (int)((bitsPerSample == 16 ? (int)short.MaxValue : (int)byte.MaxValue) * volume);

            while (i < samples)
            {
                // TODO: write the data to the array
                i++;
            }

            return data;
        }
    }


}
