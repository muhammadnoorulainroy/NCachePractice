using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;
using DataModel;

namespace NCachePractice
{
    public class Streaming
    {
        private static ICache _cache;

        /// <summary>
        /// Executing this method will perform the operations of the sample using streaming api.
        /// Streaming allows to read data from cache in chunks just like any buffered stream.
        /// </summary>
        public static void Run()
        {
            string key = "StreamingObject:1";

            // Generate a new byte buffer with some data.
            byte[] buffer = GenerateByteBuffer();

            // Initialize Cache 
            InitializeCache();

            // Write the byte buffer in cache using streaming.
            WriteUsingStream(key, buffer);

            // Read the data inserted using streaming api.
            ReadUsingStream(key);

            // Dispose the cache once done
            _cache.Dispose();
        }

        /// <summary>
        /// This method generates a new byte buffer with data.
        /// </summary>
        /// <returns> Returns a byte buffer with data.</returns>
        private static byte[] GenerateByteBuffer()
        {
            byte[] byteBuffer = new byte[1024];
            for (int i = 0; i < byteBuffer.Length; i++)
                byteBuffer[i] = Convert.ToByte(i % 256);

            return byteBuffer;
        }

        /// <summary>
        /// This method initializes the cache.
        /// </summary>
        private static void InitializeCache()
        {
            // Initialize an instance of the cache to begin performing operations:
            _cache = CacheManager.GetCache("ClusteredCache-2");

            Console.WriteLine("Cache initialized successfully");
        }

        /// <summary>
        /// This methods inserts data in the cache using cache stream.
        /// </summary>
        /// <param name="key"> The key against which stream will be written. </param>
        /// <param name="writeBuffer"> data that will be written in the stream. </param>
        private static void WriteUsingStream(string key, byte[] writeBuffer)
        {
            // Declaring NCacheStream
            CacheStreamAttributes cacheStreamAttributes = new CacheStreamAttributes(StreamMode.Write);
            CacheStream stream = _cache.GetCacheStream(key, cacheStreamAttributes);
            stream.Write(writeBuffer, 0, writeBuffer.Length);
            stream.Close();

            Console.WriteLine("Stream written to cache.");
        }

        /// <summary>
        /// This method fetches data from the cache using streams.
        /// </summary>
        /// <param name="key"> The key of the stream that needs to be fetched from the cache. </param>
        private static void ReadUsingStream(string key)
        {
            byte[] readBuffer = new byte[1024];
            CacheStreamAttributes cacheStreamAttributes = new CacheStreamAttributes(StreamMode.Read);
            // StramMode.Read allows only simultaneous reads but no writes!
            CacheStream stream = _cache.GetCacheStream(key, cacheStreamAttributes);
            // Now you have stream perform operations on it just like any regular stream.
            var readCount = stream.Read(readBuffer, 0, readBuffer.Length);
            stream.Close();

            Console.WriteLine("Bytes read = " + readCount);
        }
    }
}
