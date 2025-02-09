using CCGP.AspectContainer;

namespace CCGP.Client
{
    public static class ClientFactory
    {
        public static Client Create()
        {
            var client = new Client();
            client.AddAspect<ClientMessageSystem>();

            return client;
        }
    }
}