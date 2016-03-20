using System.Collections.Generic;

namespace Rangeen.Transport
{
    public interface ITransport
    {
        List<byte[]> Download();
        bool Upload(byte[] binData, byte[] info);

    }
}
