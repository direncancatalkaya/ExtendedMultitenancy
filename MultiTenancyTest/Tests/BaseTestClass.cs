using Microsoft.EntityFrameworkCore.Storage;

namespace MultiTenancyTest.Tests
{
    public class BaseTestClass
    {
        protected InMemoryDatabaseRoot _root;

        protected InMemoryDatabaseRoot Root
        {
            get
            {
                if (_root == null)
                    _root = new InMemoryDatabaseRoot();

                return _root;
            }
        }
    }
}