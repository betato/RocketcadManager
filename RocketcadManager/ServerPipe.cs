using RocketcadManagerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketcadManager
{
    class ServerPipe : AbstractPipe
    {
        public ServerPipe(string name) : base(name) { }

        private NamedPipeServerStream serverPipe;

        public override void Start()
        {
            serverPipe = new NamedPipeServerStream(Name, PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            pipe = serverPipe;
            serverPipe.BeginWaitForConnection(new AsyncCallback(PipeConnected), null);
        }

        private void PipeConnected(IAsyncResult ar)
        {
            serverPipe.EndWaitForConnection(ar);
            BeginRead();
        }
    }
}
