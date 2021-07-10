using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatsonWebserver
{
    /// <summary>
    /// Watson webserver statistics.
    /// </summary>
    public class WatsonWebserverStatistics
    {
        #region Public-Members

        /// <summary>
        /// The time at which the client or server was started.
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }
        }

        /// <summary>
        /// The amount of time which the client or server has been up.
        /// </summary>
        public TimeSpan UpTime
        {
            get
            {
                return DateTime.Now.ToUniversalTime() - _StartTime;
            }
        }

        /// <summary>
        /// The number of payload bytes received (incoming request body).
        /// </summary>
        public long ReceivedPayloadBytes
        {
            get
            {
                return _ReceivedPayloadBytes;
            }
            internal set
            {
                _ReceivedPayloadBytes = value;
            }
        }

        /// <summary>
        /// The number of payload bytes sent (outgoing request body).
        /// </summary>
        public long SentPayloadBytes
        {
            get
            {
                return _SentPayloadBytes;
            }
            internal set
            {
                _SentPayloadBytes = value;
            }
        }

        #endregion

        #region Private-Members

        private DateTime _StartTime = DateTime.Now.ToUniversalTime();
        private long _ReceivedPayloadBytes = 0;
        private long _SentPayloadBytes = 0;
        private long[] _RequestsByMethod; // _RequestsByMethod[(int)HttpMethod.Xyz] = Count

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Initialize the statistics object.
        /// </summary>
        public WatsonWebserverStatistics()
        {
            // Calculating the length for _RequestsByMethod array
            int max = 0;
            foreach (var value in Enum.GetValues(typeof(HttpMethod)))
            {
                if ((int)value > max)
                    max = (int)value;
            }

            _RequestsByMethod = new long[max + 1];
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Human-readable version of the object.
        /// </summary>
        /// <returns>String.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"--- Statistics ---");
            sb.AppendLine($"    Start Time     : {StartTime}");
            sb.AppendLine($"    Up Time        : {UpTime}");
            sb.AppendLine($"    Received Payload Bytes : {ReceivedPayloadBytes.ToString("N0")} bytes");
            sb.AppendLine($"    Sent Payload Bytes     : {SentPayloadBytes.ToString("N0")} bytes");
            sb.AppendLine($"    Requests By Method     : ");

            bool foundAtLeastOne = false;
            for (int i = 0; i < _RequestsByMethod.Length; i++)
            {
                var count = Interlocked.Read(ref _RequestsByMethod[i]);
                if (count > 0)
                {
                    foundAtLeastOne = true;
                    sb.AppendLine($"        { ((HttpMethod)i).ToString().PadRight(18)} : {count.ToString("N0")}");
                }
            }

            if (!foundAtLeastOne)
                sb.AppendLine("        (none)");

            return sb.ToString();
        }

        /// <summary>
        /// Reset statistics other than StartTime and UpTime.
        /// </summary>
        public void Reset()
        {
            Interlocked.Exchange(ref _ReceivedPayloadBytes, 0);
            Interlocked.Exchange(ref _SentPayloadBytes, 0);

            for (int i = 0; i < _RequestsByMethod.Length; i++)
                Interlocked.Exchange(ref _RequestsByMethod[i], 0);
        }

        /// <summary>
        /// Retrieve the number of requests received using a specific HTTP method.
        /// </summary>
        /// <param name="method">HTTP method.</param>
        /// <returns>Number of requests received using this method.</returns>
        public long RequestCountByMethod(HttpMethod method)
        {
            return Interlocked.Read(ref _RequestsByMethod[(int)method]);
        }

        #endregion

        #region Private-and-Internal-Methods

        internal void IncrementRequestCounter(HttpMethod method)
        {
            Interlocked.Increment(ref _RequestsByMethod[(int)method]);
        }

        internal void IncrementReceivedPayloadBytes(long len)
        {
            Interlocked.Add(ref _ReceivedPayloadBytes, len);
        }

        internal void IncrementSentPayloadBytes(long len)
        {
            Interlocked.Add(ref _SentPayloadBytes, len);
        }

        #endregion
    }
}
