// <auto-generated/>
#pragma warning disable

namespace Internal.Generated.WolverineHandlers
{
    // START: PongMessageHandler1540302571
    public class PongMessageHandler1540302571 : Wolverine.Runtime.Handlers.MessageHandler
    {


        public override System.Threading.Tasks.Task HandleAsync(Wolverine.Runtime.MessageContext context, System.Threading.CancellationToken cancellation)
        {
            var pongHandler = new MyApp.PongHandler();
            var pongMessage = (TestingSupport.Compliance.PongMessage)context.Envelope.Message;
            return pongHandler.Handle(pongMessage);
        }

    }

    // END: PongMessageHandler1540302571
    
    
}

