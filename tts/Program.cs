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
            wave test = new wave(2, 11025 * 4);
            test.generateSquare(440f, 0.5f, 15000);
            //test.generateSine(261.63, length: 15 * 1000);
            test.saveToWAV("test.wav");
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
        private uint subChunk2Size => (uint)(data.Length * bitsPerSample / 8);
        public int[] data;

        public wave(ushort numChannels = 1, uint sampleRate = 11025)
        {
            this.numChannels = numChannels;
            this.sampleRate = sampleRate;
        }

        public void saveToWAV(string path)
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
            foreach (short v in data)
            {
                bw.Write(v);
            }

            fs.Close();
            fs.Dispose();
            bw.Close();
            bw.Dispose();
        }

        public void generateSine(double frequency = 440.0f, float volume = 0.5f, uint length = 1000, bool square = false)
        {
            // http://pages.mtu.edu/~suits/notefreqs.html
            // https://blogs.msdn.microsoft.com/dawate/2009/06/24/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c/
            double t = (Math.PI * 2 * frequency) / (sampleRate * numChannels);
            uint samples = (uint)(sampleRate * (double)(length / 1000.0f));
            data = new int[samples];
            int ampOffset = 0;
            int amplitude = 0;

            if (bitsPerSample == 8)
            {
                ampOffset = byte.MaxValue / 2;
                amplitude = (int)(((float)byte.MaxValue / 2.0f) * volume);
            } else if (bitsPerSample == 16)
            {
                ampOffset = 0;
                amplitude = (int)((float)short.MaxValue * volume);
            }

            for (uint i = 0; i < samples - 1; i++)
            {
                for (short channel = 0; channel < numChannels; channel++)
                {
                    if (square)
                    {
                        data[i + channel] = Convert.ToInt16(ampOffset + amplitude * Math.Sign(Math.Sin(t * i)));
                    }
                    else
                    {
                        data[i + channel] = Convert.ToInt16(ampOffset + amplitude * Math.Sin(t * i));
                    }
                }
            }
        }

        public void generateSquare(float frequency = 440.0f, float volume = 0.5f, uint length = 1000)
        {
            generateSine(frequency, volume, length, true);
        }
    }


}
