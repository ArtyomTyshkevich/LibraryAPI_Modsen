using Library.Data.Context;

namespace Library.Tests.Common
{
    public abstract class TestCommandBase : IDisposable
    {
        protected readonly LibraryDbContext Context;

        public TestCommandBase()
        {
            Context = LibraryContextFactory.CreateInMemory();
        }

        public void Dispose()
        {
            LibraryContextFactory.Destroy(Context);
        }
    }
}