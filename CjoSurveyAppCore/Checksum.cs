using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CjoSurveyAppCore
{
    public static class Checksum
    {
        public static byte[] VerifyChecksum(byte[] message)
        {
            int ckA = 0;
            int ckB = 0;
            bool correctChecksum = false;
            
            for (int i = 2; i < message.Length - 3; i++)
            {
                ckA += message[i];
                ckB += ckA;
            }
            byte[] newChecksum = new byte[] { (byte)ckA, (byte)ckB };
            if (ckA == message[message.Length - 2] && ckB == message[message.Length - 1])
            {
                correctChecksum = true;
            }
            else
            {
                correctChecksum = false;
            }

            return newChecksum;
        }
    }
}
