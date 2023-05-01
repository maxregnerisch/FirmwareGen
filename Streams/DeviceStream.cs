/*

Copyright (c) 2019, Gustave Monce - gus33000.me - @gus33000
Copyright (c) 2018, Proto Beta Test - protobetatest.com - @ProtoBetaTest

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
using System.IO;
using System.Text;

namespace FirmwareGen.Streams
{
    public class DeviceStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly byte[] _buffer;
        private int _bufferIndex;
        private int _bufferLength;

        public DeviceStream(Stream baseStream, int bufferSize = 4096)
        {
            _baseStream = baseStream;
            _buffer = new byte[bufferSize];
        }

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => _baseStream.CanWrite;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = 0;
            while (count > 0)
            {
                if (_bufferIndex >= _bufferLength)
                {
                    _bufferLength = _baseStream.Read(_buffer, 0, _buffer.Length);
                    _bufferIndex = 0;
                }
                if (_bufferLength == 0) break;

                var bytesToCopy = Math.Min(_bufferLength - _bufferIndex, count);
                Array.Copy(_buffer, _bufferIndex, buffer, offset, bytesToCopy);

                offset += bytesToCopy;
                count -= bytesToCopy;
                _bufferIndex += bytesToCopy;
                bytesRead += bytesToCopy;
            }
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            _baseStream.Close();
        }
    }
}
