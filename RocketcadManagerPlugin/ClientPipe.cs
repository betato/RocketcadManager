using RocketcadManagerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketcadManagerPlugin
{
    class ClientPipe : AbstractPipe
    {
        NamedPipeClientStream clientPipe;

        public ClientPipe(string name) : base(name) { }

        public override void Start()
        {
            clientPipe = new NamedPipeClientStream(".", Name,
                PipeDirection.InOut, PipeOptions.Asynchronous);
            pipe = clientPipe;
            clientPipe.ConnectAsync().ContinueWith(task =>
            {
                BeginRead();
            });
        }
    }
}
