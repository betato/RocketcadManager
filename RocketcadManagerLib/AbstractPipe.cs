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
        private const int EndOfStream = -1;

        public event Action Connected;
        public event Action Disconnected;
        public event Action<string> MessageRecieved;

        public AbstractPipe(string name)
        {
            Name = name;
        }

        public void Run()
        {
            if (pipe == null)
                RunAsync();
            else
                throw new InvalidOperationException("Pipe already started.");
        }

        protected abstract void RunAsync();

        public void Stop()
        {
            if (pipe == null)
                return;
            if (pipe.IsConnected)
            {
                byte[] eos = BitConverter.GetBytes(EndOfStream);
                pipe.Write(eos, 0, eos.Length);
            }
            pipe.WaitForPipeDrain();
            pipe.Close();
            pipe.Dispose();
            pipe = null;
        }

        public bool IsConnected()
        {
            return pipe != null && pipe.IsConnected;
        }

        protected void BeginRead()
        {
            Connected?.Invoke();
            Task.Run(() => ReadAsync());
        }

        public void Write(string message)
        {
            byte[] messageData = Encoding.UTF8.GetBytes(message);
            byte[] lengthHeader = BitConverter.GetBytes(messageData.Length);

            pipe.Write(lengthHeader, 0, lengthHeader.Length);
            pipe.Write(messageData, 0, messageData.Length);
        }

        private void ReadAsync()
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
                    if (messageLength == EndOfStream)
                        break; // Connection closed

                    // Get message
                    byte[] messageData = new byte[messageLength];
                    if (pipe.Read(messageData, 0, messageLength) != messageLength)
                        break; // This is an error
                    MessageRecieved?.Invoke(Encoding.UTF8.GetString(messageData));
                }
            }
            catch (Exception)
            {
                // Pipe broken, wait for new connection
            }

            Disconnected?.Invoke();
            RunAsync();
        }
    }
}
