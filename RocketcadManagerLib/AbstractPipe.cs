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

        public abstract void Start();

        public void Stop()
        {
            if (pipe == null)
                return;
            if (pipe.IsConnected)
                pipe.Write(new byte[0], 0, 0);
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
            byte[] response = BitConverter.GetBytes(0);
            pipe.Write(response, 0, response.Length);
        }

        private void ReadAsync()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[sizeof(int)];
                    if (pipe.Read(buffer, 0, buffer.Length) <= 0)
                    {
                        // Connection closed, wait for new connection
                        break;
                    }

                    MessageRecieved?.Invoke("recieved");
                }
            }
            catch (Exception)
            {
                // Pipe broken, wait for new connection
            }

            Disconnected?.Invoke();
            Start();
        }
    }
}
