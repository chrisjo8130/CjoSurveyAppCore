using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CjoSurveyApp
{
    public class ParseNav
    {
         
        public const byte Header1 = 0xB5;
        public const byte Header2 = 0x62;

        public static StringBuilder ParseNow(byte[] payload)
        {
            //string[] a = new string[4];
            Int32 latitude = 0;
            Int32 longitude = 0;
            Int32 heightElipsoid = 0;
            Int32 heightMSL = 0;
            Int32 timeInt = 0;
            
            StringBuilder timeNow = new StringBuilder();
            if (payload.Length < 2 || payload[0] != Header1 || payload[1] != Header2)
            {
                timeNow.Append("not ubx message");
                return timeNow;
            }
                //throw new InvalidMessageHeaderException();

            byte classId = payload[2];
            byte messageId = payload[3];
            int messageLength = payload[4] | (payload[5] << 8);
            if ((classId == 0x01) & (messageId == 0x02))
            {
                int a = BitConverter.ToInt32(payload, 14);
                int b = BitConverter.ToInt32(payload, 10);
                int c = BitConverter.ToInt32(payload, 18);
                int d = BitConverter.ToInt32(payload, 22);
                double ab = (double)a / 10000000;
                double bb = (double)b / 10000000;
                float cb = (float)c /1000;
                float db = (float)d /1000;
                //a[0] = BitConverter.ToString(payload, 14);
                //a[1] = BitConverter.ToString(payload, 10);
                //a[2] = BitConverter.ToString(payload, 18);
                //a[3] = BitConverter.ToString(payload, 22);
                timeNow.AppendFormat("{0:N7}, {1:N7}, {2:N3}, {3:N3}", ab, bb, cb, db);    
            //timeInt = payload[13];
                //string.Format("[0}[1}", latitude.ToString(), longitude.ToString());

            }
            else
            {
                timeNow.Equals("failed");
            }
            
            return timeNow;


            //try
            //{
            //    var ubxType = parsableTypeIndex[new UBXMessageIndex(classId, messageId)];

            //    ushort expectedChecksum = (ushort)((payload[payload.Length - 2]) | (payload[payload.Length - 1] << 8));
            //    ushort computedChecksum = GetChecksum(payload, 2, payload.Length - 2);

            //    if (expectedChecksum != computedChecksum)
            //        throw new InvalidChecksumException(String.Format("Checksum expected {0}, computed {1}", expectedChecksum, computedChecksum));

            //    BinaryReader reader = new BinaryReader(new MemoryStream(payload, 6, messageLength));

            //    UBXModelBase retVal = (UBXModelBase)Activator.CreateInstance(ubxType.MessageClass);

            //    foreach (var property in ubxType.PropertyMap)
            //    {
            //        if (!property.ListType)
            //        {
            //            // If property is not a list type, parse normally using its underlying type
            //            property.Property.SetValue(retVal, reader.Read(property.Property.PropertyType));
            //        }
            //        else
            //        {
            //            // If property is a list type, infer the type content
            //            var typeInfoOfPropertyType = property.Property.PropertyType.GetTypeInfo();
            //            var theStructureType = typeInfoOfPropertyType.GenericTypeArguments[0]; // Get the T of IEnumerable<T>

            //            // Get the size of the structure
            //            var structureSize = UBXStructureMapper.PayloadSizeOf(theStructureType);

            //            // Get the item count
            //            var itemCount = Convert.ToInt32(ubxType.PropertyMap[property.ListIndexRef.Value].Property.GetValue(retVal));

            //            // Construct list of it
            //            var theList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(theStructureType));


            //            for (int i = 0; i < itemCount; i++)
            //            {
            //                var buf = reader.ReadBytes(structureSize);
            //                theList.Add(UBXStructureMapper.TryParse(theStructureType, buf));
            //            }

            //            // Set the value to property
            //            property.Property.SetValue(retVal, theList);
            //        }
            //    }

            //    return retVal;
            //}
            //catch (KeyNotFoundException)
            //{
            //    throw new UnknownMessageException(string.Format("Unknown message with Class: {0}, MessageID: {1}", classId, messageId));
            //}
            //catch (NotSupportedException ex)
            //{
            //    throw new UnknownMessageException(string.Format("Failed to parse Class: {0}, MessageID: {1}", classId, messageId), ex);
            //}
        }

    }
    public class InvalidMessageHeaderException : Exception
    {
        public InvalidMessageHeaderException() { }
        public InvalidMessageHeaderException(string message) : base(message) { }
        public InvalidMessageHeaderException(string message, Exception inner) : base(message, inner) { }
    }

}
