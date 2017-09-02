﻿using System;
using System.Linq;
using VGAudio.Utilities;

namespace VGAudio.Codecs.CriHca
{
    public class CriHcaFrame
    {
        private const int SubframesPerFrame = 8;
        private const int FrameSizeBits = 7;
        private const int FrameSize = 1 << FrameSizeBits;

        public HcaInfo Hca { get; }
        public int ChannelCount { get; }
        public int[] ScaleLength { get; }
        public ChannelType[] ChannelType { get; }
        public int[][] Scale { get; }
        public int[][] Intensity { get; }
        public int[][] Resolution { get; }
        public int[][] HfrScale { get; }
        public float[][] Gain { get; }
        public int[][][] QuantizedSpectra { get; }
        public double[][][] Spectra { get; }
        public double[][][] PcmFloat { get; }
        public Mdct[] Mdct { get; }


        public CriHcaFrame(HcaInfo hca)
        {
            Hca = hca;
            ChannelCount = hca.ChannelCount;
            ChannelType = GetChannelTypes(hca).Select(x => (ChannelType)x).ToArray();
            ScaleLength = new int[hca.ChannelCount];
            Mdct = new Mdct[hca.ChannelCount];
            Scale = Helpers.CreateJaggedArray<int[][]>(ChannelCount, FrameSize);
            Intensity = Helpers.CreateJaggedArray<int[][]>(ChannelCount, 8);
            Resolution = Helpers.CreateJaggedArray<int[][]>(ChannelCount, FrameSize);
            HfrScale = Helpers.CreateJaggedArray<int[][]>(ChannelCount, FrameSize);
            Gain = Helpers.CreateJaggedArray<float[][]>(ChannelCount, FrameSize);
            QuantizedSpectra = Helpers.CreateJaggedArray<int[][][]>(SubframesPerFrame, ChannelCount, FrameSize);
            Spectra = Helpers.CreateJaggedArray<double[][][]>(SubframesPerFrame, ChannelCount, FrameSize);
            PcmFloat = Helpers.CreateJaggedArray<double[][][]>(SubframesPerFrame, ChannelCount, FrameSize);

            for (int i = 0; i < hca.ChannelCount; i++)
            {
                ScaleLength[i] = hca.BaseBandCount;
                if (ChannelType[i] != CriHca.ChannelType.IntensityStereoSecondary) ScaleLength[i] += hca.StereoBandCount;
                Mdct[i] = new Mdct(FrameSizeBits, CriHcaTables.MdctWindow, Math.Sqrt(2.0 / FrameSize));
            }
        }

        private static int[] GetChannelTypes(HcaInfo hca)
        {
            int channelsPerTrack = hca.ChannelCount / hca.TrackCount;
            if (hca.StereoBandCount == 0 || channelsPerTrack == 1) { return new int[8]; }

            switch (channelsPerTrack)
            {
                case 2: return new[] { 1, 2 };
                case 3: return new[] { 1, 2, 0 };
                case 4: return hca.ChannelConfig > 0 ? new[] { 1, 2, 0, 0 } : new[] { 1, 2, 1, 2 };
                case 5: return hca.ChannelConfig > 2 ? new[] { 2, 0, 0, 0 } : new[] { 2, 0, 1, 2 };
                case 6: return new[] { 1, 2, 0, 0, 1, 2 };
                case 7: return new[] { 1, 2, 0, 0, 1, 2, 0 };
                case 8: return new[] { 1, 2, 0, 0, 1, 2, 1, 2 };
                default: return new int[channelsPerTrack];
            }
        }
    }
}
