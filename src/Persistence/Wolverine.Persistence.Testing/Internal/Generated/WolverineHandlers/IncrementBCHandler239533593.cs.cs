// <auto-generated/>
#pragma warning disable
using Wolverine.Persistence.Marten.Publishing;

namespace Internal.Generated.WolverineHandlers
{
    // START: IncrementBCHandler239533593
    public class IncrementBCHandler239533593 : Wolverine.Runtime.Handlers.MessageHandler
    {
        private readonly Wolverine.Persistence.Marten.Publishing.OutboxedSessionFactory _outboxedSessionFactory;

        public IncrementBCHandler239533593(Wolverine.Persistence.Marten.Publishing.OutboxedSessionFactory outboxedSessionFactory)
        {
            _outboxedSessionFactory = outboxedSessionFactory;
        }



        public override async System.Threading.Tasks.Task HandleAsync(Wolverine.IMessageContext context, System.Threading.CancellationToken cancellation)
        {
            var letterHandler = new Wolverine.Persistence.Testing.Marten.LetterHandler();
            var incrementBC = (Wolverine.Persistence.Testing.Marten.IncrementBC)context.Envelope.Message;
            await using var documentSession = _outboxedSessionFactory.OpenSession(context);
            var eventStore = documentSession.Events;
            // Loading Marten aggregate
            var eventStream = await eventStore.FetchForWriting<Wolverine.Persistence.Testing.Marten.LetterAggregate>(incrementBC.LetterAggregateId, cancellation).ConfigureAwait(false);

            var outgoing1 = letterHandler.Handle(incrementBC, eventStream.Aggregate);
            if (outgoing1 != null)
            {
                // Capturing any possible events returned from the command handlers
                eventStream.AppendMany(outgoing1);

            }

            await documentSession.SaveChangesAsync(cancellation).ConfigureAwait(false);
        }

    }

    // END: IncrementBCHandler239533593
    
    
}

