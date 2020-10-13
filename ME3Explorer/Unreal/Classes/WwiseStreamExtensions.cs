﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ME3Explorer.Soundplorer;
using ME3ExplorerCore.Helpers;
using ME3ExplorerCore.Unreal.BinaryConverters;
using NAudio.Wave;

namespace ME3Explorer.Unreal.Classes
{
    public static class WwiseStreamExtensions
    {

        /// <summary>
        /// This method is deprecated and will be removed eventually
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathtoafc"></param>
        public static void ImportFromFile(this WwiseStream ws, string path, string pathtoafc = "")
        {
            if (ws.Filename == "")
                return;
            using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            if (ws.IsPCCStored)
            {
                ws.ImportWwiseOgg(pathtoafc, stream);
            }
            else if (pathtoafc != "")
            {
                if (File.Exists(pathtoafc))
                    ws.ImportWwiseOgg(pathtoafc, stream);
                else if (File.Exists(pathtoafc + ws.Filename + ".afc")) //legacy code for old soundplorer
                    ws.ImportWwiseOgg(pathtoafc + ws.Filename + ".afc", stream);
                else
                {
                    OpenFileDialog d = new OpenFileDialog();
                    d.Filter = ws.Filename + ".afc|" + ws.Filename + ".afc";
                    if (d.ShowDialog() == DialogResult.OK)
                        ws.ImportWwiseOgg(d.FileName, stream);
                }
            }
            else
            {
                OpenFileDialog d = new OpenFileDialog();
                d.Filter = ws.Filename + ".afc|" + ws.Filename + ".afc";
                if (d.ShowDialog() == DialogResult.OK)
                    ws.ImportWwiseOgg(d.FileName, stream);
            }
        }

        public static TimeSpan? GetSoundLength(this WwiseStream ws)
        {
            Stream waveStream = ws.CreateWaveStream(); 
            if (waveStream != null)
            {
                //Check it is RIFF
                byte[] riffHeaderBytes = new byte[4];
                waveStream.SeekBegin();
                waveStream.Read(riffHeaderBytes, 0, 4);
                string wemHeader = "" + (char)riffHeaderBytes[0] + (char)riffHeaderBytes[1] + (char)riffHeaderBytes[2] + (char)riffHeaderBytes[3];
                if (wemHeader == "RIFF") //this is terrible code and will not work for RIFX (big endian). Length can be calculated by sound header (look at vgmstream source) with samplerate * bits per sample divided by number of samples
                {
                    waveStream.SeekBegin();
                    WaveFileReader wf = new WaveFileReader(waveStream);
                    return wf.TotalTime;
                }
            }
            return null;
        }



        /// <summary>
        /// Creates wav file in temp directory
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        public static string CreateWave(this WwiseStream ws)
        {
            string basePath = WwiseStreamHelper.GetATempSoundPath();
            string wavPath = basePath + ".wav";
            if (ws.CreateWaveStream() is MemoryStream dataStream)
            {
                File.WriteAllBytes(wavPath, dataStream.ToArray());
            }
            return wavPath;
        }

        /// <summary>
        /// Creates wav stream from this WwiseStream
        /// </summary>
        /// <param name="afcPath"></param>
        /// <returns></returns>
        public static MemoryStream CreateWaveStream(this WwiseStream ws)
        {
            string basePath = WwiseStreamHelper.GetATempSoundPath();
            string wemPath = basePath + ".wem";
            if (ws.ExtractRawFromSourceToFile(wemPath))
            {
                return null;
            }

            return ISBankEntry.ConvertAudioToWave(wemPath);
            //return ConvertRiffToWav(basePath + ".wem", export.FileRef.Game == MEGame.ME2);
        }

        public static bool ExtractRawFromSourceToFile(this WwiseStream ws, string outputFile)
        {
            if (ws.IsPCCStored)
            {
                if (ws.EmbeddedData is null || ws.EmbeddedData.Length == 0)
                {
                    return false;
                }
                if (File.Exists(outputFile)) File.Delete(outputFile);
                File.WriteAllBytes(outputFile, ws.EmbeddedData);
                return true;
            }
            return WwiseStreamHelper.ExtractRawFromSourceToFile(outputFile, ws.GetPathToAFC(), ws.DataSize, ws.DataOffset);
        }

        private static void ImportWwiseOgg(this WwiseStream ws, string pathafc, Stream wwiseOggStream)
        {
            if (!File.Exists(pathafc) || wwiseOggStream == null)
                return;
            //Convert wwiseoggstream
            MemoryStream convertedStream = WwiseStreamHelper.ConvertWwiseOggToME3Ogg(wwiseOggStream);
            byte[] newWavfile = convertedStream.ToArray();

            if (ws.IsPCCStored)
            {
                ws.EmbeddedData = newWavfile;
                //DataSize and DataOffset are automatically calculated during serialization
                //when EmbeddedData != null
                return;
            }


            //Open AFC
            FileStream fs = new FileStream(pathafc, FileMode.Open, FileAccess.Read);
            var Header = new byte[94];

            //Seek to data we are replacing and read header
            fs.Seek(ws.DataOffset, SeekOrigin.Begin);
            fs.Read(Header, 0, 94);
            fs.Close();


            //append new wav
            fs = new FileStream(pathafc, FileMode.Append, FileAccess.Write, FileShare.Write);
            int newWavDataOffset = (int)fs.Length;
            int newWavSize = newWavfile.Length;
            fs.Write(newWavfile, 0, newWavSize);
            fs.Close();

            ws.DataSize = newWavSize;
            ws.DataOffset = newWavDataOffset;
            ws.EmbeddedData = null;
        }
    }
}