using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrossMod.Tools
{
    public class SwitchSwizzler
    {

        public static int[] BCn_formats = new int[]{
            0x1a,
            0x1b,
            0x1c,
            0x1d,
            0x1e,
            0x1f,
            0x20
        };

        public static int[] ASTC_formats = new int[] { 0x2d, 0x2e, 0x2f, 0x30,
                0x31, 0x32, 0x33, 0x34,
                0x35, 0x36, 0x37, 0x38,
                0x39, 0x3a};

        public static Dictionary<int, Vector2> blk_dims = new Dictionary<int, Vector2>()
        {
            { 0x2d, new Vector2(4, 4)},
{0x2e, new Vector2(5, 4)},
{0x2f, new Vector2(5, 5)},
{0x30, new Vector2(6, 5)},
{0x31, new Vector2(6, 6)},
{0x32, new Vector2(8, 5)},
{0x33, new Vector2(8, 6)},
{0x34, new Vector2(8, 8)},
{0x35, new Vector2(10, 5)},
{0x36, new Vector2(10, 6)},
{0x37, new Vector2(10, 8)},
{0x38, new Vector2(10, 10)},
{0x39, new Vector2(12, 10)},
{0x3a, new Vector2(12, 12)}
        };

        public static Dictionary<int, int> bpps = new Dictionary<int, int>()
        {
            {0xb, 4}, {7, 2}, {2, 1}, {9, 2}, {0x1a, 8 },
            { 0x1b, 16}, {0x1c, 16}, {0x1d, 8}, {0x1e, 16 },
            { 0x1f, 16}, {0x20, 16}, {0x2d, 16}, {0x2e, 16},
            { 0x2f, 16}, {0x30, 16}, {0x31, 16}, {0x32, 16},
            { 0x33, 16}, {0x34, 16}, {0x35, 16}, {0x36, 16},
            { 0x37, 16}, {0x38, 16}, {0x39, 16}, {0x3a, 16}
        };

        public static Dictionary<int, int> xBases = new Dictionary<int, int>()
        {
            { 1, 4},
            { 2, 3},
            { 4, 2},
            { 8, 1},
            { 16, 0}
        };
        public static Dictionary<int, int> padds = new Dictionary<int, int>()
        {
            {1, 64},
            {2, 32},
            {4, 16},
            {8, 8},
            {16, 4}
        };

        public static byte[] Deswizzle(int width, int height, int format, byte[] data)
        {
            int bpp = bpps[format];

            int origin_width = width;
            int origin_height = height;

            if (BCn_formats.Contains(format))
            {
                origin_width = (origin_width + 3) / 4;
                origin_height = (origin_height + 3) / 4;
            }
            if (ASTC_formats.Contains(format))
            {
                Vector2 blkdim = blk_dims[format];
                origin_width = (origin_width + (int)blkdim.X - 1) / (int)blkdim.X;
                origin_height = (origin_height + (int)blkdim.Y - 1) / (int)blkdim.Y;
            }

            int xb = CountZeros(Pow2RoundUp(origin_width));
            int yb = CountZeros(Pow2RoundUp(origin_height));

            int hh = Pow2RoundUp(origin_height) >> 1;

            if (!IsPow2(origin_height) && origin_height <= hh + hh)
            {
                yb -= 1;
            }

            width = RoundSize(origin_width, padds[bpp]);

            int xBase = xBases[bpp];

            byte[] result = new byte[bpp * width * height];

            int pos_ = 0;
            for (int y = 0; y < origin_height; y++)
            {
                for (int x = 0; x < origin_width; x++)
                {
                    int pos = GetAddr(x, y, xb, yb, width, xBase) * bpp;

                    if (pos + bpp <= data.Length && pos_ + bpp <= data.Length)
                        Array.Copy(data, pos, result, pos_, bpp);

                    pos_ += bpp;
                }
            }

            return result;
        }

        public static int GetAddr(int x, int y, int xb, int yb, int width, int xBase)
        {
            int xCnt = xBase;
            int yCnt = 1;
            int xUsed = 0;
            int yUsed = 0;
            int address = 0;

            while ((xUsed < xBase + 2) && (xUsed + xCnt < xb))
            {
                int xMask = (1 << xCnt) - 1;
                int yMask = (1 << yCnt) - 1;

                address |= (x & xMask) << xUsed + yUsed;
                address |= (y & yMask) << xUsed + yUsed + xCnt;

                x >>= xCnt;
                y >>= yCnt;

                xUsed += xCnt;
                yUsed += yCnt;

                xCnt = Math.Max(Math.Min(xb - xUsed, 1), 0);
                yCnt = Math.Max(Math.Min(yb - yUsed, yCnt << 1), 0);

            }
            address |= (x + y * (width >> xUsed)) << (xUsed + yUsed);

            return address;
        }

        public static int CountZeros(int v)
        {
            int numZeros = 0;

            for (int i = 0; i < 32; i++)
            {
                if ((v & (1 << i)) != 0)
                    break;
                numZeros += 1;
            }

            return numZeros;
        }

        public static int Pow2RoundUp(int v)
        {
            v -= 1;

            v |= (v + 1) >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;

            return v + 1;
        }

        public static bool IsPow2(int v)
        {
            return v != 0 && !((v & (v - 1)) != 0);
        }

        public static int RoundSize(int size, int pad)
        {
            int mask = pad - 1;
            if ((size & mask) != 0)
            {
                size &= ~mask;
                size += pad;
            }

            return size;
        }
    }
}
