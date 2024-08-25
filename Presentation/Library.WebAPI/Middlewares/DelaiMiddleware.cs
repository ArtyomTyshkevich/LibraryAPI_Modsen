using MassTransit;

public class DelayMiddleware : IFilter<ConsumeContext>
{
    public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
    {
        await next.Send(context);
        await Task.Delay(20000); 
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("delay");
    }
}