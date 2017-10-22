using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace Verse
{
	public static class CompressUtility
	{
		public static byte[] Compress(byte[] input)
		{
			MemoryStream memoryStream = new MemoryStream();
			DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress);
			deflateStream.Write(input, 0, input.Length);
			deflateStream.Close();
			return memoryStream.ToArray();
		}

		public static byte[] Decompress(byte[] input)
		{
			MemoryStream stream = new MemoryStream(input);
			DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
			List<byte[]> list = null;
			byte[] result;
			while (true)
			{
				byte[] array = new byte[65536];
				int num = deflateStream.Read(array, 0, array.Length);
				if (num < array.Length && list == null)
				{
					byte[] array2 = new byte[num];
					Array.Copy(array, array2, num);
					result = array2;
					break;
				}
				if (num < array.Length)
				{
					byte[] array3 = new byte[num + list.Count * array.Length];
					for (int i = 0; i < list.Count; i++)
					{
						Array.ConstrainedCopy(list[i], 0, array3, i * array.Length, array.Length);
					}
					Array.ConstrainedCopy(array, 0, array3, list.Count * array.Length, num);
					result = array3;
					break;
				}
				if (list == null)
				{
					list = new List<byte[]>();
				}
				list.Add(array);
			}
			return result;
		}
	}
}
