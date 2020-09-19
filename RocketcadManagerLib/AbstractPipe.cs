using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketcadManagerLib
{
    public abstract class AbstractPipe
    {
        public string Name { get; private set; }
        protected PipeStream pipe;

        public event Action Connected;
        public event Action Disconnected;
        public event Action<string> MessageRecieved;

        public AbstractPipe(string name)
        {
            Name = name;
        }

        public void Start()
        {
            if (pipe == null)
                RunAsync();
            else
                throw new InvalidOperationException("Pipe already started.");
        }

        protected abstract void RunAsync();

        public bool IsConnected()
        {
            return pipe != null && pipe.IsConnected;
        }

        protected void BeginRead()
        {
            Connected?.Invoke();
            Read();
        }

        public void Write(string message)
        {
            byte[] messageData = Encoding.UTF8.GetBytes(message);
            byte[] lengthHeader = BitConverter.GetBytes(messageData.Length);
            byte[] fullMessage = new byte[lengthHeader.Length + messageData.Length];
            Buffer.BlockCopy(lengthHeader, 0, fullMessage, 0, lengthHeader.Length);
            Buffer.BlockCopy(messageData, 0, fullMessage, lengthHeader.Length, messageData.Length);
            pipe.WriteAsync(fullMessage, 0, fullMessage.Length);
        }

        private void Read()
        {
            try
            {
                while (true)
                {
                    // Get message length
                    byte[] lengthHeader = new byte[sizeof(int)];
                    if (pipe.Read(lengthHeader, 0, lengthHeader.Length) <= 0)
                        break; // Connection closed, wait for new connection
                    int messageLength = BitConverter.ToInt32(lengthHeader, 0);
                    if (messageLength <= 0)
                        break; // Connection closed

                    // Get message
                    byte[] messageData = new byte[messageLength];
                    if (pipe.Read(messageData, 0, messageLength) != messageLength)
                        throw new ArgumentOutOfRangeException("Message length does not match buffer size");
                    MessageRecieved?.Invoke(Encoding.UTF8.GetString(messageData));
                }
                // Send disconnect event and begin looking for a new connection
                Disconnected?.Invoke();
                RunAsync();
            }
            catch (Exception e)
            {
                MessageRecieved?.Invoke(e.Message);
                MessageRecieved?.Invoke(e.StackTrace);
                LogWriter.Write(LogType.AddinLinkError, e.StackTrace);
            }
        }
    }
}
