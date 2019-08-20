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
            test.generateSquare(440f, 0.5f, 15000);
            test.saveToFile("test.wav");
            Console.ReadKey();
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
        private ushort blockAlign => (ushort)(numChannels * bitsPerSample / 8);
        ushort bitsPerSample = 16;
        string subChunk2ID = "data";
        private uint subChunk2Size => (uint)(data.Length * numChannels * bitsPerSample / 8);
        public int[] data;

        public wave(ushort numChannels = 1, uint sampleRate = 11025)
        {
            this.numChannels = numChannels;
            this.sampleRate = sampleRate;
        }

        public void saveToFile(string path)
        {
            // http://soundfile.sapp.org/doc/WaveFormat/
            // https://stackoverflow.com/questions/14659684/creating-a-wav-file-in-c-sharp
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.CreateNew);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);

            Console.WriteLine("Chunk ID: " + chunkID);
            Console.WriteLine("Chunk Size: " + chunkSize.ToString());
            Console.WriteLine("Format: " + format);
            Console.WriteLine("Subchunk 1 ID: " + subChunk1ID);
            Console.WriteLine("Subchunk 1 Size: " + subChunk1Size.ToString());
            Console.WriteLine("Audio Format: " + audioFormat.ToString());
            Console.WriteLine("Channels: " + numChannels.ToString());
            Console.WriteLine("Sample Rate: " + sampleRate.ToString());
            Console.WriteLine("Byte Rate: " + byteRate.ToString());
            Console.WriteLine("Block Align: " + blockAlign.ToString());
            Console.WriteLine("Bits Per Sample: " + bitsPerSample.ToString());
            Console.WriteLine("Subchunk 2 ID: " + subChunk2ID);
            Console.WriteLine("Subchunk 2 Size: " + subChunk2Size.ToString());

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
            foreach (int v in data)
            {
                if (bitsPerSample == 8)
                {
                    bw.Write((byte)v);
                } else
                {
                    bw.Write(v);
                }
            }

            fs.Close();
            fs.Dispose();
            bw.Close();
            bw.Dispose();
        }

        public void generateSquare(float frequency = 440.0f, float volume = 0.5f, uint length = 1000)
        {
            volume = Math.Abs(volume);
            int i = 0;
            uint samples = (uint)(sampleRate * (double)(length / 1000.0f));
            Console.WriteLine(samples.ToString());
            
            int[] data;
            data = new int[samples];
            int min = 0;
            int max = 0;
            if (bitsPerSample == 8)
            {
                min = (int)((float)byte.MaxValue / 2.0f - (float)byte.MaxValue * volume / 2.0f);
                max = (int)((float)byte.MaxValue / 2.0f + (float)byte.MaxValue * volume / 2.0f);
            } else if (bitsPerSample == 16)
            {
                min = (int)((float)short.MinValue * volume);
                max = (int)((float)short.MaxValue * volume);
            }
            
            // 11,025 / 440 = 25.057
            // if i % 25.057 >= (25.057 / 2) then HIGH else LOW
            float freqMod = (float)sampleRate / frequency;

            while (i < samples)
            {
                if ((float)i % freqMod > freqMod / 2.0f)
                {
                    data[i] = min;
                } else
                {
                    data[i] = max;
                }
                //Console.WriteLine(i.ToString() + " - " + data[i].ToString());
                
                i++;
            }

            this.data = data;
        }
    }


}
